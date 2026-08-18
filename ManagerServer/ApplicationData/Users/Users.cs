using System.Collections.Concurrent;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ManagerServer
{
    sealed class Users(IFileSystem fs)
    {
        readonly ConcurrentDictionary<string, UserRecord> cache = new ConcurrentDictionary<string, UserRecord>();

        static bool IsValidUsername(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return false;
            if (username == "." || username == "..") return false;
            var path = Path.Combine(Path.GetTempPath(), username);

            return Path.TrimEndingDirectorySeparator(Path.GetDirectoryName(path)) == Path.TrimEndingDirectorySeparator(Path.GetTempPath());
        }

        internal UserRecord GetDesktopUser()
        {
            return cache.GetOrAdd(Environment.UserName, username => new UserRecord
            {
                Username = username,
                Name = username,
                Type = Model.UserType.Administrator
            });
        }

        internal async Task<UserRecord> GetByUsernameAsync(string username)
        {
            if (string.IsNullOrWhiteSpace(username)) return null;
            if (!cache.ContainsKey(username))
            {
                if (!IsValidUsername(username)) return null;
                using (var stream = await fs.ReadAsync($"Users/{username}"))
                {
                    var found = false;
                    if (stream != null)
                    {
                        var user = Serializer.Deserialize<UserRecord>(stream);
                        if (user != null)
                        {
                            cache[user.Username] = user;
                            found = true;
                        }
                    }
                    
                    if (!found && username == "administrator")
                    {
                        cache[username] = new UserRecord()
                        {
                            Name = "Administrator",
                            Username = "administrator"
                        };
                    }
                }
            }

            var cachedUser = cache.GetValueOrDefault(username);
            if (cachedUser?.Username == "administrator")
            {
                cachedUser.Type = Model.UserType.Administrator;

                var hash = Environment.GetEnvironmentVariable("MANAGER_ADMINISTRATOR_PASSWORD_HASH", EnvironmentVariableTarget.Process);
                if (string.IsNullOrWhiteSpace(cachedUser.Password)) cachedUser.Password = hash;

                var email = Environment.GetEnvironmentVariable("MANAGER_ADMINISTRATOR_EMAIL", EnvironmentVariableTarget.Process);
                if (!string.IsNullOrWhiteSpace(email)) cachedUser.EmailAddress = email;
            }
            return cachedUser;
        }

        internal async Task<UserRecord> GetBySessionAsync(string session)
        {
            var userCookie = UserCookie.Deserialize(session);
            if (userCookie != null)
            {
                var userRecord = await GetByUsernameAsync(userCookie.Username);
                if (userRecord != null)
                {
                    var userSession = userRecord.Sessions?.FirstOrDefault(x => x.Key == userCookie.UserSession);
                    if (userSession != null)
                    {
                        if (userRecord.Type == Model.UserType.Administrator)
                        {
                            var impersonatedUserRecord = await GetByUsernameAsync(userCookie.OnBehalfOf);
                            if (impersonatedUserRecord != null)
                            {
                                return impersonatedUserRecord;
                            }
                        }
                        return userRecord;
                    }
                }
            }
            return null;
        }

        internal async Task<string> CreateSession(UserRecord userRecord)
        {
            userRecord.Sessions ??= new System.Collections.Generic.List<UserSession>();
            var userSession = new UserSession() { Key = Guid.CreateVersion7() };
            userRecord.Sessions.Add(userSession);
            await Save(userRecord);
            var userCookie = new UserCookie()
            {
                Username = userRecord.Username,
                UserSession = userSession.Key
            };
            return userCookie.Serialize();
        }

        internal async Task<bool> AnyExist()
        {
            var users = await GetAllAsync();
            return users.Length > 1;
        }

        internal async Task Save(UserRecord user)
        {
            if (user == null) return;
            if (!IsValidUsername(user.Username)) return;

            cache[user.Username] = user;

            using (var ms = new MemoryStream())
            {
                Serializer.Serialize(ms, user);
                ms.Position = 0;
                await fs.WriteAsync($"Users/{user.Username}", ms);
            }
        }

        internal async Task Delete(string username)
        {
            cache.TryRemove(username, out _);

            if (IsValidUsername(username))
            {
                await fs.DeleteAsync($"Users/{username}");
            }
        }

        internal async Task<UserRecord[]> GetAllAsync()
        {
            var keys = await fs.GetKeysAsync("Users/");
            if (!keys.Contains("Users/administrator")) keys = keys.Concat(["Users/administrator"]).ToArray();
            var users = await Task.WhenAll(keys.Select(key => GetByUsernameAsync(key.Split('/').Last())));
            return users.Where(u => u != null).ToArray();
        }
    }
}
