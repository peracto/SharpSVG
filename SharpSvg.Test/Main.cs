using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SharpSvg.Test
{
  class MainX
  {
    public static async Task Main()
    {
      var fullPath = Path.GetDirectoryName(typeof(MainX).Assembly.Location);
      var path = Path.GetFullPath(Path.Combine(fullPath, "..", "..", "W3CTestSuite"));
      //get the folder that's in
      await Tests.Process(path, @"d:\temp\bingo9.pdf");
    }
  }
}
