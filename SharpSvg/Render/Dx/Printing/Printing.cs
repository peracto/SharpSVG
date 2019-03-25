using System.IO;
using System.Linq;
using System.Printing;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Peracto.Svg.Render.Dx.IO;
using DX = SharpDX;

namespace Peracto.Svg.Render.Dx.Printing
{
  public static class Printing
  {
    public static DX.ComObject CreatePrintTarget(PrintTicket ticket, string printerName, Stream outputStream)
    {
      var bin = new Bingo();
      // ReSharper disable once SuspiciousTypeConversion.Global
      var bon = (Bongo) bin;
      return new DX.ComObject(bon.CreateDocumentPackageTargetForPrintJob(
        printerName,
        "",
        new ManagedIStream(outputStream),
        new ManagedIStream(ticket.GetXmlStream())
      ));
    }

    public static PrintTicket CreateTicketForPrinter(string printerName)
    {
      using (var localPrintServer = new LocalPrintServer())
      {
        return localPrintServer
          .GetPrintQueues()
          .Where(lp => lp.Name == printerName)
          .Select(lp => lp.DefaultPrintTicket)
          .FirstOrDefault();
      }
    }

    [Guid("348EF17D-6C81-4982-92B4-EE188A43867A")]
    //    [TypeLibType(TypeLibTypeFlags.FCanCreate)]
    [ComImport]
    private class Bingo
    {
    }

    [ComSourceInterfaces("PrintDocumentTargetLib.IPrintDocumentPackageStatusEvent")]
    [ClassInterface(ClassInterfaceType.None)]
    [Guid("4842669E-9947-46EA-8BA2-D8CCE432C2CA")]
    [ComImport]
    private class PrintDocumentPackageTargetClass
    {
    }

    [CoClass(typeof(PrintDocumentPackageTargetClass))]
    [Guid("1B8EFEC4-3019-4C27-964E-367202156906")]
    [ComImport]
    private interface PrintDocumentPackageTarget
    {
    }

    [InterfaceType(ComInterfaceType.InterfaceIsIUnknown)]
    [Guid("D2959BF7-B31B-4A3D-9600-712EB1335BA4")]
    [ComImport]
    private interface Bongo
    {
      [MethodImpl(MethodImplOptions.InternalCall, MethodCodeType = MethodCodeType.Runtime)]
      PrintDocumentPackageTarget CreateDocumentPackageTargetForPrintJob(
        [MarshalAs(UnmanagedType.LPWStr), In] string printerName,
        [MarshalAs(UnmanagedType.LPWStr), In] string jobName,
        //[MarshalAs(UnmanagedType.IUnknown), In]
        System.Runtime.InteropServices.ComTypes.IStream jobOutputStream,
        //[MarshalAs(UnmanagedType.IUnknown), In]
        System.Runtime.InteropServices.ComTypes.IStream jobPrintTicketStream
      );
    }
  }
}
