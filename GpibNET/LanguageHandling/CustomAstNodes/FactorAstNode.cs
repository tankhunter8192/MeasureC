using Irony.Ast;
using Irony.Interpreter.Ast;
using Irony.Parsing;

namespace Gpib.Web.LanguageHandling.CustomAstNodes
{
    public class FactorAstNode : AstNode
    {
        public List<ParseTreeNode> children { get; set; }= new List<ParseTreeNode>();
        public ParseTreeNode ThisNode { get; set; }
        public override void Init(AstContext context, Irony.Parsing.ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            ThisNode = treeNode;
            foreach (var child in treeNode.ChildNodes)
            {
                if (child is not null)
                {
                    AddChild("child", child);
                    children.Add(child);
                }
            }
            AsString = "Factor";
        }

        protected override object DoEvaluate(Irony.Interpreter.ScriptThread thread)
        {
            //TODO: Should be done - 21.12.2024
            thread.CurrentNode = this;
            if (children.Count == 1)
            {
                return ChildNodes[0].Evaluate(thread);
            }
            else if (children.Count == 2)
            {
                ErrorAnchor = Location;
                thread.ThrowScriptError("not expected to come here: Factor Ast Node, 2 Children [use case function in class construction]");
                //return ChildNodes[1].Evaluate(thread);
            }
            else
            {
                return null;
            }

            thread.CurrentNode = Parent;
            return null;    
        }

        public override void DoSetValue(Irony.Interpreter.ScriptThread thread, object value)
        {
            thread.CurrentNode = this;
            base.DoSetValue(thread, value);
            thread.CurrentNode = Parent;
        }

        public override string ToString()
        {
            string mes = AsString;
            mes += "\n[\n";
            foreach (var child in ChildNodes)
            {
                mes += child.ToString();
            }
            mes += "]\n";
            return mes;
        }
    }
}
