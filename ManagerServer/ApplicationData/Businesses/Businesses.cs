using ManagerServer.Orm;
using Newtonsoft.Json;
using SQLitePCL;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Formats.Tar;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;

namespace ManagerServer
{
    sealed class Businesses(FileSystem fs)
    {
        Dictionary<string, Database> businesses;
        Dictionary<string, CancellationTokenSource> cancellationTokens = new Dictionary<string, CancellationTokenSource>();

        internal string GetStaleBusiness()
        {
            if (businesses == null) Refresh();
            return businesses
                .Where(x => x.Value.Status == Database.DatabaseStatus.Ready)
                .OrderBy(x => x.Value.Priority)
                .Select(x => x.Key)
                .FirstOrDefault();
        }

        internal void Unload(string fileId)
        {
            if (businesses == null) Refresh();
            var oldest = GetStaleBusiness();
            if (oldest != null) businesses[oldest] = new Database();
            businesses[fileId] = new Database();
            GC.Collect();
        }

        internal async Task<bool> Backup(string fileId, Microsoft.AspNetCore.Http.HttpResponse response)
        {
            var ct = new CancellationTokenSource();
            if (!cancellationTokens.TryAdd(fileId, ct))
            {
                return false;
            }
            try
            {
                using (var stream = await fs.ReadAsync($"Businesses/{fileId}.manager"))
                {
                    response.ContentLength = stream.Length;
                    await stream.CopyToAsync(response.Body, ct.Token);
                }
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                cancellationTokens.Remove(fileId);
            }
            return true;
        }

        internal async Task<bool> Backup(string fileId, string entryName, TarWriter tarWriter)
        {
            var ct = new CancellationTokenSource();
            if (!cancellationTokens.TryAdd(fileId, ct))
            {
                return false;
            }
            try
            {
                using (var stream = await fs.ReadAsync($"Businesses/{fileId}.manager"))
                {
                    var entry = new PaxTarEntry(TarEntryType.RegularFile, entryName) { DataStream = stream };
                    await tarWriter.WriteEntryAsync(entry, ct.Token);
                }

                // TODO: Write attachment entries into TAR
            }
            catch (OperationCanceledException)
            {
            }
            finally
            {
                cancellationTokens.Remove(fileId);
            }
            return true;
        }

        internal bool IsBackupInProgress(string fileId)
        {
            return cancellationTokens.ContainsKey(fileId);
        }

        internal void CancelBackup(string fileId)
        {
            var ct = cancellationTokens.GetValueOrDefault(fileId);
            if (ct != null) ct.Cancel();
        }

        internal void Refresh()
        {
            var currentBusinesses = businesses ?? new Dictionary<string, Database>();
            var freshBusinesses = new Dictionary<string, Database>();

            var files = fs.GetKeysAsync("Businesses/").GetAwaiter().GetResult();

            files = files.Where(k => k.EndsWith(".manager"))
                .Select(k => Path.GetFileNameWithoutExtension(k.Substring("Businesses/".Length)))
                .Where(x => x != "238097db79f84dc5a44a0c6f8aa04e6c")
                .ToArray();            

            foreach (var e in files)
            {
                if (currentBusinesses.TryGetValue(e, out Database value))
                {
                    freshBusinesses.Add(e, value);
                }
                else
                {
                    freshBusinesses.Add(e, new Database());
                }
            }

            businesses = freshBusinesses;
        }

        internal async Task CreateAsync(string name)
        {
            var key = $"Businesses/{name}.manager";
            using (var ms = new MemoryStream(new byte[0])) await fs.WriteAsync(key, ms);
            Refresh();
        }

        internal async Task<Stream> OpenReadAsync(string entityId)
        {
            return await fs.ReadAsync($"Businesses/{entityId}.manager");
        }

        internal async Task DeleteAsync(string entityId)
        {
            await fs.DeleteAsync($"Businesses/{entityId}.manager");
        }

        internal string[] GetAll()
        {
            if (businesses == null) Refresh();
            return businesses.Keys.ToArray();
        }

