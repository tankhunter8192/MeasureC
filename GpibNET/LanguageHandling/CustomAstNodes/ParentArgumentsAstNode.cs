using Irony.Ast;
using Irony.Interpreter;
using Irony.Interpreter.Ast;
using Irony.Parsing;

namespace Gpib.Web.LanguageHandling.CustomAstNodes
{
    public class ParentArgumentsAstNode : AstNode
    {
        public override void Init(AstContext context, ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            AsString = "Parent Arguments";
            foreach (var node in treeNode.ChildNodes)
            {
                if(node is not null) AddChild("child", node);
            }
        }

        protected override object DoEvaluate(ScriptThread thread)
        {
            thread.CurrentNode = this;
            foreach (var child in ChildNodes)
            {
                child.Evaluate(thread);
            }
            thread.CurrentNode = Parent;
            return base.DoEvaluate(thread);
        }

        public override void DoSetValue(ScriptThread thread, object value)
        {
            thread.CurrentNode = this;
            base.DoSetValue(thread, value);
            thread.CurrentNode = Parent;
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
