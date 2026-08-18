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
        private static async Task<IEnumerable<Model.Object>> Upgrade286(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();
            var receiptCustomFields = objects.OfType<ManagerServer.Model.CustomField>().Where(x => x.Obsolete_FormType == ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Receipt)) && !string.IsNullOrWhiteSpace(x.Name)).ToDictionary(x => x.Key);
            var paymentCustomFields = objects.OfType<ManagerServer.Model.CustomField>().Where(x => x.Obsolete_FormType == ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.Payment)) && !string.IsNullOrWhiteSpace(x.Name)).GroupBy(x => x.Name).ToDictionary(x => x.Key, x => x.First());

            foreach (var e in objects.OfType<ManagerServer.Model.Payment>().ToArray())
            {
                if (e.CustomFields == null) continue;
                if (e.CustomFields.Count == 0) continue;
                foreach (var e2 in e.CustomFields.ToArray())
                {
                    if (receiptCustomFields.ContainsKey(e2.Key))
                    {
                        var receiptCustomField = receiptCustomFields[e2.Key];
                        if (paymentCustomFields.ContainsKey(receiptCustomField.Name))
                        {
                            var paymentCustomField = paymentCustomFields[receiptCustomField.Name];
                            if (paymentCustomField.Type == receiptCustomField.Type)
                            {
                                if (!e.CustomFields.ContainsKey(paymentCustomField.Key))
                                {
                                    e.CustomFields.Add(paymentCustomField.Key, e2.Value);
                                    list.Add(e);
                                }
                            }
                        }
                    }
                }
            }

            return list.Distinct();

        }
    }
}
