using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Peracto.Svg.Types;
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
      var svg = document.Children.FirstOrDefault(e => e.ElementType == "svg");
      if (svg == null) throw new Exception("No svg element");


      var xx = svg.TryGetViewBox(out var vb) ? vb.Size : new PxSize(1024, 1024);
      var state = FrameContext.CreateRoot(xx.Width,xx.Height);
      var size = svg.GetSize(state, xx);

      using (var renderer = Create(size, out var bm))
      {
        var dc = renderer.Target;
        dc.BeginDraw();
        var r = RenderRegistry.Get("svg");
          if(r!=null) await r(svg, FrameContext.CreateRoot(dc.Size.Width, dc.Size.Height), renderer);
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
        (int)size.Width,
        (int)size.Height,
        WIC.PixelFormat.Format32bppPBGRA,
        WIC.BitmapCreateCacheOption.CacheOnLoad
      );
      wicBitmap.SetResolution(600, 600);

      var renderProps = new D2D1.RenderTargetProperties(
        D2D1.RenderTargetType.Default,
        new D2D1.PixelFormat(
          DXGI.Format.B8G8R8A8_UNorm,
          D2D1.AlphaMode.Premultiplied
        ),
        600,
        600,
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