        internal bool Exists(string entityId)
        {
            if (entityId == null) return false;
            if (businesses == null) Refresh();
            return businesses.ContainsKey(entityId);
        }

        internal string GetProgress(string entityId)
        {
            if (businesses == null) Refresh();
            if (!businesses.TryGetValue(entityId, out Database database)) return null;
            if (database.Status == Database.DatabaseStatus.Fresh)
            {
                Task.Run(() => EnsureNotFresh(database, entityId));
                return "0%";
            }
            else if (database.Status == Database.DatabaseStatus.Progress)
            {
                var percentage = 0;
                if (database.ProgressLength != 0) percentage = database.ProgressPosition * 100 / database.ProgressLength;
                return @$"{percentage}% &mdash; {database.ProgressPosition} / {database.ProgressLength}";
            }
            return null;
        }

        internal Database GetWithoutEnsuringFresh(string entityId)
        {
            if (string.IsNullOrWhiteSpace(entityId)) return null;

            if (businesses == null) Refresh();
            if (!businesses.TryGetValue(entityId, out Database business)) return null;

            return business;
        }

        internal Database Get(string entityId)
        {
            if (string.IsNullOrWhiteSpace(entityId)) return null;

            if (businesses == null) Refresh();
            if (!businesses.TryGetValue(entityId, out Database business)) return null;

            EnsureNotFresh(business, entityId);

            business.Priority = DateTime.UtcNow.Ticks;
            return business;
        }

        void EnsureNotFresh(Database business, string entityId)
        {
            if (business.Status == Database.DatabaseStatus.Ready) return;

            lock (business)
            {
                if (business.Status == Database.DatabaseStatus.Fresh)
                {
                    try
                    {
                        business.SetProgress(0, 0);

                        if (!Upgrade.Process(entityId, business).GetAwaiter().GetResult())
                        {
                            business.SetIncompatible();
                            return;
                        }

                        using (var db = SQLiteConnection(entityId))
                        {
                            using (var tx = db.BeginTransaction())
                            {
                                tx.CreateTable<ApplicationData.Object>();
                                tx.CreateTable<ApplicationData.Blob>();
                                tx.CreateTable<ApplicationData.Change>();
                                tx.CreateTable<ApplicationData.Email>();
                                tx.CreateTable<ApplicationData.Image>();
                                tx.Commit();
                            }

                            var count = db.Table<ApplicationData.Object>().Count();

                            business.SetProgress(0, count);

                            var images = db.Query<ApplicationData.Image>($"SELECT {nameof(ApplicationData.Image.Key)}, {nameof(ApplicationData.Image.Timestamp)} FROM Images").ToDictionary(x => x.Key, x => x.Timestamp);

                            var hasNonCriticalError = false;
                            using (var stmt = SQLite3.Prepare2(db.Handle, $"SELECT {nameof(ApplicationData.Object.Key)}, {nameof(ApplicationData.Object.ContentType)}, {nameof(ApplicationData.Object.Content)}, {nameof(ApplicationData.Object.Timestamp)} FROM Objects"))
                            {
                                var prototypes = new Dictionary<Guid, ManagerServer.Model.Object>();
                                var objects = new TypePartitionedDictionary<Guid, Model.Object>();

                                var index = 0;
                                while (SQLite3.Step(stmt) == SQLite3.Result.Row)
                                {
                                    business.SetProgress(index, count);
                                    index++;

                                    if (Guid.TryParse(SQLite3.ColumnString(stmt, 0), out Guid key))
                                    {
                                        if (Guid.TryParse(SQLite3.ColumnString(stmt, 1), out Guid contentType))
                                        {
                                            var content = raw.sqlite3_column_blob(stmt, 2);
                                            var timestamp = SQLite3.ColumnInt64(stmt, 3);

                                            try
                                            {
                                                var o = Serialization.Deserialize(contentType, content, key, timestamp);
                                                if (o != null)
                                                {
                                                    if (key == contentType) prototypes.Add(key, o);
                                                    else objects.Add(key, o);
                                                }
                                            }
                                            catch (ProtoBuf.ProtoException)
                                            {
                                                hasNonCriticalError = true;
                                            }
                                        }
                                    }
                                }

                                business.SetData(prototypes, objects, images, hasNonCriticalError);
                            }
                        }

                        business.Precompute();
                        business.SetReady();
                    }
                    catch (SQLiteException ex) when (ex.Result == SQLite3.Result.Corrupt)
                    {
                        business.SetCorrupted();
                    }
                    catch (SQLiteException ex) when (ex.Result == SQLite3.Result.NonDBFile)
                    {
                        business.SetInvalid();
                    }
                    catch (OutOfMemoryException)
                    {
                        business.SetOutOfMemory();
                    }
                }
            }
        }

