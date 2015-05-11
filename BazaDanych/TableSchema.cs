using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaDanych
{
    public class TableSchema
    {
        public string Name { get; set; }
        public List<ColumnSchema> Columns { get; set; }

        public TableSchema()
        {
            Columns = new List<ColumnSchema>();
        }

        public override string ToString()
        {
            return Name;
        }

    }
}
