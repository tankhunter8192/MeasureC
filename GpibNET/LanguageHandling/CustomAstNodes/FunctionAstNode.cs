using Irony.Ast;
using Irony.Interpreter.Ast;
using Irony.Parsing;

namespace Gpib.Web.LanguageHandling.CustomAstNodes
{
    public class FunctionAstNode: AstNode
    {

        public override void Init(AstContext context, ParseTreeNode parseNode)
        {
            base.Init(context, parseNode);
        }
        public object DoEvaluate(Irony.Interpreter.ScriptThread thread)
        {
            return base.Evaluate(thread);
        }
        public void DoSetValue(Irony.Interpreter.ScriptThread thread, object value)
        {
            base.SetValue(thread, value);
        }
        public override string ToString()
        {
            string mes = "";
            mes += "functionNode\n";
            foreach (var child in ChildNodes)
            {
                mes += child.ToString();
            }
            return mes;
        }
    }
}
