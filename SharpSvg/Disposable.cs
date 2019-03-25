using System;

namespace Peracto.Svg
{
  public static class Disposable
  {

    public static readonly IDisposable Empty = new AggregateDisposable();

    public static IDisposable CreateAggregateDispose(IDisposable d1, IDisposable d2)
    {
      if (d1 == null) return d2;
      return d2 == null ? d1 : new AggregateDisposable(d1, d2);
    }

    private class AggregateDisposable : IDisposable
    {
      private readonly IDisposable _d1;
      private readonly IDisposable _d2;
      public AggregateDisposable()
      {
      }
      public AggregateDisposable(IDisposable d1, IDisposable d2)
      {
        _d1 = d1;
        _d2 = d2;
      }
      public void Dispose()
      {
        _d1?.Dispose();
        _d2?.Dispose();
      }
    }
  }
}
