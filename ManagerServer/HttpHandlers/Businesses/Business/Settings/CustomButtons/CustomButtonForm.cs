using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Attributes;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.CustomButtons
{
    [ProtoContract]
    [Title(nameof(Strings.Extension))]
    [Guide("Install and configure extensions to add custom functionality.")]
    [Guide("Extensions allow you to customize Manager with additional features and integrations.")]
    [Fields(typeof(ManagerServer.Model.CustomButton))]
    internal sealed class CustomButtonForm : NakedVueForm<ManagerServer.Model.CustomButton>
    {
    }
}
