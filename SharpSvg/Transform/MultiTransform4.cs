using Peracto.Svg.Types;

namespace Peracto.Svg.Transform
{
    public class MultiTransform4 : ITransform
    {
        public TransformType TransformType => TransformType.Multi;

        private readonly ITransform _t0;
        private readonly ITransform _t1;
        private readonly ITransform _t2;
        private readonly ITransform _t3;

        public MultiTransform4(ITransform t0, ITransform t1, ITransform t2, ITransform t3)
        {
            _t0 = t0;
            _t1 = t1;
            _t2 = t2;
            _t3 = t3;

            //    Matrix = _t3.Matrix * (_t2.Matrix * (_t1.Matrix * _t0.Matrix));
        }

        public override string ToString()
        {
            return $"{_t0} {_t1} {_t2} {_t3}";
        }

        public PxMatrix Resolve(IElement element, IFrameContext context)
        {
            return _t3.Resolve(element, context) *
                   (_t2.Resolve(element, context) *
                    (_t1.Resolve(element, context) *
                     _t0.Resolve(element, context)));
//        return PxMatrix.Skew(X.Resolve(element, context), Y.Resolve(element, context));
        }

    }
}