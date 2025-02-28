using Irony.Interpreter.Ast;

namespace Gpib.Web.LanguageHandling.CustomAstNodes
{
    public class TimeAstNode : AstNode
    {
        //TODO: Implement TimeAstNode -> write timing in main/Thread , evaluate returns null
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
