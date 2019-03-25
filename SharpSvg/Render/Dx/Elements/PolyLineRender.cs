﻿using Peracto.Svg.Render.Dx.Render;
using Peracto.Svg.Render.Dx.Utility;

namespace Peracto.Svg.Render.Dx.Elements
{
  public static class PolyLineRender 
  {
    public static async System.Threading.Tasks.Task Render(IElement element, IFrameContext context, RendererDirect2D render)
    {
      using (LayerHelper.Create(render.Target, element, context, false))
      {
        render.DrawPolyline(
          element,
          context,
          element.GetPoints(),
          element.GetFill(context),
          element.GetStroke(context)
        );
      }
    }
  }
}