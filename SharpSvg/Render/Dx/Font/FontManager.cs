using System;
using Peracto.Svg.Text;
using DW = SharpDX.DirectWrite;

namespace Peracto.Svg.Render.Dx.Font
{
  public class FontManager
  {
    private readonly SystemFontDictionary _fontDictionary;
    private readonly DW.Factory _factory;
    public FontManager(DW.Factory factory)
    {
      _fontDictionary = new SystemFontDictionary(factory);
      _factory = factory;
    }

    public DW.FontFace GetFontFace(Text.Font font)
    {
      var fontFamily = _fontDictionary.GetFontFamily(font.Family);

      var xFont = fontFamily.GetFirstMatchingFont(
        GetFontWeight(font.Weight),
        GetFontStretch(font.Stretch),
        GetFontStyle(font.Style)
      );
      return new DW.FontFace(xFont);
    }

    public DW.TextFormat GetTextFormat(Text.Font font)
    {
      var fontName = _fontDictionary.SelectFontFamilyName(font.Family, out var fontCollection);

      return new DW.TextFormat(
        _factory,
        fontName,
        fontCollection,
        GetFontWeight(font.Weight),
        GetFontStyle(font.Style),
        GetFontStretch(font.Stretch),
        font.Size
      );
    }

    public DW.TextLayout GetTextLayout(Text.Font font, string text, float maxWidth)
    {
      var tl = new DW.TextLayout(
        _factory,
        text,
        GetTextFormat(font),
        maxWidth,
        9999999
        );

      // ReSharper disable once SwitchStatementMissingSomeCases
      switch (font.TextDecoration)
      {
        case TextDecoration.LineThrough:
          tl.SetStrikethrough(true, new DW.TextRange(0, text.Length));
          break;
        case TextDecoration.Underline:
          tl.SetUnderline(true, new DW.TextRange(0, text.Length));
          break;
      }

      return tl;
    }

    private static DW.FontStyle GetFontStyle(FontStyle value)
    {
      // ReSharper disable once SwitchStatementMissingSomeCases
      switch (value)
      {
        case FontStyle.Normal:
          return DW.FontStyle.Normal;
        case FontStyle.Italic:
        case FontStyle.Oblique:
          return DW.FontStyle.Oblique;
        default:
          return DW.FontStyle.Normal;
      }
    }

    private static DW.FontStretch GetFontStretch(FontStretch value)
    {
      switch (value)
      {
        case FontStretch.Normal: return DW.FontStretch.Normal;
        case FontStretch.Wider: return DW.FontStretch.Medium; //TODO: Implement this
        case FontStretch.Narrower: return DW.FontStretch.Medium;//TODO: Implement this
        case FontStretch.Condensed: return DW.FontStretch.Condensed;
        case FontStretch.Expanded: return DW.FontStretch.Expanded;
        case FontStretch.UltraCondensed: return DW.FontStretch.UltraCondensed;
        case FontStretch.ExtraCondensed: return DW.FontStretch.ExtraCondensed;
        case FontStretch.SemiCondensed: return DW.FontStretch.SemiCondensed;
        case FontStretch.SemiExpanded: return DW.FontStretch.UltraExpanded;
        case FontStretch.ExtraExpanded: return DW.FontStretch.ExtraExpanded;
        case FontStretch.UltraExpanded: return DW.FontStretch.UltraExpanded;
        default: return DW.FontStretch.Normal;
      }
    }

    private static DW.FontWeight GetFontWeight(FontWeight value)
    {
      switch (value)
      {
        case FontWeight.Normal: return DW.FontWeight.Normal;
        case FontWeight.Bold: return DW.FontWeight.Bold;
        case FontWeight.Bolder: return DW.FontWeight.Bold;
        case FontWeight.Lighter: return DW.FontWeight.DemiBold;
        case FontWeight.W100: return DW.FontWeight.Thin;
        case FontWeight.W200: return DW.FontWeight.ExtraLight;
        case FontWeight.W300: return DW.FontWeight.Light;
        case FontWeight.W400: return DW.FontWeight.Normal;
        case FontWeight.W500: return DW.FontWeight.Medium;
        case FontWeight.W600: return DW.FontWeight.DemiBold;
        case FontWeight.W700: return DW.FontWeight.Bold;
        case FontWeight.W800: return DW.FontWeight.ExtraBold;
        case FontWeight.W900: return DW.FontWeight.Heavy;
        case FontWeight.Inherit: throw new NotImplementedException();
        default:
          return DW.FontWeight.Normal;
      }
    }
  }
}