        internal void Process(string entityId, Guid key, string user) { Process(entityId, new ApplicationData.Action[] { new ApplicationData.DeleteAction(key) }, user); }
        internal void Process(string entityId, Guid[] keys, string user) { Process(entityId, keys.Select(x => new ApplicationData.DeleteAction(x)).ToArray(), user); }
        internal void Process(string entityId, ManagerServer.Model.Object o, string user) { Process(entityId, new ApplicationData.Action[] { new ApplicationData.CreateOrUpdateAction(o) }, user); }
        internal void Process(string entityId, ManagerServer.Model.Object[] o, string user) { Process(entityId, o.Select(x => new ApplicationData.CreateOrUpdateAction(x)).ToArray(), user); }
        internal void Process(string entityId, ApplicationData.Action[] actions, string user)
        {
            var business = Get(entityId);

            lock (business)
            {
                try
                {
                    var utcNow = DateTime.UtcNow;
                    var commit = Guid.CreateVersion7();

                    using (var db = SQLiteConnection(entityId))
                    {
                        using var tx = db.BeginTransaction();

                        var keys = actions.Select(x => x.Key).Where(x => x != Guid.Empty).ToArray();
                        var objectsBefore = new Dictionary<Guid, ApplicationData.Object>();
                        foreach (var e in keys.Chunk(999)) // There's a default limit of 999 variables in a prepared statement
                        {
                            var rows = db.Table<ApplicationData.Object>().Where(x => e.Contains(x.Key));
                            foreach (var e2 in rows) objectsBefore.Add(e2.Key, e2);
                        }

                        if (actions != null)
                        {
                            foreach (var e in actions)
                            {
                                if (e is ApplicationData.CreateOrUpdateAction)
                                {
                                    var createOrUpdateAction = (ApplicationData.CreateOrUpdateAction)e;

                                    createOrUpdateAction.Object.Timestamp = utcNow.Ticks;
                                    var objectBefore = default(ApplicationData.Object);

                                    if (createOrUpdateAction.Object.Key != Guid.Empty)
                                    {
                                        objectsBefore.TryGetValue(createOrUpdateAction.Key, out objectBefore);
                                    }
                                    else
                                    {
                                        createOrUpdateAction.Object.Key = Guid.CreateVersion7();
                                    }

                                    var objectAfter = ApplicationData.Object.From(createOrUpdateAction.Object);

                                    if (!objectAfter.Equals2(objectBefore))
                                    {
                                        tx.InsertOrReplace(objectAfter);
                                        tx.Insert(new ApplicationData.Change() { Key = Guid.CreateVersion7(), Object = createOrUpdateAction.Key, User = user, Timestamp = utcNow.Ticks, Commit = commit, ContentTypeBefore = objectBefore?.ContentType ?? Guid.Empty, ContentBefore = objectBefore?.Content, ContentTypeAfter = objectAfter.ContentType, ContentAfter = objectAfter.Content });
                                    }
                                }
                                else if (e is ApplicationData.DeleteAction)
                                {
                                    var deleteAction = (ApplicationData.DeleteAction)e;

                                    objectsBefore.TryGetValue(deleteAction.Key, out var objectBefore);

                                    if (objectBefore != null)
                                    {
                                        tx.Delete<ApplicationData.Object>(deleteAction.Key);
                                        tx.Insert(new ApplicationData.Change() { Key = Guid.CreateVersion7(), Object = deleteAction.Key, User = user, Timestamp = utcNow.Ticks, Commit = commit, ContentTypeBefore = objectBefore.ContentType, ContentBefore = objectBefore.Content });
                                    }
                                }
                            }

                            tx.Commit();

                            var toAdd = actions.OfType<ApplicationData.CreateOrUpdateAction>().Select(x => x.Object).ToArray();
                            var toRemove = actions.OfType<ApplicationData.DeleteAction>().Select(x => x.Key).ToArray();
                            business.ApplyChanges(toAdd, toRemove);
                        }
                    }
                }
                catch (SQLiteException ex) when (ex.Result == SQLite3.Result.Corrupt)
                {
                    business.SetCorrupted();
                }
            }
        }

