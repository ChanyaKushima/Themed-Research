using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace DeadlyOnline.Logic
{
    /// <summary>
    /// LoginWindow.xaml の相互作用ロジック
    /// </summary>
    public partial class LoginWindow : Window
    {
        public string Password { get; set; }
        public string PlayerID { get; set; }

        public LoginWindow()
        {
            InitializeComponent();
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            Password = PasswordBox.Password;
            PlayerID = IDTextBox.Text;
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            if (IDTextBox.Text.CanUseAsPlayerID())
            {
                DialogResult = true;
                Close();
            }
            else
            {
                CannotUseThisID.Visibility = Visibility.Visible;
            }
        }

        private void IDTextBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            CannotUseThisID.Visibility = Visibility.Hidden;
        }
    }
}
