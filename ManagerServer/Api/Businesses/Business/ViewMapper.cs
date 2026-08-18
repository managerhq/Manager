using ManagerServer.Globalization;
using System;
using System.Collections.Generic;
using System.Linq;
using ReportColumn = ManagerServer.Api.Businesses.Business.Reports.Column;
using ReportRow = ManagerServer.Api.Businesses.Business.Reports.Row;
using ReportRows = ManagerServer.Api.Businesses.Business.Reports.Rows;
using ReportCell = ManagerServer.Api.Businesses.Business.Reports.Cell;
using ReportModel = ManagerServer.Api.Businesses.Business.Reports.ReportModel;
using ReportColumn2 = ManagerServer.Api.Businesses.Business.Reports.V2.Column;
using ReportRow2 = ManagerServer.Api.Businesses.Business.Reports.V2.Row;
using ReportCell2 = ManagerServer.Api.Businesses.Business.Reports.V2.Cell;
using ReportModel2 = ManagerServer.Api.Businesses.Business.Reports.V2.ReportModel2;
using ReportNumberFormat = ManagerServer.Api.Businesses.Business.Reports.ReportNumberFormat;

namespace ManagerServer.Api.Businesses.Business
{
    internal static class ViewMapper
    {
        public static View From(TransactionView source)
        {
            if (source == null) return null;

            var pv = new View
            {
                Title = source.title,
                Reference = source.reference,
                Direction = ParseDirection(source.direction),
                Language = Languages.GetLanguage(),
                Totals = []
            };

            if (source.business != null)
            {
                pv.BusinessName = source.business.name;
                pv.Business = new View.BusinessInfo
                {
                    Name = source.business.name,
                    Logo = source.business.logo,
                    Address = source.business.address,
                    Fields = (source.business.custom_fields ?? new()).Select(MapCustomField).ToList(),
                };
            }

            if (source.recipient != null)
            {
                pv.Recipient = new View.RecipientInfo
                {
                    Code = source.recipient.code,
                    Name = source.recipient.name,
                    Address = source.recipient.address,
                    Email = source.recipient.email
                };
            }

            if (source.emphasis != null && !string.IsNullOrEmpty(source.emphasis.text))
            {
                pv.Status = new View.StatusInfo
                {
                    Text = source.emphasis.text,
                    Tone = source.emphasis.positive ? Tone.Positive : source.emphasis.negative ? Tone.Negative : Tone.Neutral,
                };
            }

            foreach (var f in source.fields ?? new())
            {
                pv.Fields.Add(new View.FieldInfo
                {
                    Key = f.key,
                    Label = f.label,
                    Text = f.text,
                    Emphasis = f.emphasis,
                    DisplayAtTheTop = true,
                });
            }
            foreach (var cf in source.custom_fields ?? new())
            {
                pv.Fields.Add(MapCustomField(cf));
            }

            pv.Table = new View.TableInfo { Description = source.description };
            if (source.table != null)
            {
                pv.Table.Columns = (source.table.columns ?? new()).Select(c => new View.ColumnInfo
                {
                    Label = c.label,
                    Align = ParseAlign(c.align),
                    Nowrap = c.nowrap,
                    Emphasis = c.emphasis,
                    ShrinkToFit = c.minWidth
                }).ToList();

                pv.Table.Rows = (source.table.rows ?? new()).Select(r => new View.RowInfo
                {
                    Cells = (r.cells ?? new()).Select(cell => new View.CellInfo
                    {
                        Text = cell.text,
                        Image = MapImage(cell.image),
                    }).ToList(),
                }).ToList();

                var columns = source.table.columns ?? new();
                if (columns.Any(c => c.total))
                {
                    pv.Table.Rows.Add(new View.RowInfo
                    {
                        IsTotalRow = true,
                        Cells = columns.Select(c => new View.CellInfo
                        {
                            Text = c.sumText ?? string.Empty,
                        }).ToList(),
                    });
                }

                foreach (var t in source.table.totals ?? new())
                {                    
                    pv.Totals.Add(new View.TotalInfo
                    {
                        Key = t.key,
                        Class = t.@class,
                        Label = t.label,
                        Text = t.text,
                        Number = t.number,
                        Emphasis = t.emphasis,
                    });
                }
            }

            if (source.footers != null)
            {
                pv.Footers = source.footers.ToList();
            }

            return pv;
        }

