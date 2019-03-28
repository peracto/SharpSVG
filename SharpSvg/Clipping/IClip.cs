using System.Collections.Generic;
using Peracto.Svg.Brush;

namespace Peracto.Svg.Clipping
{
  public interface IClip
  {
    IEnumerable<IElement> Children { get; }
    string Tag { get; }
    ClipPathUnits ClipPathUnits { get; }
  }
}