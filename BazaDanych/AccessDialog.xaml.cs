﻿using System;
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
    /// Interaction logic for AccessDialog.xaml
    /// </summary>
    public partial class AccessDialog : Window
    {
        private String __loginServerUser = "LoginServer";
        private String __loginServerPasswd = "passwd1";
        private ConnectionSettings __connSettings;
        private Users.UserList __userList;
        private DatabaseConnector __dbc;

        public AccessDialog()
        {
            InitializeComponent();
            __userList = new Users.UserList();
            __connSettings = new ConnectionSettings();
            __connSettings.SetDefault();
            __connSettings.Login = __loginServerUser;
            __connSettings.Password = __loginServerPasswd;
            __dbc = new DatabaseConnector();
            GetUsers();
        }

        private void GetUsers()
        {
            __dbc.ConnectToDatabase(__connSettings);
            if (__dbc.Connected == false)
            {
                this.Close();
                return;
            }
            __dbc.GetUserList(__userList);
        }

        private void btnReset_Click(object sender, RoutedEventArgs e)
        {
            textBoxLogin.Text = "";
            passwordBox.Password = "";
        }

        private void btnConfirm_Click(object sender, RoutedEventArgs e)
        {
            User u = __userList.FindUser(textBoxLogin.Text);
            if (u == null || !u.IsPasswdValid(passwordBox.Password))
            {
                MessageBox.Show("Nazwa użytkownika bądź hasło niepoprawne!");
                return;
            }
            this.Hide();
            if (u.Type == 0) // admin
                OpenAdminWindow();
            else if (u.Type == 1) // dostawca
                OpenCourierWindow(u);
            Logger.Log(String.Format("Użytkownik ID={0}({1}) zalogował się do systemu.", u.EmplId, u.Login));
            this.Close();
        }

        private void OpenAdminWindow()
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
        }

        private void OpenCourierWindow(User u)
        {
            ConnectionSettings connSettings = new ConnectionSettings();
            connSettings.SetDefault();
            connSettings.Login = "sys";
            connSettings.Password = "db123";
            connSettings.AsAdmin = true;
            DatabaseConnector dbc = new DatabaseConnector();
            dbc.ConnectToDatabase(connSettings);
            CourierWindow courierWindow = new CourierWindow(dbc, connSettings, u);
            courierWindow.Show();
        }
    }
}