        public static View From(ReportModel source)
        {
            if (source == null) return null;

            var pv = new View
            {
                BusinessName = source.Business,
                Title = source.Title,
                Direction = ParseDirection(source.Direction),
                Language = Languages.GetLanguage(),
            };

            if (!string.IsNullOrEmpty(source.Subtitle)) pv.Subtitles.Add(source.Subtitle);
            if (!string.IsNullOrEmpty(source.Subtitle2)) pv.Subtitles.Add(source.Subtitle2);

            var columns = new List<View.ColumnInfo>
            {
                new View.ColumnInfo { Label = "", Align = Align.Start },
            };
            columns.AddRange((source.Columns ?? new()).Select(MapColumn));

            pv.Table = new View.TableInfo { Columns = columns };

            if (source.Rows != null)
            {
                pv.Table.Rows = MapRowList(source.Rows);
            }

            if (!string.IsNullOrEmpty(source.Footer))
            {
                pv.Footers.Add(@"<div style=""text-align: center; white-space: pre-line"">"+source.Footer+"</div>");
            }

            return pv;
        }

        public static View From(ReportModel2 source)
        {
            if (source == null) return null;

            var pv = new View
            {
                BusinessName = source.Business,
                Title = source.Title,
                Direction = ParseDirection(source.Direction),
                Language = Languages.GetLanguage(),
            };

            foreach (var s in source.Subtitles ?? new())
            {
                if (!string.IsNullOrEmpty(s)) pv.Subtitles.Add(s);
            }

            var columns = new List<View.ColumnInfo>
            {
                new View.ColumnInfo { Label = "", Align = Align.Start },
            };
            columns.AddRange((source.Columns ?? new()).Select(MapColumn2));

            var leafColumns = FlattenColumns2(source.Columns);
            pv.Table = new View.TableInfo
            {
                Columns = columns,
                Rows = (source.Rows ?? new()).Select(r => MapRow2(r, leafColumns)).ToList(),
            };

            if (!string.IsNullOrEmpty(source.Footer))
            {
                pv.Footers.Add(@"<div style=""text-align: center; white-space: pre-line"">" + source.Footer + "</div>");
            }

            return pv;
        }

        private static View.FieldInfo MapCustomField(TransactionView.CustomField cf) => new()
        {
            Key = cf.key,
            Label = cf.label,
            Text = cf.text,
            Image = MapImage(cf.image),
            DisplayAtTheTop = cf.displayAtTheTop,
        };

        private static View.ImageInfo MapImage(TransactionView.Image image)
        {
            if (image == null || string.IsNullOrEmpty(image.url)) return null;
            return new View.ImageInfo
            {
                Url = image.url,
                Width = image.width,
                Height = image.height,
            };
        }

        private static View.ColumnInfo MapColumn(ReportColumn c) => new()
        {
            Label = c.Name,
            Key = c.Key,
            Align = Align.Right,
            Emphasis = c.IsBold,
            Subcolumns = c.Subcolumns?.Select(MapColumn).ToList(),
        };

        private static View.ColumnInfo MapColumn2(ReportColumn2 c) => new()
        {
            Label = c.Name,
            Key = c.Key,
            Align = Align.Right,
            Emphasis = c.IsBold,
            Subcolumns = c.Subcolumns?.Select(MapColumn2).ToList(),
        };

        private static View.RowInfo MapRow2(ReportRow2 r, List<ReportColumn2> leafColumns)
        {
            var labelCell = new View.CellInfo { Text = r.Name ?? "" };
            if (r.Rows != null)
            {
                var nestedRows = r.Rows.Select(c => MapRow2(c, leafColumns)).ToList();
                if (r.Rows.Count > 1)
                {
                    var subtotalLabel = !string.IsNullOrEmpty(r.Name) ? Strings.Total + " — " + r.Name : "";
                    var subtotalCells = new List<View.CellInfo> { new View.CellInfo { Text = subtotalLabel } };
                    if (r.Cells != null)
                    {
                        for (int i = 0; i < r.Cells.Count; i++)
                        {
                            if (i < leafColumns.Count && leafColumns[i].HideTotals)
                            {
                                subtotalCells.Add(new View.CellInfo { Text = "" });
                            }
                            else
                            {
                                subtotalCells.Add(MapCell2(r.Cells[i]));
                            }
                        }
                    }
                    nestedRows.Add(new View.RowInfo { Cells = subtotalCells, IsTotalRow = true });
                }
                return new View.RowInfo
                {
                    Cells = new List<View.CellInfo> { labelCell },
                    Rows = nestedRows,
                };
            }
            var cells = new List<View.CellInfo> { labelCell };
            if (r.Cells != null) cells.AddRange(r.Cells.Select(MapCell2));
            return new View.RowInfo { Cells = cells, IsTotalRow = r.IsBold };
        }

