using Peracto.Svg.Types;

namespace Peracto.Svg.Transform
{
    public class MatrixTransform : ITransform
    {
        public TransformType TransformType => TransformType.Matrix;
      
        public float M11 { get; }
        public float M12 { get; }
        public float M21 { get; }
        public float M22 { get; }
        public float M31 { get; }
        public float M32 { get; }

        public PxMatrix Matrix { get; }

        public MatrixTransform(float m11, float m12, float m21, float m22, float m31, float m32)
        {
            M11 = m11;
            M12 = m12;
            M21 = m21;
            M22 = m22;
            M31 = m31;
            M32 = m32;
            Matrix = new PxMatrix(m11, m12, m21, m22, m31, m32);
        }

        public override string ToString()
        {
            return $"matrix({M11},{M12},{M21},{M22},{M31},{M32})";
        }
    }

   
}