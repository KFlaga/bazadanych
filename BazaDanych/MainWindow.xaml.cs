using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BazaDanych
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        DatabaseConnector dbConnector;
        ConnectionSettings connSettings;
        SqlCommandBuilder sqlBuilder;
        SqlResultInterpreter sqlInterpreter;

        public MainWindow()
        {
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
                List<string> tables = dbConnector.GetTableNames("COURIER");
                foreach (string table in tables)
                {
                    TableSchema tabSchema = dbConnector.GetTableSchema(table);
                    addTable(tabSchema);
                }
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

        private void addTable(TableSchema tabSchema)
        {
            ListBoxItem table = new ListBoxItem();
            table.Content = tabSchema;
            table.ContextMenu = (ContextMenu)TryFindResource("tableContextMenu");
            table.PreviewMouseRightButtonDown += (sender, e) =>
            {
                ((ListBoxItem)sender).ContextMenu.Visibility = Visibility.Visible;
            };
            table.PreviewMouseDoubleClick += (sender, e) =>
            {
                ShowTable(((ListBoxItem)sender).Content as TableSchema);
            };
            _tableList.Items.Add(table);
            
        }

        private void ShowTable(TableSchema tableSchema, List<ColumnSchema> columns = null)
        {
            if (columns == null)
            {
                columns = tableSchema.Columns;
            }
            // Stwórz tabelę z podanymi kolumnami

            _tableView.ShowTable(LoadTable(tableSchema));
        }

    }
}
