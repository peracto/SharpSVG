﻿using System.Threading.Tasks;
using Peracto.Svg.Render.Dx.Render;
using Peracto.Svg.Render.Dx.Utility;

namespace Peracto.Svg.Render.Dx.Elements
{
  public static class CircleRender 
  {
    public static Task Render(IElement element, IFrameContext context, RendererDirect2D render)
    {
      using (TransformHelper.Create(render.Target, element, context, false))
      using (LayerHelper.Create(render.Target, render.FontManager, element, context))
      {
        render.DrawCircle(
          element,
          context,
          element.GetCxCy(context),
          element.GetRadius(context),
          element.GetFill(context),
          element.GetStroke(context)
        );
        return Task.CompletedTask;

      }
    }
  }
}
