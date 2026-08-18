using System.Collections.Generic;
using System.Linq;
using System.Text;
using HttpFramework;

namespace ManagerServer
{
    public static class Assembly
    {
        private static Dictionary<string, Type> httpHandlers = new Dictionary<string,Type>();

        static Assembly()
        {
            foreach (var type in typeof(Program).Assembly.GetTypes().Where(x => x.IsClass && !x.IsAbstract && !x.IsNested))
            {
                if (!type.IsSubclassOf(typeof(HttpHandler))) continue;
                if (type.GetConstructor(Type.EmptyTypes) == null) continue;
                var sb = new StringBuilder();
                foreach (var e in type.Name)
                {
                    if (e == '_')
                    {
                        sb.Append('.');
                    }
                    else
                    {
                        if (char.IsUpper(e) && sb.Length > 0) sb.Append('-');
                        sb.Append(char.ToLowerInvariant(e));
                    }
                }
                var key = sb.ToString();
                if (httpHandlers.ContainsKey(key)) throw new Exception(key);
                httpHandlers.Add(key, type);
                if (type.Name == "Default") httpHandlers.Add(string.Empty, type);
            }
        }

        public static bool ContainsHttpHandler(string key)
        {
            return httpHandlers.ContainsKey(key.Split('.')[0]);
        }

        public static Type GetHttpHandlerType(string key)
        {
            return httpHandlers[key.Split('.')[0]];
        }

        public static Type GetHttpHandlerTypeByCamelCaseKey(string key)
        {
            return httpHandlers.Values.SingleOrDefault(x => x.Name == key);
        }
    }
}