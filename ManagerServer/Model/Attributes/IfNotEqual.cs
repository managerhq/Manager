using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model.Attributes
{
    public sealed class IfNotEqual : IfAttribute
    {
        private string path1;
        private string path2;

        public IfNotEqual(string path1, string path2)
        {
            this.path1 = path1;
            this.path2 = path2;
        }

        public override string GetIfExpression()
        {
            var expression1 = string.Empty;
            foreach (var e in path1.Split('.'))
            {
                if (string.IsNullOrWhiteSpace(expression1)) expression1 = "this."+e;
                else expression1 = "(" + expression1 + " || {})."+e;
            }

            var expression2 = string.Empty;
            foreach (var e in path2.Split('.'))
            {
                if (string.IsNullOrWhiteSpace(expression2)) expression2 = "this." + e;
                else expression2 = "(" + expression2 + " || {})." + e;
            }

            return $"{expression1} != {expression2}";
        }
    }
}
