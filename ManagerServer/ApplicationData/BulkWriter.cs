using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ManagerServer.Storage
{
    public sealed class BulkWriter(Storage storage, long maxPackSize) : IAsyncDisposable
    {
        Pack pack = new();
        HashSet<string> written = new();

        public async Task<byte[]> WriteAsync(Stream inputStream)
        {
            await using var rs = new FileBufferingReadStream(inputStream, 4 * 1024 * 1024);
            var sha256 = await SHA256.HashDataAsync(rs);
            var hex = Convert.ToHexStringLower(sha256);
            rs.Position = 0;

            if (await storage.ExistsAsync(sha256) || !written.Add(hex))
                return sha256;

            await pack.WriteAsync(sha256, rs);

            if (pack.CurrentSize >= maxPackSize)
                await FlushAsync();

            return sha256;
        }

        public async Task FlushAsync()
        {
            if (pack.Entries.Count == 0)
                return;

            await storage.WriteAsync(pack);
            await pack.DisposeAsync();
            pack = new Pack();
            written = new();
        }

        public async ValueTask DisposeAsync()
        {
            await FlushAsync();
            await pack.DisposeAsync();
        }
    }
}
