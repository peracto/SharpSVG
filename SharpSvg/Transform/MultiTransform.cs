using System.Linq;
using Peracto.Svg.Types;

namespace Peracto.Svg.Transform
{
    public class MultiTransform : ITransform
    {
        public TransformType TransformType => TransformType.Multi;

        private ITransform[] _t;

        public PxMatrix Matrix { get; }

        public MultiTransform(ITransform[] transforms)
        {
            _t = transforms;
            Matrix = transforms.Aggregate(PxMatrix.Identity, (current, m) => m.Matrix * current);
        }
        public override string ToString()
        {
            return string.Join(",",_t.Select((v) => v.ToString()));
        }

    }
}