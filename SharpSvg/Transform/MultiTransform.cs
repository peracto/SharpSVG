using System.Linq;
using Peracto.Svg.Types;

namespace Peracto.Svg.Transform
{
    public class TransformOrigin : ITransformOrigin
    {
        private readonly Measure _xMeasure;
        private readonly Measure _yMeasure;

        public TransformOrigin(Measure xMeasure, Measure yMeasure)
        {
            _xMeasure = xMeasure;
            _yMeasure = yMeasure;
        }

        public Measure X => _xMeasure;
        public Measure Y => _yMeasure;
    }


    public class MultiTransform : ITransform
    {
        public TransformType TransformType => TransformType.Multi;

        private ITransform[] _t;

  //      public PxMatrix Matrix { get; }

        public MultiTransform(ITransform[] transforms)
        {
            _t = transforms;
//            Matrix = transforms.Aggregate(PxMatrix.Identity, (current, m) => m.Matrix * current);
        }
        public override string ToString()
        {
            return string.Join(",",_t.Select((v) => v.ToString()));
        }

        public PxMatrix Resolve(IElement element, IFrameContext context)
        {
            return _t.Aggregate(PxMatrix.Identity, (current, m) => m.Resolve(element, context) * current);
        }
    }
}