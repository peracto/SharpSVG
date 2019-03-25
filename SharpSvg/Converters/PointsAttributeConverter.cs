using System;
using Peracto.Svg.Types;

namespace Peracto.Svg.Converters
{
    public class PointsAttributeConverter : AttributeConverterBase<PxPoint[]>
    {
        public PointsAttributeConverter(string name) : base(name)
        {
        }
        protected override bool  TryCreate(string attributeValue, IElement elementFactory,out PxPoint[] rc)
        {
            throw new NotImplementedException();
        }
    }
}