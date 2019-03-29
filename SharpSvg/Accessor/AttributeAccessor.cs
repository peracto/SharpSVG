using Peracto.Svg.Types;

namespace Peracto.Svg.Accessor
{
  public class AttributeAccessor<T> : IAccessor<T>
  {

    private bool Inherit { get; }
    public string AttributeName { get; }
    public T DefaultValue { get; }

    public AttributeAccessor(string name, T defaultValue, bool inherit = false)
    {
      AttributeName = name;
      DefaultValue = defaultValue;
      Inherit = inherit;
    }


    public T GetValue(IElement element)
    {
      while (element != null)
      {
        if (element.Attributes.TryGetValue<T>(AttributeName, out var value))
          return value;
        element = Inherit ? element.Parent : null;
      }

      return DefaultValue;
    }

    public bool TryGetValue(IElement element, out T value)
    {
      return element.Attributes.TryGetValue(AttributeName, out value);
    }
  }
}