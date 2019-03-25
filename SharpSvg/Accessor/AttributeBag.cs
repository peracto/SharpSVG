using System.Collections.Generic;

namespace Peracto.Svg.Accessor
{
  public class AttributeBag : IAttributeBag
  {
    private readonly IList<IElementAttribute> _attributes;

    public AttributeBag()
    {
      _attributes = new List<IElementAttribute>();
    }

    public void Add(IEnumerable<IElementAttribute> list)
    {
      foreach (var l in list)
        Set(l);
    }

    public void Set(IElementAttribute value)
    {
      for (var i = 0; i < _attributes.Count; i++)
      {
        if (_attributes[i].Name != value.Name) continue;
        _attributes[i] = value;
        return;
      }

      _attributes.Add(value);
    }

    public T GetValue<T>(string name)
    {
      return TryGetValue<T>(name, out var value) ? value : default(T);
    }

    public bool TryGetValue<T>(string name, out T value)
    {
      if (_attributes != null)
      {
        foreach (var a in _attributes)
        {
          if (a.Name != name) continue;
          value = (T) a.Value;
          return true;
        }
      }

      value = default(T);
      return false;
    }
  }
}