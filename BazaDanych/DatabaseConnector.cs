using Oracle.ManagedDataAccess.Client;
using System;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Windows;

namespace BazaDanych
{
    public class DatabaseConnector
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
                MessageBox.Show("Nie można utworzyć połączenia:\n" + ex.Message);
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
            cmd.CommandTimeout = 35;

            try
            {
                cmd.ExecuteNonQuery();
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Błąd podczas wykonywania polecenia SQL:\n" + ex.Message);
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
                MessageBox.Show("Błąd podczas wykonywania zapytania SQL:\n" + ex.Message);
            }
            return reader;
        }

        public TableSchema GetTableSchema(TableSchema tabSchema)
        {
            OracleCommand cmd = GetCommand("SELECT column_name, data_type, DATA_LENGTH, NULLABLE "+
                    "FROM all_tab_columns WHERE table_name = '" + tabSchema.Name + "'", null);
            cmd.CommandTimeout = 5;

            try
            {
                OracleDataReader reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    ColumnSchema column = new ColumnSchema();
                    column.ParseColumn(reader.GetString(0), reader.GetString(3), reader.GetString(1), reader.GetInt32(2));
                    tabSchema.Columns.Insert(0, column);
                }
                if (tabSchema.Columns[0].Name != "ID")
                    tabSchema.Columns.Reverse();
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Błąd podczas pobierania danych tabel:\n" + ex.Message);
            }
            return tabSchema;
        }

        public TableSchema GetTablePrivileges(TableSchema schema)
        {
            OracleCommand cmd = GetCommand("SELECT privilege FROM ALL_TAB_PRIVS WHERE table_name='"+schema.Name+"'", null);
            cmd.CommandTimeout = 15;
            try
            {
                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    schema.GrantPrivilege(reader.GetString(0));
                }
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Błąd podczas pobierania praw użytkownika:\n" + ex.Message);
            }
            return schema;
        }

        public List<TableSchema> GetTableNames(string tablespace = "USERS")
        {
            OracleCommand cmd = GetCommand("select table_name, owner from dba_tables where tablespace_name='"+tablespace+"'", null);
            cmd.CommandTimeout = 5;
            List<TableSchema> tables = new List<TableSchema>();

            try
            {
                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    TableSchema tab = new TableSchema();
                    tab.Name = reader.GetString(0);
                    tab.Owner = reader.GetString(1);
                    tables.Add(tab);
                }
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Nie można pobrać tabel:\n" + ex.Message);
            }
            return tables;
        }

        public TableSchema GetTableForeignKeys(TableSchema schema)
        {
            OracleCommand cmd = GetCommand("SELECT detail_table.column_name, ref_Table.TABLE_NAME, ref_Table.column_name " +
                                            "FROM all_constraints constraint_info, " +
                                            "all_cons_columns detail_table, " +
                                            "all_cons_columns ref_Table " +
                                            "WHERE constraint_info.constraint_name = detail_table.constraint_name " +
                                            "AND constraint_info.r_constraint_name = ref_Table.constraint_name " +
                                            "AND detail_table.POSITION = ref_Table.POSITION " +
                                            "AND constraint_info.constraint_type = 'R' " +
                                            "AND detail_table.TABLE_NAME = '"+schema.Name+"'", null);
            cmd.CommandTimeout = 15;
            try
            {
                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    schema.ParseForeignKey(reader.GetString(0), reader.GetString(1), reader.GetString(2));
                }
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Błąd podczas pobierania obcych kluczy:\n" + ex.Message);
            }
            return schema;
        }

        public TableSchema GetValueConstraints(TableSchema schema)
        {
            OracleCommand cmd = GetCommand("SELECT detail_table.COLUMN_NAME, constraint_info.SEARCH_CONDITION_VC " +
                                            "FROM all_constraints  constraint_info, "+
                                            "all_cons_columns detail_table "+
                                            "WHERE constraint_info.constraint_name = detail_table.constraint_name "+
                                            "AND constraint_info.constraint_type = 'C' "+
                                            "AND detail_table.TABLE_NAME = '"+schema.Name+"'", null);
            cmd.CommandTimeout = 15;
            try
            {
                OracleDataReader reader = cmd.ExecuteReader();
                while (reader.Read())
                {
                    if (reader.IsDBNull(0) || reader.IsDBNull(1))
                        continue;
                    schema.ParseConstraint(reader.GetString(0), reader.GetString(1));
                }
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Błąd podczas pobierania obcych kluczy:\n" + ex.Message);
            }
            return schema;
        }

        public void GetUserList(Users.UserList userList)
        {
            OracleCommand cmd = GetCommand("SELECT * FROM SYS.UZYTKOWNICY");
            cmd.CommandTimeout = 5;
            try
            {
                OracleDataReader reader = cmd.ExecuteReader();
                while(reader.Read())
                {
                    if (reader.IsDBNull(0) || reader.IsDBNull(1))
                        continue;
                    int uId = reader.GetInt32(0);
                    String uName = reader.GetString(1);
                    String uPasswd = reader.GetString(2);
                    int uType = reader.GetInt32(3);
                    int uEmplId = -1;
                    if (reader[4] != DBNull.Value)
                        uEmplId = reader.GetInt32(4);
                    userList.ParseUser(uId, uName, uPasswd, uType, uEmplId);
                }
            }
            catch (OracleException ex)
            {
                MessageBox.Show("Błąd pobierania użytkowników:\n " + ex.Message);
            }
        }

    }
}
