using System;
using ProtoBuf;
using ManagerServer.Model.Enums;
using ManagerServer.Model.Attributes;

namespace ManagerServer.Model.Obsolete.Obsolete65
{
    [ProtoContract]
    [Guid("8653b8ac-a43d-485f-a2e3-edbc76add6a5")]
    public sealed class CustomReport : Object
    {
        [ProtoMember(1)]
        public string Name;
        [ProtoMember(2)]
        public SelectElement[] Select;
        [ProtoMember(3)]
        public WhereElement[] Where;
        [ProtoMember(4)]
        public OrderByElement[] OrderBy;
        [ProtoMember(5)]
        public bool HasGroupBy;
        [ProtoMember(7)]
        public bool HasOrderBy;
        [ProtoMember(8)]
        public string Description;
        [ProtoMember(9)]
        public GroupByElement[] GroupBy;
        [ProtoMember(10)]
        public bool CollapseGroups;
        [ProtoMember(11)]
        public DateTime From;
        [ProtoMember(12)]
        public DateTime To;
        [ProtoMember(14)]
        public string ReportTransformation;
        [ProtoMember(15)]
        public AccountingBasis AccountingBasis;

        [ProtoContract]
        public sealed class SelectElement
        {
            [ProtoMember(1)]
            public string Name;
            [ProtoMember(2)]
            public string InnerName;
            [ProtoMember(3)]
            public string DisplayName;
        }

        [ProtoContract]
        public sealed class OrderByElement
        {
            [ProtoMember(1)]
            public string Name;
            [ProtoMember(2)]
            public SortOrder SortOrder;
            [ProtoMember(3)]
            public string InnerName;
        }

        [ProtoContract]
        public sealed class GroupByElement
        {
            [ProtoMember(1)]
            public string Name;
            [ProtoMember(2)]
            public string InnerName;
        }

        [ProtoContract]
        public sealed class WhereElement
        {
            [ProtoMember(1)]
            public string Name;
            [ProtoMember(2)]
            public int StringOperator;
            [ProtoMember(3)]
            public string String;
            [ProtoMember(4)]
            public int DecimalOperator;
            [ProtoMember(5)]
            public decimal? Decimal;
            [ProtoMember(6)]
            public int BooleanOperator;
            [ProtoMember(7)]
            public int ObjectOperator;
            [ProtoMember(8)]
            public Guid? Object;
            [ProtoMember(9)]
            public DateTime? StartDate;
            [ProtoMember(10)]
            public DateTime? EndDate;
            [ProtoMember(11)]
            public int DateOperator;
            [ProtoMember(12)]
            public string InnerName;
        }
    }
}