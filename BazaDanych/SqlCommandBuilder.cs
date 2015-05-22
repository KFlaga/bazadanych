
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

        public string BuildUpdateStatement(string tableName, string[] columns, string[] vals)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append("UPDATE ");
            sql.Append(tableName);
            sql.Append(" SET ");
            for (int i = 0; i < columns.Length; i++)
            {
                sql.Append(String.Format("{0}='{1}',", columns[i], vals[i]));
            }
            sql.Remove(sql.Length - 1, 1);
            sql.Append(" WHERE ");
            for (int i = 0; i < columns.Length; i++)
            {
                if (columns[i] == "ID")
                {
                    sql.Append(String.Format("{0} = '{1}'", columns[i], vals[i]));
                }
            }
            return sql.ToString();
        }

        public string BuildDeleteStatement(string tableName, string[] columns, string[] vals)
        {
            StringBuilder sql = new StringBuilder();

            sql.Append("DELETE FROM ");
            sql.Append(tableName);
            sql.Append(" WHERE ");
            for (int i = 0; i < columns.Length; i++)
            {
                if (columns[i] == "ID")
                {
                    sql.Append(String.Format("{0} = '{1}'", columns[i], vals[i]));
                }
            }
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

        public string EditRecord(string tableName, List<ColumnSchema> columns, object[] vals)
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

            return BuildUpdateStatement(tableName, cols.ToArray(), row);
        }

        public string DeleteRecord(string tableName, List<ColumnSchema> columns, object[] vals)
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

            return BuildDeleteStatement(tableName, cols.ToArray(), row);
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

            return BuildSelectStatement(schema.Owner+"."+schema.Name, cols.ToArray());
        }

        
    }
}
