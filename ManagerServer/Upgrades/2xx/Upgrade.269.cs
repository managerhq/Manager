using System;
using System.Collections.Generic;
using System.Linq;
using ManagerServer.Model.Enums;
using ManagerServer.Globalization;
using System.Text;
using System.IO;
using ManagerServer.Model;
using System.Reflection;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Obsolete;
using System.Threading.Tasks;

namespace ManagerServer
{
    public static partial class Upgrade
    {
        private static async Task<IEnumerable<Model.Object>> Upgrade269(Orm.SQLiteConnection objects, IProgress<Tuple<int, int>> progress)
        {
            await Task.CompletedTask;
            var getPrimaryMemberInfo = new Func<string, ManagerServer.Model.MemberInfo>(x =>
            {
                if (string.IsNullOrWhiteSpace(x)) return null;
                var memberInfo = typeof(ManagerServer.Query.GeneralLedger.GeneralLedgerTransaction).GetMember(x).FirstOrDefault();
                if (memberInfo == null) return null;
                return new ManagerServer.Model.MemberInfo(memberInfo);
            });

            var getSecondaryMemberInfo = new Func<string, string, ManagerServer.Model.MemberInfo>((x1, x2) =>
            {
                if (string.IsNullOrWhiteSpace(x1)) return null;
                if (string.IsNullOrWhiteSpace(x2)) return null;

                var memberInfo = getPrimaryMemberInfo(x1);
                if (memberInfo == null) return null;

                var memberInfo2 = typeof(Upgrade).Assembly.GetType(memberInfo.DeclaringType)?.GetMember(x2.Split('.').First()).FirstOrDefault();
                if (memberInfo2 == null) return null;

                return new ManagerServer.Model.MemberInfo(memberInfo2);
            });

            var getCustomFieldKey = new Func<string, Guid?>(x =>
            {
                if (string.IsNullOrWhiteSpace(x)) return null;
                try
                {
                    if (x.StartsWith("CustomFields.")) return new Guid(x.Split('.')[1]);
                }
                catch (Exception) { }
                return null;
            });

            var list = new List<ManagerServer.Model.Object>();
            foreach (var e in objects.OfType<ManagerServer.Model.Obsolete.Obsolete65.CustomReport>())
            {
                var customReport = new CustomReport()
                {
                    Key = Guid.CreateVersion7(),
                    Name = e.Name,
                    Description = e.Description,
                    FromDate = e.From,
                    ToDate = e.To,
                    GroupsToCollapse = e.CollapseGroups,
                    AccountingMethod = e.AccountingBasis,
                    HasGroupBy = e.HasGroupBy,
                    HasOrderBy = e.HasOrderBy,
                    HasWhere = true,
                    Select = e.Select.Select(x => new CustomReport.SelectElement()
                    {
                        SelectPrimaryField = getPrimaryMemberInfo(x.Name),
                        SelectSecondaryField = getSecondaryMemberInfo(x.Name, x.InnerName),
                        SelectCustomField = getCustomFieldKey(x.InnerName),
                        DisplayName = x.DisplayName
                    }).ToArray(),
                    Where = e.Where.Select(x => new CustomReport.WhereElement()
                    {
                        WherePrimaryField = getPrimaryMemberInfo(x.Name),
                        WhereSecondaryField = getSecondaryMemberInfo(x.Name, x.InnerName),
                        WhereCustomField = getCustomFieldKey(x.InnerName),
                        DateOperator = (CustomReport.DateOperator)x.DateOperator,
                        Decimal = x.Decimal,
                        DecimalOperator = (CustomReport.DecimalOperator)x.DecimalOperator,
                        StartDate = x.StartDate ?? DateTime.MinValue,
                        EndDate = x.EndDate ?? DateTime.MinValue,
                        BooleanOperator = (CustomReport.BooleanOperator)x.BooleanOperator,
                        Object = x.Object,
                        ObjectOperator = (CustomReport.ObjectOperator)x.ObjectOperator,
                        String = x.String,
                        StringOperator = (CustomReport.StringOperator)x.StringOperator
                    }).ToArray(),
                    GroupBy = e.GroupBy.Select(x => new CustomReport.GroupByElement()
                    {
                        GroupByPrimaryField = getPrimaryMemberInfo(x.Name),
                        GroupBySecondaryField = getSecondaryMemberInfo(x.Name, x.InnerName),
                        GroupByCustomField = getCustomFieldKey(x.InnerName),
                    }).ToArray(),
                    OrderBy = e.OrderBy.Select(x => new CustomReport.OrderByElement()
                    {
                        OrderByPrimaryField = getPrimaryMemberInfo(x.Name),
                        OrderBySecondaryField = getSecondaryMemberInfo(x.Name, x.InnerName),
                        OrderByCustomField = getCustomFieldKey(x.InnerName),
                        SortOrder = x.SortOrder
                    }).ToArray()
                };
                list.Add(customReport);
            }
            return list;
        }
    }
}
