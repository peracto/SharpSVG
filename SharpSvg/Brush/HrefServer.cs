using System;
using System.Runtime.Remoting.Messaging;

namespace Peracto.Svg.Brush
{
  public class HrefServer
  {
      private readonly Uri _uri;

      public HrefServer(string uriString)
      {
          _uri = uriString.Length > 65519 
              ? new BigUri(uriString) 
              : new Uri(Standardize(uriString.Trim()), UriKind.RelativeOrAbsolute);
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

  public class BigUri : Uri
  {
      public readonly string Text;

      public BigUri(string base64) : base("bigdata:image/jpeg;base64," + base64.GetHashCode().ToString("X"))
      {
          Text = base64;
      }

      public override string ToString()
      {
          return Text;
      }
  }

}