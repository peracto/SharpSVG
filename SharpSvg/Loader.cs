using ExCSS;
using Peracto.Svg.Accessor;
using Peracto.Svg.Css;
using Peracto.Svg.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;

namespace Peracto.Svg
{
  public class Loader : ILoader
  {
    private ElementFactory ElementFactory { get; }
    public AttributeFactory AttributeFactory { get; }
    public Uri DefaultBaseUri { get; }

    public static ILoader Create(Uri defaultBaseUri = null)
    {
      return new Loader(defaultBaseUri ?? new Uri(Directory.GetCurrentDirectory(), UriKind.RelativeOrAbsolute));
    }

    private Loader(Uri defaultBaseUri)
    {
      DefaultBaseUri = defaultBaseUri;
      ElementFactory = new ElementFactory();
      AttributeFactory = new AttributeFactory();
    }

    private IEnumerable<IElementAttribute> GetAttributes(XmlReader reader)
    {
      if (!reader.HasAttributes) yield break;
      while (reader.MoveToNextAttribute())
      {
        var newName = TransformName(reader.NamespaceURI, reader.LocalName);
        if (newName == null) continue;
        yield return new ElementAttribute(newName, reader.HasValue ? reader.Value : string.Empty);
      }
    }

    public bool IsValidNamespace(string namespaceUri)
    {
      return (namespaceUri == "" || 
              namespaceUri == "http://www.w3.org/2000/svg" ||
              namespaceUri == "http://www.w3.org/1999/xlink");
    }

    public virtual string TransformName(string namespaceUri, string localName)
    {
      return IsValidNamespace(namespaceUri)
          ? localName
          : null;
    }

    public async Task<IDocument> Load(Uri uri, Uri baseUri = null)
    {
      var sourceUri = uri.IsAbsoluteUri ? uri : new Uri(uri, baseUri ?? DefaultBaseUri);

      if (sourceUri.Scheme != "file")
        throw new Exception($"Don't know how to load {uri}");

      using (var stream = File.OpenRead(sourceUri.AbsolutePath))
        return await Load(stream, sourceUri);
    }

    public async Task<IDocument> Load(Stream stream, Uri baseUri = null)
    {
      var settings = new XmlReaderSettings
      {
        DtdProcessing = DtdProcessing.Parse,
        IgnoreComments = true,
  //      IgnoreProcessingInstructions = true,
        IgnoreWhitespace = true,
        Async = true
      };

      using (var reader = XmlReader.Create(stream, settings))
        return ApplyStyles(await Parse(reader, baseUri));
    }


    private class StyleHelper
    {
      private readonly IDictionary<string,(IElementAttribute attr,int specificity)> _styles = new Dictionary<string, (IElementAttribute Attr, int specificity)>();

      public void Add(IElementAttribute attr, int specificity)
      {
        if (!_styles.TryGetValue(attr.Name, out var rules))
          _styles.Add(attr.Name, (attr, specificity));
        else if (specificity >= rules.specificity)
          _styles[attr.Name] = (attr, specificity);
      }

      public IEnumerable<IElementAttribute> GetFinalStyles()
      {
        return _styles.Select(s => s.Value.attr);
      }
    }

    private static IEnumerable<(StyleRule Rule, BaseSelector Selector)> GetSelectors(ExCSS.Parser cssParser,IDocument document)
    {
      foreach (var style in document.RootElement.Descendants("style"))
      {
        var sheet = cssParser.Parse(style.Content);

        foreach (var rule in sheet.StyleRules)
        {
          if (rule.Selector is AggregateSelectorList list && list.Delimiter == ",")
          {
            foreach (var v in list)
              yield return (Rule: rule, Selector: v);
          }
          else
          {
            yield return (Rule: rule, Selector: rule.Selector);
          }
        }
      }
    }

    private static IEnumerable<IElementAttribute> GetStyles(ExCSS.Parser cssParser, string value)
    {
      var sheet = cssParser.Parse("#dummyxx{" + value + "}");
      foreach (var rule in sheet.StyleRules)
      foreach (var decl in rule.Declarations)
        yield return new ElementAttribute(decl.Name, decl.Term.ToString());
    }

    private IDocument ApplyStyles(IDocument document)
    {
      var elementDict = new Dictionary<IElement, StyleHelper>();
      var cssParser = new ExCSS.Parser();

      foreach (var element in document.Descendants())
      {
        if(element.Attributes.Count==0) continue;
        var sh = new StyleHelper();
        elementDict.Add(element, sh);
        foreach (var attribute in element.Attributes)
        {
          sh.Add(attribute,0);
          if (attribute.Name != "style") continue;
          foreach(var kvp in GetStyles(cssParser, attribute.Value as string))
            sh.Add(kvp,1>>16);
        }
      }

      foreach (var (rule, selector) in GetSelectors(cssParser, document))
      {
        foreach (var elem in document.QuerySelectorAll(selector.ToString()))
        {
          if (!elementDict.TryGetValue(elem, out var styleHelper))
          {
            styleHelper = new StyleHelper();
            elementDict.Add(elem, styleHelper);
          }

          foreach (var decl in rule.Declarations)
          {
            styleHelper.Add(new ElementAttribute(decl.Name, decl.Term.ToString()), selector.GetSpecificity());
          }
        }
      }

      foreach (var elem in elementDict)
      {
        elem.Key.Attributes.Replace(
          from kvp in elem.Value.GetFinalStyles()
          let attr = AttributeFactory.Create(elem.Key.ElementType, kvp.Name, (kvp.Value as string)?.Trim())
          where attr != null
          select attr
        );
      }

      return document;
    }


    private async Task<Document> Parse(XmlReader reader, Uri baseUri)
    {
      var stack = new Stack<IElement>();
      var doc = new Document(baseUri);

      IElement current = doc;

      while (await reader.ReadAsync())
      {
        // ReSharper disable once SwitchStatementMissingSomeCases
        switch (reader.NodeType)
        {
          case XmlNodeType.Element:
            var isEmptyElement = reader.IsEmptyElement;
            var elementType = reader.Name;
            var element = ElementFactory.Create(elementType);
            element.Attributes.Add(GetAttributes(reader));
            current.AddChild(element);

            if (!isEmptyElement)
            {
              stack.Push(current);
              current = element;
            }

            break;
          case XmlNodeType.EndElement:
            current = stack.Pop();
            break;
          case XmlNodeType.Text:
          case XmlNodeType.CDATA:
            switch (current.Definition.ElementContentType)
            {
              case ElementContentType.Embedded:
                current.Content = await reader.GetValueAsync();
                break;
              case ElementContentType.None:
                break;
              case ElementContentType.Element:
                current.AddChild(TextElement.Create(await reader.GetValueAsync()));
                break;
              default:
                throw new ArgumentOutOfRangeException();
            }

            break;
        }
      }

      return doc;
    }
  }
}