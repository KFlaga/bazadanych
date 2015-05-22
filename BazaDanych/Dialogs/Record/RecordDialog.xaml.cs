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
using System.Windows.Shapes;

namespace BazaDanych
{
    /// <summary>
    /// Interaction logic for AddRecordDialog.xaml
    /// </summary>
    public partial class RecordDialog : Window
    {
        private int seqTextBox = 0;
        private int seqLabel = 0;
        private Button btn_confirm;
        private Button btn_cancel;
        private List<object> obj_list;
        private string[] client_type = new string[] { "nadawca indywidualny", "nadawca masowy", "odbiorca indywidualny", "odbiorca masowy" };
        public RecordEventArgs REvArgs { get; private set; }
        public Boolean IsEdited { get; private set; }
        public Boolean BeingEdited { get; private set; }


        public RecordDialog()
        {
            InitializeComponent();
            //panel1.Children.Add(l);
            //panel1.RegisterName(l.Name, l);
            //panel1.Children.Add(tb);
            //panel1.RegisterName(tb.Name, tb);
        }

        public RecordDialog(RecordEventArgs evt, bool editing = false)
        {
            InitializeComponent();
            REvArgs = evt;
            BeingEdited = editing;
            obj_list = new List<object>();
            ColumnDefinition col_1 = new ColumnDefinition();
            ColumnDefinition col_2 = new ColumnDefinition();
            grid1.ColumnDefinitions.Add(col_1);
            grid1.ColumnDefinitions.Add(col_2);
            if (evt.EditedColummns[0].Name != "ID")
                evt.EditedColummns.Reverse();

            int i = 0;
            foreach (var column in REvArgs.EditedColummns)
            {
                RowDefinition row = new RowDefinition();
                row.Height = GridLength.Auto;
                grid1.RowDefinitions.Add(row);
                Label lab = CreateCustomLabel(column.Name);
                string text_inside = "";
                if (editing && evt.EditedRows != null)
                    text_inside = evt.EditedRows[evt.EditedIndex][i].ToString();
                if (column.type == typeof(String) || column.type == typeof(Int16) || column.type == typeof(Int32))
                {
                    if (column.Name == "TYP_KLIENTA")
                    {
                        ComboBox cb = CreateCustomComboBox(client_type, text_inside);
                        AddComboBoxToGrid(grid1, cb);
                        obj_list.Add(cb);
                    } else
                    {
                        TextBox tb;
                        if (column.type == typeof(Int16) || column.type == typeof(Int32))
                            tb = CreateCustomIntegerTextBox("");
                        else
                            tb = CreateCustomTextBox("");
                        tb.Text = text_inside;
                        AddTextBoxToGrid(grid1, tb);
                        obj_list.Add(tb);
                    }
                }
                AddLabelToGrid(grid1, lab);
                col_1.Width = GridLength.Auto;
                i++;
            }
            AddAndInitButtons(grid1);
            grid1.Height = grid1.RowDefinitions.Count * 30;
            this.Height = grid1.Height + 50;
        }

        private TextBox CreateCustomTextBox(String text)
        {
            TextBox text_box = new TextBox();
            text_box.Text = text;
            text_box.BorderThickness = new Thickness(1.0);
            text_box.BorderBrush = SystemColors.ControlDarkBrush;
            text_box.Name = "textbox" + seqTextBox.ToString(); seqTextBox++;
            text_box.RenderSize = new Size(text.Length + 5, 20);
            return text_box;
        }

        private TextBox CreateCustomIntegerTextBox(String text)
        {
            TextBox text_box = CreateCustomTextBox(text);
            text_box.PreviewTextInput += textBox_PreviewTextInputInteger;
            DataObject.AddPastingHandler(text_box, textBox_PasteHandlerInteger);
            return text_box;
        }

        private Label CreateCustomLabel(String text)
        {
            Label label = new Label();
            label.Content = text;
            label.Name = "label" + seqLabel.ToString(); seqLabel++;
            return label;
        }

        private ComboBox CreateCustomComboBox(String[] source, string text_inside)
        {
            ComboBox combo_box = new ComboBox();
            combo_box.ItemsSource = source; seqTextBox++;
            if (text_inside != "")
                combo_box.SelectedValue = text_inside; 
            return combo_box;
        }

        private void AddComboBoxToGrid(Grid grid, ComboBox combo_box)
        {
            Grid.SetColumn(combo_box, 1);
            Grid.SetRow(combo_box, seqTextBox - 1);
            grid.Children.Add(combo_box);
        }

        private void AddLabelToGrid(Grid grid, Label label)
        {
            Grid.SetColumn(label, 0);
            Grid.SetRow(label, seqLabel - 1);
            grid.Children.Add(label);
        }

        private void AddTextBoxToGrid(Grid grid, TextBox text_box)
        {
            Grid.SetColumn(text_box, 1);
            Grid.SetRow(text_box, seqTextBox - 1);
            grid.Children.Add(text_box);
        }

        private void textBox_PreviewTextInputInteger(object sender, TextCompositionEventArgs e)
        {
            if (!char.IsDigit(e.Text, e.Text.Length - 1))
            {
                e.Handled = true;
            }
        }

        private void textBox_PasteHandlerInteger(object sender, DataObjectPastingEventArgs e)
        {
            TextBox tb = sender as TextBox;
            bool textOk = false;
            
            if (e.DataObject.GetDataPresent(typeof(string)))
            {
                string paste_text = e.DataObject.GetData(typeof(string)) as string;
                int n;
                if (int.TryParse(paste_text, out n))
                {
                    textOk = true;
                }
            }

            if (!textOk)
                e.CancelCommand();
        }

        private void AddAndInitButtons(Grid grid)
        {

            RowDefinition row = new RowDefinition();
            row.Height = GridLength.Auto;
            grid.RowDefinitions.Add(row);
            btn_confirm = new Button();
            btn_confirm.Content = "Potwierdź";
            btn_confirm.Click += btn_confirm_Click;
            Grid.SetRow(btn_confirm, seqLabel);
            Grid.SetColumn(btn_confirm, 0);
            grid.Children.Add(btn_confirm);
            btn_confirm.Height = 30;

            btn_cancel = new Button();
            btn_cancel.Content = "Anuluj";
            btn_cancel.Click += btn_cancelClickEvent;
            Grid.SetRow(btn_cancel, seqLabel);
            Grid.SetColumn(btn_cancel, 1);
            grid.Children.Add(btn_cancel);
        }

        void btn_confirm_Click(object sender, RoutedEventArgs e)
        {
            REvArgs.EditedRows = new List<object[]>();
            REvArgs.EditedRows.Add(new object[REvArgs.EditedColummns.Count]);
            for (int i = 0; i < REvArgs.EditedColummns.Count; i++)
            {
                Type t = REvArgs.EditedColummns[i].type;
                if (t == Type.GetType("System.String"))
                {
                    if (REvArgs.EditedColummns[i].Name == "TYP_KLIENTA")
                    {
                        var cb = obj_list[i] as ComboBox;
                        REvArgs.EditedRows[0][i] = cb.SelectedValue;
                    }
                    else
                    {
                        var tb = obj_list[i] as TextBox;
                        REvArgs.EditedRows[0][i] = tb.Text.ToString();
                    }
                } else if (t == Type.GetType("System.Int32") || t == Type.GetType("System.Int16"))
                {
                    var tb = obj_list[i] as TextBox;
                    int no;
                    Int32.TryParse(tb.Text.ToString(), out no);
                    REvArgs.EditedRows[0][i] = no;
                }
            }
            IsEdited = true;
            this.Close();
        }

        private void btn_cancelClickEvent(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
