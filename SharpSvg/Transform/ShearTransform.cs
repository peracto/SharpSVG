using Peracto.Svg.Types;

namespace Peracto.Svg.Transform
{
    public class ShearTransform : ITransform
    {
        public TransformType TransformType => TransformType.Shear;
        public Measure X { get; }
        public Measure Y { get; }

        public ShearTransform(Measure x, Measure y)
        {
            X = x;
            Y = y;
        }
        public override string ToString()
        {
            return $"shear({X},{Y})";
        }

        public PxMatrix Resolve(IElement element, IFrameContext context)
        {
            return PxMatrix.Skew(X.Resolve(element, context), Y.Resolve(element, context));
        }

    }
}