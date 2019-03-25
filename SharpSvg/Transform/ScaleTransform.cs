using Peracto.Svg.Types;

namespace Peracto.Svg.Transform
{
    public class ScaleTransform : ITransform
    {
        public TransformType TransformType => TransformType.Scale;

        public float X { get; }
        public float Y { get; }
        public PxMatrix Matrix { get; }

        public ScaleTransform(float x, float y)
        {
            X = x;
            Y = y;
            Matrix = PxMatrix.Scale(x, y);
        }

        public override string ToString()
        {
            return $"scale({X},{Y})";
        }


    }
}