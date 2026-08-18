using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("a3283b79-76be-44b6-9639-fa22d9b63246")]
    public sealed class GeneralLedgerTransactions : Object, IHasCustomTheme
    {
        [Guide("Enter an optional description for this report.")]
        [ProtoMember(3), Long] public string Description { get; set; }
        [Guide("Enter the start date for the transaction period.")]
        [ProtoMember(1), NoWrap] public DateTime FromDate { get; set; }
        [Guide("Enter the end date for the transaction period.")]
        [ProtoMember(2)] public DateTime ToDate { get; set; }
        [Guide("Select the account to view transactions for. Leave blank to show all accounts.")]
        [ProtoMember(4), Autocomplete(typeof(IGeneralLedgerAccount))] public Guid? Account { get; set; }

        [ProtoMember(5), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(6), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }
    }
}
