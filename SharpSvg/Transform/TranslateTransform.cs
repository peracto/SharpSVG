using Peracto.Svg.Types;

namespace Peracto.Svg.Transform
{
    public class TranslateTransform : ITransform
    {
        public TransformType TransformType => TransformType.Translate;

        public Measure X { get; }
        public Measure Y { get; }

        public PxMatrix Resolve(IElement element, IFrameContext context)
        {
            return PxMatrix.Translate(X.Resolve(element,context),Y.Resolve(element, context));
        }

        public TranslateTransform(Measure x, Measure y)
        {
            X = x;
            Y = y;
        }
        public override string ToString()
        {
            return $"translate({X},{Y})";
        }
    }


}