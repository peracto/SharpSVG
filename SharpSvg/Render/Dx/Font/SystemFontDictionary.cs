using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DW = SharpDX.DirectWrite;

namespace Peracto.Svg.Render.Dx.Font
{
  public class SystemFontDictionary
  {
    private readonly IDictionary<string, KeyValuePair<string, DW.FontCollection>> _fontDict;
    //private readonly FontCollection _fontCollection;
    private readonly DW.FontCollection _systemFontCollection;

    public SystemFontDictionary(DW.Factory factory)
    {
      _systemFontCollection = factory.GetSystemFontCollection(false);
      _fontDict = GetFontFamilies(_systemFontCollection).ToDictionary(k => k.Key.ToLower(), v => v);
    }

    private static IEnumerable<KeyValuePair<string, DW.FontCollection>> GetFontFamilies(DW.FontCollection fontCollection)
    {
      var familyCount = fontCollection.FontFamilyCount;

      for (var i = 0; i < familyCount; i++)
      {
        var fontFamily = fontCollection.GetFontFamily(i);
        var familyNames = fontFamily.FamilyNames;
        if (!familyNames.FindLocaleName(CultureInfo.CurrentCulture.Name, out var index))
          familyNames.FindLocaleName("en-us", out index);
        yield return new KeyValuePair<string, DW.FontCollection>(familyNames.GetString(index), fontCollection);
      }
    }

    public string SelectFontFamilyName(IEnumerable<string> fontFamilyList, out DW.FontCollection fontCollection)
    {
      if (fontFamilyList != null)
      {
        foreach (var f in fontFamilyList)
        {
          if (_fontDict.TryGetValue(f.ToLower(), out var ff))
          {
            fontCollection = ff.Value;
            return ff.Key;
          }
        }
      }
      System.Console.WriteLine("Could not load fonts ... {0}", string.Join(",", fontFamilyList));
      fontCollection = _systemFontCollection;
      return "Arial";
    }
  }
}