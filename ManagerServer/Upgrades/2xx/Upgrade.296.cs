using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;
using System.Text;
using System.IO;
using ManagerServer.Model;
using System.Reflection;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Obsolete;
using System.Threading.Tasks;

namespace ManagerServer
{
    public static partial class Upgrade
    {
        private static async Task<IEnumerable<Model.Object>> Upgrade296(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            var stp = objects.SingleOrDefault<ManagerServer.Model.Obsolete.Obsolete74.ReportTransformation>(new Guid("07332ba3-3e82-4dc1-9451-1350f5d84e24"));
            var customFieldKey = new Guid("b8c661cd-ff56-4862-853b-75d0f2920776");
            if (stp != null)
            {
                list.Add(new CustomField()
                {
                    Key = customFieldKey,
                    Name = "STP - Payroll number",
                    Position = 401,
                    Type = CustomFieldStyle.SingleLineText,
                    Size = CustomFieldSize.Small,
                    Description = "A unique number that identifies an employee for Single Touch Payroll",
                    Obsolete_FormType = new Guid("dadb7f95-a5dd-45c0-945d-6ad4ee28776e")
                });
                foreach (var e in objects.OfType<ManagerServer.Model.Employee>())
                {
                    if (e.CustomFields == null) e.CustomFields = new Dictionary<Guid, string>();
                    if (!string.IsNullOrWhiteSpace(e.Code)) e.CustomFields[customFieldKey] = e.Code;
                    else e.CustomFields[customFieldKey] = e.Key.ToString();
                }
            }
            return list;
        }
    }
}
