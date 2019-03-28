namespace Peracto.Svg.Converters
{
    public class TokenAttributeConverter : AttributeConverterBase<string>
    {
        public TokenAttributeConverter(string name) : base(name)
        {
        }

        protected override bool TryCreate(string attributeValue,out string rc)
        {
            rc = attributeValue.Trim();
            return rc.Length > 0;
        }
    }
}