        internal bool Undo(string entityId, Guid key)
        {
            var business = Get(entityId);
            lock (business)
            {
                var utcNow = DateTime.UtcNow;
                using (var db = SQLiteConnection(entityId))
                {
                    using var tx = db.BeginTransaction();

                    var changes = db.Table<ApplicationData.Change>().Where(x => x.Commit == key).ToArray();

                    foreach (var e in changes)
                    {
                        var current = default(ApplicationData.Object);
                        try { current = db.Get<ApplicationData.Object>(e.Object); }
                        catch (InvalidOperationException) {}

                        // Delete this if you don't remember why
                        if (e.Object == new Guid("3273aded-a786-4116-8ba5-67fa800558ab"))
                        {
                            tx.InsertOrReplace(new ApplicationData.Object() { Key = e.Object, Timestamp = utcNow.Ticks, ContentType = e.ContentTypeAfter, Content = e.ContentAfter });
                            continue;
                        }
                        else if (e.IsCreatingChange)
                        {
                            if (current == null) return false;
                            if (current.ContentType != e.ContentTypeAfter) return false;
                            if (!(current.Content ?? []).SequenceEqual(e.ContentAfter ?? [])) return false;
                            tx.Delete<ApplicationData.Object>(e.Object);
                        }
                        else if (e.IsUpdatingChange)
                        {
                            if (current == null) return false;
                            if (current.ContentType != e.ContentTypeAfter) return false;
                            if (!(current.Content ?? []).SequenceEqual(e.ContentAfter ?? [])) return false;
                            tx.InsertOrReplace(new ApplicationData.Object() { Key = e.Object, Timestamp = utcNow.Ticks, ContentType = e.ContentTypeBefore, Content = e.ContentBefore });
                        }
                        else if (e.IsDeletingChange)
                        {
                            if (current != null) return false;
                            tx.InsertOrReplace(new ApplicationData.Object() { Key = e.Object, Timestamp = utcNow.Ticks, ContentType = e.ContentTypeBefore, Content = e.ContentBefore });
                        }

                        tx.Delete<ApplicationData.Change>(e.Key);
                    }

                    var toAdd = changes.Where(x => x.IsUpdatingChange || x.IsDeletingChange).Select(x => Serialization.Deserialize(x.ContentTypeBefore, x.ContentBefore, x.Object, utcNow.Ticks)).ToArray();
                    var toRemove = changes.Where(x => x.IsCreatingChange).Select(x => x.Object).ToArray();
                    business.ApplyChanges(toAdd, toRemove);
                    tx.Commit();
                }
            }

            return true;
        }

        internal Guid InsertBlob(string fileId, ApplicationData.Blob blob)
        {
            var business = Get(fileId);
            lock (business)
            {
                using (var conn = SQLiteConnection(fileId))
                {
                    using var tx = conn.BeginTransaction();
                    blob.Key = Guid.CreateVersion7();
                    tx.Insert(blob);
                    tx.Commit();
                    return blob.Key;
                }
            }
        }

        internal byte[] GetBlob(string fileId, Guid key)
        {
            var business = Get(fileId);
            lock (business)
            {
                using (var db = SQLiteConnection(fileId))
                {
                    var blob = db.Find<ApplicationData.Blob>(key);
                    if (blob == null) return null;
                    return blob.Content;
                }
            }
        }

