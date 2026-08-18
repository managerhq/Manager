using System;
using System.Collections.Generic;
using System.Formats.Tar;
using System.IO;
using System.Threading.Tasks;

namespace ManagerServer.Storage
{
    sealed class Pack : IAsyncDisposable
    {
        readonly Guid id = Guid.CreateVersion7();
        readonly string tempPath = Path.Combine(Path.GetTempPath(), Guid.CreateVersion7().ToString("N") + ".tar");
        FileStream tarStream;
        TarWriter tarWriter;
        readonly List<(byte[] Hash, long Offset, int Length)> entries = new();
        long currentSize;

        internal Guid Id => id;
        internal string TempPath => tempPath;
        internal List<(byte[] Hash, long Offset, int Length)> Entries => entries;
        internal long CurrentSize => currentSize;

        internal async Task WriteAsync(byte[] hash, Stream data)
        {
            if (tarStream == null)
            {
                tarStream = new FileStream(tempPath, FileMode.Create, FileAccess.Write);
                tarWriter = new TarWriter(tarStream, TarEntryFormat.Ustar, leaveOpen: true);
            }

            var dataOffset = currentSize + 512;
            var length = (int)data.Length;
            var entry = new UstarTarEntry(TarEntryType.RegularFile, Convert.ToHexStringLower(hash)) { DataStream = data };
            await tarWriter.WriteEntryAsync(entry);
            entries.Add((hash, dataOffset, length));
            currentSize += 512 + TarScanner.RoundUp(length, 512);
        }

        internal async Task CloseAsync()
        {
            if (tarWriter != null) { await tarWriter.DisposeAsync(); tarWriter = null; }
            if (tarStream != null) { await tarStream.DisposeAsync(); tarStream = null; }
        }

        public async ValueTask DisposeAsync()
        {
            await CloseAsync();
            File.Delete(tempPath);
        }
    }
}
