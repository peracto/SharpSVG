using System;
using Peracto.Svg.Image;

namespace Peracto.Svg.Converters
{
    public class PreserveAspectRatioConverter : AttributeConverterBase<PreserveAspectRatio>
    {
        public PreserveAspectRatioConverter(string name) : base(name)
        {
        }

        protected override bool TryCreate(string value, IElement elementFactory,out PreserveAspectRatio  rc)
        {
            rc = Create(value.Trim());
            return rc != null;
        }

        protected PreserveAspectRatio Create(string value)
        {
            if (string.IsNullOrEmpty(value)) return null;
            var sParts = value.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            if (!Enum.TryParse<PreserveAspectRatioType>(sParts[0], true, out var align))
                throw new ArgumentOutOfRangeException("value is not a member of PreserveAspectRatioType");

            var option = PreserveAspectRatioOption.None;
            if (sParts.Length > 1 && !Enum.TryParse<PreserveAspectRatioOption>(sParts[1], true, out option))
                throw new ArgumentOutOfRangeException("value is not a member of PreserveAspectRatioType");

            return new PreserveAspectRatio(align, option);
        }

    }
}