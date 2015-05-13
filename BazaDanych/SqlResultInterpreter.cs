using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaDanych
{
    class SqlResultInterpreter
    {
        public Table InterpretSelectResult(OracleDataReader reader, TableSchema tabSchema)
        {
            Table table = new Table(tabSchema);

            if (reader == null)
                return table;

            while (reader.Read())
            {
                object[] vals = new object[tabSchema.Columns.Count];
                for (int col = 0; col < tabSchema.Columns.Count; col++)
                {
                    if (reader.IsDBNull(col))
                        vals[col] = null;
                    else
                        vals[col] = reader.GetValue(col);
                }
                table.Rows.Add(vals);
            }

            return table;
        }
    }
}
