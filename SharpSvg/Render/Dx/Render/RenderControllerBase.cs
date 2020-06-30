using Peracto.Svg.Render.Dx.Elements;
using Peracto.Svg.Render.Dx.Font;
using Peracto.Svg.Render.Dx.IO;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Peracto.Svg.Types;
using SharpDX;
using D2D1 = SharpDX.Direct2D1;
using DW = SharpDX.DirectWrite;
using WIC = SharpDX.WIC;

namespace Peracto.Svg.Render.Dx.Render
{
  public abstract class RenderControllerBase : IDisposable
  {
    static readonly HttpClient Client = new HttpClient();

    private static readonly Regex sWhitespace = new Regex(@"\s+");
    public static string ReplaceWhitespace(string input, string replacement) { return sWhitespace.Replace(input, replacement); }

        public ElementRenderRegistry<RendererDirect2D> RenderRegistry { get; } =
      new ElementRenderRegistry<RendererDirect2D>();

    public D2D1.Factory D2DFactory { get; }
    public WIC.ImagingFactory2 WicFactory { get; }
    public DW.Factory DwFactory { get; }
    public FontManager FontManager { get; }

    protected ILoader Loader { get; }

    public async Task<RenderStream> LoadToStream(Uri href, Uri baseUri, Func<Stream, D2D1.Bitmap> bitmapFactory)
    {
        switch (href.Scheme)
        {
            case "internal":
                return new RenderStream(href.Fragment.Substring(1));
            case "file":
                var fn = Uri.UnescapeDataString(href.AbsolutePath);

                if (System.IO.Path.GetExtension(fn) == ".svg")
                    return new RenderStream(await Loader.Load(href, baseUri));
                Console.WriteLine($"Opening image file {fn}");
                using (var fs = File.OpenRead(fn))
                    return new RenderStream(bitmapFactory(fs));
            case "bigdata":
                var data2 = href.ToString();
                var i2 = data2.IndexOf(",", StringComparison.Ordinal);
                var bytes2 = Convert.FromBase64String(data2.Substring(i2+ 1));
                using (var ms = new MemoryStream(bytes2))
                    return new RenderStream(bitmapFactory(ms));
            case "data":
                var data = href.OriginalString;
                var i = data.IndexOf(",", StringComparison.Ordinal);
                var bytes = Convert.FromBase64String(data.Substring(i + 1));
                using (var ms = new MemoryStream(bytes))
                    return new RenderStream(bitmapFactory(ms));
            default:
                var response = await Client.GetAsync(href);
                if (!response.IsSuccessStatusCode) return null;
                using (var stream3 = await response.Content.ReadAsStreamAsync())
                    return new RenderStream(bitmapFactory(stream3));
        }
    }

    private Uri GetFilename(Uri uri)
    {
      var path = uri.AbsolutePath;
      var extension = Uri.UnescapeDataString(System.IO.Path.GetExtension(path)).Split('|');

      if (extension.Length <= 1)
        return File.Exists(path) ? uri : null;

      var folder = System.IO.Path.GetDirectoryName(path);
      var filename = System.IO.Path.GetFileNameWithoutExtension(path);

      return (
        from e in extension
        select System.IO.Path.Combine(folder, filename + "." + e)
        into n
        where File.Exists(n)
        select new Uri(n)
      ).FirstOrDefault();
    }

    protected RenderControllerBase(ILoader loader)
    {
      Loader = loader;
      RenderRegistry.Add("circle", CircleRender.Render);
      RenderRegistry.Add("ellipse",EllipseRender.Render);
      RenderRegistry.Add("line", LineRender.Render);
      RenderRegistry.Add("path", PathRender.Render);
      RenderRegistry.Add("polygon", PolygonRender.Render);
      RenderRegistry.Add("svg",SvgRender.Render);
      RenderRegistry.Add("use",UseRender.Render);
      RenderRegistry.Add("symbol",SymbolRender.Render);
      RenderRegistry.Add("g", GRender.Render);
      RenderRegistry.Add("text", TextRender.Render);
      RenderRegistry.Add("barcode", BarcodeRender.Render);
      RenderRegistry.Add("polyline", PolyLineRender.Render);
      RenderRegistry.Add("image", ImageRender.Render);
      RenderRegistry.Add("rect", RectangleRender.Render);
      RenderRegistry.Add("px-text", PxTextRender.Render);

      D2DFactory = new D2D1.Factory(D2D1.FactoryType.SingleThreaded);
      WicFactory = new WIC.ImagingFactory2();
      DwFactory = new DW.Factory();
      FontManager = new FontManager(DwFactory);
    }

    public virtual void Dispose()
    {
      D2DFactory?.Dispose();
      WicFactory?.Dispose();
      DwFactory?.Dispose();
    }
  }
}
