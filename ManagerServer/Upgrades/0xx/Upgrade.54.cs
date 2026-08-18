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
        private static async Task<IEnumerable<Model.Object>> Upgrade54(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<Model.Object>();
            var plugins = objects.OfType<Model.Obsolete.Obsolete08.Plugin08>().ToDictionary(x => x.Key);
            var viewTemplate = objects.OfType<Model.Obsolete.Obsolete09.SalesInvoiceTemplate09>().FirstOrDefault(x => x.Key == new Guid("0b96900e-552d-4d14-a070-9086e44f188d"));
            if (viewTemplate != null)
            {
                if (!string.IsNullOrWhiteSpace(viewTemplate.Obsolete_Australia_ABN_Number)) viewTemplate.TaxIdentifier = "ABN " + viewTemplate.Obsolete_Australia_ABN_Number;
                if (!string.IsNullOrWhiteSpace(viewTemplate.Obsolete_NewZealand_GST_Number)) viewTemplate.TaxIdentifier = "GST " + viewTemplate.Obsolete_NewZealand_GST_Number;
                if (!string.IsNullOrWhiteSpace(viewTemplate.Obsolete_Philippines_TIN_Number)) viewTemplate.TaxIdentifier = "TIN " + viewTemplate.Obsolete_Philippines_TIN_Number;
                if (!string.IsNullOrWhiteSpace(viewTemplate.Obsolete_SouthAfrica_VAT_Number)) viewTemplate.TaxIdentifier = "VAT " + viewTemplate.Obsolete_SouthAfrica_VAT_Number;
                if (plugins.ContainsKey(ManagerServer.Model.Obsolete.Obsolete08.Plugins08.Obsolete_AustraliaGst)) viewTemplate.SalesInvoiceTitle = "Tax Invoice";
                else if (plugins.ContainsKey(ManagerServer.Model.Obsolete.Obsolete08.Plugins08.Obsolete_SouthAfricaVat)) viewTemplate.SalesInvoiceTitle = "Tax Invoice";
                else if (plugins.ContainsKey(ManagerServer.Model.Obsolete.Obsolete08.Plugins08.Obsolete_NewZealandGst)) viewTemplate.SalesInvoiceTitle = "Tax Invoice";
                else if (plugins.ContainsKey(ManagerServer.Model.Obsolete.Obsolete08.Plugins08.Obsolete_IndiaCentralStateTax)) viewTemplate.SalesInvoiceTitle = "Tax Invoice";
                else if (plugins.ContainsKey(ManagerServer.Model.Obsolete.Obsolete08.Plugins08.Obsolete_IndiaServiceTax)) viewTemplate.SalesInvoiceTitle = "Tax Invoice";
                list.Add(viewTemplate);
            }
            return list;
        }
    }
}
