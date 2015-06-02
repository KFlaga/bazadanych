using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace BazaDanych
{
    public partial class MainWindow : Window
    {
        BindingList<TableViewer> cardsTableViews;
        List<TableSchema> mainTableSchemas;

        DatabaseConnector dbConnector;
        ConnectionSettings connSettings;
        SqlCommandBuilder sqlBuilder;
        SqlResultInterpreter sqlInterpreter;

        public MainWindow()
        {
            mainTableSchemas = new List<TableSchema>();
            cardsTableViews = new BindingList<TableViewer>();

            InitializeComponent();
            this.Closing += (s, e) => { dbConnector.Disconnect(); };
            this.Closed += (s, e) => { App.Current.Shutdown(); };

            dbConnector = new DatabaseConnector();
            connSettings = new ConnectionSettings();
            connSettings.SetDefault();

            sqlBuilder = new SqlCommandBuilder();
            sqlInterpreter = new SqlResultInterpreter();

            _tableViewSwitcher.ItemsSource = cardsTableViews;
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
            _statusBarLabel.Content = "Połączono z bazą danych: ładowanie danych";
            RefreshTables(this, new RoutedEventArgs());
            _submenuConnect.IsEnabled = false;
            _submenuDisconnect.IsEnabled = true;
            _statusBarLabel.Content = "Połączono z bazą danych";
        }

        private void DisconectedFromDatabase()
        {
            _submenuConnect.IsEnabled = true;
            _submenuDisconnect.IsEnabled = false;
            _statusBarLabel.Content = "Nie ma połączenia z bazą";
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
                    //dbConnector.GetTablePrivileges(mainTableSchemas[t]);
                    //dbConnector.GetTableForeignKeys(mainTableSchemas[t]);
                    //dbConnector.GetValueConstraints(mainTableSchemas[t]);
                    //if (mainTableSchemas[t].CanSelect)
                        addTableToView(mainTableSchemas[t]);
                }
                ResolveForeignKeys();
            }
            
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
                AddTableView_SwitchToCardIfAlreadyAdded(((ListBoxItem)sender).Content as TableSchema);
            };
            _tableList.Items.Add(tableItem);
        }

        private void AddTableView_SwitchToCardIfAlreadyAdded(TableSchema tableSchema)
        {
            foreach (TableViewer viewer in cardsTableViews)
            {
                if ( viewer.TableSource.TableSchema == tableSchema )
                {
                    _tableViewSwitcher.SelectedItem = viewer;
                    return;
                }
            }

            if (!dbConnector.Connected)
                return;

            TableViewer tabView = new TableViewer();
            tabView.ShowEditButtons = true;
            tabView.IsMainWindow = true;
            tabView.RecordReadyToInsert += InsertRecords;
            tabView.RecordReadyToEdit += tabView_RecordReadyToEdit;
            tabView.RecordReadyToDelete += tabView_RecordReadyToDelete;
            tabView.ShowTable(LoadTable(tableSchema));
            tabView.CloseTab += CloseTableViewer;
            cardsTableViews.Add(tabView);
            _tableViewSwitcher.SelectedItem = tabView;
        }

        void InsertRecords(object sender, RecordEventArgs e)
        {
            // TEST 
            //if (e.Table.TableSchema.Name != "MAGAZYNY")
            //  return;
            var table_viewer = sender as TableViewer;
            string sql = "";
            try
            {
                sql = sqlBuilder.InsertRecord(e.Table.TableSchema.Name, e.EditedColummns, e.EditedRows[0]);
            } catch (ArgumentOutOfRangeException ex)
            {
                MessageBox.Show("Argument z poza dostępnego zakresu " + (ex.GetBaseException()).ToString());
                return;
            }
            dbConnector.SendSqlNonQuerry(sql);
            table_viewer.ShowTable(LoadTable(table_viewer.TableSource.TableSchema));
        }

        void tabView_RecordReadyToDelete(object sender, RecordEventArgs e)
        {
            var table_viewer = sender as TableViewer;
            string sql = sqlBuilder.DeleteRecord(e.Table.TableSchema.Name, e.EditedColummns, e.EditedRows[e.EditedIndex]);
            dbConnector.SendSqlNonQuerry(sql);
            table_viewer.ShowTable(LoadTable(table_viewer.TableSource.TableSchema));
        }

        void tabView_RecordReadyToEdit(object sender, RecordEventArgs e)
        {
            var table_viewer = sender as TableViewer;
            string sql = "";
            try
            {
                sql = sqlBuilder.EditRecord(e.Table.TableSchema.Name, e.EditedColummns, e.EditedRows[e.EditedIndex]);
            } catch (ArgumentOutOfRangeException ex)
            {
                MessageBox.Show("Argument z poza dostępnego zakresu " + (ex.GetBaseException()).ToString());
                return;
            }
            dbConnector.SendSqlNonQuerry(sql);
            table_viewer.ShowTable(LoadTable(table_viewer.TableSource.TableSchema));
        }

        void CloseTableViewer(object sender, RoutedEventArgs e)
        {
            cardsTableViews.Remove((TableViewer)sender);
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

        private void MenuItem_Click(object sender, RoutedEventArgs e)
        {
            //ColumnSchema cs1 = new ColumnSchema() { Name = "cos", type = Type.GetType("System.String") };
            //ColumnSchema cs2 = new ColumnSchema() { Name = "cos_dluzszego", type = Type.GetType("System.String") };
            //ColumnSchema cs3 = new ColumnSchema() { Name = "cos-num", type = Type.GetType("System.Int32") };
            //List<ColumnSchema> css = new List<ColumnSchema>();
            //css.Add(cs1);
            //css.Add(cs2);
            //css.Add(cs3);
            //AddRecordDialog adr = new AddRecordDialog(css);
            //adr.Show();
        }
    }
}
