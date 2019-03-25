using System;
using Peracto.Svg.Types;

namespace Peracto.Svg.Render.Dx.Core
{
  public class FrameContext : IFrameContext
  {
    const float Dpi = 96f;

    public static IFrameContext CreateRoot(float width, float height)
    {
      return new FrameContext(width, height,1);
    }

    private FrameContext(float width, float height, int layerId)
    {
      Size = new PxSize(width, height);
      LayerId = layerId;
    }

    public IFrameContext Create(PxSize size)
    {
      return new FrameContext(size.Width, size.Height,LayerId+1);
    }

    public PxSize Size { get; }
    public int LayerId { get; }

    public float ToDeviceValue(Measure unit)
    {
      switch (unit.Unit)
      {
        case MeasureUnit.Em:
        case MeasureUnit.Ex:
          throw new Exception("Cannot resolve em/ex here");
        case MeasureUnit.Centimeter:
          return (unit.Value / 2.54f) * Dpi;
        case MeasureUnit.Inch:
          return unit.Value * Dpi;
        case MeasureUnit.Millimeter:
          return ((unit.Value / 10) / 2.54f) * Dpi;
        case MeasureUnit.Pica:
          return ((unit.Value * 12) / 72);
        case MeasureUnit.Point:
          return (unit.Value / 72) * Dpi;
        case MeasureUnit.Pixel:
        case MeasureUnit.User:
          return unit.Value;
        case MeasureUnit.Percentage:
          // ReSharper disable once SwitchStatementMissingSomeCases
          switch (unit.Usage)
          {
            case MeasureUsage.Horizontal:
              return (Size.Width / 100) * unit.Value;
            case MeasureUsage.Vertical:
              return (Size.Height / 100) * unit.Value;
            default:
              return 0.0f;
          }
        case MeasureUnit.None:
        case MeasureUnit.Unknown:
          return 0.0f;
        default:
          return unit.Value;
      }
    }
  }
}