using Peracto.Svg.Types;

namespace Peracto.Svg.Transform
{
    public class ScaleTransform : ITransform
    {
        public TransformType TransformType => TransformType.Scale;

        public Measure X { get; }
        public Measure Y { get; }

        public ScaleTransform(Measure x, Measure y)
        {
            X = x;
            Y = y;
        }

        public override string ToString()
        {
            return $"scale({X},{Y})";
        }
        public PxMatrix Resolve(IElement element, IFrameContext context)
        {
            return PxMatrix.Scale(X.Resolve(element, context), Y.Resolve(element, context));
        }


    }
}