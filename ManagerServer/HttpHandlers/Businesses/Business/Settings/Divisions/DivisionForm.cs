using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.Divisions
{
    [ProtoContract]
    [Title(nameof(Strings.Division), nameof(Strings.Edit))]
    [Guide("Create divisions to track different departments or business segments.")]
    [Guide("Divisions enable separate reporting for different parts of your business.")]
    [Fields(typeof(ManagerServer.Model.Division))]
    internal sealed class DivisionForm : NakedVueForm<ManagerServer.Model.Division>
    {
    }
}
