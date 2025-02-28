using System.Text.RegularExpressions;
using Gpib.Web.Data.DBClasses;
using Irony.Parsing;

namespace Gpib.Web.LanguageHandling.CustomTerminals
{
    public class TimeTerminal(string name) : RegexBasedTerminal(name, @"(\d{4}-\d{2}-\d{2}( \d{2}:\d{2}:\d{2})?)|(\d{2}:\d{2}:\d{2})")
    {
        public MeasurePointVariable value = new MeasurePointVariable();
        public DateTime time = DateTime.MinValue;

        public override Token TryMatch(ParsingContext context, ISourceStream source)
        {
            Match match = Regex.Match(source.Text, Pattern);
            if (match.Success && match.Index == 0)
            {
                string value = match.Value;
                time = DateTime.Parse(value);
                this.value.DateTime = time;
                this.value.IsSet = true;
                this.value.Type = MPVPrimeType.DateTime;
                this.value.OrginalValue = source.Text;
                source.PreviewPosition += match.Length;
                return source.CreateToken(OutputTerminal, time);
            }
            return null;
        }

        public override void Init(GrammarData grammarData)
        {
            base.Init(grammarData);
        }
    }


}
