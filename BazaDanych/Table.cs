using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaDanych
{
    public class Table : IEnumerable
    {
        public TableSchema TableSchema { get; private set; }

        public List<ColumnSchema> Columns 
        {
            get
            {
                return TableSchema.Columns;
            }
        }

        public List<object[]> Rows { get; set; }

        public Table(TableSchema schema)
        {
            TableSchema = schema;
            Rows = new List<object[]>();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return new GenericEnumerator(Rows.ToArray());
        }

        public object FindColumnData(int idx, string colName)
        {
            for(int col= 0; col < Rows[idx].Length; col++)
            {
                if (Columns[col].Name.Equals(colName,StringComparison.OrdinalIgnoreCase))
                    return Rows[idx][col];
            }
            return null;
        }

        public void SetColumnData(int idx, string colName, object value)
        {
            for (int col = 0; col < Rows[idx].Length; col++)
            {
                if (Columns[col].Name.Equals(colName, StringComparison.OrdinalIgnoreCase))
                    Rows[idx][col] = value;
            }
        }
    }
}
