using ManagerServer.Globalization;

namespace ManagerServer.Api.Businesses.Business.Folders
{
    [ProtoContract]
    internal sealed class GetFolderView : GetTransactionView<Model.Folder>
    {
        protected override TransactionView GetViewData(Model.Folder o)
        {
            var viewData = new TransactionView();
            viewData.title = Strings.Folder;
            viewData.description = o.Description;
            return viewData;
        }
    }
}
