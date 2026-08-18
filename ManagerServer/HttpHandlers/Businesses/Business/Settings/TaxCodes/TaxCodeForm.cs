using System;
using System.Linq;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.HttpHandlers.Businesses.Business.Settings.TaxCodes
{
    [ProtoContract]
    [Title(nameof(Strings.TaxCode), nameof(Strings.Edit))]
    [Guide("When configuring a tax code, several fields must be filled out.")]
    [Fields(typeof(TaxCode))]
    internal sealed class TaxCodeForm : NakedVueForm<TaxCode>
    {
    }
}
