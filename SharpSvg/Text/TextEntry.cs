using Peracto.Svg.Brush;

namespace Peracto.Svg.Text
{
  public class TextEntry
  {
    public Font Font { get; }
    public Fill Fill { get; }
    public float? X { get; }
    public float? Y { get; }
    public float Dx { get; }
    public float Dy { get; }
    public string Content { get; }

    public TextEntryType TextEntryType {get;}

    public TextEntry(Font font, Fill fill, float? x, float? y, float dx, float dy)
    {
      Font = font;
      Fill = fill;
      X = x;
      Y = y;
      Dx = dx;
      Dy = dy;
      TextEntryType = TextEntryType.Cursor;
    }
    public TextEntry(Font font, Fill fill, string content)
    {
      Font = font;
      Fill = fill;
      Content = content;
      TextEntryType = TextEntryType.Text;
    }
  }
}