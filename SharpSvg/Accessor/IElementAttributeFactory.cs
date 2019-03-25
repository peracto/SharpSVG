namespace Peracto.Svg.Accessor
{
    public interface IElementAttributeFactory
    {
        string AttributeName { get; }
        IElementAttribute Create(string attributeValue, IElement elementFactory);
    }
}