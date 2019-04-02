using Peracto.Svg.Types;
using System;
// ReSharper disable SwitchStatementMissingSomeCases

namespace Peracto.Svg.Image
{
  public class PreserveAspectRatio
  {
    public static readonly PreserveAspectRatio XMidYMidMeet =
      new PreserveAspectRatio(PreserveAspectRatioType.XMidYMid, PreserveAspectRatioOption.Meet);

    public PreserveAspectRatio(PreserveAspectRatioType align, PreserveAspectRatioOption option)
    {
      Align = align;
      Option = align == PreserveAspectRatioType.None ? PreserveAspectRatioOption.None : option;
    }

    public PerspectiveRatioAlign AlignX => (PerspectiveRatioAlign) ((((int) Align) & 0xF0) >> 4);
    public PerspectiveRatioAlign AlignY => (PerspectiveRatioAlign) ((((int) Align) & 0x0F));
    public PreserveAspectRatioType Align { get; }
    public PreserveAspectRatioOption Option { get; }

    public override string ToString()
    {
      return
        Option == PreserveAspectRatioOption.Unknown
          ? ""
          : Option == PreserveAspectRatioOption.None
            ? $"{Align}"
            : $"{Align} {Option}";
    }


    private float CalcOffset(PerspectiveRatioAlign a, float rh, float ih)
    {
      switch (a)
      {
        case PerspectiveRatioAlign.Min: return 0;
        case PerspectiveRatioAlign.Mid: return (rh - ih) / 2;
        case PerspectiveRatioAlign.Max: return rh - ih;
        default: return 0;
      }
    }

    public PxMatrix CalcMatrix(PxSize viewPort, PxRectangle imageSize)
    {
      if (Option == PreserveAspectRatioOption.None)
      {
        return PxMatrix.Translation(
          viewPort.Width / imageSize.Width,
          viewPort.Height / imageSize.Height,
          0,
          0
        );
      }

      var scale = Option == PreserveAspectRatioOption.Meet
        ? Math.Min(
          viewPort.Height / imageSize.Height,
          viewPort.Width / imageSize.Width
        )
        : Math.Max(
          viewPort.Height / imageSize.Height,
          viewPort.Width / imageSize.Width
        );

      var scaledW = imageSize.Width * scale;
      var scaledH = imageSize.Height * scale;

      var scaleX = scaledW / imageSize.Width;
      var scaleY = scaledH / imageSize.Height;

      return PxMatrix.Translation(
        scaleX,
        scaleY,
        imageSize.X + CalcOffset(AlignX, viewPort.Width, scaledW),  // / scaleX,
        imageSize.Y + CalcOffset(AlignY, viewPort.Height, scaledH) // / scaleY
      );
    }
  }
}