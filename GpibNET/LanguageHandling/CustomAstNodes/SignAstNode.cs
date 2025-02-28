using Irony.Ast;
using Irony.Interpreter.Ast;
using NuGet.Protocol;

namespace Gpib.Web.LanguageHandling.CustomAstNodes
{
    public class SignAstNode : AstNode
    {
        public override void Init(AstContext context, Irony.Parsing.ParseTreeNode treeNode)
        {
            base.Init(context, treeNode);
            if (this.ChildNodes.Count == 1)
            {
                AsString = "Sign" + treeNode.ChildNodes[0].Token.Text;
            }
            else
            {
                AsString= "Sign" + treeNode.Token.Text;
            }
            foreach (var node in treeNode.ChildNodes)
            {
                if (node != null) AddChild("child", node);
            }
        }

        protected override object DoEvaluate(Irony.Interpreter.ScriptThread thread)
        {
            thread.CurrentNode = this;
            var sign = ChildNodes[0].Evaluate(thread);
            thread.CurrentNode = Parent;
            return sign;
        }

        public override void DoSetValue(Irony.Interpreter.ScriptThread thread, object value)
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
