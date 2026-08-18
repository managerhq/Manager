using ManagerServer.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagerServer.Globalization;

namespace ManagerServer.HttpHandlers.Businesses.Business
{
    internal abstract class NakedObjectsWithRunningTotals : NakedObjectsWithSlicing
    {
        protected override void InnerGet4(Context context)
        {
            var rows = context.Get<Array>();
            var columns = context.Get<Column[]>();
            var columns2 = new List<Column>();
            foreach (var e in columns)
            {
                columns2.Add(e);
                if (e.Attributes.OfType<RunningTotal>().Any() && e is Column<decimal> && e.Visible)
                {
                    e.EnsureCells(rows);
                    var runningTotal = 0m;
                    var runningTotalValues = new List<decimal>();
                    for (int i = rows.Length-1; i >= 0; i--)
                    {
                        var value = (decimal)e.GetValue(rows.GetValue(i));
                        runningTotal += value;
                        runningTotalValues.Insert(0, runningTotal);
                    }
                    columns2.Add(new RunningTotalColumn(rows, runningTotalValues.ToArray())
                    {
                        Attributes = new Attribute[]
                        {
                            new RightAttribute()
                        },
                        Label = Strings.Balance,
                        Visible = true
                    });
                }

                if (e.Attributes.OfType<RunningTotal2>().Any() && e.Visible)
                {
                    if (this is NakedObjectsWithSorting nakedObjectsWithSorting && nakedObjectsWithSorting.SortBy.HasValue)
                    {
                        e.Visible = false;
                    }
                    else
                    {
                        if (e.CanEnsureCells(rows))
                        {
                            e.EnsureCells(rows);
                        }
                        else
                        {
                            e.Visible = false;
                        }
                    }
                }
            }

            context.Set<Column[]>(columns2.ToArray());

            base.InnerGet4(context);
        }

        public sealed class RunningTotal : Attribute { }
        public sealed class RunningTotal2 : Attribute { }

        protected sealed class RunningTotalColumn : Column<decimal>
        {
            public RunningTotalColumn(Array rows, decimal[] values)
            {
                AddValues(rows, values);
            }

            public override void EnsureCells(Array rows)
            {
                var values = rows.Cast<object>().Select(x => 0m).ToArray();
                AddValues(rows, values);
            }
        }
    }
}