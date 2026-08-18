global using ManagerServer.Attributes;
global using ProtoBuf;
global using System;
using System.IO;
using System.Threading.Tasks;

namespace ManagerServer
{
    public static class Program
    {
        public static async Task Main(string[] args)
        {
            // Handle legacy parameters
            var portIndex = Array.IndexOf(args, "-port");
            if (portIndex >= 0 && portIndex + 1 < args.Length)
            {
                var port = args[portIndex + 1];
                args[portIndex] = "--urls";
                args[portIndex + 1] = $"http://*:{port}";
            }

#if DEBUG
            //Environment.SetEnvironmentVariable("MANAGER_BANNER", @"Payment overdue for <b>abc.manager.io</b>. Please <a href=""#"" target=""_blank""><b>click here</b></a> to complete your payment and avoid service interruption. If payment has already been made, you may safely dismiss this message.");
#endif

            InheritedProtoMemberAttribute.AddInheritedMembersIn(ProtoBuf.Meta.RuntimeTypeModel.Default);
            ProtoBuf.Meta.RuntimeTypeModel.Default.MetadataTimeoutMilliseconds = 5000; // https://stackoverflow.com/questions/7372585/protobuf-net-exception-timeout-while-inspecting-metadata

            var dir = System.IO.Path.Combine(Environment.GetFolderPath(System.Environment.SpecialFolder.MyDocuments), "Manager.io");

            Console.WriteLine("Manager Server [Version " + typeof(Program).Assembly.GetName().Version.ToString() + @"]");
            Console.WriteLine();

            var managerServer = "ManagerServer.exe ";
            if (Environment.OSVersion.Platform != PlatformID.Win32NT) managerServer = "ManagerServer ";

            Console.WriteLine("Syntax:");
            Console.WriteLine();
            Console.WriteLine($"    {managerServer} [options]");
            Console.WriteLine();

            Console.WriteLine("Options:");
            Console.WriteLine();
            Console.WriteLine("    --urls <binding>      Kestrel URL(s). Default: http://localhost:5000");
            Console.WriteLine("                          Multiple values separated by ';'.");
            Console.WriteLine($"    --path <directory>    Data directory. Default: {dir}");
            Console.WriteLine("    --smtp <uri>          SMTP server for password reset emails.");
            Console.WriteLine("                          Example: smtp://user:pass@host:587?from=noreply@example.com");
            Console.WriteLine();

            Console.WriteLine("Examples:");
            Console.WriteLine();
            Console.WriteLine($"    {managerServer}");
            Console.WriteLine($"    {managerServer} --urls http://localhost:80");
            Console.WriteLine($@"    {managerServer} --urls http://*:80 --path ""{dir}""");
            Console.WriteLine();

            var pathIndex = Array.IndexOf(args, "--s3");
            var pathValue = pathIndex >= 0 && pathIndex + 1 < args.Length ? args[pathIndex + 1] : null;

            var directory = DirectoryResolver.Resolve();
            if (!Directory.Exists(directory)) Directory.CreateDirectory(directory);
            var fileSystem = new FileSystem(directory);
            var s3 = S3.FromUri(pathValue);
            ApplicationData.Instance = new ApplicationData(fileSystem, s3);

            Console.WriteLine("Data directory: " + directory);
            Console.WriteLine();

            Console.WriteLine("Default username is \"administrator\". If you forgot the password, reset it by");
            Console.WriteLine("deleting " + Path.Combine(directory, "Users", "administrator"));
            Console.WriteLine();

            await HttpServer.Run(args);
        }
    }
}