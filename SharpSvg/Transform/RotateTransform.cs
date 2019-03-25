using Peracto.Svg.Types;

namespace Peracto.Svg.Transform
{
    public class RotateTransform : ITransform
    {
        public TransformType TransformType => TransformType.Rotate;

        public readonly Angle Angle;
        public readonly PxPoint Point;
        public bool HasCentre { get;  }

        public PxMatrix Matrix { get; }

        public RotateTransform(Angle angle)
        {
            Angle = angle;
            Point = new PxPoint(0, 0);
            HasCentre = false;
            Matrix = PxMatrix.Rotate(angle.ToRadians());
        }

        public RotateTransform(Angle angle, float x, float y)
        {
            Angle = angle;
            Point = new PxPoint(x, y);
            HasCentre = true;
            Matrix =
                PxMatrix.Translate(-x, -y) *
                PxMatrix.Rotate(angle.ToRadians()) *
                PxMatrix.Translate(x, y);
        }

        public override string ToString()
        {
            return HasCentre 
                ? $"rotate({Angle},{Point.X},{Point.Y})" 
                : $"rotate({Angle})";
        }

    }
}