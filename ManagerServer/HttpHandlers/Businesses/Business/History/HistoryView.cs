using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Helpers;
using ManagerServer.Attributes;
using Sentry.Infrastructure;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    [ProtoContract]
    [Title(nameof(Strings.History))]
    [Guide("The **History** screen provides a comprehensive audit trail of all changes made to your transactions and settings in Manager.")]
    [Guide("This powerful feature allows you to track exactly what was changed, when it was changed, and see both the original and modified values.")]
    [Header("Understanding the History Display")]
    [Guide("Each change is displayed in a separate panel showing the complete before-and-after state of the modified record.")]
    [Guide("Field names are shown on the left, with the original value (if changed) displayed with a strikethrough in red, followed by the new value.")]
    [Guide("Unchanged fields are shown in gray to help you quickly identify what was actually modified.")]
    [Header("Using the Undo Feature")]
    [Guide("The **Undo** button at the bottom of the screen allows you to reverse changes and restore the previous state of your data.")]
    [Guide("When you click **Undo**, the system will revert all changes shown on the current history page back to their original values.")]
    [Guide("Use this feature carefully, as undoing changes cannot itself be undone. You would need to manually re-enter the data.")]
    internal sealed class HistoryView : BusinessTemplate
    {
        [ProtoMember(1)] public Guid Key;
        [ProtoMember(2)] public int Skip;

        protected override void InnerGet2()
        {
            var userPermissions = this.GetCurrentUserPermissions(Business);
            if (!userPermissions.FullAccess) return;

            using (Div(@class: "card"))
            {
                using (Div(@class: "card-header"))
                {
                    using (Div(@class: "card-title")) Write(Strings.History);
                }

                using (var c = ApplicationData.Businesses.SQLiteConnection(Business))
                {
                    var total = c.Table<ManagerServer.ApplicationData.Change>().Count(x => x.Commit == Key);
                    var changes = c.Table<ManagerServer.ApplicationData.Change>().Where(x => x.Commit == Key).Skip(Skip).Take(10).ToArray();

                    if (Skip > 0) ShowPagination(Skip, 10, total, x => new HistoryView() { Business = Business, Referrer = Referrer, Key = Key, Skip = x }.ToUrl());

                    using (Div(@class: "card-inset flex flex-col gap-2"))
                    {
                        foreach (var e in changes)
                        {
                            var changeViewUrl = new ManagerServer.Api.Businesses.Business.GetView()
                            {
                                Business = Business,
                                Key = e.Key,
                                Handler = typeof(ManagerServer.Api.Businesses.Business.History.GetChangeView).FullName,
                                Language = Strings.CurrentLanguage.Value,
                            }.ToUrl();
                            using (IFrame(src: changeViewUrl, loading: "lazy", @class: "w-full", onload: "autoResizeIframe(this)"))
                            {
                            }
                        }
                    }

                    ShowPagination(Skip, 10, total, x => new HistoryView() { Business = Business, Referrer = Referrer, Key = Key, Skip = x }.ToUrl());
                }
                using (Div(@class: "card-header print:hidden"))
                {
                    using (Form(method: "POST"))
                    {
                        InputSubmit(@class: "btn btn-danger", value: Strings.Undo, onClick: "return confirm(" + Strings.Are_you_sure.EncodeJsString().Replace('"', '\'') + ");");
                    }
                }
            }            
        }              

        protected override Task InnerPost()
        {
            var result = ApplicationData.Businesses.Undo(Business, Key);
            if (result)
            {
                if (Referrer != null && Referrer.Length > 0) Response.Redirect(Referrer);
                else Response.Redirect(new History() { Business = Business }.ToUrl());
            }
            else
            {
                Response.Redirect(this.ToUrl());
            }
            return Task.CompletedTask;
        }
    }
}