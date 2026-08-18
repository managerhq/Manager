using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    [Key("batch-delete")]
    [Title(nameof(Strings.BatchDelete))]
    [Guide("To delete multiple rows simultaneously, click the `BatchDelete` button. This action will add a new column with checkboxes. Select the checkboxes next to the rows you wish to remove and then click the `BatchDelete` button again, located at the bottom of the screen, to complete the deletion.")]
    internal abstract class NakedObjectsWithBatchDelete<T> : NakedObjectsWithEditAndViewButtonColumns<T> where T : ManagerServer.Model.Object, new()
    {
        [InheritedProtoMember(270)] public bool BatchDelete;

        protected override void InnerGet4(Context context)
        {
            if (BatchDelete)
            {
                var cancelHandler = (NakedObjectsWithBatchDelete<T>)this.MemberwiseClone();
                cancelHandler.BatchDelete = false;

                context.Set(new BatchOperation()
                {
                    Name = Strings.BatchDelete,
                    IsDanger = true,
                    Cancel = cancelHandler
                });
            }
            base.InnerGet4(context);
        }

        public override Tuple<string, byte[]>[] GetBatchOperation(T[] rows)
        {
            if (BatchDelete)
            {
                var database = ApplicationData.Businesses.Get(Business);
                var lockDate = database.Single<LockDate>().GetLockDate();

                var list = new List<Tuple<string, byte[]>>();
                foreach (var e in rows)
                {
                    if (lockDate.HasValue && e is Transaction transaction && transaction.GetGeneralLedgerTransactions(database).Any() && transaction.GetGeneralLedgerTransactions(database).Min(x => x.Date) < lockDate.Value)
                    {
                        list.Add(null);
                    }
                    else if (e is InventoryUnitCost)
                    {
                        list.Add(new Tuple<string, byte[]>("BatchDeleteItem", e.Key.ToByteArray()));
                    }
                    else if (database.GetGeneralLedgerTransactions().GetTransactionsByForeignKey(e.Key).Any())
                    {
                        list.Add(null);
                    }
                    else
                    {
                        list.Add(new Tuple<string, byte[]>("BatchDeleteItem", e.Key.ToByteArray()));
                    }
                }
                return list.ToArray();
            }
            return base.GetBatchOperation(rows);
        }

        protected override async Task InnerPost()
        {
            if (Request.HasFormContentType)
            {
                var form = await Request.ReadFormAsync();
                if (form.ContainsKey("BatchDeleteItem"))
                {
                    var item = form["BatchDeleteItem"].ToString();
                    if (!string.IsNullOrWhiteSpace(item))
                    {
                        var userPermissions = this.GetCurrentUserPermissions(Business);
                        if (userPermissions.CanDelete(this.GetType().Namespace))
                        {
                            var items = item.Split(',').Select(x => Convert.FromBase64String(x)).ToArray();
                            ApplicationData.Businesses.Process(Business, items.Where(x => x.Length == 16).Select(x => new Guid(x)).ToArray(), GetUserName());
                            Response.Redirect(this.ToUrl());
                            return;
                        }
                    }
                }
            }
            await base.InnerPost();
        }

        protected override void OnFooterEndSection(Context context)
        {
            var userPermissions = this.GetCurrentUserPermissions(Business);
            if (userPermissions.CanDelete(this.GetType().Namespace))
            {
                var batchOperations = GetBatchOperations(context);

                var batchDeleteHandler = (NakedObjectsWithBatchDelete<T>)this.MemberwiseClone();
                batchDeleteHandler.BatchDelete = true;

                if (batchOperations.Items.Any()) batchOperations.Items.Add(null);
                batchOperations.Items.Add(new Tuple<string, BusinessTemplate>(Strings.BatchDelete, batchDeleteHandler));
            }

            base.OnFooterEndSection(context);
        }
    }
}
