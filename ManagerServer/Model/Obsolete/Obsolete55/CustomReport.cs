using System;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Model.Enums;
using ManagerServer.Model.Attributes;

namespace ManagerServer.Model.Obsolete.Obsolete55
{
    [ProtoContract]
    [Guid("ef59dc06-d91a-4319-8d51-de215f15c8e4")]
    public sealed class CustomReport : Object
    {
        [ProtoMember(1)]
        public string Description;
        [ProtoMember(2)]
        public string From;
        [ProtoMember(3)]
        public SelectElement[] Select;
        [ProtoMember(4)]
        public OrderByElement[] OrderBy;
        [ProtoMember(5)]
        public string GroupBy;
        [ProtoMember(6)]
        public WhereElement[] Where;
        [ProtoMember(7)]
        public bool HasWhere;
        [ProtoMember(8)]
        public bool HasOrderBy;
        [ProtoMember(9)]
        public bool HasGroupBy;
        [ProtoMember(10)]
        public bool RenameColumns;
        [ProtoMember(11)]
        public bool RenameReport;
        [ProtoMember(12)]
        public string CustomReportName;

        [ProtoContract]
        public sealed class SelectElement
        {
            [ProtoMember(1)]
            public string Name;
            [ProtoMember(2)]
            public string DisplayName;
        }

        [ProtoContract]
        public sealed class OrderByElement
        {
            [ProtoMember(1)]
            public string Name;
            [ProtoMember(2)]
            public SortOrder SortOrder;
        }

        [ProtoContract]
        public sealed class WhereElement
        {
            [ProtoMember(1)]
            public string Name;
            [ProtoMember(3)]
            public string String;
            [ProtoMember(4)]
            public decimal? Decimal;
            [ProtoMember(5)]
            public DateTime? FromDate;
            [ProtoMember(6)]
            public DateTime? UntilDate;
            [ProtoMember(7)]
            public Guid? Object;
            [ProtoMember(8)]
            public Guid? ComparisonOperator;
        }
    }
}
