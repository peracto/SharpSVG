﻿using Peracto.Svg.Accessor;
using Peracto.Svg.Parser;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Xml;
using ExCSS;

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

    private IEnumerable<KeyValuePair<string, string>> GetAttributes(XmlReader reader)
    {
      if (!reader.HasAttributes) yield break;
      while (reader.MoveToNextAttribute())
      {
        var newName = TransformName(reader.NamespaceURI, reader.LocalName);
        if (newName == null) continue;
        yield return new KeyValuePair<string, string>(newName, reader.HasValue ? reader.Value : null);
      }
    }

    public virtual string TransformName(string namespaceUri, string localName)
    {
      return
        (namespaceUri == "" || namespaceUri == "http://www.w3.org/2000/svg" ||
         namespaceUri == "http://www.w3.org/1999/xlink")
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
        DtdProcessing = DtdProcessing.Ignore,
        IgnoreComments = true,
        IgnoreProcessingInstructions = true,
        IgnoreWhitespace = true,
        Async = true
      };

      using (var reader = XmlReader.Create(stream, settings))
        return await Parse(reader, baseUri);
    }

    private void ApplyStyles(IDocument document)
    {
      var styles = document.Descendants("style").ToList();
      if (styles.Count == 0) return;

      var cssTotal = styles.Select((s) => s.Content).Aggregate((p, c) => p + Environment.NewLine + c);
      var cssParser = new ExCSS.Parser();
      var sheet = cssParser.Parse(cssTotal);
      AggregateSelectorList aggList;
      IEnumerable<BaseSelector> selectors;
      IEnumerable<IElement> elemsToStyle;

      foreach (var rule in sheet.StyleRules)
      {
        aggList = rule.Selector as AggregateSelectorList;
        if (aggList != null && aggList.Delimiter == ",")
        {
          selectors = aggList;
        }
        else
        {
          selectors = Enumerable.Repeat(rule.Selector, 1);
        }

        foreach (var selector in selectors)
        {
          elemsToStyle = document.QuerySelectorAll(rule.Selector.ToString(), elementFactory);
          foreach (var elem in elemsToStyle)
          {
            foreach (var decl in rule.Declarations)
            {
              elem.AddStyle(decl.Name, decl.Term.ToString(), rule.Selector.GetSpecificity());
            }
          }
        }
      }
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

            var mappedAttributes = (
              from attr in GetAttributes(reader)
              let attribute = AttributeFactory.Create(elementType, attr.Key.Trim(), attr.Value.Trim())
              where attribute != null
              select attribute
            );

            element.Attributes.Add(mappedAttributes);

            if (element.Attributes.TryGetValue<IList<KeyValuePair<string, string>>>("style", out var styles))
            {
              var mappedAttributes2 = (
                from attr in styles
                let attribute = AttributeFactory.Create(elementType, attr.Key.Trim(), attr.Value.Trim())
                where attribute != null
                select attribute
              );
              element.Attributes.Add(mappedAttributes2);
            }

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

/*
            var parser = new ExCSS.StylesheetParser();

            foreach (var element in doc.Descendants("style"))
            {
                Console.WriteLine(element.Content);
                var v = parser.Parse(element.Content);
                foreach (var c in v.Children)
                {
                }
            }
*/
      return doc;
    }
  }
}