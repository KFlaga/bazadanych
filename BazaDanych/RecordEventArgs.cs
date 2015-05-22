using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BazaDanych
{
    public class RecordEventArgs : EventArgs
    {
        public Table Table { get; set; }
        public List<ColumnSchema> EditedColummns { get; set; }
        public List<object[]> EditedRows { get; set; }
        public int EditedIndex { get; set; }
    }
}
