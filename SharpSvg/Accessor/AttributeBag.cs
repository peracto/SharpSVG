using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Peracto.Svg.Accessor
{
  public class AttributeBag : IAttributeBag
  {
    private IList<IElementAttribute> _attributes;

    public AttributeBag()
    {
      _attributes = new List<IElementAttribute>();
    }

    public void Add(IEnumerable<IElementAttribute> list)
    {
      foreach (var l in list)
        Set(l);
    }

    public void Replace(IEnumerable<IElementAttribute> list)
    {
      _attributes = list.ToList();
    }

    public int Count => _attributes.Count;

    public void Set(IElementAttribute value)
    {
      for (var i = 0; i < _attributes.Count; i++)
      {
        if(_attributes[i]==null || value==null) 
          Debugger.Break();
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

    public bool TryGetOriginal(string name, out string value)
    {
      if (_attributes != null)
      {
        foreach (var a in _attributes)
        {
          if (a.Name != name) continue;
          value = a.Value as string;
          return true;
        }
      }

      value = string.Empty;
      return false;
    }

    public bool Contains(string name)
    {
      if (_attributes == null) return false;
      foreach (var a in _attributes)
        if (a.Name == name) return true;

      return false;
    }

    public IEnumerator<IElementAttribute> GetEnumerator()
    {
      return _attributes.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
      return _attributes.GetEnumerator();
    }
  }
}