using Microsoft.Extensions.FileProviders;

namespace ManagerServer
{
    // Translations and localizations ship inside the executable as embedded resources so that a
    // published build is a single file with nothing to copy alongside it. The manifest produced by
    // GenerateEmbeddedFilesManifest keeps their original relative paths, which is what allows a
    // directory to still be listed — language discovery depends on that.
    internal static class EmbeddedAssets
    {
        private static readonly IFileProvider provider = Create();

        private static IFileProvider Create()
        {
            try
            {
                return new ManifestEmbeddedFileProvider(typeof(EmbeddedAssets).Assembly, "Assets");
            }
            catch
            {
                // No manifest means no embedded assets. Callers already handle their absence.
                return new NullFileProvider();
            }
        }

        internal static IFileInfo GetFile(string path)
        {
            return provider.GetFileInfo(path);
        }

        internal static IDirectoryContents GetDirectory(string path)
        {
            return provider.GetDirectoryContents(path);
        }
    }
}
