using Gpib.Web.Data.DBClasses;
using Irony.Parsing;

namespace Gpib.Web.LanguageHandling.CustomTerminals
{
    public class CustomNumberTerminal : NumberLiteral
    {
        public MeasurePointVariable value = new MeasurePointVariable();
        public CustomNumberTerminal(string name) : base(name)
        {
            
            base.DefaultIntTypes = new TypeCode[]{TypeCode.Int32, TypeCode.Int64 };
            base.DefaultFloatType = TypeCode.Double;
        }

        public override Token TryMatch(ParsingContext context, ISourceStream source)
        {
            var token = base.TryMatch(context, source);
            if (token != null)
            {
                if (token.Value is int intValue)
                {
                    token.Value = new MeasurePointVariable(intValue);
                }
                else if (token.Value is double doubleValue)
                {
                    token.Value = new MeasurePointVariable((decimal) doubleValue);
                }
            }
            return token!;
        }
    }
}
