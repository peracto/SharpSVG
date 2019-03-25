using System.IO;
using System.Threading.Tasks;

namespace Peracto.Svg
{
    public interface ILoader
    {
        Task<IDocument> Load(System.Uri uri, System.Uri baseUri = null);
        Task<IDocument> Load(Stream  stream, System.Uri baseUri = null);
//        ISvgContext Context { get; }
  }
}