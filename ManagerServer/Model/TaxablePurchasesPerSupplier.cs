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
    [Guid("1f957bf2-d198-4a9a-bb51-8d7d26b2fc5c")]
    public sealed class TaxablePurchasesPerSupplier : Object, IHasCustomTheme
    {
        [Guide("Add an optional description or note that will appear on the report.")]
        [ProtoMember(4)] public string Description { get; set; }
        [Guide("The starting date for calculating taxable purchases.")]
        [ProtoMember(1)] public DateTime FromDate { get; set; }
        [Guide("The ending date for calculating taxable purchases.")]
        [ProtoMember(2)] public DateTime ToDate { get; set; }
        [Guide("Select whether to use accrual or cash basis accounting for this report.")]
        [ProtoMember(3)] public AccountingBasis AccountingMethod { get; set; }

        [ProtoMember(5), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(6), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }
    }
}
