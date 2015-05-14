using System;
using System.Collections.Generic;
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
    public partial class ViewCreator : Window
    {
        List<TableSchema> tables;

        public ViewCreator(List<TableSchema> availableTables)
        {
            tables = availableTables;
            InitializeComponent();

            combMainTable.ItemsSource = tables;
            checkUseForeignKey.IsEnabled = false;
        }

        private void combMainTable_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (combMainTable.SelectedIndex == -1)
                return;
            // Pokaż dostępne kolumny
            combMainColumn.ItemsSource = ((TableSchema)combMainTable.SelectedItem).Columns;
        }

        private void combMainColumn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (combMainColumn.SelectedIndex == -1)
                return;
            ColumnSchema col = combMainColumn.SelectedItem as ColumnSchema;
            if (col.IsForeignKey)
            {
                checkUseForeignKey.IsEnabled = true;
                tbRefTable.Text = col.ReferenceTable.Name;
                combRefColumn.ItemsSource = col.ReferenceTable.Columns;
            }
            else
            {
                checkUseForeignKey.IsEnabled = false;
                checkUseForeignKey.IsChecked = false;
                tbRefTable.Text = "";
                combRefColumn.ItemsSource = null;
            }
        }

        private void checkUseForeginKey_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void combRefColumn_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void butPreview_Click(object sender, RoutedEventArgs e)
        {

        }

        private void butAddColumn_Click(object sender, RoutedEventArgs e)
        {

        }

        private void butAddCheck_Click(object sender, RoutedEventArgs e)
        {

        }

        private void butOK_Click(object sender, RoutedEventArgs e)
        {
            if (ViewCreated != null)
                ViewCreated();
        }

        private void butCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public delegate void ViewDelegate();
        public event ViewDelegate ViewCreated;
    }
}
