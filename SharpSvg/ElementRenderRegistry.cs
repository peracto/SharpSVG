using System.Collections.Generic;
using System.Threading.Tasks;
using Peracto.Svg.Types;

namespace Peracto.Svg
{
  public class ElementRenderRegistry<TRenderer> where TRenderer : class
  {
    private readonly IDictionary<string, RenderDelegate<TRenderer>> _dict =
      new Dictionary<string, RenderDelegate<TRenderer>>();

    public void Add(string name, RenderDelegate<TRenderer> fn)
    {
      _dict[name] = fn;
    }

    public RenderDelegate<TRenderer> Get(string name)
    {
      return _dict.TryGetValue(name, out var renderer) ? renderer : DummyRender;
    }

    private static readonly RenderDelegate<TRenderer> DummyRender = DummyRenderImpl;

    private static Task DummyRenderImpl(IElement element, IFrameContext context, TRenderer render)
    {
      return Task.CompletedTask;
    }
  }
}
