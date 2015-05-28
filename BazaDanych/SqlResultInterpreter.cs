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
                    else if (tabSchema.Columns[col].type == Type.GetType("System.String"))
                        vals[col] = reader.GetString(col);
                    else if (tabSchema.Columns[col].type == Type.GetType("System.Int32"))
                        vals[col] = reader.GetInt32(col);
                    else if (tabSchema.Columns[col].type == Type.GetType("System.Int16"))
                        vals[col] = reader.GetInt16(col);
                    else if (tabSchema.Columns[col].type == Type.GetType("System.DateTime"))
                    {
                        DateOnly date = new DateOnly();
                        date.Date = reader.GetDateTime(col);
                        vals[col] = date;
                    }
                    else
                        vals[col] = reader.GetValue(col);
                }
                table.Rows.Add(vals);
            }

            return table;
        }

        public object InterpretSingleSelection(OracleDataReader reader, string type = "object")
        {
            if (reader == null)
                return null;

            if (reader.Read() && !reader.IsDBNull(0))
            {
                if (type == "int")
                    return reader.GetInt32(0);
                else
                    return reader.GetValue(0);
            }
            else
            {
                return null;
            }
        }
    }
}
