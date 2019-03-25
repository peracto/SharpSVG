using Peracto.Svg.Types;

namespace Peracto.Svg.Transform
{
    public class ShearTransform : ITransform
    {
        public TransformType TransformType => TransformType.Shear;

        public float X { get; }
        public float Y { get; }
        public PxMatrix Matrix { get; }

        public ShearTransform(float x, float y)
        {
            X = x;
            Y = y;
            Matrix = PxMatrix.Skew(x,y);
        }
        public override string ToString()
        {
            return $"shear({X},{Y})";
        }
    }
}