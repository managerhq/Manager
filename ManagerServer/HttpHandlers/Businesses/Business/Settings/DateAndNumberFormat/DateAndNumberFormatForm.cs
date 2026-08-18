using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Text;
using ManagerServer.Helpers;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.DateAndNumberFormat
{
    [ProtoContract]
    [NamespaceEntry]
    [Title(nameof(Strings.DateAndNumberFormat))]
    [Guide("The `DateAndNumberFormat` form, found under the `Settings` tab, enables you to input details that will be displayed on transaction forms and utilized by the software.")]
    [SettingsItemScreenshot("fa-calendar-alt", nameof(Strings.DateAndNumberFormat))]
    [Guide("Please fill in the following fields:")]
    [Fields(typeof(ManagerServer.Model.DateAndNumberFormat))]
    [Guide("Next, click the `Update` button to save your changes.")]
    [SuccessButtonScreenshot(nameof(Strings.Update))]
    internal sealed class DateAndNumberFormatForm : NakedVueForm<ManagerServer.Model.DateAndNumberFormat>
    {
    }
}
