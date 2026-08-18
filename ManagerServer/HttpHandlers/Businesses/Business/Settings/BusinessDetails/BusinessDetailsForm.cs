using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Helpers;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.BusinessDetails
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.BusinessDetails))]
    [Guide("The `BusinessDetails` form, located under the `Settings` tab, enables you to input information that will be displayed on your printed documents.")]
    [SettingsItemScreenshot("fa-info-square", nameof(Strings.BusinessDetails))]
    [Guide("The form includes the following fields:")]
    [Fields(typeof(ManagerServer.Model.BusinessDetails))]
    [Guide("You can set a business logo by uploading a file to the `Image` section.")]
    [Guide("To save the changes, click the `Update` button.")]
    [SuccessButtonScreenshot(nameof(Strings.Update))]
    internal sealed class BusinessDetailsForm : NakedVueForm<ManagerServer.Model.BusinessDetails>
    {
        protected override bool CanHaveImage() => true;
    }
}
