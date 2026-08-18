using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace ManagerComponents
{
    public sealed class Table : ComponentBase
    {
        public string Term;
        public readonly List<TableColumn> Columns = new();

        // Container classes
        private const string ContainerOverflow = "overflow-x-auto lg:overflow-visible no-scrollbar";

        public override void BuildString(StringBuilder sb)
        {
            using (sb.Div(@class: ContainerOverflow))
            {
                using (sb.Table(@class: "card-table"))
                {
                    using (sb.THead())
                    {
                        using (sb.Tr())
                        {
                            foreach (var e in Columns)
                            {
                                e.BuildString(sb);
                            }
                        }
                    }

                    var rows = Columns.Max(x => x.Cells.Length);

                    using (sb.TBody())
                    {
                        for (int i = 0; i < rows; i++)
                        {
                            using (sb.Tr())
                            {
                                foreach (var e in Columns)
                                {
                                    var cellClasses = e.GetCellClasses();
                                    var cell = e.Cells.ElementAtOrDefault(i);
                                    if (cell != null) cellClasses += " " + cell.GetCellClasses();

                                    using (sb.Td(@class: cellClasses))
                                    {
                                        if (cell != null)
                                        {
                                            if (e.TabularNums)
                                            {
                                                using (sb.Div(@class: "tabular-nums observer:blur-sm observer:hover:blur-none observer:hover:transition"))
                                                {
                                                    cell.BuildString(sb);
                                                }
                                            }
                                            else
                                            {
                                                cell.BuildString(sb);
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    if (Columns.Any(x => !string.IsNullOrWhiteSpace(x.Sum)))
                    {
                        using (sb.TFoot())
                        {
                            using (sb.Tr())
                            {
                                foreach (var e in Columns)
                                {
                                    var footerText = string.Empty;
                                    if (e.Right) footerText += "text-right";
                                    if (e.Center) footerText += "text-center";

                                    using (sb.Th(@class: footerText))
                                    {
                                        if (e.TabularNums)
                                        {
                                            using (sb.Div(@class: "tabular-nums observer:blur-sm observer:hover:blur-none observer:hover:transition"))
                                            {
                                                sb.Append(e.Sum);
                                            }
                                        }
                                        else
                                        {
                                            sb.Append(e.Sum);
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(Term))
            {
                sb.Script(src: "resources/mark/mark-min.js");
                using (sb.Script())
                {
                    sb.Append("var instance = new Mark('td');");
                    sb.Append($"instance.mark('{JavaScriptEncoder.Default.Encode(Term)}');");
                }
            }
        }
    }
}
