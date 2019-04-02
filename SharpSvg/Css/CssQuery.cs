using ExCSS;
using Fizzler;
using System.Collections.Generic;
using System.Linq;

namespace Peracto.Svg.Css
{
  internal static class CssQuery
  {

    public static IEnumerable<IElement> QuerySelectorAll(this IElement elem, string selector)
    {
      var generator = new SelectorGenerator<IElement>(new ElementOps());
      Fizzler.Parser.Parse(selector, generator);
      return generator.Selector(Enumerable.Repeat(elem, 1));
    }

    private static int GetSimpleSpecificity(string selector)
    {
      while (!string.IsNullOrWhiteSpace(selector))
      {
        switch (selector[0])
        {
          case '#': // ID selector
            return 1 << 12;
          case ':':
            if (!selector.StartsWith(":not("))
              return (selector.StartsWith("::") || selector == ":after" || selector == ":before" ||
                      selector == ":first-letter" || selector == ":first-line" || selector == ":selection")
                ? 1 << 4 // pseudo-element
                : 1 << 8; // pseudo-class;
            selector = selector.Substring(5, selector.Length - 6).Trim();
            continue;
          case '[': //attribute
            return 1 << 8;
          case '.': // class
            return 1 << 8;
          case '*':
            return selector == "*" ? 0 : 1 << 4;
          default: // element selector
            return 1 << 4;
        }
      }
      return 0;
    }

    public static int GetSpecificity(this BaseSelector selector)
    {
      switch (selector)
      {
        case SimpleSelector _:
          return GetSimpleSpecificity(selector.ToString().ToLowerInvariant());
        case IEnumerable<BaseSelector> list:
          return (from s in list select GetSpecificity(s)).Aggregate((p, c) => p + c);
        case IEnumerable<CombinatorSelector> complex:
          return (from s in complex select GetSpecificity(s.Selector)).Aggregate((p, c) => p + c);
        default:
          return 0;
      }
    }
  }
}
