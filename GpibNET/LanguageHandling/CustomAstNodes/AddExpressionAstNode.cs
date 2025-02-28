using Gpib.Web.Data.DBClasses;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Parsing;

namespace Gpib.Web.LanguageHandling.CustomAstNodes
{
    public class AddExpressionAstNode : AstNode
    {
        //TODO: INFO: Should be done - 21.12.2024
        private List<ParseTreeNode> children { get; set; } = new List<ParseTreeNode>();
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            AsString = "AddExpression";
            foreach (var node in treeNode.ChildNodes)
            {
                if (node is not null) AddChild("child", node);
                if (node is not null) children.Add(node);
            }
        }

        protected override object DoEvaluate(ScriptThread thread)
        {
            thread.CurrentNode = this;
            if (ChildNodes.Count == 1)
            {
                MeasurePointVariable res = (MeasurePointVariable)ChildNodes[0].Evaluate(thread);
                thread.CurrentNode = Parent;
                return res;
            }else if (ChildNodes.Count == 3)
            {
                MeasurePointVariable res = new MeasurePointVariable();
                MeasurePointVariable multiplyExpressionRes = (MeasurePointVariable)ChildNodes[0].Evaluate(thread);
                MeasurePointVariable addOperatorRes = (MeasurePointVariable)ChildNodes[1].Evaluate(thread);
                MeasurePointVariable addExpressionRes = (MeasurePointVariable)ChildNodes[2].Evaluate(thread);
                if (addExpressionRes is not null && addOperatorRes is not null && multiplyExpressionRes is not null)
                {
                    switch (addOperatorRes.Text)
                    {
                        case "+":
                            res = multiplyExpressionRes + addExpressionRes;
                            break;
                        case "-":
                            res = multiplyExpressionRes - addExpressionRes;
                            break;
                        default:
                            ErrorAnchor = Location;
                            thread.ThrowScriptError("Operator not found");
                            break;
                    }
                    thread.CurrentNode = Parent;
                    return res;
                }
            }
            thread.CurrentNode = Parent;
            return null;
        }

        public override void DoSetValue(ScriptThread thread, object value)
        {
            base.DoSetValue(thread, value);
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
