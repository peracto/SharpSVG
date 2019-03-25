using System.Collections.Generic;
using Peracto.Svg.Accessor;

namespace Peracto.Svg
{
  public interface IElement
  {
    IElementDefinition Definition { get; }
    IAttributeBag Attributes { get; }
    void AddChild(IElement element);
    void RemoveChild(IElement element);
    IElement Parent { get; set; }
    IDocument OwnerDocument { get; }
    string Content { get; set; }
    IList<IElement> Children { get; }
    string ElementType { get; }
    string Id { get; }
    bool IsRootElement { get; }
    void SetParentOverride(IElement parent);
  }
}