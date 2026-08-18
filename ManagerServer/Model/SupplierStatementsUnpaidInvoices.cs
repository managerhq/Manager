using ManagerServer.Attributes;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Enums;
using ProtoBuf;
using System;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("119e71a0-3ea5-4c6f-a8f2-76384b86831a")]
    public sealed class SupplierStatementsUnpaidInvoices : Object, IHasCustomTheme
    {
        [Guide("Choose whether to show unpaid invoices as of today or a custom date.")]
        [ProtoMember(3), NoWrap] public DateType Date { get; set; }
        [Guide("Enter the date to show unpaid invoices as of.")]
        [ProtoMember(1), IfEnum(nameof(Date), (int)DateType.Custom), EmptyLabel] public DateTime CustomDate { get; set; }
        [Guide("Select a theme to customize the appearance of the statements.")]
        [ProtoMember(4), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(2), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }

        public DateTime GetDate()
        {
            if (Date == DateType.Today) return DateTime.Today;
            return CustomDate;
        }
    }
}