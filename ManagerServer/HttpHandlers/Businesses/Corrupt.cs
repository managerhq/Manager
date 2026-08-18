using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ProtoBuf;

namespace ManagerServer.HttpHandlers.Businesses
{
    [ProtoContract]
    [Title(nameof(Strings.CorruptDatabase))]
    [Guide("Manager uses SQLite databases, which are generally robust but can become corrupted due to hardware malfunctions or rogue programs.")]
    [Guide("The easiest way to manually recover a corrupt database is using the Command Line Interface (CLI) for SQLite. The CLI is a program named `sqlite3`.")]
    [Guide("This guide will walk you through the process of recovering a corrupt database file.")]
    [Header("Step 1: Download SQLite CLI")]
    [Guide("Download SQLite CLI from the [SQLite Download page](https://www.sqlite.org/download.html).")]
    [Guide("Download the precompiled binaries for your operating system:")]
    [Guide("• For Windows, look for `sqlite-tools-win-x64-*.zip`")]
    [Guide("• For macOS, look for `sqlite-tools-osx-x64-*.zip`")]
    [Guide("• For Linux, look for `sqlite-tools-linux-x64-*.zip`")]
    [Header("Step 2: Unzip the Downloaded Archive")]
    [Guide("Extract the contents of the downloaded zip file into a new folder.")]
    [Header("Step 3: Prepare the Corrupt Database File")]
    [Guide("Copy your corrupted SQLite database into the folder with the unzipped contents.")]
    [Guide("Rename your `.manager` file to `corrupted.manager`.")]
    [Header("Step 4: Run the Recovery Command")]
    [Guide("Open a command line interface (`Command Prompt` on Windows, `Terminal` on macOS/Linux).")]
    [Guide("Navigate to the folder containing the `sqlite3` executable and the `corrupted.manager` file.")]
    [Guide("Run the following command to attempt recovery:")]
    [Guide(@"`sqlite3 corrupted.manager "".recover"" | sqlite3 new.manager`")]
    [Header("Step 5: Import and Open the Recovered Database")]
    [Guide("After the recovery command completes, you will have a new file named `new.manager`.")]
    [Guide("Import `new.manager` back into Manager and attempt to open it.")]
    [LinkGuide("Learn more:", typeof(ImportBusiness))]
    [Guide("Following these steps can help you recover data from a corrupt Manager database file.")]
    internal sealed class Corrupt : Template
    {
        [ProtoMember(1)] public string Business;

        protected override Task InnerGet()
        {
            using (Div(@class: "p-8 mx-auto max-w-prose"))
            {
                using (Div(@class: "card"))
                {
                    using (Div(@class: "card-body p-8"))
                    {
                        using (Div(@class: "flex flex-col space-y-4"))
                        {
                            using (Div(@class: "text-xl font-bold")) Write(Strings.CorruptDatabase);

                            Hr();

                            using (Div(@class: "font-semibold")) Write("This business database is corrupted");

                            if (ApplicationData.Businesses.CanRepair())
                            {
                                using (Div(@class: "flex flex-col space-y-4"))
                                {
                                    using (Div()) Write("You can attempt to repair database.");

                                    using (Div())
                                    {
                                        FormPrimaryButton("Repair");
                                    }
                                }
                            }
                            else
                            {
                                using (Div()) Write("Manager database is SQLite database. When your business database is corrupted, we recommend you restore earlier non-corrupted copy from the backup.");

                                using (Div()) using (DefaultLink(new Businesses().ToUrl())) Write(Strings.Back);
                            }
                        }
                    }
                }
            }
            return Task.CompletedTask;
        }

        protected override async Task InnerPost()
        {
            await ApplicationData.Businesses.Repair(Business, ApplicationData.Trash);

            Response.Redirect(new Businesses().ToUrl());
        }
    }
}
