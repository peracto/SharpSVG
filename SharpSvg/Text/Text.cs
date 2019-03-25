using System.Collections.Generic;
using Peracto.Svg.Brush;
using Peracto.Svg.Types;

namespace Peracto.Svg.Text
{
  public class Text
  {
    public PxRectangle Bounds { get; }
    public IEnumerable<TextEntry> TextEntries { get; }

    public Text(IEnumerable<TextEntry> textEntries, PxRectangle bounds)
    {
      Bounds = bounds;
      TextEntries = textEntries;
    }

    public static IEnumerable<TextEntry> Create(IElement element, IFrameContext context)
    {
      var stack = new Stack<(IEnumerator<IElement> e,Font font,Fill fill)>();
      var enumerator = element.Children.GetEnumerator();

      var font = element.GetFont(context);
      var fill = element.GetFill(context);

      yield return new TextEntry(
        font,
        fill,
        element.GetX(context),
        element.GetY(context),
        element.GetDx(context),
        element.GetDy(context)
      );

      while (true)
      {
        while (enumerator.MoveNext())
        {
          var e = enumerator.Current;
          if (e == null) continue;

          if (e is ITextContent text)
          {
            yield return new TextEntry(font, fill, text.Content);
          }
          else
          {
            font = e.GetFont(context,font);
            fill = e.GetFill(context);

            yield return new TextEntry(
              font,
              fill,
              e.GetX(context),
              e.GetY(context),
              e.GetDx(context),
              e.GetDy(context)
            );

            var c = e.Children;
            if (c.Count == 0) continue;
            stack.Push((enumerator, font, fill));
            enumerator = c.GetEnumerator();
          }
        }

        if (stack.Count == 0) break;
        (enumerator, font, fill) = stack.Pop();
      }
    }
  }
}