using System.Collections.Generic;

namespace Peracto.Svg.Accessor
{
    public interface IAttributeBag
    {
        T GetValue<T>(string attributeName);
        void Add(IEnumerable<IElementAttribute> attributes);
        bool TryGetValue<T>(string attributeName, out T outValue);
    }
}