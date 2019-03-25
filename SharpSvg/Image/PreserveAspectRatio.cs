using System;
using Peracto.Svg.Transform;
using Peracto.Svg.Types;

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

    private float CalcOffset2(PerspectiveRatioAlign a, float rh, float ih)
    {
      switch (a)
      {
        case PerspectiveRatioAlign.Min: return 0;
        case PerspectiveRatioAlign.Mid: return -((ih / 2) - (rh / 2));
        case PerspectiveRatioAlign.Max: return rh - ih;
        default: return 0;
      }
    }


    public PxMatrix CalcMatrix(PxSize viewPort, PxRectangle imageSize)
    {
      switch (Option)
      {
        case PreserveAspectRatioOption.None:
        {
          return PxMatrix.Translation(
            viewPort.Height / imageSize.Height,
            viewPort.Width / imageSize.Width,
            0,
            0
          );
          }
        case PreserveAspectRatioOption.Meet:
        {
          var scale = Math.Min(
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
            imageSize.X + Math.Max(CalcOffset(AlignX, viewPort.Width, scaledW), 0),// / scaleX,
            imageSize.Y + Math.Max(CalcOffset(AlignY, viewPort.Height, scaledH), 0)// / scaleY
          );
        }
        case PreserveAspectRatioOption.Slice:
        {
          var scale = viewPort.Width / imageSize.Width;
          var scaledH = imageSize.Height * scale;

          return PxMatrix.Translation(
            0,
            scaledH / imageSize.Height,
            0,
            CalcOffset2(AlignY, viewPort.Height, scaledH)
          );
        }
        default:
          throw new ArgumentOutOfRangeException();
      }
    }

    public PxRectangle ApplyAspectRatio(PxRectangle viewPort, PxSize imageSize)
    {
      switch (Option)
      {
        case PreserveAspectRatioOption.None:
        {
          return viewPort;
        }
        case PreserveAspectRatioOption.Meet:
        {
          var scale = Math.Min(
            viewPort.Height / imageSize.Height,
            viewPort.Width / imageSize.Width
          );

          var scaledW = imageSize.Width * scale;
          var scaledH = imageSize.Height * scale;

          return new PxRectangle(
            viewPort.X + Math.Max(CalcOffset(AlignX, viewPort.Width, scaledW), 0),
            viewPort.Y + Math.Max(CalcOffset(AlignY, viewPort.Height, scaledH), 0),
            scaledW,
            scaledH
          );
        }
        case PreserveAspectRatioOption.Slice:
        {
          var scale = viewPort.Width / imageSize.Width;
          var scaledH = imageSize.Height * scale;

          return new PxRectangle(
            viewPort.X,
            viewPort.Y + CalcOffset2(AlignY, viewPort.Height, scaledH),
            viewPort.Width,
            scaledH
          );
        }
        default:
          throw new ArgumentOutOfRangeException();
      }
    }
  }
}