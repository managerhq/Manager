using System;
using System.Collections.Generic;
using System.Linq;

namespace ManagerServer.Model
{
    public interface IHasAutomaticReference : IObject
    {
        string Reference { get; set; }
        bool AutomaticReference { get; set; }

        public long GetNextReference(IEnumerable<IHasAutomaticReference> items)
        {
            long reference = 1;
            foreach (var e in items.Where(x => x.Key != this.Key && !x.AutomaticReference && !string.IsNullOrWhiteSpace(x.Reference)).Select(x => x.Reference))
            {
                var s = string.Join("", e.ToCharArray().Where(x => char.IsDigit(x)));
                if (string.IsNullOrWhiteSpace(s)) continue;
                var i = 0L;
                if (long.TryParse(s, out i))
                {
                    if (i >= reference) reference = i + 1;
                }
            }
            return reference;
        }
    }
}