using Irony.Ast;
using Irony.Interpreter;
using Irony.Interpreter.Ast;

namespace Gpib.Web.LanguageHandling.CustomAstNodes
{
    public class BlockAstNode : AstNode
    {
        //TODO: INFO: Should be done - 21.12.2024
        public override void Init(AstContext context, Irony.Parsing.ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            AsString = "Block";
            foreach (var node in treeNode.ChildNodes)
            {
                if (node is not null) AddChild("child", node);
            }
        }

        protected override object DoEvaluate(Irony.Interpreter.ScriptThread thread)
        {
            thread.CurrentNode = this;
            if (ChildNodes.Count != 3)
            {
                ErrorAnchor = Location;
                thread.ThrowScriptError("Block must have 3 children");
            }
            ChildNodes[1].Evaluate(thread);
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
