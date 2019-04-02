using System.Collections.Concurrent;
using System.Collections.Generic;
using Peracto.Svg.Types;

namespace Peracto.Svg.Brush
{
  public class SolidColourBrushFactory : IBrushFactory
  {
    public static readonly IBrushFactory Black = new SolidColourBrushFactory(PxColors.Black);
    private static readonly IDictionary<PxColor, IBrushFactory> Colors =
      new ConcurrentDictionary<PxColor, IBrushFactory>();

    public static IBrushFactory GetColor(PxColor color)
    {
      if (Colors.TryGetValue(color, out var outVal)) return outVal;
      outVal = new SolidColourBrushFactory(color);
      Colors.Add(color, outVal);
      return outVal;
    }

    private readonly IBrush _brush;

    private SolidColourBrushFactory(PxColor color)
    {
      _brush = new SolidColorBrush(color);
    }

    IBrush IBrushFactory.Create(IElement context)
    {
      return _brush;
    }
  }
}