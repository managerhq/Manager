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
        private static async Task<IEnumerable<Model.Object>> Upgrade181(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var list = new List<ManagerServer.Model.Object>();

            var salesOrders = objects.OfType<Model.SalesOrder>().ToArray();
            if (salesOrders.Any())
            {
                var authorizedBy = salesOrders.Any(x => !string.IsNullOrWhiteSpace(x.Obsolete_AuthorizedBy));
                var deliveryAddress = salesOrders.Any(x => !string.IsNullOrWhiteSpace(x.Obsolete_DeliveryAddress));
                var deliveryDate = salesOrders.Any(x => x.Obsolete_DeliveryDate.HasValue);
                var deliveryInstructions = salesOrders.Any(x => !string.IsNullOrWhiteSpace(x.Obsolete_DeliveryInstructions));

                var authorizedByCustomFieldKey = new Guid("472334c3-23a5-45ee-a1e5-771e4a7fb4ea");
                var deliveryAddressCustomFieldKey = new Guid("89afb50d-e740-454d-bd5f-3a1790e72514");
                var deliveryDateCustomFieldKey = new Guid("3c41c4c6-9437-4116-b1cd-b9184a399a22");
                var deliveryInstructionsCustomFieldKey = new Guid("471a38fe-cf45-4f73-bf56-4c4722c1e891");

                if (deliveryDate)
                {
                    list.Add(new ManagerServer.Model.CustomField() { Key = deliveryDateCustomFieldKey, Position = 1, Name = Strings.DeliveryDate, Size = CustomFieldSize.Medium, Type = CustomFieldStyle.Date, Obsolete_FormType = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.SalesOrder)), DisplayOnView = true });
                }
                if (deliveryAddress)
                {
                    list.Add(new ManagerServer.Model.CustomField() { Key = deliveryAddressCustomFieldKey, Position = 2, Name = Strings.DeliveryAddress, Size = CustomFieldSize.Medium, Type = CustomFieldStyle.ParagraphText, Obsolete_FormType = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.SalesOrder)), DisplayOnView = true });
                }
                if (deliveryInstructions)
                {
                    list.Add(new ManagerServer.Model.CustomField() { Key = deliveryInstructionsCustomFieldKey, Position = 3, Name = Strings.DeliveryInstructions, Size = CustomFieldSize.Medium, Type = CustomFieldStyle.ParagraphText, Obsolete_FormType = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.SalesOrder)), DisplayOnView = true });
                }
                if (authorizedBy)
                {
                    list.Add(new ManagerServer.Model.CustomField() { Key = authorizedByCustomFieldKey, Position = 4, Name = "Authorized by", Size = CustomFieldSize.Medium, Type = CustomFieldStyle.SingleLineText, Obsolete_FormType = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.SalesOrder)), DisplayOnView = true });
                }

                foreach (var e in salesOrders)
                {
                    if (e.CustomFields == null) e.CustomFields = new Dictionary<Guid, string>();
                    var count = e.CustomFields.Count;
                    if (e.Obsolete_DeliveryDate.HasValue) e.CustomFields.Add(deliveryDateCustomFieldKey, e.Obsolete_DeliveryDate.Value.ToString("yyyy-MM-dd"));
                    if (!string.IsNullOrWhiteSpace(e.Obsolete_DeliveryAddress)) e.CustomFields.Add(deliveryAddressCustomFieldKey, e.Obsolete_DeliveryAddress);
                    if (!string.IsNullOrWhiteSpace(e.Obsolete_DeliveryInstructions)) e.CustomFields.Add(deliveryInstructionsCustomFieldKey, e.Obsolete_DeliveryInstructions);
                    if (!string.IsNullOrWhiteSpace(e.Obsolete_AuthorizedBy)) e.CustomFields.Add(authorizedByCustomFieldKey, e.Obsolete_AuthorizedBy);
                    if (e.CustomFields.Count > count) list.Add(e);
                }
            }

            var purchaseOrders = objects.OfType<Model.PurchaseOrder>().ToArray();
            if (purchaseOrders.Any())
            {
                var authorizedBy = purchaseOrders.Any(x => !string.IsNullOrWhiteSpace(x.Obsolete_AuthorizedBy));
                var deliveryAddress = purchaseOrders.Any(x => !string.IsNullOrWhiteSpace(x.Obsolete_DeliveryAddress));
                var deliveryDate = purchaseOrders.Any(x => x.Obsolete_DeliveryDate.HasValue);
                var deliveryInstructions = purchaseOrders.Any(x => !string.IsNullOrWhiteSpace(x.Obsolete_DeliveryInstructions));

                var authorizedByCustomFieldKey = new Guid("02d3d9cc-ef4a-4ca8-8a69-4ac20fc2ed2d");
                var deliveryAddressCustomFieldKey = new Guid("e027461f-01c1-4ab4-a164-bbd3b9a79d40");
                var deliveryDateCustomFieldKey = new Guid("02b6d503-d389-499a-b6c4-aa1ffda678ba");
                var deliveryInstructionsCustomFieldKey = new Guid("5494ef34-c376-4567-a6e4-87c56cc3f496");

                if (deliveryDate)
                {
                    list.Add(new ManagerServer.Model.CustomField() { Key = deliveryDateCustomFieldKey, Position = 1, Name = Strings.DeliveryDate, Size = CustomFieldSize.Medium, Type = CustomFieldStyle.Date, Obsolete_FormType = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.PurchaseOrder)), DisplayOnView = true });
                }
                if (deliveryAddress)
                {
                    list.Add(new ManagerServer.Model.CustomField() { Key = deliveryAddressCustomFieldKey, Position = 2, Name = Strings.DeliveryAddress, Size = CustomFieldSize.Medium, Type = CustomFieldStyle.ParagraphText, Obsolete_FormType = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.PurchaseOrder)), DisplayOnView = true });
                }
                if (deliveryInstructions)
                {
                    list.Add(new ManagerServer.Model.CustomField() { Key = deliveryInstructionsCustomFieldKey, Position = 3, Name = Strings.DeliveryInstructions, Size = CustomFieldSize.Medium, Type = CustomFieldStyle.ParagraphText, Obsolete_FormType = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.PurchaseOrder)), DisplayOnView = true });
                }
                if (authorizedBy)
                {
                    list.Add(new ManagerServer.Model.CustomField() { Key = authorizedByCustomFieldKey, Position = 4, Name = "Authorized by", Size = CustomFieldSize.Medium, Type = CustomFieldStyle.SingleLineText, Obsolete_FormType = ManagerServer.Model.Object.GetGuidByType(typeof(ManagerServer.Model.PurchaseOrder)), DisplayOnView = true });
                }

                foreach (var e in purchaseOrders)
                {
                    if (e.CustomFields == null) e.CustomFields = new Dictionary<Guid, string>();
                    var count = e.CustomFields.Count;
                    if (e.Obsolete_DeliveryDate.HasValue) e.CustomFields.Add(deliveryDateCustomFieldKey, e.Obsolete_DeliveryDate.Value.ToString("yyyy-MM-dd"));
                    if (!string.IsNullOrWhiteSpace(e.Obsolete_DeliveryAddress)) e.CustomFields.Add(deliveryAddressCustomFieldKey, e.Obsolete_DeliveryAddress);
                    if (!string.IsNullOrWhiteSpace(e.Obsolete_DeliveryInstructions)) e.CustomFields.Add(deliveryInstructionsCustomFieldKey, e.Obsolete_DeliveryInstructions);
                    if (!string.IsNullOrWhiteSpace(e.Obsolete_AuthorizedBy)) e.CustomFields.Add(authorizedByCustomFieldKey, e.Obsolete_AuthorizedBy);
                    if (e.CustomFields.Count > count) list.Add(e);
                }
            }

            return list;
        }
    }
}
