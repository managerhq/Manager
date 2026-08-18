using System;
using ManagerServer.Model.Enums;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("39f00628-27a7-4924-9030-5bc655ee234f")]
    public sealed class AgedReceivables : Object, IHasCustomTheme
    {
        [Guide("Select the date for aging calculations. Today uses the current date, while Custom lets you specify a specific date.")]
        [ProtoMember(4), NoWrap] public DateType Date { get; set; }
        [Guide("Enter the specific date to calculate aging from when using Custom date type.")]
        [ProtoMember(1), IfEnum(nameof(Date), (int)DateType.Custom), EmptyLabel] public DateTime CustomDate { get; set; }
        [Guide("Select a division to filter the report to show only receivables for that division.")]
        [ProtoMember(6), Autocomplete(typeof(Division))] public Guid? Division { get; set; }
        [Guide("Select how to sort the report: by customer name or by total amount owed.")]
        [ProtoMember(3)] public SortBy SortBy { get; set; }
        [Guide("Enter an optional description for this report configuration.")]
        [ProtoMember(2)] public string Description { get; set; }
        [Guide("Check this box to show individual invoice details for each customer.")]
        [ProtoMember(5)] public bool ShowInvoices { get; set; }

        [ProtoMember(7), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(8), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }
    }
}
