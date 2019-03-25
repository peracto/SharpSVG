using System;

namespace Peracto.Svg
{
  public interface IDocument : IElement
  {
    Uri BaseUri { get; }
    IElement RootElement { get; }
    object GetResource(string name);
    void SetResource(string name, object value);
  }
}
