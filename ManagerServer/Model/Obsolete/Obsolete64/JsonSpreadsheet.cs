using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ManagerServer.Model.Obsolete.Obsolete64
{
    public sealed class JsonSpreadsheet
    {
        public int[] colWidths;
        public string[][] data;
        public Cell[] cell;
        public MergeCell[] mergeCells;

        public sealed class Cell
        {
            public int col;
            public int row;
            public string className;
        }

        public sealed class MergeCell
        {
            public int col;
            public int colSpan;
            public int row;
            public int rowSpan;
        }
    }
}
