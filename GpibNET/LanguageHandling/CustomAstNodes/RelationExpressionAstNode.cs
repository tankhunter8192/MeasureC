using Gpib.Web.Data.DBClasses;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Parsing;

namespace Gpib.Web.LanguageHandling.CustomAstNodes
{
    public class RelationExpressionAstNode : AstNode
    {
        //TODO: INFO: Should be done - 21.12.2024
        private List<ParseTreeNode> children { get; set; } = new List<ParseTreeNode>();
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            AsString = "RelationExpression";
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
            }
            else if (ChildNodes.Count == 3)
            {
                MeasurePointVariable res = new MeasurePointVariable();
                MeasurePointVariable bitExpressionRes = (MeasurePointVariable)ChildNodes[0].Evaluate(thread);
                MeasurePointVariable relationOperatorRes = (MeasurePointVariable)ChildNodes[1].Evaluate(thread);
                MeasurePointVariable bitExpressionRes2 = (MeasurePointVariable)ChildNodes[2].Evaluate(thread);
                if (bitExpressionRes is not null && relationOperatorRes is not null && bitExpressionRes2 is not null)
                {
                    switch(relationOperatorRes.Text)
                    {
                        case "==":
                            res = bitExpressionRes == bitExpressionRes2;
                            break;
                        case "!=":
                            res = bitExpressionRes != bitExpressionRes2;
                            break;
                        case ">":
                            res = bitExpressionRes > bitExpressionRes2;
                            break;
                        case "<":
                            res = bitExpressionRes < bitExpressionRes2;
                            break;
                        case ">=":
                            res = bitExpressionRes >= bitExpressionRes2;
                            break;
                        case "<=":
                            res = bitExpressionRes <= bitExpressionRes2;
                            break;
                        default:
                            res = new MeasurePointVariable(MPVPrimeType.Error);
                            break;
                    }
                }
                else
                {
                    ErrorAnchor = Location;
                    thread.ThrowScriptError("Relation Expression Error: inputs are null?");
                }
            }
            else
            {
                ErrorAnchor = Location;
                thread.ThrowScriptError("Relation Expression Error: ChildNodes.Count is not 1 or 3");
            }
            thread.CurrentNode = Parent;
            return null!;
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
