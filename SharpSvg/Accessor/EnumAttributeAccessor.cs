namespace Peracto.Svg.Accessor
{
  public class EnumAttributeAccessor<T> : IAccessor<T> 
  { 
    private bool Inherit { get; }

    public string AttributeName { get; }
    public T DefaultValue { get; }
    public T InheritValue { get; }

    public EnumAttributeAccessor(string name, T defaultValue, T inheritValue, bool inherit = false)
    {
      AttributeName = name;
      DefaultValue = defaultValue;
      Inherit = inherit;
      InheritValue = inheritValue;
    }

    public T GetValue(IElement element)
    {
      while (element != null)
      {
        if (element.Attributes.TryGetValue<T>(AttributeName, out var value) && !value.Equals(InheritValue))
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