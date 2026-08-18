using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace ManagerServer
{
    static class DirectoryResolver
    {
        internal static string Resolve()
        {
            var args = Environment.GetCommandLineArgs();

            var pathIndex = Array.IndexOf(args, "-path");
            if (pathIndex != -1)
            {
                return Path.TrimEndingDirectorySeparator(args[pathIndex + 1]);
            }

            var pathIndex2 = Array.IndexOf(args, "--path");
            if (pathIndex2 != -1)
            {
                return Path.TrimEndingDirectorySeparator(args[pathIndex2 + 1]);
            }

            var prefix = "--path=";
            if (args.Any(x => x.StartsWith(prefix)))
            {
                return Path.TrimEndingDirectorySeparator(args.First(x => x.StartsWith(prefix)).Substring(prefix.Length));
            }

            if (args.Length > 1 && !string.IsNullOrWhiteSpace(args[1]))
            {
                var directory = Path.GetDirectoryName(args[1]);
                if (Directory.Exists(directory)) return Path.TrimEndingDirectorySeparator(directory);
            }

            var probingDirectory = GetProbingDirectory();

            var customDirPath = Path.Combine(probingDirectory, "data");
            if (File.Exists(customDirPath))
            {
                var customDir = File.ReadAllText(customDirPath);
                if (Directory.Exists(customDir))
                {
                    return customDir;
                }
            }

            return probingDirectory;
        }

        private static string GetFolderPath(System.Environment.SpecialFolder specialFolder, string subfolder)
        {
            var specialFolderPath = Environment.GetFolderPath(specialFolder, Environment.SpecialFolderOption.DoNotVerify);
            if (string.IsNullOrWhiteSpace(specialFolderPath)) return null;
            return Path.Combine(specialFolderPath, subfolder);
        }

        private static string[] GetProbingDirectories()
        {
            var probingDirs = new List<string>
            {
                GetFolderPath(System.Environment.SpecialFolder.MyDocuments, "Manager.io"),
                GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData, "Manager")
            };

            // SpecialFolder.LocalApplicationData has changed on Mac OS X
            var localApplicationData = Environment.GetFolderPath(System.Environment.SpecialFolder.LocalApplicationData, Environment.SpecialFolderOption.DoNotVerify);
            if (localApplicationData.EndsWith("/Library/Application Support/Manager"))
            {
                var legacyBaseDirectory = localApplicationData.Replace("/Library/Application Support/Manager", "/.local/share/Manager");
                probingDirs.Add(legacyBaseDirectory);
            }

            return probingDirs.Where(x => !string.IsNullOrWhiteSpace(x)).ToArray();
        }

        private static string GetProbingDirectory()
        {
            var probingDirectories = GetProbingDirectories();
            foreach (var e in probingDirectories)
            {
                try
                {
                    if (Directory.Exists(e))
                    {
                        var customDirPath = Path.Combine(e, "data");
                        if (File.Exists(customDirPath)) return e;
                        if (Directory.Exists(Path.Combine(e, "Businesses")) && Directory.GetFiles(Path.Combine(e, "Businesses"), "*.manager").Any()) return e;
                        if (Directory.GetFiles(e, "*.manager").Any()) return e;
                    }
                }
                catch (System.UnauthorizedAccessException)
                {
                }
            }

            return probingDirectories.First();
        }
    }
}
