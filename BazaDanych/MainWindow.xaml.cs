using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BazaDanych
{
    public partial class MainWindow : Window
    {
        List<TableSchema> mainTableSchemas;

        DatabaseConnector dbConnector;
        ConnectionSettings connSettings;
        SqlCommandBuilder sqlBuilder;
        SqlResultInterpreter sqlInterpreter;

        public MainWindow()
        {
            mainTableSchemas = new List<TableSchema>();

            InitializeComponent();
            this.Closing += (s, e) => { dbConnector.Disconnect(); };
            this.Closed += (s, e) => { App.Current.Shutdown(); };

            dbConnector = new DatabaseConnector();
            connSettings = new ConnectionSettings();
            connSettings.SetDefault();

            sqlBuilder = new SqlCommandBuilder();
            sqlInterpreter = new SqlResultInterpreter();
        }

        private void onConnectToDatabaseClicked(object sender, RoutedEventArgs e)
        {
            LoginDialog logWindow = new LoginDialog(connSettings);
            logWindow.Connect += (opts) =>
                {
                    connSettings = opts;
                    if (dbConnector.ConnectToDatabase(connSettings))
                    {
                        logWindow.Close();
                        ConnectedToDatabase();
                    }
                };
            logWindow.ShowDialog();
        }

        private void onDisconnectFromDatabaseClicked(object sender, RoutedEventArgs e)
        {
            dbConnector.Disconnect();
            DisconectedFromDatabase();
        }

        private void onConnectionSettingsClicked(object sender, RoutedEventArgs e)
        {
            ConnectionSettingsWindow connSettings = new ConnectionSettingsWindow();
            connSettings.ConnectionOptionsChanged += ConnectionSettingsChanged;
            connSettings.ShowDialog();
        }

        void ConnectionSettingsChanged(ConnectionSettings opts)
        {
            connSettings = opts;
        }

        private void ConnectedToDatabase()
        {
            RefreshTables(this, new RoutedEventArgs());
            _submenuConnect.IsEnabled = false;
            _submenuDisconnect.IsEnabled = true;

            _tableView.RecordReadyToInsert += InsertRecords;
        }

        private void DisconectedFromDatabase()
        {
            _submenuConnect.IsEnabled = true;
            _submenuDisconnect.IsEnabled = false;

            _tableView.RecordReadyToInsert -= InsertRecords;
        }

        private void RefreshTables(object sender, RoutedEventArgs e)
        {
            if (dbConnector.Connected)
            {
                _tableList.Items.Clear();
                mainTableSchemas.Clear();
                mainTableSchemas = dbConnector.GetTableNames("COURIER");
                for (int t = 0; t < mainTableSchemas.Count; t++ )
                {
                    dbConnector.GetTableSchema(mainTableSchemas[t]);
                    dbConnector.GetTablePrivileges(mainTableSchemas[t]);
                    dbConnector.GetTableForeignKeys(mainTableSchemas[t]);
                    dbConnector.GetValueConstraints(mainTableSchemas[t]);
                    if (mainTableSchemas[t].CanSelect)
                        addTableToView(mainTableSchemas[t]);
                }
                ResolveForeignKeys();
            }
        }

        void InsertRecords(object sender, RecordEventArgs e)
        {
            // TEST 
            if (e.Table.TableSchema.Name != "MAGAZYNY")
                return;

            string sql = sqlBuilder.InsertRecord(e.Table.TableSchema.Name, e.EditedColummns, e.EditedRows[0]);

            dbConnector.SendSqlNonQuerry(sql);
        }

        Table LoadTable(TableSchema schema)
        {
            string sql = sqlBuilder.SelectAllRecords(schema);
            return sqlInterpreter.InterpretSelectResult(dbConnector.SendSqlQuerry(sql),schema);
        }

        private void addTableToView(TableSchema tabSchema)
        {
            ListBoxItem tableItem = new ListBoxItem();
            tableItem.Content = tabSchema;
            tableItem.ContextMenu = (ContextMenu)TryFindResource("tableContextMenu");
            tableItem.PreviewMouseRightButtonDown += (sender, e) =>
            {
                ((ListBoxItem)sender).ContextMenu.Visibility = Visibility.Visible;
            };
            tableItem.PreviewMouseDoubleClick += (sender, e) =>
            {
                ShowTable(((ListBoxItem)sender).Content as TableSchema);
            };
            _tableList.Items.Add(tableItem);
        }

        private void ShowTable(TableSchema tableSchema, List<ColumnSchema> columns = null)
        {
            if (!dbConnector.Connected)
                return;

            if (columns == null)
            {
                columns = tableSchema.Columns;
            }
            // Stwórz tabelę z podanymi kolumnami

            _tableView.ShowTable(LoadTable(tableSchema));
        }

        private void ResolveForeignKeys()
        {
            for (int t = 0; t < mainTableSchemas.Count; t++)
            {
                mainTableSchemas[t].ResolveForeignKeys(mainTableSchemas);
            }
        }

        private void CreateView(object sender, RoutedEventArgs e)
        {
            if (dbConnector.Connected)
            {
                List<TableSchema> selectableTabs = new List<TableSchema>();
                foreach (TableSchema tabSchema in mainTableSchemas)
                {
                 //   if(tabSchema.CanSelect)
                        selectableTabs.Add(tabSchema);
                }

                ViewCreator creator = new ViewCreator(selectableTabs);
                creator.ShowDialog();
            }
        }

    }
}
