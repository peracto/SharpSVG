using System;
using System.Collections.Generic;
using System.Linq;
using Fizzler;

namespace Peracto.Svg.Css
{
  internal class ElementOps : IElementOps<IElement>
  {
    private static readonly Selector<IElement> EmptySelector = (nodes => Enumerable.Empty<IElement>());

    public Selector<IElement> Type(NamespacePrefix prefix, string name)
    {
      return nodes => nodes.Where(n => n.ElementType == name);
    }

    public Selector<IElement> Universal(NamespacePrefix prefix)
    {
      return nodes => nodes;
    }

    public Selector<IElement> Id(string id)
    {
      return nodes => nodes.Where(n => n.Id == id);
    }

    public Selector<IElement> Class(string clazz)
    {
      return AttributeIncludes(NamespacePrefix.None, "class", clazz);
    }

    public Selector<IElement> AttributeExists(NamespacePrefix prefix, string name)
    {
      return nodes => nodes.Where(n => n.Attributes.TryGetOriginal(name, out var dummy));
    }

    public Selector<IElement> AttributeExact(NamespacePrefix prefix, string name, string value)
    {
      return nodes => nodes.Where(n => (n.Attributes.TryGetOriginal(name, out var val) && val == value));
    }

    public Selector<IElement> AttributeIncludes(NamespacePrefix prefix, string name, string value)
    {
      return nodes =>
        nodes.Where(n => (n.Attributes.TryGetOriginal(name, out var val) && val.Split(' ').Contains(value)));
    }

    public Selector<IElement> AttributeDashMatch(NamespacePrefix prefix, string name, string value)
    {
      return string.IsNullOrEmpty(value)
        ? EmptySelector
        : (nodes => nodes.Where(n =>
          (n.Attributes.TryGetOriginal(name, out var val) && val.Split('-').Contains(value))));
    }

    public Selector<IElement> AttributePrefixMatch(NamespacePrefix prefix, string name, string value)
    {
      return string.IsNullOrEmpty(value)
        ? EmptySelector
        : (nodes => nodes.Where(n => (n.Attributes.TryGetOriginal(name, out var val) && val.StartsWith(value))));
    }

    public Selector<IElement> AttributeSuffixMatch(NamespacePrefix prefix, string name, string value)
    {
      return string.IsNullOrEmpty(value)
        ? EmptySelector
        : (nodes => nodes.Where(n => (n.Attributes.TryGetOriginal(name, out var val) && val.EndsWith(value))));
    }

    public Selector<IElement> AttributeSubstring(NamespacePrefix prefix, string name, string value)
    {
      return string.IsNullOrEmpty(value)
        ? EmptySelector
        : (nodes => nodes.Where(n => (n.Attributes.TryGetOriginal(name, out var val) && val.Contains(value))));
    }

    public Selector<IElement> FirstChild()
    {
      return nodes => nodes.Where(n => n.Parent == null || n.Parent.Children.First() == n);
    }

    public Selector<IElement> LastChild()
    {
      return nodes => nodes.Where(n => n.Parent == null || n.Parent.Children.Last() == n);
    }

    private IEnumerable<T> GetByIds<T>(IList<T> items, IEnumerable<int> indices)
    {
      return from i in indices where i >= 0 && i < items.Count select items[i];
    }

    public Selector<IElement> NthChild(int a, int b)
    {
      return nodes => nodes.Where(n => n.Parent != null && GetByIds(n.Parent.Children,
                                           (from i in Enumerable.Range(0, n.Parent.Children.Count / a)
                                             select a * i + b))
                                         .Contains(n));
    }

    public Selector<IElement> OnlyChild()
    {
      return nodes => nodes.Where(n => n.Parent == null || n.Parent.Children.Count == 1);
    }

    public Selector<IElement> Empty()
    {
      return nodes => nodes.Where(n => n.Children.Count == 0);
    }

    public Selector<IElement> Child()
    {
      return nodes => nodes.SelectMany(n => n.Children);
    }

    public Selector<IElement> Descendant()
    {
      return nodes => nodes.SelectMany(n => n.Descendants());
    }

    public Selector<IElement> Adjacent()
    {
      return nodes => nodes.SelectMany(n => ElementsAfterSelf(n).Take(1));
    }

    public Selector<IElement> GeneralSibling()
    {
      return nodes => nodes.SelectMany(ElementsAfterSelf);
    }

    private IEnumerable<IElement> ElementsAfterSelf(IElement self)
    {
      return (self.Parent == null
        ? Enumerable.Empty<IElement>()
        : self.Parent.Children.Skip(self.Parent.Children.IndexOf(self) + 1));
    }

    public Selector<IElement> NthLastChild(int a, int b)
    {
      throw new NotImplementedException();
    }
  }
}
