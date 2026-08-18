using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Model.Enums;
using ManagerServer.Query;
using ManagerServer.Helpers;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Folders
{
    [ProtoContract]
    [Title(nameof(Strings.Folder), nameof(Strings.Edit))]
    [Guide("Folders help you organize attachments and documents within your business file. You can create a hierarchical structure of folders to categorize documents by type, purpose, or any system that works for your business.")]
    [Header("Using Folders")]
    [Guide("When uploading attachments to transactions, customers, suppliers, or other records, you can select which folder to store them in. This makes it easy to find related documents later.")]
    [Guide("For example, you might create folders for `Receipts`, `Contracts`, `Tax Documents`, or organize them by year such as `2024 Documents`.")]
    [Header("Benefits")]
    [Guide("Organizing attachments into folders helps you maintain a clean document management system. You can quickly locate specific documents without searching through all attachments.")]
    [Guide("Folders support nested structures, allowing you to create subfolders for even more detailed organization.")]
    [Fields(typeof(ManagerServer.Model.Folder))]
    internal sealed class FolderForm : NakedVueForm<Folder>
    {
    }
}
