using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace ManagerServer.Storage
{
    static class PackIndex
    {
        const int EntrySize = 44; // 32 hash + 8 offset + 4 length

        public static void Write(Stream stream, List<(byte[] Hash, long Offset, int Length)> entries)
        {
            Span<byte> buf = stackalloc byte[12];
            foreach (var (hash, offset, length) in entries)
            {
                stream.Write(hash);
                BitConverter.TryWriteBytes(buf[..8], offset);
                BitConverter.TryWriteBytes(buf[8..], length);
                stream.Write(buf);
            }
        }

        public static async Task<(long Offset, int Length)?> SearchAsync(Stream stream, byte[] sha256)
        {
            var count = (int)(stream.Length / EntrySize);
            var entry = new byte[EntrySize];

            int lo = 0, hi = count - 1;
            while (lo <= hi)
            {
                var mid = lo + (hi - lo) / 2;
                stream.Position = (long)mid * EntrySize;
                await stream.ReadExactlyAsync(entry);

                var cmp = entry.AsSpan(0, 32).SequenceCompareTo(sha256);
                if (cmp == 0)
                {
                    var offset = BitConverter.ToInt64(entry.AsSpan(32, 8));
                    var length = BitConverter.ToInt32(entry.AsSpan(40, 4));
                    return (offset, length);
                }

                if (cmp < 0) lo = mid + 1;
                else hi = mid - 1;
            }

            return null;
        }
    }
}
