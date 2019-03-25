using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Peracto.Svg.Utility;

namespace Peracto.Svg.Converters
{
  public class GenericEnumConverter<T> : AttributeConverterBase<T> where T : struct, IComparable
  {
    private static IDictionary<string, T> Dict { get; }

    static GenericEnumConverter()
    {
      Dict = (from x in typeof(T).GetFields(BindingFlags.Public | BindingFlags.Static)
          let a = x.GetCustomAttribute<ExternalNameAttribute>()
          select new KeyValuePair<string, T>(a == null ? x.Name : a.Name, (T) x.GetValue(null))
        ).ToDictionary((k) => k.Key.ToLower(), (v) => v.Value);
    }

    public GenericEnumConverter(string name) : base(name)
    {
    }

    protected override bool TryCreate(string attributeValue, IElement elementFactory, out T rc)
    {
      return Dict.TryGetValue(attributeValue.ToLower(), out rc);
    }
  }
}