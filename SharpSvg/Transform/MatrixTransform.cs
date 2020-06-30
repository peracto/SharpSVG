using Peracto.Svg.Types;

namespace Peracto.Svg.Transform
{
    public class MatrixTransform : ITransform
    {
        public TransformType TransformType => TransformType.Matrix;

        public Measure M11 { get; }
        public Measure M12 { get; }
        public Measure M21 { get; }
        public Measure M22 { get; }
        public Measure M31 { get; }
        public Measure M32 { get; }

//        public PxMatrix Matrix { get; }

        public MatrixTransform(Measure m11, Measure m12, Measure m21, Measure m22, Measure m31, Measure m32)
        {
            M11 = m11;
            M12 = m12;
            M21 = m21;
            M22 = m22;
            M31 = m31;
            M32 = m32;
            //    Matrix = new PxMatrix(m11, m12, m21, m22, m31, m32);
        }

        public override string ToString()
        {
            return $"matrix({M11},{M12},{M21},{M22},{M31},{M32})";
        }

        public PxMatrix Resolve(IElement element, IFrameContext context)
        {
            return new PxMatrix(
                M11.Resolve(element, context),
                M12.Resolve(element, context),
                M21.Resolve(element, context),
                M22.Resolve(element, context),
                M31.Resolve(element, context),
                M32.Resolve(element, context)
            );
        }
    }
}