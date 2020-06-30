using System.Text.RegularExpressions;
using Peracto.Svg.Transform;
using Peracto.Svg.Types;

namespace Peracto.Svg.Converters
{
    public class TransformOriginAttributeConverter : AttributeConverterBase<ITransformOrigin>
    {
        public TransformOriginAttributeConverter(string name) : base(name)
        {
        }

        protected override bool TryCreate(string attributeValue, out ITransformOrigin rc)
        {
            var args = Regex.Replace(attributeValue.Trim(), @"[\s,]+", ",")
                .Split(',');
            if (args.Length != 2)
            {
                rc = null;
                return false;
            }

            if (!Measure.TryParse(args[0], MeasureUsage.Horizontal, out var xMeasure) ||
                !Measure.TryParse(args[1], MeasureUsage.Vertical, out var yMeasure))
            {
                rc = null;
                return false;
            }

            rc =new TransformOrigin(xMeasure, yMeasure );
            return true;
        }
    }
}