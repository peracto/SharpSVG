using System.Runtime.InteropServices;
using Peracto.Svg.Types;

namespace Peracto.Svg.Transform
{
    public class RotateTransform : ITransform
    {
        public TransformType TransformType => TransformType.Rotate;

        private readonly Measure X;
        private readonly Measure Y;
        public Angle Angle;
        private readonly float _radians;
        public bool HasCentre { get; }

        public RotateTransform(Angle angle)
        {
            Angle = angle;
            HasCentre = false;
            _radians = angle.ToRadians();
        }

        public RotateTransform(Angle angle, Measure x, Measure y)
        {
            Angle = angle;
            X = x;
            Y = y;
            _radians = angle.ToRadians();
            HasCentre = true;
        }

        public override string ToString()
        {
            return HasCentre
                ? $"rotate({Angle},{X},{Y})"
                : $"rotate({Angle})";
        }

        public PxMatrix Resolve(IElement element, IFrameContext context)
        {
            if (!HasCentre) return PxMatrix.Rotate(_radians);
            var x = X.Resolve(element, context);
            var y = Y.Resolve(element, context);
            return
                PxMatrix.Translate(-x, -y) *
                PxMatrix.Rotate(_radians) *
                PxMatrix.Translate(x, y);
        }
    }
}