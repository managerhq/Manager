#nullable enable
using ManagerServer.Globalization;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace ManagerServer.Api.Businesses.Business
{
    [Description("Top-level document view returned by view endpoints. The shape is pre-formatted for theme rendering: all Text values are server-formatted strings that the client renders verbatim — no client-side rounding, currency formatting, or date formatting.")]
    internal sealed class View
    {
        [Description("Text direction.")]
        public Direction Direction { get; set; }

        [Description("Language code the document was rendered in (e.g. 'en', 'ar', 'zh-hk'). Defaults to 'en' when no override is provided.")]
        public string? Language { get; set; }

        [Description("Issuing business name surfaced at the top level so it can be used as the document's PDF/email subject. Themes display this above the Title only when Business is null; when Business is non-null, the same value is mirrored as BusinessInfo.Name and themes render it from there.")]
        public string? BusinessName { get; set; }

        [Description("Primary heading for the document.")]
        public required string Title { get; set; }

        [Description("Sub-heading lines beneath the title (e.g. period range, comparison period).")]
        public List<string> Subtitles { get; set; } = new();

        [Description("Document reference / identifier (e.g. invoice number).")]
        public string? Reference { get; set; }

        [Description("Issuing business — name, address, logo, custom fields.")]
        public BusinessInfo? Business { get; set; }

        [Description("Recipient party (customer, supplier, employee). Null for non-document views like reports.")]
        public RecipientInfo? Recipient { get; set; }

        [Description("Optional status for the document (e.g. 'Paid in Full', 'Overdue', 'Draft').")]
        public StatusInfo? Status { get; set; }

        [Description("Labeled key/value rows attached to the document (e.g. Date, Due Date, user-defined custom fields).")]
        public List<FieldInfo> Fields { get; set; } = new();

        [Description("The single body table. Rows can be flat (transactions) or hierarchical (reports).")]
        public TableInfo Table { get; set; } = new();

        [Description("Stacked totals beneath the table (e.g. Subtotal, Tax, Total). Each entry has its own label.")]
        public List<TotalInfo>? Totals { get; set; }

        [Description("Pre-rendered HTML footer blocks shown beneath the table.")]
        public List<string> Footers { get; set; } = new();

        [Description("Issuing business shown in the document header — name, address, logo, and labeled custom fields.")]
        internal sealed class BusinessInfo
        {
            [Description("Business name. Themes render this inside the Business section. Mirrors the top-level View.BusinessName so the section can be rendered as a self-contained block.")]
            public string? Name { get; set; }

            [Description("URL to the business logo image.")]
            public string? Logo { get; set; }

            [Description("Business postal address.")]
            public string? Address { get; set; }

            [Description("Labeled key/value rows displayed alongside the business address (e.g. ABN, registration number).")]
            public List<FieldInfo> Fields { get; set; } = new();
        }

        [Description("Recipient party for the document (customer, supplier, employee). Null for non-document views like reports.")]
        internal sealed class RecipientInfo
        {
            [Description("Recipient short code (customer code, supplier code, employee code).")]
            public string? Code { get; set; }

            [Description("Recipient name.")]
            public string? Name { get; set; }

            [Description("Recipient postal address.")]
            public string? Address { get; set; }

            [Description("Recipient email address.")]
            public string? Email { get; set; }
        }

        [Description("Document status badge (e.g. 'Paid in Full', 'Overdue', 'Draft') with an associated visual tone.")]
        internal sealed class StatusInfo
        {
            [Description("Status text.")]
            public required string Text { get; set; }

            [Description("Visual tone of the status — positive (e.g. paid in full), negative (e.g. overdue), or neutral (e.g. draft).")]
            public Tone Tone { get; set; }
        }

        [Description("Labeled key/value entry. May carry leaf content (Text or Image), nested sub-fields, or both.")]
        internal sealed class FieldInfo
        {
            [Description("Stable identifier for the field, when one exists (e.g. 'DueDate'). Null when the field has no canonical identifier (e.g. ad-hoc custom fields).")]
            public string? Key { get; set; }

            [Description("Display label.")]
            public required string Label { get; set; }

            [Description("Display-formatted text.")]
            public string? Text { get; set; }

            [Description("Optional inline image rendered in place of text.")]
            public ImageInfo? Image { get; set; }

            [Description("True if the field should appear in the header area rather than within the body.")]
            public bool DisplayAtTheTop { get; set; }

            [Description("True if the field should be visually emphasized.")]
            public bool Emphasis { get; set; }

            [Description("Optional hyperlink attached to the field.")]
            public LinkInfo? Link { get; set; }

            [Description("Optional nested sub-fields. May be combined with Text/Image on the same field.")]
            public List<FieldInfo>? Fields { get; set; }
        }

        [Description("Labeled total displayed beneath the table (e.g. Subtotal, Tax, Total).")]
        internal sealed class TotalInfo
        {
            [Description("Stable identifier for the total, when one exists (e.g. 'Total'). Null when the total has no canonical identifier.")]
            public string? Key { get; set; }

            [Description("Classification or type of the total, when applicable. Null when the total does not belong to a predefined class.")]
            public string? Class { get; set; }

            [Description("Display label (e.g. 'Subtotal', 'Tax', 'Total').")]
            public required string Label { get; set; }

            [Description("Pre-formatted display text. Themes render this verbatim.")]
            public required string Text { get; set; }

            [Description("Numeric value of the total. Exposed alongside Text for non-rendering consumers (exports, integrations); themes should render Text.")]
            public decimal Number { get; set; }

            [Description("True if the total should be visually emphasized (e.g. the grand total).")]
            public bool Emphasis { get; set; }
        }

        [Description("The single body table — columns and rows (flat or hierarchical). Total rows are regular rows with IsTotalRow set.")]
        internal sealed class TableInfo
        {
            [Description("Free-form prose attached to the table (e.g. transaction description above the line items).")]
            public string? Description { get; set; }

            [Description("Column definitions, in display order.")]
            public List<ColumnInfo> Columns { get; set; } = new();

            [Description("Row data. Items may nest into sub-groups for hierarchical reports. Total rows appear as regular rows with IsTotalRow set.")]
            public List<RowInfo> Rows { get; set; } = new();
        }

        [Description("Column definition for the body table — header label, alignment, formatting flags, computed sum, and optional nested sub-columns for grouped headers.")]
        internal sealed class ColumnInfo
        {
            [Description("Column header label.")]
            public required string Label { get; set; }

            [Description("Stable key for the column (used by themes for selectors).")]
            public string? Key { get; set; }

            [Description("Text alignment. Use Start/End for direction-aware alignment that flips under RTL; use Right when the column must always align right regardless of document direction (e.g. monetary values).")]
            public Align Align { get; set; }

            [Description("True if cell content should not wrap.")]
            public bool Nowrap { get; set; }

            [Description("True if the column should be visually emphasized.")]
            public bool Emphasis { get; set; }

            [Description("True if the column should hug its content rather than expand.")]
            public bool ShrinkToFit { get; set; }

            [Description("Optional nested columns for grouped report headers.")]
            public List<ColumnInfo>? Subcolumns { get; set; }
        }

        [Description("Single row in the body table. May be a leaf (with Cells), a group container (with nested Rows), or a total row (IsTotalRow).")]
        internal sealed class RowInfo
        {
            [Description("Cells aligned 1:1 with leaf columns. For group container rows (with nested Rows), Cells contains a single header cell which themes render as a full-width row spanning all columns.")]
            public List<CellInfo>? Cells { get; set; }

            [Description("Optional nested sub-rows; null for leaf and total rows.")]
            public List<RowInfo>? Rows { get; set; }

            [Description("True if this row is a total (running subtotal between leaves, or closing total at the end of a group). Total rows carry Cells but never nested Rows, and are visually emphasized.")]
            public bool IsTotalRow { get; set; }
        }

        [Description("Single cell in a row. Carries pre-formatted display text and may optionally include an inline image and a hyperlink — they may co-exist (e.g. a clickable image).")]
        internal sealed class CellInfo
        {
            [Description("Pre-formatted display text. Themes render this verbatim — no client-side rounding or reformatting. When Image is set, this serves as the image's accessible label / alt text.")]
            public required string Text { get; set; }

            [Description("Optional inline image rendered in place of text within the cell; Text becomes the image's accessible label. May be combined with Link to make the image clickable.")]
            public ImageInfo? Image { get; set; }

            [Description("Optional hyperlink wrapping the cell content. Combine with Image for a clickable image, or with Text for a clickable label.")]
            public LinkInfo? Link { get; set; }
        }

        [Description("Inline image reference with optional natural dimensions.")]
        internal sealed class ImageInfo
        {
            [Description("URL of the image asset.")]
            public required string Url { get; set; }

            [Description("Optional natural width in pixels.")]
            public int? Width { get; set; }

            [Description("Optional natural height in pixels.")]
            public int? Height { get; set; }
        }

        [Description("Hyperlink target attached to a field or cell. When rendered as an <a href=...> anchor, the theme must set target=\"_top\" so navigation replaces the top-level page rather than loading inside the iframe the report is hosted in.")]
        internal sealed class LinkInfo
        {
            [Description("Destination URL. Anchors built from this value must include target=\"_top\" so the link navigates the top frame instead of the embedding iframe.")]
            public required string Url { get; set; }
        }
    }

    [JsonConverter(typeof(JsonStringEnumConverter<Direction>))]
    internal enum Direction
    {
        [Description("Left-to-right text direction (e.g. English, French).")]
        [JsonStringEnumMemberName("ltr")] Ltr,

        [Description("Right-to-left text direction (e.g. Arabic, Hebrew).")]
        [JsonStringEnumMemberName("rtl")] Rtl,
    }

    [JsonConverter(typeof(JsonStringEnumConverter<Align>))]
    internal enum Align
    {
        [Description("Aligned to the leading edge — left in LTR, right in RTL.")]
        [JsonStringEnumMemberName("start")] Start,

        [Description("Centered horizontally.")]
        [JsonStringEnumMemberName("center")] Center,

        [Description("Aligned to the trailing edge — right in LTR, left in RTL.")]
        [JsonStringEnumMemberName("end")] End,

        [Description("Always aligned to the right edge regardless of text direction.")]
        [JsonStringEnumMemberName("right")] Right,
    }

    [JsonConverter(typeof(JsonStringEnumConverter<Tone>))]
    internal enum Tone
    {
        [Description("Neutral status — informational, not pass/fail (e.g. 'Draft').")]
        [JsonStringEnumMemberName("neutral")] Neutral,

        [Description("Positive status — favourable outcome (e.g. 'Paid in Full').")]
        [JsonStringEnumMemberName("positive")] Positive,

        [Description("Negative status — adverse outcome (e.g. 'Overdue').")]
        [JsonStringEnumMemberName("negative")] Negative,
    }
}