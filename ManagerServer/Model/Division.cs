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
    [Guid("cc7fc110-e3e4-4b3b-823d-86c4a4cdabbc")]
    public sealed class Division : NamedObject, ICode
    {
        [Guide("Enter the name of the division, such as 'North Region', 'Manufacturing Department', or 'Online Sales'.")]
        [Guide("Divisions help you track performance and profitability of different business segments independently.")]
        [Guide("Each transaction can be assigned to a division to build divisional financial statements.")]
        [ProtoMember(2), NoWrap] public string Name { get; set; }
        [Guide("Enter an optional code for this division to make it easier to identify and select in reports and transactions.")]
        [Guide("Codes are useful for quick data entry and can follow your existing organizational structure.")]
        [Guide("Examples: 'NORTH', 'MFG', 'ONLINE', or use numeric codes like '100', '200', '300'.")]
        [ProtoMember(4), Short, Placeholder(nameof(Strings.Optional))] public string Code { get; set; }
        [Guide("Mark this division as inactive to hide it from dropdown lists while preserving historical data.")]
        [Guide("Useful for closed divisions, discontinued operations, or temporarily suspended business segments.")]
        [Guide("Inactive divisions remain in historical reports but cannot be selected for new transactions.")]
        [ProtoMember(3)] public bool Inactive { get; set; }

        string ICode.Code => Code;

        public override string GetName()
        {
            return Name;
        }

        public override bool OnAutocomplete(Object filter)
        {
            if (Inactive) return false;
            return true;
        }
    }
}
