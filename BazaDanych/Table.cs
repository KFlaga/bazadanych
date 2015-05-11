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
                if (Columns[idx].Name == colName)
                    return Rows[idx][col];
            }
            return null;
        }
    }
}
