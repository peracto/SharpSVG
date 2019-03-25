namespace Peracto.Svg.Accessor
{
    public interface IElementFactory
    {
        string ElementType { get; }
        IElement Create();
    }
}