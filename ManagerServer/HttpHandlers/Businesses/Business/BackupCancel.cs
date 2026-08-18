using System.Threading.Tasks;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    [ProtoContract]
    [Title(nameof(Strings.CancelBackup))]
    [Guide("Backup in progress can be cancelled. This will immediately stop the download for the user who is downloading the backup and remove read-only lock from the database.")]
    internal sealed class BackupCancel : BusinessTemplate
    {
        protected override void InnerGet2()
        {
            var userPermissions = this.GetCurrentUserPermissions(Business);
            if (!userPermissions.FullAccess) return;

            using (Form(method: "POST", hxBoost: true))
            {
                using (Div(@class: "card-header"))
                {
                    using (Div(@class: "card-title")) Write(Strings.CancelBackup);
                }

                using (Div(@class: "card-header"))
                {
                    if (ApplicationData.Businesses.IsBackupInProgress(Business))
                    {
                        InputSubmit(@class: "btn btn-danger", value: Strings.CancelBackup);
                    }
                    else
                    {
                        using (Button(@class: "btn btn-danger", disabled: true))
                        {
                            Write(Strings.CancelBackup);
                        }
                    }
                }
            }
        }

        protected override Task InnerPost()
        {
            var userPermissions = this.GetCurrentUserPermissions(Business);
            if (!userPermissions.FullAccess)
            {
                Response.StatusCode = 403;
                return Task.CompletedTask;
            }

            ApplicationData.Businesses.CancelBackup(Business);
            Response.Redirect(this.ToUrl());
            return Task.CompletedTask;
        }
    }
}