using System;
using System.Linq;
using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model;
using ProtoBuf;

namespace ManagerServer.HttpHandlers.Businesses.Business.CapitalAccounts
{
    [ProtoContract]
    [Title(nameof(Strings.CapitalAccount), nameof(Strings.Edit))]
    [Guide("Use this form to create or edit capital accounts for business owners or partners.")]
    [Guide("Capital accounts track owner equity, including investments, drawings, and profit shares.")]
    [Header("Form Fields")]
    [Guide("Complete the following fields:")]
    [Fields(typeof(ManagerServer.Model.CapitalAccount))]
    internal sealed class CapitalAccountForm : NakedVueForm<CapitalAccount>
    {
    }
}
