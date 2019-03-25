using System;
using System.Collections.Generic;
using Peracto.Svg.Accessor;

namespace Peracto.Svg.Parser
{
  public class Document : Element, IDocument
  {
    public Document(Uri baseUri) : base(
      new ElementDefinition("#document", ElementContentType.None)
    )
    {
      BaseUri = baseUri;
    }

    public override IDocument OwnerDocument => this;
    public Uri BaseUri { get; }
    public IElement RootElement => Children[0];

    private IDictionary<string, object> _resources;

    public object GetResource(string name)
    {
      if (_resources == null) return null;
      return _resources.TryGetValue(name, out var value) ? value : null;
    }

    public void SetResource(string name, object value)
    {
      if (_resources == null) _resources = new Dictionary<string, object>();
      _resources[name] = value;
    }
  }
}