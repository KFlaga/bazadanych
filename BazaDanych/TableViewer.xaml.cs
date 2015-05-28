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
        bool showToolbar = true;
        public Table TableSource { get; set; }
        public bool ShowEditButtons 
        {
            get
            {
                return showToolbar;
            }
            set
            {
                showToolbar = value;
                if (value == true)
                    toolbar.Height = 30;
                else
                    toolbar.Height = 0;
            }
        }
        public int SelectedRow { get { return mainView.SelectedIndex; } }

        public TableViewer()
        {
            InitializeComponent();

           // test();
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
                        Width = col.Name.Length*10
                    });
                count++;
            }
            if (table.TableSchema.CanInsert)
                butNewRecord.IsEnabled = true;
            else if (table.TableSchema.CanUpdate)
                butEditRecord.IsEnabled = true;
            else if (table.TableSchema.CanDelete)
                butDeleteRecord.IsEnabled = true;
            mainView.SelectionMode = SelectionMode.Single;
            mainView.PreviewMouseDoubleClick += (s, e) =>
                {
                    this.RaiseEvent(e);
                };
        }

        public void Update()
        {
            ICollectionView view = CollectionViewSource.GetDefaultView(mainView.ItemsSource);
            view.Refresh();
        }
        
        private void butNewRecord_Click(object sender, RoutedEventArgs e)
        {
            RecordEventArgs evt = new RecordEventArgs();
            evt.Table = TableSource;
            evt.EditedColummns = TableSource.Columns;
            RecordDialog adr = new RecordDialog(evt);
            adr.Closed += adr_Closed;
            adr.ShowDialog();


            //object[] record = new object[3] { (int)5, "TestMiasto", "TestAdres"};
            //evt.EditedRows = new List<object[]>();
            //evt.EditedRows.Add(record);

            //if (RecordReadyToInsert != null)
            //    RecordReadyToInsert(this, evt);
        }

        private void butDeleteRecord_Click(object sender, RoutedEventArgs e)
        {
            var evt = new RecordEventArgs();
            evt.Table = TableSource;
            evt.EditedColummns = TableSource.Columns;
            evt.EditedRows = TableSource.Rows;
            evt.EditedIndex = mainView.SelectedIndex;
            if (RecordReadyToDelete != null)
                RecordReadyToDelete(this, evt);
        }

        private void butEditRecord_Click(object sender, RoutedEventArgs e)
        {
            RecordEventArgs evt = new RecordEventArgs();
            evt.Table = TableSource;
            evt.EditedColummns = TableSource.Columns;
            evt.EditedRows = TableSource.Rows;
            evt.EditedIndex = mainView.SelectedIndex;
            RecordDialog rd = new RecordDialog(evt, true);
            rd.Closed += adr_Closed;
            rd.ShowDialog();
        }

        void adr_Closed(object sender, EventArgs e)
        {
            var adr = sender as RecordDialog;
            if (adr.IsEdited )
            {
                if (RecordReadyToInsert != null && !adr.BeingEdited)
                    RecordReadyToInsert(this, adr.REvArgs);
                else if (RecordReadyToEdit != null && adr.BeingEdited)
                    RecordReadyToEdit(this, adr.REvArgs);
            }
            //{
            //    SqlCommandBuilder sql = new SqlCommandBuilder();
            //    String sql_rec = sql.InsertRecord(adr.REvArgs.Table.TableSchema.Name, adr.REvArgs.EditedColummns, adr.REvArgs.EditedRows[0]);
            //    var db = new DatabaseConnector();
            //    db.SendSqlQuerry(sql_rec);
        }

        private void butCloseTab_Click(object sender, RoutedEventArgs e)
        {
            if (CloseTab != null)
                CloseTab(this, e);
        }

        public delegate void RecordDelegate(object sender, RecordEventArgs e);
        public event RecordDelegate RecordReadyToInsert;
        public event RecordDelegate RecordReadyToEdit;
        public event RecordDelegate RecordReadyToDelete;
        public event RoutedEventHandler CloseTab;


    }
}
