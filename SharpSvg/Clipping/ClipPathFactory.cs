using System;
using System.Linq;

namespace Peracto.Svg.Clipping
{
  public class ClipPathFactory : IClipPathFactory
  {
    private readonly Uri _uri;

    public ClipPathFactory(Uri uri)
    {
      _uri = uri;
    }

    IClip IClipPathFactory.Create(IElement context)
    {
      var tag = _uri.Fragment.Substring(1);

      if (context.OwnerDocument.GetResource("CLIPPATH::" + tag) is IClip clip)
        return clip;

      var element = context.GetElementById(tag);
      if (element == null) return null;

      clip = CreateNew(element, tag);
      context.OwnerDocument.SetResource("CLIPPATH::" + tag, clip);
      return clip;
    }

    private IClip CreateNew(IElement element, string tag)
    {
      var children = element.Children.ToList();
      return new Clip(children, tag, element.GetClipPathUnits());
    }
  }
}