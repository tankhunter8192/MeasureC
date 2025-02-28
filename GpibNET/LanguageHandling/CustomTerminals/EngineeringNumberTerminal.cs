using Irony.Parsing;
using System.Text.RegularExpressions;
using Gpib.Web.Data.DBClasses;

namespace Gpib.Web.LanguageHandling.CustomTerminals
{
    public class EngineeringNumberTerminal(string name) : RegexBasedTerminal(name, @"\d+[QRYZEPTGMkmunpfazyrq.]\d+")
    {
        public MeasurePointVariable value = new MeasurePointVariable();
        public override Token TryMatch(ParsingContext context, ISourceStream source)
        {
            Match match = Regex.Match(source.Text, Pattern);
            if (match.Success && match.Index == 0)
            {
                string value = match.Value;
                int baseNumber = int.Parse(match.Groups[1].Value);
                string prefix = match.Groups[2].Value;
                string suffixNumber = match.Groups[3].Value;

                double multiplier = prefix switch
                {
                    "Q" => 1e30,
                    "R" => 1e27,
                    "Y" => 1e24,
                    "Z" => 1e21,
                    "E" => 1e18,
                    "P" => 1e15,
                    "T" => 1e12,
                    "G" => 1e9,
                    "M" => 1e6,
                    "k" => 1e3,
                    "m" => 1e-3,
                    "u" => 1e-6, // would be "µ" in a perfect world but not sure if that's supported and ASCII is an easy fallback
                    "n" => 1e-9,
                    "p" => 1e-12,
                    "f" => 1e-15,
                    "a" => 1e-18,
                    "z" => 1e-21,
                    "y" => 1e-24,
                    "r" => 1e-27,
                    "q" => 1e-30,
                    "." => 1,
                    "," => 1
                };

                decimal result = (decimal)(baseNumber * multiplier + (suffixNumber != string.Empty ? int.Parse(suffixNumber) * Math.Pow(10, -suffixNumber.Length) * multiplier : 0));
                this.value = new MeasurePointVariable(result);
                this.value.OrginalValue = value;
                source.PreviewPosition += match.Length;
                return source.CreateToken(OutputTerminal, result);
            }
            return null;
        }
    }
}
