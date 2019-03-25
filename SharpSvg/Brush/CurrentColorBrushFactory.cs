using Peracto.Svg.Accessor;

namespace Peracto.Svg.Brush
{
  public class CurrentColorBrushFactory : IBrushFactory
  {
    private static IBrushFactory _instance;

    public static IBrushFactory Instance => _instance ?? (_instance = new CurrentColorBrushFactory());

    private CurrentColorBrushFactory()
    {
    }

    IBrush IBrushFactory.Create(IElement context)
    {
      while(context!=null)
      {
        if (AttributeAccessors.Color.TryGetValue(context, out var brush))
          return brush.Create(context);
        context = context.Parent;
      }
      return AttributeAccessors.Color.DefaultValue.Create(context);
    }
  }
}