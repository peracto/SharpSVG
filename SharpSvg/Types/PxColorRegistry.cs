using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Peracto.Svg.Types
{
  public static class PxColorRegistry
  {
    private static IDictionary<string, PxColor> Dict { get; }

    static PxColorRegistry()
    {
      Dict = (
        from property in typeof(PxColors).GetFields(BindingFlags.Static | BindingFlags.Public)
        where property.FieldType == typeof(PxColor)
        select property
      ).ToDictionary(k => k.Name.ToLower(), v => (PxColor) v.GetValue(null));
    }

    public static bool TryGetColor(string name, out PxColor value)
    {
      return Dict.TryGetValue(name.ToLower().Trim(), out value);
    }
  }
}