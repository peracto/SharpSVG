namespace Peracto.Svg.Accessor
{
  public interface IAccessor<TOut>
  {
    string AttributeName { get; }
    TOut DefaultValue { get; }
    TOut GetValue(IElement element);
    bool TryGetValue(IElement element, out TOut value);
  }
}