using System.Threading.Tasks;

namespace Peracto.Svg.Types
{
  public delegate Task RenderDelegate<in TRenderer>(IElement element, IFrameContext context, TRenderer render)
    where TRenderer : class;
}
