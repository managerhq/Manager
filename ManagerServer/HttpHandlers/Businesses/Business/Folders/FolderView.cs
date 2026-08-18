using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Helpers;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Folders
{
    [ProtoContract]
    [Title(nameof(Strings.Folder))]
    [Guide("The `Folder` view displays comprehensive information about a selected folder, including its name, description, and any attachments stored within it.")]
    [Guide("Folders help organize your documents and attachments by grouping related items together. Each folder can contain multiple attachments and provides a centralized location for managing related files.")]
    [Header("Available Actions")]
    [Guide("From this view, you can manage your folder and its contents:")]
    [Guide("• Click the `Edit` button to modify the folder's name and description")]
    [Guide("• View and download all attachments contained within the folder")]
    [Guide("• Add new attachments to organize related files together")]
    [Guide("• Delete the folder and all its contents when no longer needed")]
    [Header("Related Topics")]
    [LinkGuide("To learn how to create or edit folders, see:", typeof(FolderForm))]
    internal sealed class FolderView : TransactionView<ManagerServer.Model.Folder>
    {
    }
}