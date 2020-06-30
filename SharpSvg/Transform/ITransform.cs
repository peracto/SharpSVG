using Peracto.Svg.Types;

namespace Peracto.Svg.Transform
{
    public interface ITransform
    {
        TransformType TransformType { get; }
        PxMatrix Resolve(IElement element, IFrameContext context);
    }

    public interface ITransformOrigin
    {
        Measure X { get; }
        Measure Y { get; }
    }

}