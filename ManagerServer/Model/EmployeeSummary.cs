using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("e7b3a4f4-35d7-4f92-a8fc-e9eadbc140a8")]
    public sealed class EmployeeSummary : Object, IHasCustomTheme
    {
        [Guide("Select a specific employee to generate a summary for that individual, or leave blank to include all employees.")]
        [ProtoMember(3), Autocomplete(typeof(ManagerServer.Model.Employee))] public Guid? Employee { get; set; }
        [Guide("The starting date for the period covered by the report.")]
        [ProtoMember(1)] public DateTime FromDate { get; set; }
        [Guide("The ending date for the period covered by the report.")]
        [ProtoMember(2)] public DateTime ToDate { get; set; }
        [Guide("When checked, employees with zero balances will be excluded from the report.")]
        [ProtoMember(4)] public bool ExcludeZeroBalances { get; set; }

        [ProtoMember(5), IfContains<CustomTheme>] public bool CustomTheme { get; set; }
        [ProtoMember(6), IfTrue(nameof(CustomTheme)), Autocomplete(typeof(CustomTheme)), NoLabel] public Guid? CustomThemeId { get; set; }
    }
}
