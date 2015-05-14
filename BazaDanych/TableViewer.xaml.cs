using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace BazaDanych
{
    /// <summary>
    /// Interaction logic for TableViewer.xaml
    /// </summary>
    public partial class TableViewer : TabItem
    {
        public Table TableSource { get; private set; }

        public TableViewer()
        {
            InitializeComponent();

            test();
        }

        public string GetRecord()
        {
            return "TEST";
        }

        public void ShowTable(Table table)
        {
            TableSource = table;
            mainView.ItemsSource = table;
            this.Header = table.TableSchema.Name;
            int count = 0;
            tableView.Columns.Clear();
            foreach (var col in table.Columns)
            {
                tableView.Columns.Add(
                    new GridViewColumn
                    {
                        Header = col.Name,
                        DisplayMemberBinding = new Binding(string.Format("[{0}]", count)),
                        Width = 70
                    });
                count++;
            }
           // if (table.TableSchema.CanInsert)
                butNewRecord.IsEnabled = true;
           // else if (table.TableSchema.CanUpdate)
                butEditRecord.IsEnabled = true;
           // else if (table.TableSchema.CanDelete)
                butDeleteRecord.IsEnabled = true;
        }

        private void test()
        {
            ColumnSchema col1 = new ColumnSchema();
            col1.ParseColumn("TEST1", "N", "VARCHAR", 32);
            ColumnSchema col2 = new ColumnSchema();
            col2.ParseColumn("TEST2", "N", "NUMBER", 32);

            TableSchema tab = new TableSchema();
            tab.Name = "TAB";
            tab.Columns.Add(col1);
            tab.Columns.Add(col2);

            Table table = new Table(tab);

            string rec11 = "TEST";
            int rec12 = 10;
            object[] rec1 = new object[] { rec11, rec12 };
            string rec21 = "TEST2";
            int rec22 = 20;
            object[] rec2 = new object[] { rec21, rec22 };

            table.Rows.Add(rec1);
            table.Rows.Add(rec2);

            ShowTable(table);
        }

        private void butNewRecord_Click(object sender, RoutedEventArgs e)
        {
            // TEST ( tylko dla tablei MAGAZYNY )
            RecordEventArgs evt = new RecordEventArgs();
            evt.Table = TableSource;
            evt.EditedColummns = TableSource.Columns;
            object[] record = new object[3] { (int)5, "TestMiasto", "TestAdres"};
            evt.EditedRows = new List<object[]>();
            evt.EditedRows.Add(record);

            if (RecordReadyToInsert != null)
                RecordReadyToInsert(this, evt);
        }

        private void butCloseTab_Click(object sender, RoutedEventArgs e)
        {
            if (CloseTab != null)
                CloseTab(this, e);
        }

        public delegate void RecordDelegate(object sender, RecordEventArgs e);
        public event RecordDelegate RecordReadyToInsert;
        public event RoutedEventHandler CloseTab;
    }
}
