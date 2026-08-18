#if DEBUG
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Threading.Tasks;
using ProtoBuf;
using System.IO;

namespace ManagerServer.HttpHandlers.Meta
{
    [ProtoContract]
    internal sealed class OrphanedStrings : HttpHandler
    {
        public override Task Get()
        {
            var solutionDir = new DirectoryInfo(AppDomain.CurrentDomain.BaseDirectory).Parent.Parent.Parent.Parent;
            var files = solutionDir.GetFiles("*.cs", SearchOption.AllDirectories);

            var content = string.Join(Environment.NewLine, files.SelectMany(x => File.ReadAllLines(x.FullName).Where(x => x.Contains(nameof(ManagerServer.Globalization.Strings)))));
            var fields = new HashSet<string>(typeof(ManagerServer.Model.Object).Assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(ManagerServer.Model.Object)) && x.Namespace == "Manager.Model").SelectMany(x => x.GetFieldsAndProperties(BindingFlags.Instance | BindingFlags.Public)).Select(x => x.Name).Distinct());
            var fields2 = new HashSet<string>(typeof(ManagerServer.Model.Object).Assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(ManagerServer.Model.Object)) && x.Namespace == "Manager.Model").SelectMany(x => x.GetNestedTypes()).SelectMany(x => x.GetFieldsAndProperties(BindingFlags.Instance | BindingFlags.Public)).Select(x => x.Name).Distinct());
            var classes = new HashSet<string>(typeof(ManagerServer.Model.Object).Assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(ManagerServer.Model.Object)) && x.Namespace == "Manager.Model").Select(x => x.Name));
            var classes2 = new HashSet<string>(classes.Select(x => $"New{x}"));
            var enums = new HashSet<string>(typeof(ManagerServer.Model.Object).Assembly.GetTypes().Where(x => x.IsEnum && x.Namespace == "Manager.Model.Enums").SelectMany(x => x.GetEnumValues().Cast<object>().Select(x => x.ToString())).Distinct());
            var enums2 = new HashSet<string>(typeof(ManagerServer.Model.Object).Assembly.GetTypes().Where(x => x.IsSubclassOf(typeof(ManagerServer.Model.Object)) && x.Namespace == "Manager.Model").SelectMany(x => x.GetNestedTypes().Where(x => x.IsEnum)).SelectMany(x => x.GetEnumValues().Cast<object>().Select(x => x.ToString())).Distinct());

            var table2 = typeof(HttpHandler).Assembly.GetTypes().FirstOrDefault(x => x.Name == "Table");
            var tables2 = new HashSet<string>(table2.Assembly.GetTypes().Where(x => !x.IsAbstract && x.IsSubclassOf(table2)).Select(x => x.Name));

            var table3 = typeof(HttpHandler).Assembly.GetTypes().FirstOrDefault(x => x.Name == "NakedNamespaces");
            var tables3 = new HashSet<string>(table3.Assembly.GetTypes().Where(x => !x.IsAbstract && x.IsSubclassOf(table3)).Select(x => x.Name));

            var strings = typeof(ManagerServer.Globalization.Strings).GetProperties(BindingFlags.Static | BindingFlags.Public);

            using (Html())
            {
                using (Head()) { }
                using (Body())
                {
                    using (H1()) Write("Orphaned Strings");
                    using (Ul())
                    {
                        foreach (var e in strings.OrderBy(x => x.Name))
                        {
                            if (fields.Contains(e.Name)) continue;
                            if (fields2.Contains(e.Name)) continue;
                            if (enums.Contains(e.Name)) continue;
                            if (enums2.Contains(e.Name)) continue;
                            if (classes.Contains(e.Name)) continue;
                            if (classes2.Contains(e.Name)) continue;
                            if (tables2.Contains(e.Name)) continue;
                            if (tables3.Contains(e.Name)) continue;

                            var key = $"{nameof(ManagerServer.Globalization.Strings)}.{e.Name}";
                            if (content.Contains(key)) continue;

                            using (Li()) Write($"Not found: {e.Name}");
                        }
                    }

                    using (H1()) Write("Stats");
                    using (Ul())
                    {
                        using (Li()) Write(solutionDir.FullName);
                        using (Li()) Write($"{files.Length} files");
                        using (Li()) Write($"{content.Length} content length");
                    }
                }
            }

            return Task.CompletedTask;
        }
    }
}
#endif