using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using Microsoft.AspNetCore.Http;
using ProtoBuf;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    [ProtoContract]
    [Title(nameof(Strings.BatchDelete))]
    [Guide("The Batch Delete handler processes deletion of multiple items at once.")]
    [Guide("Items locked by lock date cannot be deleted.")]
    internal sealed class BatchDelete : BusinessHandler
    {
        public override async Task Post()
        {
            var form = await Request.ReadFormAsync();
            if (form.ContainsKey("Keys"))
            {
                var item = form["Keys"].ToString();
                if (!string.IsNullOrWhiteSpace(item))
                {
                    var keys = item.Split(',').Select(x => Guid.Parse(x)).ToList();

                    if (keys.Any(x => !DeletePermission.CanDelete(this, Business, x)))
                    {
                        Response.StatusCode = 409; // 409 Conflict
                        await Response.WriteAsync("You don't have a permission to delete one or more of the selected objects.");
                        return;
                    }

                    var lockDate = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.LockDate>();
                    if (lockDate.GetLockDate().HasValue)
                    {
                        foreach (var e in keys.ToArray())
                        {
                            var o = ApplicationData.Businesses.Get(Business).SingleOrDefault(e) as ManagerServer.Model.Transaction;
                            if (o != null)
                            {
                                if (o.GetGeneralLedgerTransactions(ApplicationData.Businesses.Get(Business)).Any(x => x.Date <= lockDate.GetLockDate().Value))
                                {
                                    keys.Remove(e);
                                }
                            }
                        }
                    }

                    ApplicationData.Businesses.Process(Business, keys.ToArray(), GetUserName());
                }
            }            
        }
    }
}
