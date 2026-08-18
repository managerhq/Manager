using ManagerServer.Attributes;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Enums;
using ProtoBuf;
using System;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("401d5f3c-92ef-47da-942a-3cd6cf64aa9c")]
    public sealed class SupplierStatementsTransactions : Object, IHasCustomTheme
    {
        [Guide("The starting date for transactions to include in the statements.")]
        [ProtoMember(1)] public DateTime FromDate { get; set; }
        [Guide("Choose whether to use today's date or a custom date as the ending date.")]
        [ProtoMember(4), NoWrap] public DateType ToDate { get; set; }
        [Guide("Enter the custom ending date for the statement period.")]
        [ProtoMember(2), IfEnum(nameof(ToDate), (int)DateType.Custom), EmptyLabel] public DateTime ToCustomDate { get; set; }
        [Guide("Select a theme to customize the appearance of the statements.")]
        [ProtoMember(5), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(3), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }

        public DateTime GetToDate()
        {
            if (ToDate == DateType.Today) return DateTime.Today;
            return ToCustomDate;
        }
    }
}
