using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharpDX;
using SharpDX.Direct2D1;
using DW = SharpDX.DirectWrite;

namespace Peracto.Svg.Render.Dx.Font
{
  public class TextSource : SharpDX.ComObject, DW.TextAnalysisSource
  {
    private readonly string _str;
    private readonly DW.Factory _factory;

    public TextSource(DW.Factory factory, string str)
    {
      _str = str;
      _factory = factory;
    }

    public string GetTextAtPosition(int textPosition)
    {
      return _str.Substring(textPosition);
    }

    public string GetTextBeforePosition(int textPosition)
    {
      return _str.Substring(0, textPosition - 1);
    }

    public string GetLocaleName(int textPosition, out int textLength)
    {
      var tl = System.Threading.Thread.CurrentThread.CurrentCulture.Name;
      textLength = tl.Length;
      return tl;
    }

    public DW.NumberSubstitution GetNumberSubstitution(int textPosition, out int textLength)
    {
      textLength = 0; //_str.Length;
      return new DW.NumberSubstitution(_factory, DW.NumberSubstitutionMethod.None, null, true);
    }

    public DW.ReadingDirection ReadingDirection => DW.ReadingDirection.RightToLeft;
  }
}



