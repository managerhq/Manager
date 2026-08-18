using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;

namespace ManagerServer
{
    sealed class SubStream(Stream inner, long length) : Stream
    {
        readonly long _length = length;
        long remaining = length;

        public override bool CanRead => true;
        public override bool CanSeek => true; // This should be false but we need it to be true just so TarWriter accepts this
        public override bool CanWrite => false;
        public override long Length => _length;
        public override long Position
        {
            get => _length - remaining;
            set => throw new NotSupportedException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            if (remaining <= 0) return 0;
            var toRead = (int)Math.Min(count, remaining);
            var read = inner.Read(buffer, offset, toRead);
            remaining -= read;
            return read;
        }

        public override async ValueTask<int> ReadAsync(Memory<byte> buffer, CancellationToken cancellationToken = default)
        {
            if (remaining <= 0) return 0;
            var toRead = (int)Math.Min(buffer.Length, remaining);
            var read = await inner.ReadAsync(buffer[..toRead], cancellationToken);
            remaining -= read;
            return read;
        }

        public override void Flush() { }
        public override long Seek(long offset, SeekOrigin origin) => throw new NotSupportedException();
        public override void SetLength(long value) => throw new NotSupportedException();
        public override void Write(byte[] buffer, int offset, int count) => throw new NotSupportedException();

        protected override void Dispose(bool disposing)
        {
            if (disposing) inner.Dispose();
            base.Dispose(disposing);
        }
    }
}
