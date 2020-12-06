using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Automation_of_accounting_of_MTZ_components
{
    /// <summary>
    /// Логика взаимодействия для AutorizationWindow.xaml
    /// </summary>
    public partial class AutorizationWindow : Window
    {
        SqlConnection myConnectionString = new SqlConnection(@"Data Source=(local)\SQLEXPRESS; Initial Catalog=Automation_of_accounting_of_MTZ_components; Integrated Security=True");

        public AutorizationWindow()
        {
            InitializeComponent();
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            string login = userLogin.Text;
            string password = userPassword.Password.ToString();

            string selectEmployeeInfoQuery = "SELECT * FROM Employee WHERE [employeeLogin] = '" + login + "'and [employeePassword]='" + password + "'";
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(selectEmployeeInfoQuery, myConnectionString))
            {
                DataTable table = new DataTable();
                dataAdapter.Fill(table);
                if (table.Rows.Count > 0)
                {
                    if (table.Rows[0]["employeeLogin"].ToString() == login && table.Rows[0]["employeePassword"].ToString() == password)
                    {
                        StreamWriter loginFile = new StreamWriter("UserLogin.txt");
                        loginFile.Write(login);
                        loginFile.Close();

                        StreamWriter autorizationStatus = new StreamWriter("AutorizationStatus.txt");
                        autorizationStatus.Write("Autorized");
                        autorizationStatus.Close();

                        string components = string.Empty;
                        MessageBox.Show(SelectComponents(components), "Notification", MessageBoxButton.OK, MessageBoxImage.Information);

                        MainWindow mainWindow = new MainWindow();
                        mainWindow.Show();
                        this.Close();
                    }
                    else
                    {
                        MessageBox.Show("Wrong login or password.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                        return;
                    }                
                }
                else if (table.Rows.Count == 0)
                {
                    MessageBox.Show("Wrong login or password.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
        }

        private string SelectComponents(string component)
        {
            string selectComponentsQuery = "SELECT * FROM Component JOIN TractorBrand ON Component.tractorBrandCode = TractorBrand.tractorBrandCode WHERE [componentCount] < 100";
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(selectComponentsQuery, myConnectionString))
            {
                DataTable table = new DataTable();
                dataAdapter.Fill(table);
                if (table.Rows.Count > 0)
                {
                    for (int i = 0; i < table.Rows.Count; i++)
                    {
                        component += table.Rows[i]["componentName"].ToString() + " (" + table.Rows[i]["tractorBrandName"].ToString() + ") " + "   -   " + table.Rows[i]["componentCount"].ToString() + "\n";
                    }
                }
            }
            return component;
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

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
