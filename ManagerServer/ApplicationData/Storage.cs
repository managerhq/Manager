using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace ManagerServer.Storage
{
    public sealed class Storage(IFileSystem fs, long maxPackSize = 128 * 1024 * 1024)
    {
        readonly Blobs blobs = new(fs);
        readonly Filters filters = new(fs);
        readonly Indexes indexes = new(fs);
        readonly Packs packs = new(fs);

        public record PackBucket(int Bucket, int Count, long Size);
        public record StorageStats(int BlobCount, long BlobSize, int OversizedBlobCount, long OversizedBlobSize, int PackCount, long PackSize, List<PackBucket> PackBuckets, long MaxPackSize, int FilterCount, long FilterSize, int IndexCount, long IndexSize);

        public async Task<StorageStats> Stats()
        {
            var b = await blobs.GetStats(maxPackSize);
            var p = await packs.GetStats();
            var f = await filters.GetStats();
            var i = await indexes.GetStats();
            return new StorageStats(b.Count, b.Size, b.OversizedCount, b.OversizedSize, p.Count, p.Size, p.Buckets, maxPackSize, f.Count, f.Size, i.Count, i.Size);
        }

        public BulkWriter CreateBulkWriter() => new BulkWriter(this, maxPackSize);

        public async Task<byte[]> WriteAsync(byte[] buffer)
        {
            if (buffer == null) return null;
            using var ms = new MemoryStream(buffer);
            return await WriteAsync(ms);
        }

        public async Task<byte[]> WriteAsync(Stream inputStream)
        {
            await using var rs = new FileBufferingReadStream(inputStream, 4 * 1024 * 1024);
            var sha256 = await SHA256.HashDataAsync(rs);

            if (!await ExistsAsync(sha256))
            {
                rs.Position = 0;
                await blobs.WriteAsync(sha256, rs);
            }

            return sha256;
        }

        public async Task<Stream> ReadAsync(byte[] sha256)
        {
            if (await blobs.ExistsAsync(sha256))
                return await blobs.OpenReadAsync(sha256);

            var found = await FindInPacksAsync(sha256);
            if (found != null)
                return await packs.Open(found.Id, found.Offset, found.Length);

            return null;
        }

        internal async Task<bool> ExistsAsync(byte[] sha256)
        {
            if (await blobs.ExistsAsync(sha256))
                return true;

            return await FindInPacksAsync(sha256) != null;
        }

        async Task<PackEntry> FindInPacksAsync(byte[] sha256)
        {
            foreach (var id in await filters.ListIds())
            {
                if (!(await filters.GetBloomAsync(id)).MayContain(sha256))
                    continue;

                using var idxStream = await indexes.Open(id);
                var result = await PackIndex.SearchAsync(idxStream, sha256);
                if (result is not { } idx)
                    continue;

                return new PackEntry(id, idx.Offset, idx.Length);
            }

            return null;
        }

        record PackEntry(Guid Id, long Offset, int Length);

        internal async Task WriteAsync(Pack pack)
        {
            await pack.CloseAsync();

            await using (var readStream = new FileStream(pack.TempPath, FileMode.Open, FileAccess.Read))
                await packs.WriteAsync(pack.Id, readStream);

            pack.Entries.Sort((a, b) => a.Hash.AsSpan().SequenceCompareTo(b.Hash));

            using var idxMs = new MemoryStream();
            PackIndex.Write(idxMs, pack.Entries);
            idxMs.Position = 0;
            await indexes.WriteAsync(pack.Id, idxMs);

            await using (var blmMs = new MemoryStream(Bloom.Create(pack.Entries)))
                await filters.WriteAsync(pack.Id, blmMs);
        }

        public async Task Pack()
        {
            var looseFiles = await blobs.ListAllAsync();

            var batch = new List<byte[]>();
            long currentSize = 0;

            foreach (var sha256 in looseFiles)
            {
                var fileSize = await blobs.GetFileSizeAsync(sha256);
                if (fileSize > maxPackSize)
                {
                    continue;
                }

                var tarEntrySize = 512 + TarScanner.RoundUp(fileSize, 512);

                if (currentSize + tarEntrySize > maxPackSize && batch.Count > 0)
                {
                    if (batch.Count > 1)
                    {
                        await WritePack(batch);
                    }
                    batch.Clear();
                    currentSize = 0;
                }

                batch.Add(sha256);
                currentSize += tarEntrySize;
            }

            if (batch.Count > 1)
            {
                await WritePack(batch);
            }
        }

        public async Task Consolidate()
        {
            var allPacks = await packs.ListAll();

            var buckets = allPacks
                .Where(p => p.Size < maxPackSize)
                .GroupBy(p => (int)Math.Log2(Math.Max(p.Size, 1)))
                .Where(g => g.Count() > 1);

            foreach (var bucket in buckets)
            {
                var maxPerChunk = (int)(maxPackSize >> (bucket.Key + 1));
                if (maxPerChunk < 2) continue;

                foreach (var chunk in bucket.Select(p => p.Id).Chunk(maxPerChunk))
                {
                    if (chunk.Length > 1)
                        await MergePacks(chunk.ToList());
                }
            }
        }

        async Task MergePacks(List<Guid> packIds)
        {
            await using var pack = new Pack();

            foreach (var id in packIds)
            {
                using var srcStream = await packs.Open(id);
                foreach (var (hash, _, data) in TarScanner.ReadEntries(srcStream))
                    await pack.WriteAsync(hash, data);
            }

            await WriteAsync(pack);

            foreach (var id in packIds)
            {
                await filters.Delete(id);
                await indexes.Delete(id);
                await packs.Delete(id);
            }
        }

        public async Task Reindex()
        {
            filters.ClearCache();
            indexes.ClearCache();

            foreach (var (id, _) in await packs.ListAll())
            {
                using var tarStream = await packs.Open(id);
                var entries = TarScanner.Scan(tarStream);

                using var idxMs = new MemoryStream();
                PackIndex.Write(idxMs, entries);
                idxMs.Position = 0;
                await indexes.WriteAsync(id, idxMs);

                await using (var blmMs = new MemoryStream(Bloom.Create(entries)))
                    await filters.WriteAsync(id, blmMs);
            }
        }

        async Task WritePack(List<byte[]> looseHashes)
        {
            await using var pack = new Pack();

            foreach (var sha256 in looseHashes)
            {
                using var dataStream = await blobs.OpenReadAsync(sha256);
                await pack.WriteAsync(sha256, dataStream);
            }

            await WriteAsync(pack);

            foreach (var sha256 in looseHashes)
                await blobs.DeleteAsync(sha256);
        }
    }
}
