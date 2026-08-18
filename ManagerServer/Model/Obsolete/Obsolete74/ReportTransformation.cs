using System;
using ManagerServer.Model.Attributes;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;

namespace ManagerServer.Model.Obsolete.Obsolete74
{
    [ProtoContract]
    [Guid("91c2bcbb-1f8c-4aa1-82fd-0ab38c97fb14")]
    public sealed class ReportTransformation : NamedObject
    {
        [ProtoMember(1)] public string Name;

        [ProtoMember(29), NoWrap] public DateTime FromDate;
        [ProtoMember(30), NoWrap] public DateTime ToDate;
        [ProtoMember(31), NoWrap] public AccountingBasis AccountingMethod;
        [ProtoMember(32), Autocomplete(typeof(Employee)), IfFalse(nameof(Employees))] public Guid? Employee;

        [ProtoMember(16)] public ColumnCount Columns;
        [ProtoMember(17)] public Item2[] Items2;

        [ProtoMember(18)] public bool Suppliers;
        [ProtoMember(20), IfTrue(nameof(Suppliers)), Autocomplete(typeof(ManagerServer.Model.CustomField), Filter = typeof(ManagerServer.Model.Supplier)), NoWrap, NoLabel, Prepend(nameof(Strings.HasWhere))] public Guid? SupplierCustomField;
        [ProtoMember(21), IfTrue(nameof(Suppliers)), NoLabel, Prepend(nameof(Strings.Is)), Short] public string SupplierCustomFieldValue;
        [ProtoMember(19), IfTrue(nameof(Suppliers))] public Item2[] ForEachSupplier;

        [ProtoMember(22)] public bool Employees;
        [ProtoMember(23), IfTrue(nameof(Employees))] public Item2[] ForEachEmployee;

        [ProtoMember(24)] public bool Instructions;
        [ProtoMember(28), IfTrue(nameof(Instructions)), Textarea] public InstructionStep[] InstructionLines;

        [ProtoMember(34)] public bool Script;
        [ProtoMember(35), IfTrue(nameof(Script)), Code] public string CustomScript;

        [ProtoMember(33)] public bool Published;

        [ProtoContract]
        public sealed class Item2
        {
            [ProtoMember(4), Textarea, EmptyLabel] public string Name;
            [ProtoMember(5), EmptyLabel, IfEnum(nameof(Columns), (int)ColumnCount.Two, (int)ColumnCount.Three, (int)ColumnCount.Four, (int)ColumnCount.Five, (int)ColumnCount.Six)] public Figure2[] Column1;
            [ProtoMember(6), EmptyLabel, IfEnum(nameof(Columns), (int)ColumnCount.Three, (int)ColumnCount.Four, (int)ColumnCount.Five, (int)ColumnCount.Six)] public Figure2[] Column2;
            [ProtoMember(7), EmptyLabel, IfEnum(nameof(Columns), (int)ColumnCount.Four, (int)ColumnCount.Five, (int)ColumnCount.Six)] public Figure2[] Column3;
            [ProtoMember(8), EmptyLabel, IfEnum(nameof(Columns), (int)ColumnCount.Five, (int)ColumnCount.Six)] public Figure2[] Column4;
            [ProtoMember(9), EmptyLabel, IfEnum(nameof(Columns), (int)ColumnCount.Six)] public Figure2[] Column5;
        }

        [ProtoContract]
        public sealed class InstructionStep
        {
            [ProtoMember(1), Long, Textarea, EmptyLabel] public string Text;
        }

        [ProtoContract]
        public sealed class Figure2
        {
            public string UniqueName;

            [ProtoMember(1)] public string Text;
            [ProtoMember(4)] public TaxSummaryColumns TaxSummaryColumn;
            [ProtoMember(7)] public string CustomFieldText;
            [ProtoMember(13)] public Guid? EmployeeCustomField;
            [ProtoMember(14)] public Guid? BusinessDetailsCustomField;
            [ProtoMember(15)] public string BusinessDetailsField;
            [ProtoMember(16)] public string EmployeeField;
            [ProtoMember(17)] public string SupplierField;
            [ProtoMember(18)] public string ReportTransformationField;
            [ProtoMember(19)] public Guid? TaxCodeCustomField;
            [ProtoMember(20)] public Guid? ReportingCategory;

            [ProtoMember(9)] public Guid? Obsolete_TaxCode;            

            public enum TaxSummaryColumns : int
            {
                TaxOnSales = 0,
                NetSales = 1,
                TotalSales = 2,
                TaxOnPurchases = 3,
                NetPurchases = 4,
                TotalPurchases = 5,
                TaxLiability = 6
            }            
        }

        public enum ColumnCount : int
        {
            Two = 0,
            Three = 1,
            Four = 2,
            Five = 3,
            Six = 4
        }

        [ProtoMember(7)] public string Obsolete_Source;
        [ProtoMember(2)] public Guid? Obsolete_Type;        

        public override string GetName()
        {
            return Name;
        }
    }
}