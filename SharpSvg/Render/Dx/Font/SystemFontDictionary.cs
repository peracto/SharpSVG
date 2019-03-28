using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using DW = SharpDX.DirectWrite;

namespace Peracto.Svg.Render.Dx.Font
{
  public class SystemFontDictionary
  {
    private readonly IDictionary<string, (string Name, DW.FontCollection Collection,DW.FontFamily Family)> _fontDict;
    //private readonly FontCollection _fontCollection;
    private readonly DW.FontCollection _systemFontCollection;

    public SystemFontDictionary(DW.Factory factory)
    {
      _systemFontCollection = factory.GetSystemFontCollection(false);
      _fontDict = GetFontFamilies(_systemFontCollection).ToDictionary(k => k.Name.ToLower(), v => v);
    }

    private static IEnumerable<(string Name, DW.FontCollection Collection, DW.FontFamily Family)> GetFontFamilies(DW.FontCollection fontCollection)
    {
      var familyCount = fontCollection.FontFamilyCount;


      for (var i = 0; i < familyCount; i++)
      {
        var fontFamily = fontCollection.GetFontFamily(i);
        //fontFamily.GetFirstMatchingFont()

        var familyNames = fontFamily.FamilyNames;
        if (!familyNames.FindLocaleName(CultureInfo.CurrentCulture.Name, out var index))
          familyNames.FindLocaleName("en-us", out index);

        yield return (Name: familyNames.GetString(index), Collection: fontCollection, Family: fontFamily);
      }
    }

    public DW.FontFamily GetFontFamily(IEnumerable<string> fontFamilyList)
    {
      if (fontFamilyList != null)
      {
        foreach (var f in fontFamilyList)
        {
          if (_fontDict.TryGetValue(f.ToLower(), out var ff))
            return ff.Family;
        }
      }

      return _fontDict.TryGetValue("calibri", out var ff2)
        ? ff2.Family
        : null;
    }

    public string SelectFontFamilyName(IEnumerable<string> fontFamilyList, out DW.FontCollection fontCollection)
    {
      if (fontFamilyList != null)
      {
        foreach (var f in fontFamilyList)
        {
          if (_fontDict.TryGetValue(f.ToLower(), out var ff))
          {
            fontCollection = ff.Collection;
            return ff.Name;
          }
        }
      }
      //System.Console.WriteLine("Could not load fonts ... {0}", string.Join(",", fontFamilyList));
      fontCollection = _systemFontCollection;
      return "Calibri";
    }
  }
}