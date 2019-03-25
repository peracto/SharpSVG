using System.Collections.Generic;
using System.Linq;

namespace Peracto.Svg.Clipping
{
  public class Clip : IClip
  {
    public IEnumerable<IElement> Children { get; }
    public string Tag { get; }

    public Clip(IEnumerable<IElement> children, string tag)
    {
      Tag = tag;
      Children = children.ToArray();
    }
  }
}