namespace Peracto.Svg.Text
{
  public class Font
  {
    public readonly string[] Family;
    public readonly float Size;
    public readonly FontWeight Weight;
    //public readonly TextAnchor TextAnchor;
    public readonly TextDecoration TextDecoration;
  //  public readonly DominantBaseline DominantBaseline;
    public readonly FontStyle Style;
    public readonly FontStretch Stretch;

    public Font(
        string[] family,
        float size,
        FontWeight weight,
        //TextAnchor textAnchor,
        TextDecoration textDecoration,
  //      DominantBaseline dominantBaseline,
        FontStretch fontStretch,
        FontStyle style
      )
    {
      Family = family;
      Size = size;
      Weight = weight;
    //  TextAnchor = textAnchor;
      TextDecoration = textDecoration;
   //   DominantBaseline = dominantBaseline;
      Stretch = fontStretch;
      Style = style;
    }
  }
}