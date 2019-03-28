using System.Linq;
using System.Text.RegularExpressions;

namespace Peracto.Svg.Converters
{
    public class TokenListAttributeConverter : AttributeConverterBase<string[]>
    {
        private readonly string _splitRegexString;

        public TokenListAttributeConverter(string name,string splitRegexString) : base(name)
        {
            _splitRegexString = splitRegexString;
        }

        protected override bool TryCreate(string value, out string[]  rc)
        {
            rc = (from a in Regex.Split(value.Trim(), _splitRegexString)
                where !string.IsNullOrEmpty(a)
                select a).ToArray();
            return rc.Length > 0;
        }
    }
}