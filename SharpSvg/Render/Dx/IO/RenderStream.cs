using System;
using Peracto.Svg.Types;
using SharpDX;

namespace Peracto.Svg.Render.Dx.IO
{
    public class RenderStream : IDisposable
    {
        public string Value { get; }
        public IDocument Document { get; }
        public SharpDX.Direct2D1.Bitmap Bitmap { get; }
        public RenderStreamType RenderStreamType { get; }

        public RenderStream()
        {
            RenderStreamType = RenderStreamType.Invalid;
        }

        public RenderStream(SharpDX.Direct2D1.Bitmap bitmap)
        {
            Bitmap = bitmap;
            RenderStreamType = RenderStreamType.Bitmap;
        }

        public RenderStream(string value)
        {
            Value = value;
            RenderStreamType = RenderStreamType.Internal;
        }

        public RenderStream(IDocument document)
        {
            Document = document;
            RenderStreamType = RenderStreamType.Document;
        }

        #region IDisposable Support

        private bool _disposedValue; // To detect redundant calls

        public void Dispose()
        {
            if (_disposedValue) return;
            _disposedValue = true;
            Bitmap?.Dispose();
        }

        #endregion

    }
}