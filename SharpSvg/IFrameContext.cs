using Peracto.Svg.Types;

namespace Peracto.Svg
{
  public interface IFrameContext
  {
    float ToDeviceValue(Measure unit);
    IFrameContext Create(PxSize size);
    PxSize Size { get; }
    int LayerId { get; }
  }
}
