using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Folders
{
    [ProtoContract]
    [Guid("5e7310f4-4082-4265-8741-ed1faefd0a8e")]
    [Title(nameof(Strings.Folders))]
    [Guide("`Folders` help you organize your business documents into logical groups. They work like file folders in a filing cabinet, allowing you to keep related transactions together.")]
    [Guide("You can create folders to store any type of transaction, including `Sales Invoices`, `Purchase Invoices`, `Receipts`, `Payments`, `Journal Entries`, and other documents. This makes it easier to locate specific groups of transactions later.")]
    [Guide("When creating or editing any transaction, you can assign it to a folder using the `Folder` field. Once assigned, you can filter transactions by folder to view only those items grouped together.")]
    [Guide("Common uses for folders include organizing by project, customer, time period, or any other grouping that makes sense for your business. For example, you might create folders for each financial year, major project, or department.")]
    internal sealed class Folders : NakedObjectsWithAutomaticRows<ManagerServer.Model.Folder>
    {
        [Default]
        [Guid("fc13cdab-7fc6-47e9-858d-2c7152e3e67d")]
        public string[] GetDescription(ManagerServer.Model.Folder[] rows)
        {
            return rows.Select(x => x.Description).ToArray();
        }
    }
}
