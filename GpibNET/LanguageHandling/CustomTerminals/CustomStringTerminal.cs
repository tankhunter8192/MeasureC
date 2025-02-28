using Gpib.Web.Data.DBClasses;
using Irony.Parsing;

namespace Gpib.Web.LanguageHandling.CustomTerminals
{
    public class CustomStringTerminal : StringLiteral
    {
        public MeasurePointVariable value = new MeasurePointVariable();
        public CustomStringTerminal(string name, string startEndSymbol, StringOptions options) : base(name, startEndSymbol, options)
        {

        }

        public CustomStringTerminal(string name, string startEndSymbol) : base(name, startEndSymbol)
        {
        }

        public CustomStringTerminal(string name) : base(name)
        {
        }

        public override Token TryMatch(ParsingContext context, ISourceStream source)
        {
            var token = base.TryMatch(context, source);
            if (token != null)
            {
                value = new MeasurePointVariable(token.ValueString);
            }
            return token;
        }
    }
}
