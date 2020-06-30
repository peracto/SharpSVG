using System;
using System.Collections.Generic;

namespace Peracto.Svg.Converters
{
  public class XStyleAttributeConverter : AttributeConverterBase<object>
  {
    public XStyleAttributeConverter(string name) : base(name)
    {
    }

    protected override bool TryCreate(string value, out object x)
    {
      //var rc = new ExCSS.StylesheetParser().Parse("#a{" + value + "}");
      var cssParser = new ExCSS.Parser();
      var sheet = cssParser.Parse("#dummyxx{" + value + "}");

      var list = new List<KeyValuePair<string, string>>();
      foreach (var rule in sheet.StyleRules)
      foreach (var decl in rule.Declarations)
      {
          Console.WriteLine($"Style: Name:{decl.Name} Term={decl.Term}");
          list.Add(new KeyValuePair<string, string>(decl.Name, decl.Term.ToString()));
      }

      x = list;
      return sheet != null;
    }
  }
}