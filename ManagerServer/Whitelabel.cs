using System;

namespace ManagerServer
{
    internal static class Whitelabel
    {
        internal static bool IsEnabled { get; private set; }

        static Whitelabel()
        {
            var args = Environment.GetCommandLineArgs();
            IsEnabled = (Array.IndexOf(args, "-whitelabel") != -1) || (Array.IndexOf(args, "--whitelabel") != -1);
        }
    }
}
