using Peracto.Svg.Accessor;

namespace Peracto.Svg.Parser
{
  public class TextElement : Element, ITextContent
  {
    private static readonly IElementDefinition XDefinition =
      new ElementDefinition("#text");

    private static readonly IAttributeBag EmptyAttributes = new AttributeBag();

    public static IElement Create(string content)
    {
      return new TextElement(content);
    }

    private TextElement(string content) : base(XDefinition, EmptyAttributes)
    {
      Content = content;
    }
  }
}