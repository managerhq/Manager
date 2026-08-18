using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("02c3fbc6-4473-436f-b58d-fd51937f4e77")]
    public sealed class ReportTransformation2 : NamedObject
    {
        [Guide("Enter a name for this report transformation.")]
        [ProtoMember(1)] public string Name { get; set; }

        [Guide("The starting date for the report period.")]
        [ProtoMember(2), NoWrap] public DateTime FromDate { get; set; }

        [Guide("The ending date for the report period.")]
        [ProtoMember(3)] public DateTime ToDate { get; set; }

        [Guide("Select a specific employee to filter the report, or leave blank to include all employees.")]
        [ProtoMember(5), Autocomplete(typeof(Employee)), IfFalse(nameof(Employees))] public Guid? Employee { get; set; }

        [Guide("Select the number of columns to display in the report.")]
        [ProtoMember(6)] public ColumnCount Columns { get; set; }

        [Guide("Define the report items and map them to reporting categories for each column.")]
        [ProtoMember(7)] public Item[] Items2 { get; set; }

        [Guide("Check to enable accounting method selection for this report.")]
        [ProtoMember(17)] public bool AccountingMethod { get; set; }

        [Guide("Select whether to use accrual or cash basis accounting for this report.")]
        [ProtoMember(4), IfTrue(nameof(AccountingMethod)), NoLabel] public AccountingBasis AccountingMethodOption { get; set; }

        [Guide("Check to enable grouping the report by suppliers.")]
        [ProtoMember(8)] public bool Suppliers { get; set; }

        [Guide("Select a supplier custom field to filter by.")]
        [ProtoMember(9), IfTrue(nameof(Suppliers)), Autocomplete(typeof(ManagerServer.Model.CustomField), Filter = typeof(ManagerServer.Model.Supplier)), NoWrap, NoLabel, Prepend(nameof(Strings.HasWhere))] public Guid? SupplierCustomField { get; set; }

        [Guide("Enter the custom field value to filter suppliers by.")]
        [ProtoMember(10), IfTrue(nameof(Suppliers)), NoLabel, Prepend(nameof(Strings.Is)), Short] public string SupplierCustomFieldValue { get; set; }

        [Guide("Define report items to show for each supplier.")]
        [ProtoMember(11), IfTrue(nameof(Suppliers))] public Item[] ForEachSupplier { get; set; }

        [Guide("Check to enable grouping the report by employees.")]
        [ProtoMember(12)] public bool Employees { get; set; }

        [Guide("Define report items to show for each employee.")]
        [ProtoMember(13), IfTrue(nameof(Employees))] public Item[] ForEachEmployee { get; set; }

        [Guide("Check to enable custom script processing for this report.")]
        [ProtoMember(14)] public bool Script { get; set; }

        [Guide("Enter custom JavaScript code to transform the report data.")]
        [ProtoMember(15), IfTrue(nameof(Script)), Code] public string CustomScript { get; set; }

        [Guide("Check to add custom instructions to the report.")]
        [ProtoMember(18)] public bool Instructions { get; set; }

        [Guide("Add step-by-step instructions that will appear on the report.")]
        [ProtoMember(19), IfTrue(nameof(Instructions)), Textarea] public InstructionStep[] InstructionLines { get; set; }

        [Guide("Check to make this report transformation available to other users.")]
        [ProtoMember(16)] public bool Published { get; set; }

        [ProtoContract]
        public sealed class Item
        {
            [ProtoMember(4), Textarea, EmptyLabel] public string Name { get; set; }
            [ProtoMember(5), Autocomplete(typeof(IReportingCategory)), EmptyLabel, IfEnum(nameof(Columns), (int)ColumnCount.Two, (int)ColumnCount.Three, (int)ColumnCount.Four, (int)ColumnCount.Five, (int)ColumnCount.Six)] public Guid[] Column1 { get; set; }
            [ProtoMember(6), Autocomplete(typeof(IReportingCategory)), EmptyLabel, IfEnum(nameof(Columns), (int)ColumnCount.Three, (int)ColumnCount.Four, (int)ColumnCount.Five, (int)ColumnCount.Six)] public Guid[] Column2 { get; set; }
            [ProtoMember(7), Autocomplete(typeof(IReportingCategory)), EmptyLabel, IfEnum(nameof(Columns), (int)ColumnCount.Four, (int)ColumnCount.Five, (int)ColumnCount.Six)] public Guid[] Column3 { get; set; }
            [ProtoMember(8), Autocomplete(typeof(IReportingCategory)), EmptyLabel, IfEnum(nameof(Columns), (int)ColumnCount.Five, (int)ColumnCount.Six)] public Guid[] Column4 { get; set; }
            [ProtoMember(9), Autocomplete(typeof(IReportingCategory)), EmptyLabel, IfEnum(nameof(Columns), (int)ColumnCount.Six)] public Guid[] Column5 { get; set; }

            public bool Contains(Guid key)
            {
                if (Column1 != null && Column1.Contains(key)) return true;
                if (Column2 != null && Column2.Contains(key)) return true;
                if (Column3 != null && Column3.Contains(key)) return true;
                if (Column4 != null && Column4.Contains(key)) return true;
                if (Column5 != null && Column5.Contains(key)) return true;
                return false;
            }
        }

        [ProtoContract]
        public sealed class InstructionStep
        {
            [ProtoMember(1), Long, Textarea, EmptyLabel] public string Text { get; set; }
        }

        public enum ColumnCount : int
        {
            Two = 0,
            Three = 1,
            Four = 2,
            Five = 3,
            Six = 4
        }

        public bool Contains(Guid key)
        {
            if (Items2 != null) if (Items2.Any(x => x.Contains(key))) return true;
            if (Employees && ForEachEmployee != null) if (ForEachEmployee.Any(x => x.Contains(key))) return true;
            if (Suppliers && ForEachSupplier != null) if (ForEachSupplier.Any(x => x.Contains(key))) return true;
            if (SupplierCustomField == key) return true;
            return false;
        }

        public bool HasAccountingMethod
        {
            get
            {
                return AccountingMethod;
            }
        }

        public bool HasEmployee
        {
            get
            {
                if (Employees) return false;
                if (Employee.HasValue) return true;
                return false;
            }
        }

        public override string GetName()
        {
            return Name;
        }
    }
}
