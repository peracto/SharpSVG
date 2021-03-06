﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Peracto.Svg;
using Peracto.Svg.Render.Dx.Render;

namespace SharpSvg.Test
{
  public static class Tests
  {
    public static async Task Process(string testsLocation, string pdfName)
    {
      try
      {
        var loader = Loader.Create(new Uri(testsLocation));

        var render = new PdfRenderController(loader);
        using (var stream = File.Create(pdfName))
          await render.Render(GetFiles(testsLocation), stream);
       // await GetFiles(testsLocation).TestAll(loader);
      }
      catch (Exception ex)
      {   
        throw ex;
      }

    }

    private static IEnumerable<string> GetFiles(string testsLocation)
    {
      /*
            foreach(var x in 
              Directory
                .EnumerateDirectories(@"D:\AwsDocuments\Process", "0148b671-ae6f-400f-824d-b690e935026b.aws")
                .SelectMany(folder => Directory.EnumerateFiles(folder, "output_scrib.1.svg")
                )
              )
      */
      /*foreach (var x in
          Directory
            .EnumerateDirectories(@"D:\AwsDocuments\Process", "*.aws").Take(11)
            .SelectMany(folder => Directory.EnumerateFiles(folder, "output_scrib.1.svg")
            )
        )*/
       foreach (var x in Directory.EnumerateFiles(Path.Combine(testsLocation, "svg", "text"), "*.svg"))
      {
        Console.WriteLine($"Processing {x}");
        yield return x;
      }
    }


    private static async Task TestAll(this IEnumerable<string> files, ILoader loader)
    {
      foreach (var svg in files)
      {
        Console.WriteLine($"Testing {svg}");
        var path = Path.GetDirectoryName(Path.GetFullPath(svg));
        var uriBase = new Uri(path);
        var render = new BitmapRenderController(loader);
        using (var stream = File.OpenRead(svg))
        using (var outStream = File.OpenWrite(svg + ".jpg"))
        {
          try
          {
            var doc = await loader.Load(stream,uriBase);
            try
            {
              await render.Render(doc, outStream);
            }
            catch (Exception ex)
            {
              Console.WriteLine(ex);
            }

          }
          catch (Exception ex)
          {
            Console.WriteLine(ex);
          }
        }
      }
    }
  }
}
