using Irony.Interpreter.Ast;

namespace Gpib.Web.LanguageHandling.Helpers
{
    public class ForHeaderReturn
    {
        public AstNode Assinement { get; set; }
        public AstNode Condition { get; set; }
        public AstNode Increment { get; set; }
    }
}
