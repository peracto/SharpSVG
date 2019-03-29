﻿using System.Threading.Tasks;
using Peracto.Svg.Render.Dx.Render;
using Peracto.Svg.Render.Dx.Utility;

namespace Peracto.Svg.Render.Dx.Elements
{
  public static class BarcodeRender 
  {
    public static Task Render(IElement element, IFrameContext context, RendererDirect2D render)
    {
      using (LayerHelper.Create(render.Target, render.FontManager, element, context, false))
      {
        render.DrawBarcode(
          element,
          context,
          element.GetBounds(context),
          element.GetFill(context),
          element.GetStroke(context),
          element.Attributes.GetValue<string>("value"));
        return Task.CompletedTask;
      }
    }
  }
}