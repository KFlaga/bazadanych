using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;

namespace BazaDanych
{
    class DatabaseConnector
    {
        OracleConnection connection;
        public bool Connected
        {
            get
            {
                if (connection.State == ConnectionState.Open)
                    return true;
                else
                    return false;
            }
        }

        public DatabaseConnector()
        {
            connection = new OracleConnection();
        }

        public bool ConnectToDatabase(ConnectionSettings settings)
        {
            try
            {
                connection.Close();
                StringBuilder str = new StringBuilder();
                str.Append("User Id=" + settings.Login + ";");
                str.Append("Password=" + settings.Password + ";");
                if (settings.AsAdmin)
                    str.Append("DBA Privilege=SYSDBA;");
                if (settings.UseTNS)
                    str.Append("Data Source=" + settings.TNSName + ";");
                else
                {
                    str.Append("Data Source=(DESCRIPTION = (ADDRESS = (PROTOCOL = TCP)(HOST =" + settings.Host + ")" +
                        "(PORT =" + settings.Port + "))(CONNECT_DATA=(SERVER = DEDICATED)(SERVICE_NAME=" + settings.SID + ")))");
                }
                connection.ConnectionString = str.ToString();
                connection.Open();
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Nie można utworzyć połączenia: " + ex.Message);
                return false;
            }
            if (connection.State != System.Data.ConnectionState.Open)
            {
                return false;
            }
            return true;
        }

        public void Disconnect()
        {
            connection.Close();
        }

        private OracleCommand GetCommand(string sql, string[] sqlParams = null)
        {
            OracleCommand cmd = new OracleCommand(sql, connection);
            cmd.CommandType = CommandType.Text;

            if( sqlParams != null )
            {
                foreach(string p in sqlParams )
                {
                   // OracleParameter oraParam = cmd.CreateParameter();
                    //oraParam.
                }
            }
            return cmd;
        }

        public void SendSqlNonQuerry(string sql, string[] sqlParams = null) // Polecenie bez zwrotu
        {
            OracleCommand cmd = GetCommand(sql,sqlParams);
            cmd.CommandTimeout = 5;

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Błąd podczas wykonywania polecenia SQL: " + ex.Message);
            }
        }

        public OracleDataReader SendSqlQuerry(string sql, string[] sqlParams = null)
        {
            OracleCommand cmd = GetCommand(sql, sqlParams);
            cmd.CommandTimeout = 5;
            OracleDataReader reader = null;

            try
            {
                reader = cmd.ExecuteReader();
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Błąd podczas wykonywania zapytania SQL: " + ex.Message);
            }
            return reader;
        }

        public TableSchema GetTableSchema(string tableName)
        {
            OracleCommand cmd = GetCommand("SELECT column_name, data_type, DATA_LENGTH, NULLABLE "+
                    "FROM all_tab_columns WHERE table_name = '"+ tableName + "'", null);
            cmd.CommandTimeout = 5;

            TableSchema tabSchema = new TableSchema();
            tabSchema.Name = tableName;
            try
            {
                OracleDataReader reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    ColumnSchema column = new ColumnSchema();
                    column.ParseColumn(reader.GetString(0), reader.GetString(3), reader.GetString(1), reader.GetInt32(2));
                    tabSchema.Columns.Insert(0, column);
                }
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Błąd podczas pobierania danych tabel: " + ex.Message);
            }
            return tabSchema;
        }

        public List<string> GetTableNames(string tablespace = "USER")
        {
            OracleCommand cmd = GetCommand("select table_name from tabs where tablespace_name='"+tablespace+"'", null);
            cmd.CommandTimeout = 5;
            List<string> tables = new List<string>();

            try
            {
                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    tables.Add(reader.GetString(0));
                }
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Nie można pobrać tabel: " + ex.Message);
            }
            return tables;
        }
    }
}
