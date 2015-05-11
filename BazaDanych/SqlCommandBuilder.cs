
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaDanych
{
    class SqlCommandBuilder
    {
        public string BuildInsertStatement(string tableName, string[] columns, string[] vals)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append("INSERT INTO ");
            sql.Append(tableName);
            sql.Append(" (");
            foreach (string col in columns)
            {
                sql.Append(col + ",");
            }
            sql.Remove(sql.Length - 1, 1);
            sql.Append(") VALUES (");
            foreach(string val in vals)
            {
                sql.Append("'" + val + "',");
            }
            sql.Remove(sql.Length - 1, 1);
            sql.Append(")");

            return sql.ToString();
        }

        public string InsertRecord(string tableName, List<ColumnSchema> columns, object[] vals)
        {
            List<string> cols = new List<string>();
            foreach (ColumnSchema col in columns)
            {
                cols.Add(col.Name);
            }

            string[] row = new string[vals.Length];
            for (int v = 0; v < vals.Length; v++)
            {
                row[v] = vals[v].ToString();
            }

            return BuildInsertStatement(tableName, cols.ToArray(), row);
        }

        public string BuildSelectStatement(string from, string[] columns, string condition = "", string addon = "")
        {
            StringBuilder sql = new StringBuilder();

            sql.Append("SELECT ");
            foreach (string col in columns)
            {
                sql.Append(col + ",");
            }
            sql.Remove(sql.Length - 1, 1);
            sql.Append(" FROM ");
            sql.Append(from);
            sql.Append(" " + condition);
            sql.Append(" " + addon);

            return sql.ToString();
        }

        public string SelectAllRecords(TableSchema schema)
        {
            List<string> cols = new List<string>();
            foreach (ColumnSchema col in schema.Columns)
            {
                cols.Add(col.Name);
            }

            return BuildSelectStatement(schema.Name, cols.ToArray());
        }

        
    }
}
