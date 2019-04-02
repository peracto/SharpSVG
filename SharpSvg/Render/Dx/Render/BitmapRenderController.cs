using Peracto.Svg.Types;
using SharpDX.Mathematics.Interop;
using System;
using System.IO;
using System.Threading.Tasks;
using D2D1 = SharpDX.Direct2D1;
using DXGI = SharpDX.DXGI;
using WIC = SharpDX.WIC;

namespace Peracto.Svg.Render.Dx.Render
{
  public class BitmapRenderController : RenderControllerBase
  {
    public BitmapRenderController(ILoader loader) : base(loader)
    {
    }

    public async Task Render(IDocument document, Stream stream)
    {
      var children = document.Children.Count;
      if (children == 0) return;
      var svg = document.RootElement;
      if (svg == null) throw new Exception("No svg element");

      var pageSize = new PxSize(1024, 1024);
      var context = FrameContext.CreateRoot(pageSize);
      var size = svg.GetSize(context, pageSize);
      var viewPort = svg.GetViewBox()?.AsRectangle() ?? size.AsRectangle();
      var newContext = context.Create(viewPort.Size);

      using (var render = Create(viewPort.Size, out var bm))
      {
        var dc = render.Target;
        dc.BeginDraw();
        dc.Clear(new RawColor4(1f, 1f, 1f, 1f));
        await render.GetRenderer("svg")(svg, newContext, render);
        dc.EndDraw();

        if (stream != null)
          Save(bm, stream);
      }
    }

    private WIC.Bitmap _bitmap;

    public override void Dispose()
    {
      base.Dispose();
      _bitmap?.Dispose();
    }

    private RendererDirect2D Create(PxSize size, out WIC.Bitmap wicBitmap)
    {
      _bitmap = wicBitmap = new WIC.Bitmap(
        WicFactory,
        (int)(size.Width*1.0f),
        (int)(size.Height*1.0f),
        WIC.PixelFormat.Format32bppPBGRA,
        WIC.BitmapCreateCacheOption.CacheOnLoad
      );
      wicBitmap.SetResolution(200, 200);

      var renderProps = new D2D1.RenderTargetProperties(
        D2D1.RenderTargetType.Default,
        new D2D1.PixelFormat(
          DXGI.Format.B8G8R8A8_UNorm,
          D2D1.AlphaMode.Premultiplied
        ),
        96,
        96,
        D2D1.RenderTargetUsage.None, //GdiCompatible| D2D1.RenderTargetUsage.ForceBitmapRemoting,
        D2D1.FeatureLevel.Level_DEFAULT
      );

      var wicRenderTarget = new D2D1.WicRenderTarget(
        D2DFactory,
        wicBitmap,
        renderProps
      ); // {DotsPerInch = new Size2F(600, 600)};



      return new RendererDirect2D(this, wicRenderTarget);
    }
    public void Save(WIC.Bitmap wicBitmap,Stream stream)
    {
      using (var encoder = new WIC.BitmapEncoder(WicFactory, WIC.ContainerFormatGuids.Jpeg, stream))
      using (var frame = new WIC.BitmapFrameEncode(encoder))
      {
        frame.Initialize();
        var pfx = WIC.PixelFormat.Format128bppRGBAFloat; //Format64bppRGBA; //FormatDontCare;

        
        frame.SetResolution(600,600);
        frame.SetPixelFormat(ref pfx);
        frame.WriteSource(wicBitmap);
        frame.Commit();
        encoder.Commit();
      }
    }
  }
}