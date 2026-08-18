using System;
using ManagerServer.Model.Attributes;
using ManagerServer.Model.Enums;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ProtoBuf;
using ManagerServer.Attributes;

namespace ManagerServer.Model
{
    [ProtoContract]
    [Singleton]
    [Guid("a56e89d1-7bee-4509-8b84-c9ebc3808b0c")]
    public sealed class DateAndNumberFormat : Object
    {
        [Guide("Choose a date format. This will determine how dates are entered and displayed throughout the business.")]
        [ProtoMember(1), NoWrap, DateFormat] public string DateFormat { get; set; }

        [Guide("Choose the time format. This determines how time is displayed throughout the business.")]
        [ProtoMember(4), NoWrap, TimeFormat] public string TimeFormat { get; set; }

        [Guide("Choose the first day of the week that is standard for your region. This setting adjusts the calendar picker to display the calendar in a way that is familiar to you.")]
        [ProtoMember(2)] public FirstDayOfWeek FirstDayOfWeek { get; set; }

        [Guide("Choose the number format. This format will be applied to how all numbers and currencies are displayed throughout the business.")]
        [ProtoMember(3), NumberFormat] public string NumberFormat { get; set; }

        [ProtoContract]
        public sealed class NumberFormatParts
        {
            [Guide("The character used to separate decimal places (e.g., . or ,).")]
            [ProtoMember(1)] public string DecimalSeparator { get; set; }
            [Guide("The character used to separate thousands groups (e.g., , or space).")]
            [ProtoMember(2)] public string GroupSeparator { get; set; }
            [Guide("The size of digit groups (e.g., [3] for thousands grouping).")]
            [ProtoMember(3)] public int[] GroupSizes { get; set; }
        }
    }
}
