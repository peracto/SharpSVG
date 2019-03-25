using System;

namespace Peracto.Svg.Brush
{
  public class HrefServer
  {
    private readonly Uri _uri;
    public HrefServer(string uriString)
    {
      _uri = new Uri(Standardize(uriString.Trim()), UriKind.RelativeOrAbsolute);
    }
    public Uri Resolve(IElement element)
    {
      if (_uri.IsAbsoluteUri) return _uri;
      var rc = new Uri(element.OwnerDocument.BaseUri, _uri);
      return rc;
    }

    private static string Standardize(string value)
    {
      //Uri MaxLength is 65519 (https://msdn.microsoft.com/en-us/library/z6c2z492.aspx)
      var uri = (value.Length > 65519) ? value.Substring(0, 65519) : value;
      return uri.StartsWith("#")
          ? $"internal://{uri}"
          : uri;
    }


  }
}