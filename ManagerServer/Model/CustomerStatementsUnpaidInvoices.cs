using ManagerServer.Attributes;
using ManagerServer.Globalization;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Enums;
using ManagerServer.Model.Obsolete.Obsolete32;
using ProtoBuf;
using System;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("b9b3d678-a743-46e1-aaf5-fd1b27b5e20b")]
    public sealed class CustomerStatementsUnpaidInvoices : Object, IHasCustomTheme
    {
        [Guide("Select the date for the statement. Today uses the current date, while Custom lets you specify a date.")]
        [ProtoMember(3), NoWrap] public DateType Date { get; set; }
        [Guide("Enter the specific date when using Custom date type.")]
        [ProtoMember(1), IfEnum(nameof(Date), (int)DateType.Custom), EmptyLabel] public DateTime CustomDate { get; set; }
        [Guide("Select a theme to customize the appearance of printed statements.")]
        [ProtoMember(4), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(2), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }

        public DateTime GetDate()
        {
            if (Date == DateType.Today) return DateTime.Today;
            return CustomDate;
        }
    }
}
