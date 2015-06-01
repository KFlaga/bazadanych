using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace BazaDanych
{
    /// <summary>
    /// Interaction logic for CourierWindow.xaml
    /// </summary>
    public partial class CourierWindow : Window
    {
        BindingList<TableViewer> cardsTableViews;
        TableSchema schemaPackages;
        TableSchema schemaClients;
        Table tabPackages;
        static Table tabClients;

        DatabaseConnector dbConnector;
        ConnectionSettings connSettings;
        SqlCommandBuilder sqlBuilder;
        SqlResultInterpreter sqlInterpreter;
        User user;
        int courierId;

        Table tabAllCourierPackages;
        Table tabToTakeFromStore;
        List<int> idsToTakeFromStore = new List<int>(10); // listy wskazuja na pozycje danej paczki w głównej tabeli PACKAGES
        Table tabToDeliverToClient;
        List<int> idsToDeliverToClient = new List<int>(10);
        Table tabToTakeFromClient;
        List<int> idsToTakeFromClient = new List<int>(10);
        Table tabToDeliverToStore;
        List<int> idsToDeliverToStore = new List<int>(10);

        public CourierWindow(DatabaseConnector connector, ConnectionSettings settings, User usr)
        {
            dbConnector = connector;
            connSettings = settings;
            user = usr;
            sqlBuilder = new SqlCommandBuilder();
            sqlInterpreter = new SqlResultInterpreter();
            cardsTableViews = new BindingList<TableViewer>();

            CreateSchemas();
            RefreshTables();

            InitializeComponent();

            _tableViewSwitcher.ItemsSource = cardsTableViews;
        }


        private void ConnectedToDatabase(object sender, RoutedEventArgs e)
        {
            RefreshTables();
        }

        private void RefreshTables()
        {
            if (dbConnector.Connected)
            {
                // bierzemy id dostawcy na podstawie id usera zapisanego w dane_pracownikow
                courierId = (int)sqlInterpreter.InterpretSingleSelection(dbConnector.SendSqlQuerry(
                    "SELECT ID FROM SYS.DOSTAWCY D WHERE D.DANE_PRACOWNIKA IN " +
                    "(SELECT ID FROM SYS.DANE_PRACOWNIKOW DP WHERE DP.ID = " + user.EmplId.ToString() + " )"),"int");
                // ściągamy tabele -> paczki, klienci
                schemaPackages.Columns.Clear();
                dbConnector.GetTableSchema(schemaPackages);
                schemaClients.Columns.Clear();
                dbConnector.GetTableSchema(schemaClients);
                // wybieramy paczki dostawcy
                tabPackages = sqlInterpreter.InterpretSelectResult(dbConnector.SendSqlQuerry(
                    "SELECT * FROM SYS.PRZESYLKI P WHERE P.AKTUALNY_DOSTAWCA = " + courierId.ToString()), schemaPackages);
                // wybieramy klientów których paczki przypisane są do dostawcy
                tabClients = sqlInterpreter.InterpretSelectResult(dbConnector.SendSqlQuerry(
                    "SELECT * FROM SYS.KLIENCI K WHERE K.ID IN " +
                    "(SELECT ODBIORCA FROM SYS.PRZESYLKI P WHERE P.AKTUALNY_DOSTAWCA = " + courierId.ToString() + ") OR K.ID IN " +
                    "(SELECT NADAWCA FROM SYS.PRZESYLKI P WHERE P.AKTUALNY_DOSTAWCA = " + courierId.ToString() + " )"
                    ), schemaClients);
            }
        }

        private void DisconnectedFromDatabase(object sender, RoutedEventArgs e)
        {
            // attempt to reconnect
        }

        // tworzymy tabele zawierajaca wybrane dane wszystkich paczek kuriera
        private void ShowTable_AllPackages(object sender, RoutedEventArgs e)
        {
            tabAllCourierPackages.Rows.Clear();
            if (tabPackages == null)
            {
                MessageBox.Show("Brak paczek do pokazania!");
                return;
            }

            for (int row = 0; row < tabPackages.Rows.Count; row++ )
            {
                object[] package = new object[tabAllCourierPackages.TableSchema.Columns.Count];
                tabAllCourierPackages.Rows.Add(package);

                package[0] = tabPackages.FindColumnData(row, "Id");
                int odbId = (Int16)tabPackages.FindColumnData(row, "Odbiorca");
                for (int r = 0; r < tabClients.Rows.Count; r++)
                {
                    if ((Int16)tabClients.FindColumnData(r, "ID") == odbId)
                    {
                        package[1] = (string)tabClients.FindColumnData(r, "Nazwa");
                        break;
                    }
                }
                int nadId = (Int16)tabPackages.FindColumnData(row, "Nadawca");
                for (int r = 0; r < tabClients.Rows.Count; r++)
                {
                    if ((Int16)tabClients.FindColumnData(r, "ID") == nadId)
                    {
                        package[2] = (string)tabClients.FindColumnData(r, "Nazwa");
                        break;
                    }
                }

                package[3] = tabPackages.FindColumnData(row, "Adres_Docelowy");

                if (package[3] == null)
                {
                    for (int i = 0; i < tabClients.Rows.Count; i++)
                    {
                        if ((Int16)tabClients.FindColumnData(i, "Id") == odbId)
                        {
                            package[3] = (string)tabClients.FindColumnData(i, "Adres");
                        }
                    }
                }

                package[4] = tabPackages.FindColumnData(row, "Data_Nadania");
                package[5] = tabPackages.FindColumnData(row, "Data_Odbioru");
                package[6] = tabPackages.FindColumnData(row, "Status");
            }

            ShowTable(tabAllCourierPackages);
        }
        // Wyswietla tabele z paczkami o danym statusie - z założenia wszystkie mają takie same ColumnSchema
        private void ShowTable_SpecificStatus(string status, Table table, List<int> ids)
        {
            table.Rows.Clear();
            if (tabPackages == null)
            {
                MessageBox.Show("Brak paczek do pokazania!");
                return;
            }
            for (int row = 0; row < tabPackages.Rows.Count; row++)
            {
                if (!((string)tabPackages.FindColumnData(row, "Status")).Equals(status, StringComparison.OrdinalIgnoreCase))
                    continue;

                object[] package = new object[table.TableSchema.Columns.Count];
                table.Rows.Add(package);

                package[0] = tabPackages.FindColumnData(row, "Id");
                int odbId = (Int16)tabPackages.FindColumnData(row, "Odbiorca");
                for (int r = 0; r < tabClients.Rows.Count; r++)
                {
                    if ((Int16)tabClients.FindColumnData(r, "ID") == odbId)
                    {
                        package[1] = (string)tabClients.FindColumnData(r, "Nazwa");
                        break;
                    }
                }
                int nadId = (Int16)tabPackages.FindColumnData(row, "Nadawca");
                for (int r = 0; r < tabClients.Rows.Count; r++)
                {
                    if ((Int16)tabClients.FindColumnData(r, "ID") == nadId)
                    {
                        package[2] = (string)tabClients.FindColumnData(r, "Nazwa");
                        break;
                    }
                }
                package[3] = tabPackages.FindColumnData(row, "Adres_Docelowy");

                if (package[3] == null)
                {
                    for (int i = 0; i < tabClients.Rows.Count; i++)
                    {
                        if ((Int16)tabClients.FindColumnData(i, "Id") == odbId)
                        {
                            package[3] = (string)tabClients.FindColumnData(i, "Adres");
                        }
                    }
                }
                package[4] = tabPackages.FindColumnData(row, "Status");
                ids.Add(row);
            }

            ShowTable(table);
            ((TableViewer)_tableViewSwitcher.SelectedItem).PreviewMouseDoubleClick -= ChangePackageDone;
            ((TableViewer)_tableViewSwitcher.SelectedItem).PreviewMouseDoubleClick += ChangePackageDone;
        }

        private void ShowTable_ToTakeFromClient(object sender, RoutedEventArgs e)
        {
            ShowTable_SpecificStatus("Zlecona przez klienta", tabToTakeFromClient, idsToTakeFromClient);
        }

        private void ShowTable_ToTakeFromStore(object sender, RoutedEventArgs e)
        {
            ShowTable_SpecificStatus("Gotowa do odebrania", tabToTakeFromStore, idsToTakeFromStore);
        }

        private void ShowTable_ToDeliverToStore(object sender, RoutedEventArgs e)
        {
            ShowTable_SpecificStatus("W drodze do magazynu", tabToDeliverToStore, idsToDeliverToStore);
        }

        private void ShowTable_ToDeliverToClient(object sender, RoutedEventArgs e)
        {
            ShowTable_SpecificStatus("Odebrana", tabToDeliverToClient, idsToDeliverToClient);
        }
        // zmienia pole czy załatwione na tak/nie
        private void ChangePackageDone(object sender, RoutedEventArgs e)
        {
            int row = ((TableViewer)_tableViewSwitcher.SelectedItem).SelectedRow;
            Table table = ((TableViewer)_tableViewSwitcher.SelectedItem).TableSource;
            if (row != -1)
            {
                //if ((string)table.Rows[row][4] == "NIE")
                //    table.Rows[row][4] = "TAK";
                //else
                //    table.Rows[row][4] = "NIE";
                ((TableViewer)_tableViewSwitcher.SelectedItem).Update();
            }
        }

        private void ShowTable(Table table)
        {
            foreach (TableViewer viewer in cardsTableViews)
            {
                if (viewer.TableSource.TableSchema == table.TableSchema)
                {
                    _tableViewSwitcher.SelectedItem = viewer;
                    return;
                }
            }

            if (!dbConnector.Connected)
                return;

            TableViewer tabView = new TableViewer();
            tabView.ShowTable(table);
            tabView.CloseTab += (s, e) =>
            {
                cardsTableViews.Remove((TableViewer)s);
            };
            cardsTableViews.Add(tabView);
            _tableViewSwitcher.SelectedItem = tabView;
            tabView.ShowEditButtons = true;
        }

        private void SaveChanges(object sender, RoutedEventArgs e)
        {
            if (!dbConnector.Connected)
                return;
            // Dla każdej załatwionej paczki zmień status (i ew. datę)
            for (int row = 0; row < tabToTakeFromStore.Rows.Count; row++)
            {
                if ((string)tabToTakeFromStore.Rows[row][4] == "TAK")
                    tabPackages.SetColumnData(idsToTakeFromStore[row], "Status", "W drodze do klienta");
            }
            for (int row = 0; row < tabToTakeFromStore.Rows.Count; row++)
            {
                if ((string)tabToDeliverToStore.Rows[row][4] == "TAK")
                    tabPackages.SetColumnData(idsToDeliverToStore[row], "Status", "Oczekująca");
            }
            for (int row = 0; row < tabToTakeFromStore.Rows.Count; row++)
            {
                if ((string)tabToTakeFromClient.Rows[row][4] == "TAK")
                    tabPackages.SetColumnData(idsToTakeFromClient[row], "Status", "W drodze do magazynu");
            }
            for (int row = 0; row < tabToTakeFromStore.Rows.Count; row++)
            {
                if ((string)tabToDeliverToClient.Rows[row][4] == "TAK")
                {
                    tabPackages.SetColumnData(idsToDeliverToClient[row], "Data_Odbioru", new DateOnly() { Date = DateTime.Now });
                    tabPackages.SetColumnData(idsToDeliverToClient[row], "Status", "Odebrana");
                }
            }
            Table tab = (_tableViewSwitcher.SelectedItem as TableViewer).TableSource;
            for (int i = 0; i < tab.Rows.Count; i++)
            {
                for (int j = 0; j < tab.Rows.Count; j++)
                {
                    if (tab.Rows[i][0] == tabPackages.Rows[j][0])
                    {
                        if (tabPackages.FindColumnData(j, "Status") != tab.FindColumnData(i, "Status"))
                        {
                            if (TableViewer.packageStatus.IndexOf(tabPackages.FindColumnData(j, "Status").ToString())
                                < TableViewer.packageStatus.IndexOf(tab.FindColumnData(i, "Status").ToString()) &&
                                tab.FindColumnData(i, "Status") == TableViewer.packageStatus[TableViewer.packageStatus.Count - 1])
                            {
                                tabPackages.SetColumnData(j, "Data_Odbioru", new DateOnly() { Date = DateTime.Now });
                            } 
                            tabPackages.SetColumnData(j, "Status", tab.FindColumnData(i, "Status").ToString());
                            
                            //log modyfikacji!
                        }
                    }
                }
            }


            // Wrzuć zmiany do bazy danych
            for (int row = 0; row < tabPackages.Rows.Count; row++)
            {
                string sql = sqlBuilder.EditRecord("SYS.PRZESYLKI", tabPackages.Columns, tabPackages.Rows[row]);
                dbConnector.SendSqlNonQuerry(sql);
            }

            // Odśwież tabele
            RefreshTables();
            int selectedTable = _tableViewSwitcher.SelectedIndex;
            foreach (var tabView in cardsTableViews)
            {
                if (tabView.TableSource == tabAllCourierPackages)
                    ShowTable_AllPackages(this, new RoutedEventArgs());
                else if (tabView.TableSource == tabToTakeFromStore)
                    ShowTable_ToTakeFromStore(this, new RoutedEventArgs());
                else if (tabView.TableSource == tabToTakeFromClient)
                    ShowTable_ToTakeFromClient(this, new RoutedEventArgs());
                else if (tabView.TableSource == tabToDeliverToStore)
                    ShowTable_ToDeliverToStore(this, new RoutedEventArgs());
                else if (tabView.TableSource == tabToDeliverToClient)
                    ShowTable_ToDeliverToClient(this, new RoutedEventArgs());
                tabView.Update();
            }
            _tableViewSwitcher.SelectedIndex = selectedTable;
        }

        private void CreateSchemas()
        {
            schemaPackages = new TableSchema();
            schemaPackages.Name = "PRZESYLKI";
            schemaClients = new TableSchema();
            schemaClients.Name = "KLIENCI";

            // Pierwszy widok -> wszystkie paczki przydzielone do kuriera
            // magazyn jako nazwa magazynu, a klient jako imie i nazwisko kilenta
            // Kolumny:
            // zlecenie odbiorca nadawca adres data nadania data odbioru status
            TableSchema schemaAllCourierPackages = new TableSchema();
            schemaAllCourierPackages.Name = "Wszystkie Przesyłki";
            schemaAllCourierPackages.Columns = new List<ColumnSchema>();
            schemaAllCourierPackages.Columns.Add(new ColumnSchema()
            {
                Name = "Nr Zlecenia",
                type = Type.GetType("System.Int32")
            });
            schemaAllCourierPackages.Columns.Add(new ColumnSchema()
            {
                Name = "Odbiorca",
                type = Type.GetType("System.String")
            });
            schemaAllCourierPackages.Columns.Add(new ColumnSchema()
            {
                Name = "Nadawca",
                type = Type.GetType("System.String")
            });
            schemaAllCourierPackages.Columns.Add(new ColumnSchema()
            {
                Name = "Adres",
                type = Type.GetType("System.String")
            });
            schemaAllCourierPackages.Columns.Add(new ColumnSchema()
            {
                Name = "Data Nadania",
                type = Type.GetType("System.DateTime")

            });
            schemaAllCourierPackages.Columns.Add(new ColumnSchema()
            {
                Name = "Data Odbioru",
                type = Type.GetType("System.DateTime")

            });
            schemaAllCourierPackages.Columns.Add(new ColumnSchema()
            {
                Name = "Status",
                type = Type.GetType("System.String")

            });
            tabAllCourierPackages = new Table(schemaAllCourierPackages);
            // paczki gotowe do odebrania z magazynu
            tabToTakeFromStore = new Table(CreateStatusSchema("Do Zabrania Z Magazynu"));
            // paczki do dostarczenia dla odbiorców
            tabToDeliverToClient = new Table(CreateStatusSchema("Do Dostarczenia Do Klientów"));
            // paczki do odebrania po reklamacji / od nadawcy
            tabToTakeFromClient = new Table(CreateStatusSchema("Do Zabrania Od Klientów"));
            // Paczki do dostarczenia po powyższym do magazynu
            tabToDeliverToStore = new Table(CreateStatusSchema("Do Dostrczenia Do Magazynu"));
        }

        private TableSchema CreateStatusSchema(string tabName)
        {
            var schema = new TableSchema();
            schema.Name = tabName;
            schema.Columns.Add(new ColumnSchema()
            {
                Name = "Nr Zlecenia",
                type = Type.GetType("System.Int32")
            });
            schema.Columns.Add(new ColumnSchema()
            {
                Name = "Odbiorca",
                type = Type.GetType("System.String")
            });
            schema.Columns.Add(new ColumnSchema()
            {
                Name = "Nadawca",
                type = Type.GetType("System.String")
            });
            schema.Columns.Add(new ColumnSchema()
            {
                Name = "Adres",
                type = Type.GetType("System.String")
            });
            schema.Columns.Add(new ColumnSchema()
            {
                Name = "Status",
                type = Type.GetType("System.String")
            });
            return schema;
        }

        private void btnSearch_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
