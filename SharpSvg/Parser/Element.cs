using Peracto.Svg.Accessor;
using System.Collections.Generic;

namespace Peracto.Svg.Parser
{
  public class Element : IElement
  {
    public static readonly IList<IElement> EmptyChildren = new List<IElement>().AsReadOnly();
    private IList<IElement> _children;
    private IElement _parent;
    private IElement _parentOverride;
    public IElementDefinition Definition { get; }

    public IAttributeBag Attributes { get; }

    public Element()
    {
      Attributes = new AttributeBag();
    }

    public Element(IElementDefinition definition) : this()
    {
      Definition = definition;
    }

    public Element(IElementDefinition definition, IAttributeBag attributes)
    {
      Attributes = attributes;
      Definition = definition;
    }

    void IElement.SetParentOverride(IElement parent)
    {
      _parentOverride = parent;
    }

    public string ElementType => Definition.ElementType;

    public string Id => Attributes.GetValue<string>("id");

    public void AddChild(IElement element)
    {
      if (_children != null && _children.IndexOf(element) >= 0) return;
      if (_children == null) _children = new List<IElement>();
      _children.Add(element);
      element.Parent = this;
    }

    public void RemoveChild(IElement element)
    {
      _children?.Remove(element);
    }

    public IList<IElement> Children => _children ?? EmptyChildren;

    public IElement Parent
    {
      get => _parentOverride??_parent;
      set
      {
        if (_parent == value) return;
        _parent?.RemoveChild(this);
        value?.AddChild(this);
        _parent = value;
      }
    }
   
    public string Content { get; set; }

    public virtual IDocument OwnerDocument => _parent.OwnerDocument;

    public bool IsRootElement => _parent == null || _parent.ElementType == "#document";

    public override string ToString()
    {
      return $"Element({ElementType})";
    }
  }
}