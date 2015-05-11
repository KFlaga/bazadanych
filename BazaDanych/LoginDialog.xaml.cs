using System.Windows;

namespace BazaDanych
{
    /// <summary>
    /// Interaction logic for LoginDialog.xaml
    /// </summary>
    public partial class LoginDialog : Window
    {
        ConnectionSettings settings;

        public LoginDialog(ConnectionSettings opts)
        {
            InitializeComponent();

            settings = opts;

            tbLogin.Text = settings.Login;
            tbPass.Text = settings.Password;
            checkAdmin.IsChecked = false;
        }

        private void butConnect_Click(object sender, RoutedEventArgs e)
        {
            settings.Login = tbLogin.Text;
            settings.Password = tbPass.Text;
            settings.AsAdmin = (bool)checkAdmin.IsChecked;
            if (Connect != null)
                Connect(settings);
        }

        private void butCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        public delegate void ConnectionOptionsDelegate(ConnectionSettings opts);
        public event ConnectionOptionsDelegate Connect;
    }
}
