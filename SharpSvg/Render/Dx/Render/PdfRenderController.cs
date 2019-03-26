using Peracto.Svg.Types;
using System;
using System.Collections.Generic;
using System.IO;
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

    public async System.Threading.Tasks.Task Render(IEnumerable<string> docNames, Stream outStream)
    {
      var ticket = Printing.Printing.CreateTicketForPrinter("Microsoft Print to PDF");

      var pms = ticket.PageMediaSize;

      var pageSize = new DX.Size2F(
        (pms.Width != null) ? (float)pms.Width : 4962,
        (pms.Height != null) ? (float)pms.Height : 7014
      );

      using (var target = Printing.Printing.CreatePrintTarget(ticket, "Microsoft Print to PDF", outStream))
      using (var printControl = new D2D1.PrintControl(D2DDevice, WicFactory, target))
      using (var dc = new D2D1.DeviceContext(D2DDevice, D2D1.DeviceContextOptions.None))
      {
        dc.DotsPerInch = new DX.Size2F(600, 600);

        foreach (var fn in docNames)
        {
          var document = await Loader.Load(new Uri(fn, UriKind.RelativeOrAbsolute));

          var children = document.Children.Count;

          if (children == 0)
            continue;

          var svg = document.RootElement;
          if (svg == null)
            continue;

          using (var renderer = new RendererDirect2D(this, dc))
          using (var commandList = new D2D1.CommandList(dc))
          {
            dc.Target = commandList;
            dc.BeginDraw();

            var rootFrame = FrameContext.CreateRoot(pageSize.Width, pageSize.Height);
            var frameSize = svg.GetSize(rootFrame, new PxSize(pageSize.Width, pageSize.Height));

            var r = RenderRegistry.Get(svg.ElementType);
            if (r != null)
              await r(svg, rootFrame.Create(frameSize), renderer);

            dc.EndDraw();
            commandList.Close();
            printControl.AddPage(commandList, pageSize);
          }
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
