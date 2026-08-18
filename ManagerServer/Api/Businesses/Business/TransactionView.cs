using System;
using System.Collections.Generic;

namespace ManagerServer.Api.Businesses.Business
{
    public sealed class TransactionView
    {
        public string type { get; set; }
        public Guid key { get; set; }
        public Guid? custom_theme { get; set; }
        public string direction { get; set; }
        public string title { get; set; }
        public string description { get; set; }
        public string reference { get; set; }
        public Emphasis emphasis { get; set; } = new Emphasis();
        public List<Field> fields { get; set; } = new List<Field>();
        public List<CustomField> custom_fields { get; set; } = new List<CustomField>();
        public Table table { get; set; } = new Table();
        public Business business { get; set; } = new Business();
        public Recipient recipient { get; set; } = new Recipient();
        public long timestamp { get; set; }
        public string[] footers { get; set; }
        public string serviceLink { get; set; }
        public bool legacyQrCodeForSaudiArabia { get; set; }

        public Dictionary<int, string> EmailVariables { get; set; } = new Dictionary<int, string>();

        public sealed class Emphasis
        {
            public string text { get; set; }
            public bool positive { get; set; }
            public bool negative { get; set; }
        }

        public sealed class Table
        {
            public List<Column> columns { get; set; } = new List<Column>();
            public List<Row> rows { get; set; } = new List<Row>();
            public List<Total> totals { get; set; } = new List<Total>();
        }

        public sealed class Column
        {
            public string label { get; set; }
            public string align { get; set; } = "start";
            public bool nowrap { get; set; }
            public bool total { get; set; }
            public bool emphasis { get; set; }
            public bool minWidth { get; set; }
            public bool alwaysShow { get; set; }
            public decimal? sum { get; set; }
            public string sumText { get; set; }
        }

        public sealed class Row
        {
            public List<Cell> cells { get; set; } = new List<Cell>();
        }

        public sealed class Cell
        {
            public object value { get; set; }
            public bool canBeHidden { get; set; }
            public string text { get; set; }
            public Image image { get; set; }
        }

        public sealed class Image
        {
            public string url { get; set; }
            public int? width { get; set; }
            public int? height { get; set; }
        }

        public sealed class Recipient
        {
            public string code { get; set; }
            public string name { get; set; }
            public string address { get; set; }
            public string email { get; set; }
            public string telephone { get; set; }
        }

        public sealed class Business
        {
            public string logo { get; set; }
            public string name { get; set; }
            public string address { get; set; }
            public List<CustomField> custom_fields { get; set; } = new List<CustomField>();
        }

        public sealed class Total
        {
            public string @class { get; set; } = string.Empty;
            public string key { get; set; } = string.Empty;
            public string label { get; set; }
            public string text { get; set; }
            public decimal number { get; set; }
            public bool emphasis { get; set; }
        }

        public sealed class Field
        {
            public string key { get; set; } = string.Empty;
            public string label { get; set; }
            public string text { get; set; }
            public bool emphasis { get; set; }
        }

        public sealed class CustomField
        {
            public string key { get; set; } = string.Empty;
            public string label { get; set; }
            public string text { get; set; }
            public object value { get; set; }
            public bool displayAtTheTop { get; set; }
            public Image image { get; set; }
        }
    }
}
