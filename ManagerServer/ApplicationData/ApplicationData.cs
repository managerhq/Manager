using ManagerServer.Storage;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerServer
{
    public partial class ApplicationData(FileSystem fileSystem, S3 s3)
    {
        public static ApplicationData Instance { get; internal set; }

        internal readonly Storage.Storage Storage = new Storage.Storage(s3 as IFileSystem ?? fileSystem);
        internal readonly Trash Trash = new Trash(s3 as IFileSystem ?? fileSystem);
        internal readonly Businesses Businesses = new Businesses(fileSystem);
        internal readonly Users Users = new Users(s3 as IFileSystem ?? fileSystem);
        internal readonly Assets Assets = new Assets(s3 as IFileSystem ?? fileSystem);

        // This will be eventually moved out of ApplicationData and work with basic directory access
        public async Task TryUpgrade()
        {
            await TryUpgrade1();
            await TryUpgrade2();
        }

        private async Task TryUpgrade1()
        {
            var anyBusinesses = await fileSystem.GetKeysAsync("Businesses/");
            if (anyBusinesses.Length == 0)
            {
                var files = await fileSystem.GetKeysAsync("");
                var keys = files.Where(k => k.EndsWith(".manager") && !k.Contains("/")).ToArray();
                if (keys.Length == 0) return;
                foreach (var key in keys)
                {
                    var name = Path.GetFileName(key);
                    if (Path.GetFileNameWithoutExtension(name) == Guid.Empty.ToString("N")) continue;
                    fileSystem.MoveAsync(key, $"Businesses/{name}");
                }
            }
        }

        private async Task TryUpgrade2()
        {
            if (s3 != null) return;
            var anyUsers = await fileSystem.GetKeysAsync("Users/");
            if (anyUsers.Length == 0)
            {
                var oldFile = await fileSystem.ExistsAsync($"{Guid.Empty:N}.manager");
                if (oldFile)
                {
                    var fullPath = await fileSystem.GetFullPathAsync($"{Guid.Empty:N}.manager");
                    using (var db = new Orm.SQLiteConnection(fullPath))
                    {
                        var users = db.OfType<ManagerServer.Model.Obsolete.Obsolete88.User>();
                        if (users == null) return;

                        foreach (var oldUser in users)
                        {
                            if (string.IsNullOrWhiteSpace(oldUser.Username)) continue;

                            var userRecord = new UserRecord
                            {
                                Name = oldUser.Name,
                                EmailAddress = oldUser.EmailAddress,
                                Username = oldUser.Username,
                                Password = oldUser.Password,
                                Type = oldUser.Type,
                                Businesses = oldUser.Businesses,
                                Session = oldUser.Session,
                                MultifactorAuthentication = oldUser.MultifactorAuthentication,
                                Verified = oldUser.Verified
                            };

                            if (oldUser.Sessions != null)
                            {
                                userRecord.Sessions = oldUser.Sessions.Select(s => new UserSession
                                {
                                    Key = s.Key,
                                    Timestamp = s.Timestamp,
                                    UserAgent = s.UserAgent,
                                    Location = s.Location
                                }).ToList();
                            }

                            await Users.Save(userRecord);
                        }
                    }
                }

                var oldPasswordFile = await fileSystem.ExistsAsync("password");
                if (oldPasswordFile)
                {
                    using var stream = await fileSystem.ReadAsync("password");
                    if (stream != null)
                    {
                        using var reader = new StreamReader(stream);
                        var passwordText = await reader.ReadToEndAsync();
                        if (!string.IsNullOrWhiteSpace(passwordText))
                        {
                            var record = new UserRecord
                            {
                                Username = "administrator",
                                Name = "Administrator",
                                Password = passwordText,
                                Type = Model.UserType.Administrator
                            };

                            await Users.Save(record);
                        }
                    }
                }
            }
        }
    }
}