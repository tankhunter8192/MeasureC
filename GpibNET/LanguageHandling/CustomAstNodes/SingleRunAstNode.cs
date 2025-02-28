using Gpib.Web.Data.DBClasses;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Parsing;

namespace Gpib.Web.LanguageHandling.CustomAstNodes
{
    public class SingleRunAstNode : AstNode
    {
        //TODO: INFO: Should be done - 22.12.2024
        List<ParseTreeNode> children = new List<ParseTreeNode>();
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            AsString = "SingleRun";
            foreach (var node in treeNode.ChildNodes)
            {
                if (node is not null) AddChild("child", node);
                if (node is not null) children.Add(node);
            }
        }

        protected override object DoEvaluate(ScriptThread thread)
        {
            thread.CurrentNode = this;
            if (ChildNodes.Count != 1)
            {
                ErrorAnchor = Location;
                thread.ThrowScriptError("SingleRun must have 1 child");
            }
            else
            {
                MeasurePointVariable? res = new MeasurePointVariable();
                var varRes = ChildNodes[0].Evaluate(thread);
                if (varRes is MeasurePointVariable)
                {
                    res = varRes as MeasurePointVariable;
                    thread.CurrentNode = Parent;
                    return res!;
                }
                else
                {
                    ErrorAnchor = Location;
                    thread.ThrowScriptError("SingleRun must have a MeasurePointVariable as child");
                }
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
