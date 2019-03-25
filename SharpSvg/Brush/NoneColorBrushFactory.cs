namespace Peracto.Svg.Brush
{
  public class NoneColorBrushFactory : IBrushFactory
  {
    private static IBrushFactory _instance;
    public static IBrushFactory Instance => _instance ?? (_instance = new NoneColorBrushFactory());

    private NoneColorBrushFactory()
    {
    }

    IBrush IBrushFactory.Create(IElement context)
    {
      return null;
    }
  }
}