        internal ApplicationData.Blob GetBlob2(string fileId, Guid key)
        {
            using (var db = SQLiteConnection(fileId))
            {
                return db.Find<ApplicationData.Blob>(key);
            }
        }

        internal void DeleteBlob(string fileId, Guid key)
        {
            var business = Get(fileId);
            lock (business)
            {
                using (var conn = SQLiteConnection(fileId))
                {
                    using var tx = conn.BeginTransaction();
                    tx.Delete<ApplicationData.Blob>(key);
                    tx.Commit();
                }
            }
        }

        internal void InsertOrReplaceImage(string fileId, Guid key, byte[] content, string contentType)
        {
            var business = Get(fileId);
            lock (business)
            {
                using (var conn = SQLiteConnection(fileId))
                {
                    using var tx = conn.BeginTransaction();
                    tx.InsertOrReplace(new ApplicationData.Image() { Key = key, ContentType = contentType, Content = content, Timestamp = DateTime.UtcNow.Ticks });
                    tx.Commit();
                }
                Get(fileId).SetImage(key);
            }
        }

        internal void DeleteImage(string fileId, Guid key)
        {
            var business = Get(fileId);
            lock (business)
            {
                using (var conn = SQLiteConnection(fileId))
                {
                    using var tx = conn.BeginTransaction();
                    tx.Delete<ApplicationData.Image>(key);
                    tx.Commit();
                }

                Get(fileId).RemoveImage(key);
            }
        }

        internal Tuple<byte[], string> GetImage(string fileId, Guid key)
        {
            var business = Get(fileId);
            lock (business)
            {
                using (var db = SQLiteConnection(fileId))
                {
                    var image = db.Find<ApplicationData.Image>(key);
                    if (image == null) return null;
                    return new Tuple<byte[], string>(image.Content, image.ContentType);
                }
            }
        }

        internal string GetImageDataUrl(string fileId, Guid key)
        {
            var image = GetImage(fileId, key);
            if (image == null) return null;
            return $"data:{image.Item2};base64,{Convert.ToBase64String(image.Item1)}";
        }

        internal async Task<bool> IsLegacyFormat(string fileId)
        {
            byte[] magic = { 0x4D, 0x4E, 0x47, 0x52, 0x7C };
            var key = $"Businesses/{fileId}.manager";
            if (!await fs.ExistsAsync(key)) return false;
            using var s = await fs.ReadAsync(key);
            using var r = new BinaryReader(s);
            return r.ReadBytes(magic.Length).SequenceEqual(magic);
        }

        internal async Task ConvertFromLegacy(string fileId)
        {
            await ManagerServer.Model.Obsolete.Obsolete53.PersistentDictionary.ConvertFromLegacy(fs, $"Businesses/{fileId}.manager");
            Refresh();
        }

        internal async Task<long> GetFileSize(string fileId)
        {
            return await fs.GetSizeAsync($"Businesses/{fileId}.manager");
        }

        internal bool IsValidName(string name)
        {
            // Ensure the name doesn't escape the Businesses prefix
            if (string.IsNullOrWhiteSpace(name)) return false;
            var path = Path.Combine(Path.GetTempPath(), name);
            return Path.TrimEndingDirectorySeparator(Path.GetDirectoryName(path)) == Path.TrimEndingDirectorySeparator(Path.GetTempPath());
        }

        internal async Task<bool> FileExists(string name)
        {
            return await fs.ExistsAsync($"Businesses/{name}.manager");
        }

        internal void Rename(string sourceName, string destName)
        {
            fs.MoveAsync($"Businesses/{sourceName}.manager", $"Businesses/{destName}.manager");
        }

        internal async Task ImportStream(string name, Stream source)
        {
            var key = await GetUniqueKey(name);
            await fs.WriteAsync(key, source);
        }

