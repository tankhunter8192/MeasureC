using Gpib.Web.Data.DBClasses;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Parsing;

namespace Gpib.Web.LanguageHandling.CustomAstNodes
{
    
    public class MultiplyExpressionAstNode : AstNode
    {
        //TODO: Should be done - 21.12.2024
        List<ParseTreeNode> children { get; set; } = new List<ParseTreeNode>();
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            AsString = "MultiplyExpression";
            foreach (var node in treeNode.ChildNodes)
            {
                if (node is not null) AddChild("child", node);
                if (node is not null) children.Add(node);
            }
        }

        protected override object DoEvaluate(ScriptThread thread)
        {
            thread.CurrentNode = this;
            if (children.Count == 1)
            {
                MeasurePointVariable res = ChildNodes[0].Evaluate(thread) as MeasurePointVariable;
                thread.CurrentNode = Parent;
                return res;
            }else if (children.Count == 3)
            {
                MeasurePointVariable prefixExpressionRes = ChildNodes[0].Evaluate(thread) as MeasurePointVariable;
                MeasurePointVariable multiplyOperatorRes = ChildNodes[1].Evaluate(thread) as MeasurePointVariable;
                MeasurePointVariable multiplyExpressionRes = ChildNodes[2].Evaluate(thread) as MeasurePointVariable;
                MeasurePointVariable res = new MeasurePointVariable();
                if (prefixExpressionRes is not null && multiplyOperatorRes is not null && multiplyExpressionRes is not null)
                {
                    switch (multiplyOperatorRes.Text)
                    {
                        case "*":
                            res = prefixExpressionRes * multiplyExpressionRes;
                            break;
                        case "/":
                            res = prefixExpressionRes / multiplyExpressionRes;
                            break;
                        case "%":
                            res = prefixExpressionRes % multiplyExpressionRes;
                            break;
                        default:
                            ErrorAnchor = Location;
                            thread.ThrowScriptError("Invalid multiply expression: wrong operator");
                            break;
                    }
                    thread.CurrentNode = Parent;
                    return res;
                }
                else
                {
                    ErrorAnchor = Location;
                    thread.ThrowScriptError("Invalid multiply expression: wrong children type, something is null that should be not null");
                }
            }
            else
            {
                ErrorAnchor = Location;
                thread.ThrowScriptError("Invalid multiply expression: wrong children count");
            }
            thread.CurrentNode = Parent;
            return null;
        }

        public override string ToString()
        {
            string mes = AsString;
            mes += "\n[";
            foreach (var child in ChildNodes)
            {
                mes += child.ToString();
            }
            mes += "]\n";
            return mes;
        }
    }
}
