using Irony.Ast;
using Irony.Interpreter;
using Irony.Interpreter.Ast;

namespace Gpib.Web.LanguageHandling.CustomAstNodes
{
    public class HeadLoopAstNode : AstNode
    {
        public override void Init(AstContext context, Irony.Parsing.ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            AsString = "HeadLoop";
            foreach (var node in treeNode.ChildNodes)
            {
                if (node != null) AddChild("child", node);
            }
        }

        protected override object DoEvaluate(ScriptThread thread)
        {
            thread.CurrentNode = this;
            if (ChildNodes.Count != 4)
            {
                ErrorAnchor = Location;
                thread.ThrowScriptError("HeadLoop must have 4 children");
            }
            else
            {
                foreach (AstNode child in ChildNodes)
                {
                    child.Evaluate(thread);
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