        internal async Task ImportJson(string name, byte[] json)
        {
            var jsonSettings = new JsonSerializerSettings();
            jsonSettings.ContractResolver = new ManagerServer.Localizations.Localizations.CustomContractResolver();

            var source = JsonConvert.DeserializeObject<Dictionary<Guid, Dictionary<Guid, object>>>(System.Text.UTF8Encoding.UTF8.GetString(json));

            var list = new List<ManagerServer.Model.Object>();
            foreach (var e3 in source)
            {
                var type = ManagerServer.Model.Object.GetTypeByGuid(e3.Key);
                if (type != null)
                {
                    foreach (var e4 in e3.Value)
                    {
                        var o = JsonConvert.DeserializeObject(JsonConvert.SerializeObject(e4.Value), type, jsonSettings) as ManagerServer.Model.Object;
                        if (o != null)
                        {
                            o.Key = e4.Key;
                            list.Add(o);
                        }
                    }
                }
            }

            var key = await GetUniqueKey(name);
            using (var ms = new MemoryStream()) await fs.WriteAsync(key, ms);
            Refresh();
            Process(Path.GetFileNameWithoutExtension(key.Substring("Businesses/".Length)), list.ToArray(), null);
        }

        internal bool CanRepair()
        {
            return File.Exists(GetSqlitePath());
        }

        internal async Task Repair(string fileId, Storage.Trash trash)
        {
            var key = $"Businesses/{fileId}.manager";
            var sqlite = GetSqlitePath();
            var input = Path.GetTempFileName();
            var output = Path.GetTempFileName();

            // Copy the business file to a temp location for sqlite3 to read
            using (var source = await fs.ReadAsync(key))
            await using (var dest = new FileStream(input, FileMode.Create, FileAccess.Write))
                await source.CopyToAsync(dest);

            var process1 = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = sqlite,
                    Arguments = @$"""{input}"" "".recover""",
                    RedirectStandardOutput = true,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            var process2 = new Process
            {
                StartInfo = new ProcessStartInfo
                {
                    FileName = sqlite,
                    Arguments = @$"""{output}""",
                    RedirectStandardInput = true,
                    RedirectStandardOutput = false,
                    UseShellExecute = false,
                    CreateNoWindow = true
                }
            };

            process1.Start();

            process2.Start();
            using (var writer = process2.StandardInput)
            {
                process1.StandardOutput.BaseStream.CopyTo(writer.BaseStream);
            }

            await process1.WaitForExitAsync();
            await process2.WaitForExitAsync();

            // Move original to trash and write repaired file back
            using (var original = await OpenReadAsync(fileId))
                await trash.MoveToTrashAsync($"{fileId}.manager", original);
            await DeleteAsync(fileId);
            using (var repaired = new FileStream(output, FileMode.Open, FileAccess.Read))
                await fs.WriteAsync(key, repaired);

            try { File.Delete(input); } catch { }
            try { File.Delete(output); } catch { }
        }

        private static string GetSqlitePath()
        {
            var sqlite = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Tools", $"{GetPlatform()}-{GetArchitecture()}", "sqlite3");
            if (GetPlatform() == "win") sqlite += ".exe";
            return sqlite;
        }

        private static string GetPlatform()
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows)) return "win";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux)) return "linux";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX)) return "osx";
            return string.Empty;
        }

        private static string GetArchitecture()
        {
            switch (RuntimeInformation.ProcessArchitecture)
            {
                case Architecture.X64: return "x64";
                case Architecture.Arm64: return "arm64";
                default: return string.Empty;
            }
        }

        private async Task<string> GetUniqueKey(string name)
        {
            var key = $"Businesses/{name}.manager";
            var index = 1;
            while (await fs.ExistsAsync(key))
            {
                index++;
                key = $"Businesses/{name}({index}).manager";
            }
            return key;
        }

        internal Orm.SQLiteConnection SQLiteConnection(string fileId)
        {
            var path = fs.GetFullPathAsync($"Businesses/{fileId}.manager").GetAwaiter().GetResult();

            var conn = new Orm.SQLiteConnection(path);
            conn.BusyTimeout = TimeSpan.FromSeconds(5);
#if DEBUG
            conn.Trace = true;
            conn.Tracer = s => Console.WriteLine(s);
#endif
            return conn;
        }
    }
}
