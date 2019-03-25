namespace Peracto.Svg.Accessor
{
    public interface IElementDefinition
    {
        string ElementType { get; }

        /// <summary>
        /// Describes how text is injected into the element
        /// </summary>
        ElementContentType ElementContentType { get; }
    }
}