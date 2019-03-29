using System.Collections.Generic;

namespace Peracto.Svg.Accessor
{
  public interface IAttributeBag : IEnumerable<IElementAttribute>
  {
    T GetValue<T>(string attributeName);
    void Add(IEnumerable<IElementAttribute> attributes);
    bool TryGetValue<T>(string attributeName, out T outValue);
    bool TryGetOriginal(string attributeName, out string outValue);
    bool Contains(string attributeName);
    int Count { get; }
  }
}