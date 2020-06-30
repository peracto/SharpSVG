using Peracto.Svg.Types;

namespace Peracto.Svg.Transform
{
    public class SkewTransform : ITransform
    {
        public TransformType TransformType => TransformType.Skew;

        public Measure X { get; }
        public Measure Y { get; }

        public SkewTransform(Measure x, Measure y)
        {
            X = x;
            Y = x;
        }

        public override string ToString()
        {
            return $"skew({X},{Y})";
        }

        public PxMatrix Resolve(IElement element, IFrameContext context)
        {
            return PxMatrix.Skew(X.Resolve(element, context), Y.Resolve(element, context));
        }

    }
}