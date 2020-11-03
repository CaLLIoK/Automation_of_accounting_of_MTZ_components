using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace Automation_of_accounting_of_MTZ_components
{
    /// <summary>
    /// Логика взаимодействия для AutorizationWindow.xaml
    /// </summary>
    public partial class AutorizationWindow : Window
    {
        public AutorizationWindow()
        {
            InitializeComponent();
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = userLogin.Text;
            string password = userPassword.Password.ToString();

            SqlConnection myConnectionString = new SqlConnection(@"Data Source=(local)\SQLEXPRESS; Initial Catalog=Automation_of_accounting_of_MTZ_components; Integrated Security=True");
            string selectEmployeeInfoQuery = "SELECT * FROM Employee WHERE [employeeLogin] = '" + login + "'and [employeePassword]='" + password + "'";
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(selectEmployeeInfoQuery, myConnectionString))
            {
                DataTable table = new DataTable();
                dataAdapter.Fill(table);
                if (table.Rows.Count > 0)
                {
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.Show();
                    mainWindow.login.Text = userLogin.Text;
                    this.Close();
                }
                else if (table.Rows.Count == 0)
                {
                    MessageBox.Show("Wrong login or password.");
                    return;
                }
            }
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            RegistrationWindow registrationWindow = new RegistrationWindow();
            registrationWindow.Show();
            this.Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
