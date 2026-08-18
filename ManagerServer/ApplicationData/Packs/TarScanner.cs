using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ManagerServer.Storage
{
    static class TarScanner
    {
        public static IEnumerable<(byte[] Hash, int Length, Stream Data)> ReadEntries(Stream stream)
        {
            var header = new byte[512];
            var skipBuf = new byte[512];

            while (true)
            {
                if (!ReadExact(stream, header, 512)) yield break;
                if (header[0] == 0) yield break;

                var nameEnd = Array.IndexOf(header, (byte)0, 0, 100);
                if (nameEnd < 0) nameEnd = 100;
                var name = Encoding.ASCII.GetString(header, 0, nameEnd);
                var hash = Convert.FromHexString(name);

                var sizeStr = Encoding.ASCII.GetString(header, 124, 12).TrimEnd('\0', ' ');
                var size = Convert.ToInt64(sizeStr, 8);

                var sub = new SubStream(stream, size);
                yield return (hash, (int)size, sub);

                var leftover = (size - sub.Position) + (RoundUp(size, 512) - size);
                SkipBytes(stream, leftover, skipBuf);
            }
        }

        public static List<(byte[] Hash, long Offset, int Length)> Scan(Stream stream)
        {
            var entries = new List<(byte[] Hash, long Offset, int Length)>();
            var header = new byte[512];
            var skipBuf = new byte[512];
            long pos = 0;

            while (true)
            {
                if (!ReadExact(stream, header, 512)) break;
                pos += 512;
                if (header[0] == 0) break;

                var nameEnd = Array.IndexOf(header, (byte)0, 0, 100);
                if (nameEnd < 0) nameEnd = 100;
                var name = Encoding.ASCII.GetString(header, 0, nameEnd);
                var hash = Convert.FromHexString(name);

                var sizeStr = Encoding.ASCII.GetString(header, 124, 12).TrimEnd('\0', ' ');
                var size = Convert.ToInt64(sizeStr, 8);

                entries.Add((hash, pos, (int)size));

                var toSkip = RoundUp(size, 512);
                SkipBytes(stream, toSkip, skipBuf);
                pos += toSkip;
            }

            entries.Sort((a, b) => a.Hash.AsSpan().SequenceCompareTo(b.Hash));
            return entries;
        }

        public static long RoundUp(long value, long blockSize) =>
            (value + blockSize - 1) / blockSize * blockSize;

        static bool ReadExact(Stream s, byte[] buf, int count)
        {
            var read = 0;
            while (read < count)
            {
                var n = s.Read(buf, read, count - read);
                if (n == 0)
                {
                    if (read == 0) return false;
                    throw new EndOfStreamException();
                }
                read += n;
            }
            return true;
        }

        static void SkipBytes(Stream s, long count, byte[] scratch)
        {
            while (count > 0)
            {
                var n = s.Read(scratch, 0, (int)Math.Min(scratch.Length, count));
                if (n == 0) throw new EndOfStreamException();
                count -= n;
            }
        }
    }
}
