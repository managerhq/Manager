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
    [Singleton]
    [Guid("4c5dac8f-2d5e-4634-a51b-0bbdd021a499")]
    public sealed class LockDate : Object
    {
        [Guide("Check this box to enable period locking. This prevents editing, deleting, or adding transactions dated on or before the lock date. Use this feature to protect completed accounting periods from accidental changes.")]
        [ProtoMember(2)] public bool LockAccountingPeriods { get; set; }
        [Guide("Enter the lock date. All transactions dated on or before this date will be protected from changes. Typically set to the last day of a completed accounting period after reconciliation and finalization.")]
        [ProtoMember(1), IfTrue(nameof(LockAccountingPeriods)), Prepend(nameof(Strings.Until)), NoLabel] public DateTime? Date { get; set; }

        public DateTime? GetLockDate()
        {
            if (!LockAccountingPeriods) return null;
            return Date;
        }

        public DateTime GetUnlockedDate()
        {
            if (GetLockDate().HasValue) return GetLockDate().Value.AddDays(1);
            return DateTime.MinValue;
        }

        public bool IsLocked(DateTime date)
        {
            var lockDate = GetLockDate();
            if (!lockDate.HasValue) return false;
            if (lockDate.Value < date) return false;
            return true;
        }

        public static LockDate Default { get; } = new LockDate() { Key = new Guid("4c5dac8f-2d5e-4634-a51b-0bbdd021a499") };
    }
}
