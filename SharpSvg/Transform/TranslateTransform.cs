using Peracto.Svg.Types;

namespace Peracto.Svg.Transform
{
    public class TranslateTransform : ITransform
    {
        public TransformType TransformType => TransformType.Translate;

        public float X { get; }
        public float Y { get; }
        public PxMatrix Matrix { get; }

        public TranslateTransform(float x, float y)
        {
            X = x;
            Y = y;
            Matrix = PxMatrix.Translate(x, y);
        }
        public override string ToString()
        {
            return $"translate({X},{Y})";
        }
    }
}