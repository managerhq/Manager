using System;
using System.IO;
using System.Linq;

namespace ManagerServer
{
    internal static class Edition
    {
        internal static bool IsDesktop { get; private set; }
        internal static bool IsServer { get; private set; }

        static Edition()
        {
            if (Environment.GetCommandLineArgs().Contains("--edition=desktop"))
            {
                IsDesktop = true;
            }
            else
            {
                IsServer = true;
            }
        }
    }
}