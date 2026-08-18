using System;
using System.Collections.Generic;

namespace ManagerServer.Storage
{
    class Bloom
    {
        const int HashCount = 7;

        readonly int m;
        readonly int k;
        readonly byte[] bits;

        Bloom(int m, int k, byte[] bits)
        {
            this.m = m;
            this.k = k;
            this.bits = bits;
        }

        public static byte[] Create(List<(byte[] Hash, long Offset, int Length)> entries)
        {
            var n = Math.Max(entries.Count, 1);
            var m = (int)Math.Ceiling(n * 9.585);
            var bits = new byte[(m + 7) / 8];

            foreach (var (hash, _, _) in entries)
                Set(bits, m, hash);

            var result = new byte[8 + bits.Length];
            BitConverter.TryWriteBytes(result.AsSpan(0, 4), m);
            BitConverter.TryWriteBytes(result.AsSpan(4, 4), HashCount);
            bits.CopyTo(result.AsSpan(8));
            return result;
        }

        public static Bloom Load(byte[] data)
        {
            var m = BitConverter.ToInt32(data.AsSpan(0, 4));
            var k = BitConverter.ToInt32(data.AsSpan(4, 4));
            var bits = data.AsSpan(8).ToArray();
            return new Bloom(m, k, bits);
        }

        public bool MayContain(byte[] sha256)
        {
            var h1 = BitConverter.ToUInt64(sha256, 0);
            var h2 = BitConverter.ToUInt64(sha256, 8);
            for (int i = 0; i < k; i++)
            {
                var pos = (int)((h1 + (ulong)i * h2) % (ulong)m);
                if ((bits[pos / 8] & (1 << (pos % 8))) == 0)
                    return false;
            }

            return true;
        }

        static void Set(byte[] bits, int m, byte[] sha256)
        {
            var h1 = BitConverter.ToUInt64(sha256, 0);
            var h2 = BitConverter.ToUInt64(sha256, 8);
            for (int i = 0; i < HashCount; i++)
            {
                var pos = (int)((h1 + (ulong)i * h2) % (ulong)m);
                bits[pos / 8] |= (byte)(1 << (pos % 8));
            }
        }
    }
}
