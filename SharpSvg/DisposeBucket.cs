using System;
using System.Collections.Generic;

namespace Peracto.Svg
{
  public class DisposeBucket : IDisposable
  {

    private IList<IDisposable> _bucket;

    public T Push<T>(T disposable) where T : IDisposable
    {
      if (_bucket == null) _bucket = new List<IDisposable>();
      _bucket.Add(disposable);
      return disposable;
    }

    public void Dispose()
    {
      if (_bucket == null) return;
      foreach (var d in _bucket) d.Dispose();
      _bucket = null;
    }
  }
}