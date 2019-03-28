using System.Collections.Generic;
using System.Linq;
using Peracto.Svg.Brush;

namespace Peracto.Svg.Clipping
{
  public class Clip : IClip
  {
    public IEnumerable<IElement> Children { get; }
    public string Tag { get; }
    public ClipPathUnits ClipPathUnits { get; }

    public Clip(IEnumerable<IElement> children, string tag, ClipPathUnits clipPathUnits)
    {
      Tag = tag;
      Children = children.ToArray();
      ClipPathUnits = clipPathUnits;
    }
  }
}