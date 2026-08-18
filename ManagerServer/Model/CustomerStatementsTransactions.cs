using ManagerServer.Attributes;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Enums;
using ProtoBuf;
using System;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("c81ede5f-141e-4eb3-a586-1b0ab079cae6")]
    public sealed class CustomerStatementsTransactions : Object, IHasCustomTheme
    {
        [Guide("Enter the start date for the statement period.")]
        [ProtoMember(1)] public DateTime FromDate { get; set; }
        [Guide("Select the end date type. Today uses the current date, while Custom lets you specify a date.")]
        [ProtoMember(4), NoWrap] public DateType ToDate { get; set; }
        [Guide("Enter the specific end date when using Custom date type.")]
        [ProtoMember(2), IfEnum(nameof(ToDate), (int)DateType.Custom), EmptyLabel] public DateTime ToCustomDate { get; set; }
        [Guide("Select a theme to customize the appearance of printed statements.")]
        [ProtoMember(5), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(3), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }

        public DateTime GetToDate()
        {
            if (ToDate == DateType.Today) return DateTime.Today;
            return ToCustomDate;
        }
    }
}
