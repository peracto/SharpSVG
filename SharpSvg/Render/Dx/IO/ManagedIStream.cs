using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security;
using STATSTG = System.Runtime.InteropServices.ComTypes.STATSTG;
namespace Peracto.Svg.Render.Dx.IO
{
  public class ManagedIStream : IStream
  {
    private readonly Stream _ioStream;
    private readonly string _streamName;

    internal ManagedIStream(Stream ioStream,string streamName)
    {
      _ioStream = ioStream ?? throw new ArgumentNullException(nameof(ioStream));
      _streamName = streamName;
    }

    [SecurityCritical]
    void IStream.Read(byte[] buffer, int bufferSize, IntPtr bytesReadPtr)
    {
      var val = _ioStream.Read(buffer, 0, bufferSize);
      if (bytesReadPtr != IntPtr.Zero)
        Marshal.WriteInt32(bytesReadPtr, val);
    }

    [SecurityCritical]
    void IStream.Seek(long offset, int origin, IntPtr newPositionPtr)
    {
      var val = _ioStream.Seek(offset, GetSeek(origin));
      if (newPositionPtr != IntPtr.Zero)
        Marshal.WriteInt64(newPositionPtr, val);
    }

    private SeekOrigin GetSeek(int origin)
    {
      switch (origin)
      {
        case 0:return SeekOrigin.Begin;
        case 1:return SeekOrigin.Current;
        case 2:return SeekOrigin.End;
        default:
          throw new ArgumentOutOfRangeException(nameof(origin));
      }
    }

    void IStream.SetSize(long libNewSize)
    {
      _ioStream.SetLength(libNewSize);
    }

    void IStream.Stat(out STATSTG streamStats, int grfStatFlag)
    {
      streamStats = new STATSTG
      {
        type = 2,
        cbSize = _ioStream.Length,
        grfMode = GetGrfMode()
      };
    }

    private int GetGrfMode()
    {
      if (_ioStream.CanRead && _ioStream.CanWrite)
        return 2;
      else if (_ioStream.CanRead)
        return 0;
      else if (_ioStream.CanWrite)
        return 1;
      throw new IOException("StreamObjectDisposed");
    }

    [SecurityCritical]
    void IStream.Write(byte[] buffer, int bufferSize, IntPtr bytesWrittenPtr)
    {
      _ioStream.Write(buffer, 0, bufferSize);
      if (bytesWrittenPtr != IntPtr.Zero)
        Marshal.WriteInt32(bytesWrittenPtr, bufferSize);
    }

    void IStream.Clone(out IStream streamCopy)
    {
      Console.WriteLine($"Cloning Stream {_streamName}");
      streamCopy = new ManagedIStream(_ioStream,_streamName);
   //   throw new NotSupportedException();
    }

    void IStream.CopyTo(IStream targetStream,long bufferSize,IntPtr buffer,IntPtr bytesWrittenPtr)
    {
      throw new NotSupportedException();
    }

    void IStream.Commit(int flags)
    {
      throw new NotSupportedException();
    }

    void IStream.LockRegion(long offset, long byteCount, int lockType)
    {
      throw new NotSupportedException();
    }

    void IStream.Revert()
    {
      throw new NotSupportedException();
    }

    void IStream.UnlockRegion(long offset, long byteCount, int lockType)
    {
      throw new NotSupportedException();
    }
  }
}