using System.Windows;

namespace BazaDanych
{
    /// <summary>
    /// Interaction logic for ConnectionSettingsWindow.xaml
    /// </summary>
    public partial class ConnectionSettingsWindow : Window
    {
        ConnectionSettings connOpts;

        public bool OverrideDefaultSettings
        {
            get 
            { 
                return (bool)GetValue(OverrideDefaultSettingsProperty); 
            }
            set 
            {
                SetValue(OverrideDefaultSettingsProperty, value); 
            }
        }
        
        public static readonly DependencyProperty OverrideDefaultSettingsProperty =
            DependencyProperty.Register("OverrideDefaultSettings", typeof(bool),
              typeof(ConnectionSettingsWindow), new UIPropertyMetadata(false));

        public ConnectionSettingsWindow()
        {
            InitializeComponent();

            connOpts = new ConnectionSettings();
            connOpts.SetDefault();

            checkDefault.IsChecked = false;

            tbDataSource.Text = connOpts.TNSName;
            tbHost.Text = connOpts.Host;
            tbPort.Text = connOpts.Port;
            tbSid.Text = connOpts.SID;
            checkTNS.IsChecked = false;
        }

        private void butCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void butOK_Click(object sender, RoutedEventArgs e)
        {
            connOpts.Login = tbUser.Text;
            connOpts.Password = tbPassword.Text;
            if (OverrideDefaultSettings)
            {
                connOpts.Host = tbHost.Text;
                connOpts.Port = tbPort.Text;
                connOpts.SID = tbSid.Text;
                connOpts.TNSName = tbDataSource.Text;
                connOpts.UseTNS = (bool)checkTNS.IsChecked;
            }

            if (ConnectionOptionsChanged != null)
                ConnectionOptionsChanged(connOpts);
            this.Close();
        }

        public delegate void ConnectionOptionsDelegate(ConnectionSettings opts);
        public event ConnectionOptionsDelegate ConnectionOptionsChanged;
    }
}
