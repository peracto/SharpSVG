using System.Collections.Generic;

namespace Peracto.Svg.Clipping
{
  public interface IClip
  {
    IEnumerable<IElement> Children { get; }
    string Tag { get; }
  }
}