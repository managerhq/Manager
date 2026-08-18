using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
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

                    var deleteButton = true;

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

                    /*
                    var userPermissions = GetCurrentUserPermissions(FileID);
                    if (userPermissions.PermittedActions == Manager.Model.PermittedActions.View)
                    {
                        deleteButton = false;
                    }
                    else if (userPermissions.PermittedActions == Manager.Model.PermittedActions.ViewCreate)
                    {
                        deleteButton = false;
                    }
                    else if (userPermissions.PermittedActions == Manager.Model.PermittedActions.ViewCreateUpdate)
                    {
                        deleteButton = false;
                    }
                    */

                    if (deleteButton)
                    {
                        ApplicationData.Businesses.Process(Business, keys.ToArray(), GetUserName());
                    }
                }
            }            
        }
    }
}
