using System;
using ManagerServer.Model.Attributes;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using ManagerServer.Globalization;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Guid("f361339b-932a-4436-b56e-a337c1587c72")]
    public sealed class SubAccount : NamedObject
    {
        [Guide("Enter the name of the sub-account, such as 'Dividends', 'Drawings', 'Capital Contributions', or 'Share of Profit'.")]
        [Guide("Sub-accounts help track different types of transactions within a capital account, providing better analysis of owner equity movements.")]
        [Guide("Common sub-accounts include drawings for personal withdrawals, capital contributions for additional investments, and profit distributions.")]
        [ProtoMember(1)] public string Name { get; set; }

        public static SubAccount ExpenseClaims { get; } = new SubAccount() { Key = new Guid("76329680-c7cc-44b6-8c90-f2ca586e14f8"), Name = Strings.Expense_claims };

        public override string GetName()
        {
            return Name;
        }
    }
}
