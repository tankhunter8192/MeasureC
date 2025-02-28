using Irony.Ast;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Parsing;

namespace Gpib.Web.LanguageHandling.CustomAstNodes
{
    public class BlockContentAstNode : AstNode
    {
        //TODO: INFO: Should be done - 21.12.2024
        List<ParseTreeNode> children = new List<ParseTreeNode>();
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            AsString = "BlockContent";
            foreach (var node in treeNode.ChildNodes)
            {
                if (node is not null) AddChild("child", node);
                if (node is not null) children.Add(node);
            }
        }

        protected override object DoEvaluate(Irony.Interpreter.ScriptThread thread)
        {
            thread.CurrentNode = this;
            if (ChildNodes.Count == 1)
            {
                ChildNodes.First().Evaluate(thread);
            }
            else if (ChildNodes.Count == 2)
            {
                ChildNodes.First().Evaluate(thread);
                ChildNodes.Last().Evaluate(thread);
                thread.CurrentNode = Parent;
                return null!;
            }
            else
            {
                ErrorAnchor = Location;
                thread.ThrowScriptError("BlockContent: Invalid number of children");
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
