using System;
using System.Linq;
using System.Threading.Tasks;
using ManagerServer.Globalization;
using ManagerServer.Helpers;
using Microsoft.AspNetCore.Http;
using ProtoBuf;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    [ProtoContract]
    internal sealed class FormDelete : BusinessHandler
    {
        [ProtoMember(1)] public Guid Key;
        [ProtoMember(2)] public string Referrer;

        public override Task Post()
        {
            var deleteButton = true;
            /*
            var currentUser = this.GetCurrentUser();
            if (currentUser != null && currentUser.Type == UserType.Restricted)
            {
                var userPermissions = GetCurrentUserPermissions(FileID);
                if (userPermissions != null)
                {
                    if (userPermissions.PermittedActions == PermittedActions.View)
                    {
                        deleteButton = false;
                    }
                    else if (userPermissions.PermittedActions == PermittedActions.ViewCreate)
                    {
                        deleteButton = false;
                    }
                    else if (userPermissions.PermittedActions == PermittedActions.ViewCreateUpdate)
                    {
                        deleteButton = false;
                    }
                }
            }
            */

            if (!deleteButton)
            {
                Response.StatusCode = 409; // 409 Conflict
                return Response.WriteAsync("You don't have a permission to delete this object.");
            }

            var lockDate = ApplicationData.Businesses.Get(Business).Single<ManagerServer.Model.LockDate>();
            if (lockDate.GetLockDate().HasValue)
            {
                var o = ApplicationData.Businesses.Get(Business).SingleOrDefault(Key);
                if (o is ManagerServer.Model.Transaction transaction)
                {
                    if (transaction.GetGeneralLedgerTransactions(ApplicationData.Businesses.Get(Business)).Any(x => x.Date <= lockDate.GetLockDate().Value))
                    {
                        Response.StatusCode = 409; // 409 Conflict
                        return Response.WriteAsync("No transaction dated before lock date (" + lockDate.GetLockDate().Value.ToLocalShortDisplayString() + @") can be deleted.");
                    }
                }
                if (o is ManagerServer.Model.ExchangeRate exchangeRate)
                {
                    if (exchangeRate.Date <= lockDate.GetLockDate().Value)
                    {
                        Response.StatusCode = 409; // 409 Conflict
                        return Response.WriteAsync("No exchange rate dated before lock date (" + lockDate.GetLockDate().Value.ToLocalShortDisplayString() + @") can be deleted.");
                    }
                }
            }

            ApplicationData.Businesses.Process(Business, Key, GetUserName());

            Response.Headers["HX-Redirect"] = Referrer;

            return Task.CompletedTask;
        }
    }
}