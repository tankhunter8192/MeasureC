using Irony.Interpreter.Ast;

namespace Gpib.Web.LanguageHandling.CustomAstNodes
{
    public class SemiStatementAstNode : AstNode
    {
        //TODO: Implement this?
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