        private static List<ReportColumn2> FlattenColumns2(List<ReportColumn2> columns)
        {
            var leaves = new List<ReportColumn2>();
            if (columns == null) return leaves;
            foreach (var c in columns)
            {
                if (c.Subcolumns != null && c.Subcolumns.Count > 0) leaves.AddRange(FlattenColumns2(c.Subcolumns));
                else leaves.Add(c);
            }
            return leaves;
        }

        private static View.CellInfo MapCell2(ReportCell2 c)
        {
            if (c == null) return null;
            var text = c.Text ?? (c.Value.HasValue ? ReportNumberFormat.Format(c.Value, c.Style, wholeNumbers: false) : "");
            return new View.CellInfo
            {
                Text = text ?? "",
                Link = c.Link != null ? new View.LinkInfo() { Url = c.Link.Href } : null,
            };
        }

        private static Direction ParseDirection(string s) => s == "rtl" ? Direction.Rtl : Direction.Ltr;

        private static Align ParseAlign(string s) => s switch
        {
            "center" => Align.Center,
            "end" => Align.End,
            "right" => Align.Right,
            _ => Align.Start,
        };

        private static List<View.RowInfo> MapRowList(ReportRows rows)
        {
            var result = new List<View.RowInfo>();
            foreach (var item in rows.Items ?? new List<ReportRow>())
            {
                EmitRow(result, item);
            }
            return result;
        }

        private static void EmitRow(List<View.RowInfo> target, ReportRow r)
        {
            List<View.RowInfo> nestedRows = null;
            if (r.Rows != null)
            {
                nestedRows = new List<View.RowInfo>();
                foreach (var inner in r.Rows.Items ?? new List<ReportRow>())
                {
                    EmitRow(nestedRows, inner);
                }
                if (!r.Rows.MakeTotalStandOut)
                {
                    var label = !string.IsNullOrEmpty(r.GroupTotalText) ? r.GroupTotalText : r.Rows.TotalText;
                    if (!r.Rows.HideTotals && r.Rows.Items.Count > 1)
                    {
                        AppendClosingTotal(nestedRows, label, r.Rows.TotalCells);
                    }
                }
            }

            target.Add(new View.RowInfo
            {
                Cells = BuildRowCells(r.DisplayName, r.Cells, isGroup: nestedRows != null),
                Rows = nestedRows,
                IsTotalRow = r.IsTotalRow || r.MakeStandOut,
            });

            if (r.Rows != null && r.Rows.MakeTotalStandOut)
            {
                var label = !string.IsNullOrEmpty(r.GroupTotalText) ? r.GroupTotalText : r.Rows.TotalText;
                if (!r.Rows.HideTotals && r.Rows.Items.Count > 0)
                {
                    AppendClosingTotal(target, label, r.Rows.TotalCells);
                }
            }
        }

        private static void AppendClosingTotal(List<View.RowInfo> target, string label, List<ReportCell> cells)
        {
            if (string.IsNullOrEmpty(label) && (cells == null || cells.Count == 0)) return;
            target.Add(new View.RowInfo
            {
                IsTotalRow = true,
                Cells = BuildRowCells(label, cells, isGroup: false),
            });
        }

        private static List<View.CellInfo> BuildRowCells(string label, List<ReportCell> dataCells, bool isGroup)
        {
            var labelCell = new View.CellInfo { Text = label ?? "" };
            if (isGroup) return new List<View.CellInfo> { labelCell };
            var cells = new List<View.CellInfo> { labelCell };
            if (dataCells != null) cells.AddRange(dataCells.Select(MapCell));
            return cells;
        }

        private static View.CellInfo MapCell(ReportCell c)
        {
            if (c == null) return null;
            return new View.CellInfo
            {
                Text = c.Text,
                Link = c.Link != null ? new View.LinkInfo() { Url = c.Link.Href } : null
            };
        }
    }
}
