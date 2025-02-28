using Gpib.Web.Data.DBClasses;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Parsing;

namespace Gpib.Web.LanguageHandling.CustomAstNodes
{
    public class BitExpressionAstNode : AstNode
    {
        //TODO: INFO: Shoud be done - 21.12.2024
        private List<ParseTreeNode> children { get; set; }= new List<ParseTreeNode>();
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            AsString = "BitExpression";
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
            else if(ChildNodes.Count == 3)
            {
                MeasurePointVariable res = new MeasurePointVariable();
                MeasurePointVariable addExpressionRes = (MeasurePointVariable)ChildNodes[0].Evaluate(thread);
                MeasurePointVariable bitOperatorRes = (MeasurePointVariable)ChildNodes[1].Evaluate(thread);
                MeasurePointVariable bitExpressionRes = (MeasurePointVariable)ChildNodes[2].Evaluate(thread);
                switch (bitOperatorRes.Text)
                {
                    case "&":
                        res = addExpressionRes & bitExpressionRes;
                        break;
                    case "|":
                        res = addExpressionRes | bitExpressionRes;
                        break;
                    case "^":
                        res = addExpressionRes ^ bitExpressionRes;
                        break;
                    default:
                        ErrorAnchor = Location;
                        thread.ThrowScriptError("BitOperator not defined");
                        break;
                }
                thread.CurrentNode = Parent;
                return res;
            }
            else
            {
                ErrorAnchor = Location;
                thread.ThrowScriptError("BitExpression Children count invalid");
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
