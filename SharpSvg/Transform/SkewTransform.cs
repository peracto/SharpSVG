using Peracto.Svg.Types;

namespace Peracto.Svg.Transform
{
    public class SkewTransform : ITransform
    {
        public TransformType TransformType => TransformType.Skew;

        public float X { get; }
        public float Y { get; }
        public PxMatrix Matrix { get; }

        public SkewTransform(float x, float y)
        {
            X = x;
            Y = x;
            Matrix = PxMatrix.Skew(x, y);
        }

        public override string ToString()
        {
            return $"skew({X},{Y})";
        }
    }
}