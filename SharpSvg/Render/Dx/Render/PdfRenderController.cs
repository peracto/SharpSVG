using Peracto.Svg.Types;
using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using D2D1 = SharpDX.Direct2D1;
using D3D = SharpDX.Direct3D;
using D3D11 = SharpDX.Direct3D11;
using DX = SharpDX;
using DXGI = SharpDX.DXGI;

namespace Peracto.Svg.Render.Dx.Render
{
  public class PdfRenderController : RenderControllerBase
  {
    private D3D11.Device D3DDevice { get; }
    private DXGI.Device DxgiDevice { get; }
    private D2D1.Device D2DDevice { get; }

    private const D3D11.DeviceCreationFlags Flags = D3D11.DeviceCreationFlags.Debug
                                                    | D3D11.DeviceCreationFlags.BgraSupport
                                                    | D3D11.DeviceCreationFlags.SingleThreaded;

    public PdfRenderController(ILoader loader) : base(loader)
    {
      D3DDevice = new D3D11.Device(D3D.DriverType.Hardware, Flags);
      DxgiDevice = D3DDevice.QueryInterface<DXGI.Device>();
      D2DDevice = new D2D1.Device(DxgiDevice); // initialize the D2D device
    }

    public async Task Render(IEnumerable<string> docNames, Stream outStream)
    {
      var ticket = Printing.Printing.CreateTicketForPrinter("Microsoft Print to PDF");

      var pms = ticket.PageMediaSize;

      var pageSize = new PxSize(
        (pms.Width != null) ? (float)pms.Width : 4962,
        (pms.Height != null) ? (float)pms.Height : 7014
      );

      using (var target = Printing.Printing.CreatePrintTarget(ticket, "Microsoft Print to PDF", outStream))
      using (var printControl = new D2D1.PrintControl(D2DDevice, WicFactory, target))
      using (var dc = new D2D1.DeviceContext(D2DDevice, D2D1.DeviceContextOptions.None))
      {
        dc.DotsPerInch = new DX.Size2F(600, 600);

        var pageStack = new List<RendererDirect2D> ();
        var pageY = 0;
        var frameContext = FrameContext.CreateRoot(pageSize);
        D2D1.CommandList commandList = null;

        foreach (var fn in docNames)
        {
          try
          {
            var document = await Loader.Load(new Uri(fn, UriKind.RelativeOrAbsolute));

            var children = document.Children.Count;
            if (children == 0) continue;
            var svg = document.RootElement;
            if (svg == null) continue;

            var size = svg.GetSize(frameContext, pageSize);
            var viewPort = svg.GetViewBox()?.AsRectangle() ?? size.AsRectangle();

            if (pageStack.Count > 0 && pageY+ ((int)viewPort.Height+0.5f) > pms.Height)
            {
              dc.EndDraw();
              commandList.Close();
              printControl.AddPage(commandList, new DX.Size2F(pageSize.Width, pageSize.Height));
              commandList.Dispose();
              foreach (var r in pageStack) r.Dispose();
              pageStack.Clear();
              pageY = 0;
              commandList = null;
            }

            dc.Transform = DX.Matrix3x2.Translation(0,pageY);

            if (commandList == null)
            {
              commandList = new D2D1.CommandList(dc);
              dc.Target = commandList;
              dc.BeginDraw();
            }

            var render = new RendererDirect2D(this, dc);
            await render.GetRenderer(svg.ElementType)(svg, FrameContext.CreateRoot(pageSize), render);
            pageStack.Add(render);

            pageY += (int)(viewPort.Height+0.5f);
          }
          catch (Exception ex)
          {
            Console.WriteLine("Skipping {0}", ex);
            //throw ex;
          }
        }

        if (pageStack.Count > 0)
        {
          dc.EndDraw();
          commandList.Close();
          printControl.AddPage(commandList, new DX.Size2F(pageSize.Width, pageSize.Height));
          commandList.Dispose();
          foreach (var r in pageStack) r.Dispose();
          pageStack.Clear();
        }

        // Send the job to the printing subsystem and discard
        // printing-specific resources.
        printControl.Close();
      }
    }
    
    public override void Dispose()
    {
      D3DDevice.Dispose();
      DxgiDevice.Dispose();
      D2DDevice.Dispose();
      base.Dispose();
    }
  }
}
