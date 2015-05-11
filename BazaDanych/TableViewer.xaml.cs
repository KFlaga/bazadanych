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
    public partial class TableViewer : UserControl
    {
        private Table tableSource;

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
            tableSource = table;
            mainView.ItemsSource = table;
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
        }

        public void LoadColumns(List<ColumnSchema> columns = null)
        {
            if (columns == null)
                columns = tableSource.Columns;
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

        public delegate void RecordDelegate(object sender, RecordEventArgs e);
        public event RecordDelegate RecordReadyToInsert;

        private void butNewRecord_Click(object sender, RoutedEventArgs e)
        {
            // TEST ( tylko dla tablei MAGAZYNY )
            RecordEventArgs evt = new RecordEventArgs();
            evt.Table = tableSource;
            evt.EditedColummns = tableSource.Columns;
            object[] record = new object[3] { (int)5, "TestMiasto", "TestAdres"};
            evt.EditedRows = new List<object[]>();
            evt.EditedRows.Add(record);

            if (RecordReadyToInsert != null)
                RecordReadyToInsert(this, evt);
        }
    }
}
