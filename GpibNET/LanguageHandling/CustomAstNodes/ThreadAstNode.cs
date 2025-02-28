using Irony.Ast;
using Irony.Interpreter.Ast;

namespace Gpib.Web.LanguageHandling.CustomAstNodes
{
    public class ThreadAstNode : AstNode
    {
        public override void Init(AstContext context, Irony.Parsing.ParseTreeNode parseNode)
        {
            base.Init(context, parseNode);
        }
        public object DoEvaluate(Irony.Interpreter.ScriptThread thread)
        {
            thread.CurrentNode = this;
            foreach (var child in ChildNodes)
            {
                child.Evaluate(thread);
            }
            var res = base.Evaluate(thread);
            thread.CurrentNode = Parent;
            return res;
        }
        public override string ToString()
        {
            string mes = "";
            mes += "threadNode\n";
            foreach (var child in ChildNodes)
            {
                mes += child.ToString();
            }
            return mes;
        }
    }
}
