using PuppeteerSharp;
using System;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace ManagerServer.Pdf
{
    internal static class BrowserLocator
    {
        public static async Task<string> ResolveExecutablePathAsync()
        {
            var systemPath = FindSystemBrowser();
            if (systemPath != null) return systemPath;

            var fetcher = new BrowserFetcher();
            var installed = fetcher.GetInstalledBrowsers().FirstOrDefault();
            if (installed == null)
                installed = await fetcher.DownloadAsync();
            return installed.GetExecutablePath();
        }

        private static string FindSystemBrowser()
        {
            string[] candidates;

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                var programFiles = Environment.GetEnvironmentVariable("ProgramFiles") ?? "";
                var programFilesX86 = Environment.GetEnvironmentVariable("ProgramFiles(x86)") ?? "";
                var localAppData = Environment.GetEnvironmentVariable("LocalAppData") ?? "";
                candidates = new[]
                {
                    Path.Combine(programFiles, @"Google\Chrome\Application\chrome.exe"),
                    Path.Combine(programFilesX86, @"Google\Chrome\Application\chrome.exe"),
                    Path.Combine(localAppData, @"Google\Chrome\Application\chrome.exe"),
                    Path.Combine(programFiles, @"Microsoft\Edge\Application\msedge.exe"),
                    Path.Combine(programFilesX86, @"Microsoft\Edge\Application\msedge.exe"),
                };
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            {
                var home = Environment.GetEnvironmentVariable("HOME") ?? "";
                candidates = new[]
                {
                    "/Applications/Google Chrome.app/Contents/MacOS/Google Chrome",
                    "/Applications/Chromium.app/Contents/MacOS/Chromium",
                    "/Applications/Microsoft Edge.app/Contents/MacOS/Microsoft Edge",
                    Path.Combine(home, "Applications/Google Chrome.app/Contents/MacOS/Google Chrome"),
                };
            }
            else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                candidates = new[]
                {
                    "/usr/bin/google-chrome",
                    "/usr/bin/google-chrome-stable",
                    "/usr/bin/chromium",
                    "/usr/bin/chromium-browser",
                    "/usr/bin/microsoft-edge",
                    "/usr/bin/microsoft-edge-stable",
                    "/snap/bin/chromium",
                    "/opt/google/chrome/chrome",
                    "/opt/microsoft/msedge/msedge",
                };
            }
            else
            {
                return null;
            }

            foreach (var path in candidates)
            {
                if (!string.IsNullOrEmpty(path) && File.Exists(path))
                    return path;
            }

            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                var pathVar = Environment.GetEnvironmentVariable("PATH");
                if (!string.IsNullOrEmpty(pathVar))
                {
                    var names = new[] { "google-chrome", "google-chrome-stable", "chromium", "chromium-browser", "microsoft-edge", "microsoft-edge-stable" };
                    foreach (var dir in pathVar.Split(Path.PathSeparator))
                    {
                        foreach (var name in names)
                        {
                            var full = Path.Combine(dir, name);
                            if (File.Exists(full)) return full;
                        }
                    }
                }
            }

            return null;
        }
    }
}
