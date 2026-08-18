using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    [Key("batch-operations")]
    [Title(nameof(Strings.BatchOperations))]
    [Guide("`BatchOperations` in Manager allow you to create, update, delete, and view entries in bulk. This function is available across many screens in Manager.")]
    [Guide("To use it, click on the `BatchOperations` button in the bottom-right corner.")]
    [SmallBottomButtonScreenshot(name: nameof(Strings.BatchOperations))]
    [Guide("`BatchCreate` is used to create multiple entries at once. This feature is useful when you need to add a large number of entries efficiently.")]
    [Guide("Click `BatchOperations` button.")]
    [Guide("Then click `BatchCreate`.")]
    [Guide("You will be taken to `BatchCreate` screen that contains 3 steps:")]
    [Guide("1. Copy to clipboard, then paste columns to your spreadsheet program")]
    [Guide("2. Fill in data in your spreadsheet program")]
    [Guide("3. Copy data from a spreadsheet and paste it into the text field below")]
    [Guide("Click `Next` button.")]
    [Guide("Review the entries that Manager will import.")]
    [Guide("Click the `BatchCreate` button to complete the process.")]
    [Guide("**Tip**: The biggest challenge during the batch creation process is preparing your data correctly in the spreadsheet. If unsure, create a few sample entries within Manager.io, then use the `BatchUpdate` function to see how these entries are formatted in the spreadsheet.")]
    [Guide("`BatchUpdate` is used to update multiple entries at once. This feature allows you to modify existing entries in bulk, saving time and effort.")]
    [Guide("`BatchUpdate` works similarly to `BatchCreate` except the `Copy_to_clipboard` button will copy the data of the entries you are updating (not just the columns).")]
    [Guide("`BatchRecode` is used to update single field across multiple entries at once. This feature allows you to modify existing entries in bulk, saving time and effort.")]
    [Guide("`BatchDelete` is used to delete multiple entries at once. This feature is beneficial when you need to remove a large number of entries quickly.")]
    [Guide("`BatchView` is used to view multiple entries at once. This feature is useful when you need to review or print many entries simultaneously.")]
    [Guide("When using batch create or batch update, some fields require GUID identifier which will be in the format `xxxxxxxx-xxxx-xxxx-xxxx-xxxxxxxxxxxx`. You can substitute this GUID identifier for `Code` of the object.")]
    internal abstract class NakedObjectsWithBatchOperations : NakedObjectsWithCopyToClipboard
    {
        protected override void OnFooterEndSection(Context context)
        {
            var batchOperations = GetBatchOperations(context);
            if (batchOperations.Items.Any())
            {
                using (Details(@class: "dropdown"))
                {
                    using (Summary(@class: "cursor-pointer btn btn-xs"))
                    {
                        Write(Strings.BatchOperations);
                    }
                    using (Div(@class: "dropdown-menu"))
                    {
                        foreach (var e in batchOperations.Items)
                        {
                            if (e == null)
                            {
                                Hr(@class: "my-2");
                            }
                            else
                            {
                                using (A(href: e.Item2.ToUrl(), @class: "dropdown-item")) Write(e.Item1);
                            }
                        }
                    }
                }
            }

            base.OnFooterEndSection(context);
        }

        protected BatchOperations GetBatchOperations(Context context)
        {
            var batchOperations = context.Get<BatchOperations>();
            if (batchOperations == null)
            {
                batchOperations = new BatchOperations();
                context.Set<BatchOperations>(batchOperations);
            }
            return batchOperations;
        }

        public sealed class BatchOperations
        {
            public List<Tuple<string, BusinessTemplate>> Items = new List<Tuple<string, BusinessTemplate>>();
        }
    }
}
