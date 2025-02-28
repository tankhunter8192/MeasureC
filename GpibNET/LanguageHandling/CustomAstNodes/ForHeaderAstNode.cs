using Gpib.Web.LanguageHandling.Helpers;
using Irony.Ast;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Parsing;

namespace Gpib.Web.LanguageHandling.CustomAstNodes
{
    public class ForHeaderAstNode : AstNode
    {
        //TODO: INFO: Should be done - 21.12.2024
        private List<ParseTreeNode> children { get; set; } = new List<ParseTreeNode>();
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            AsString = "ForHeader";
            foreach (var node in treeNode.ChildNodes)
            {
                if (node is not null) AddChild("child", node);
                if (node is not null) children.Add(node);
            }
        }

        protected override object DoEvaluate(Irony.Interpreter.ScriptThread thread)
        {
            thread.CurrentNode = this;
            if (ChildNodes.Count == 7)
            {
                ForHeaderReturn res = new ForHeaderReturn();
                res.Assinement = ChildNodes[1];
                res.Condition = ChildNodes[3];
                res.Increment = ChildNodes[5];
                thread.CurrentNode = Parent;
                return res;
            }
            else
            {
                ErrorAnchor = Location;
                thread.ThrowScriptError("ForHeader Error: Children count invalid");
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
