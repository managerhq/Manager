using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerServer.Model.Obsolete.Obsolete53
{
    public sealed class PersistentDictionary : IDisposable
    {
        public static byte[] MagicString = new byte[] { 0x4D, 0x4E, 0x47, 0x52, 0x7C };
        private Dictionary<Guid, long> index = new Dictionary<Guid, long>(); // key, position
        private Dictionary<long, int> empty = new Dictionary<long, int>(); // position, length

        private Stream s;
        private BinaryReader r;
        private BinaryWriter w;

        public PersistentDictionary()
        {
            s = new MemoryStream();
            w = new BinaryWriter(s);
            r = new BinaryReader(s);

            w.Write(MagicString, 0, MagicString.Length);
        }

        public PersistentDictionary(Stream stream)
        {
            s = stream;
            w = new BinaryWriter(s);
            r = new BinaryReader(s);

            if (!r.ReadBytes(MagicString.Length).SequenceEqual(MagicString)) throw new FileInvalidException();
            while (s.Position < s.Length)
            {
                if (s.Position + 4 > s.Length)
                {
                    throw new FileCorruptedException();
                }
                var size = r.ReadInt32();
                if (size < 0 || s.Position + size > s.Length)
                {
                    throw new FileCorruptedException();
                }
                var pos = s.Position;
                var present = r.ReadBoolean();
                if (present)
                {
                    var key = new Guid(r.ReadBytes(16));
                    if (!index.ContainsKey(key)) index.Add(key, pos);
                }
                else
                {
                    empty.Add(pos, size);
                }
                s.Position = pos + size;
            }
        }

        public byte[] Get(Guid key)
        {
            var pos = index[key];
            s.Position = pos;
            r.ReadBoolean();
            r.ReadBytes(16);
            var length = r.ReadInt32();
            if (length < 0) throw new FileCorruptedException();
            var value = r.ReadBytes(length);
            try
            {
                var hash = r.ReadInt32();
                if (ComputeAdditionChecksum(value) != hash) throw new FileCorruptedException();
            }
            catch (EndOfStreamException)
            {
                throw new FileCorruptedException();
            }
            return value;
        }

        public Guid[] Keys
        {
            get
            {
                return index.Select(x => x.Key).ToArray();
            }
        }

        public byte[] ToArray()
        {
            return ((MemoryStream)s).ToArray();
        }

        public void Close()
        {
            s.Close();
            r.Dispose();
            w.Dispose();
        }

        private int ComputeAdditionChecksum(byte[] data)
        {
            int sum = 0;
            unchecked
            {
                foreach (byte b in data)
                {
                    sum += b;
                }
            }
            return sum;
        }

        public void Dispose()
        {
            s.Dispose();
            r.Dispose();
            w.Dispose();
        }

        public static async Task ConvertFromLegacy(FileSystem fs, string key)
        {
            var tempPath = Path.GetTempFileName();

            using (var sourceStream = await fs.ReadAsync(key))
            {
                var ms = new MemoryStream();
                await sourceStream.CopyToAsync(ms);
                ms.Position = 0;

                using (var persistentDictionary = new PersistentDictionary(ms))
                {
                    using (var sqlite = new Orm.SQLiteConnection(tempPath))
                    {
                        using var tx = sqlite.BeginTransaction();
                        tx.CreateTable<ApplicationData.Object>();
                        foreach (var e in persistentDictionary.Keys)
                        {
                            var buffer = persistentDictionary.Get(e);
                            var guid = new byte[16];
                            Array.Copy(buffer, 0, guid, 0, guid.Length);
                            var contentType = new Guid(guid);
                            var content = new byte[buffer.Length - 16];
                            Array.Copy(buffer, 16, content, 0, content.Length);
                            tx.Insert(new ApplicationData.Object() { Key = e, ContentType = contentType, Content = content });
                        }
                        tx.Commit();
                    }
                    persistentDictionary.Close();
                }
            }

            fs.MoveAsync(key, $"{key}-{Guid.CreateVersion7():N}");

            using (var converted = new FileStream(tempPath, FileMode.Open, FileAccess.Read))
                await fs.WriteAsync(key, converted);

            try { File.Delete(tempPath); } catch { }
        }
    }

    public sealed class FileCorruptedException : Exception { }
    public sealed class FileInvalidException : Exception { }
}
