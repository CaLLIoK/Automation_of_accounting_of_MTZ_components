using MaterialDesignThemes.Wpf;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
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

namespace Automation_of_accounting_of_MTZ_components
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        SqlConnection myConnectionString = new SqlConnection(@"Data Source=(local)\SQLEXPRESS; Initial Catalog=Automation_of_accounting_of_MTZ_components; Integrated Security=True");

        public MainWindow()
        {
            InitializeComponent();

            EmployeesInfo.Visibility = Visibility.Hidden;
            AddEmployees.Visibility = Visibility.Hidden;
            DeleteEmployees.Visibility = Visibility.Hidden;

            StreamReader file = new StreamReader("UserLogin.txt");
            string employeeLogin = file.ReadLine();
            file.Close();
            login.Text = employeeLogin;

            string components = string.Empty;           
            MessageBox.Show(SelectComponents(components));

            string post = string.Empty;
            string selectEmployeePostQuery = "SELECT postName FROM Employee JOIN Post ON Employee.postCode = Post.postCode WHERE employeeLogin = '" + login.Text + "'";
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(selectEmployeePostQuery, myConnectionString))
            {
                DataTable table = new DataTable();
                dataAdapter.Fill(table);
                if (table.Rows.Count > 0)
                {               
                    post = table.Rows[0]["postName"].ToString();
                }
            }
            if (post == "Администратор")
            {
                EmployeesInfo.Visibility = Visibility.Visible;
                AddEmployees.Visibility = Visibility.Visible;
                DeleteEmployees.Visibility = Visibility.Visible;
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
                        component += table.Rows[i]["componentName"].ToString() + " (" + table.Rows[i]["tractorBrandName"].ToString() + ") " + "\t - \t" + table.Rows[i]["componentCount"].ToString() + "\n";
                    }
                }
            }
            return component;
        }

        private void ButtonPopUpLogout_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void ButtonOpenMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Collapsed;
            ButtonCloseMenu.Visibility = Visibility.Visible;
        }

        private void ButtonCloseMenu_Click(object sender, RoutedEventArgs e)
        {
            ButtonOpenMenu.Visibility = Visibility.Visible;
            ButtonCloseMenu.Visibility = Visibility.Collapsed;
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void ChangeAccButton_Click(object sender, RoutedEventArgs e)
        {
            AutorizationWindow autorizationWindow = new AutorizationWindow();
            autorizationWindow.Show();
            this.Close();
        }

        private void SettingsButton_Click(object sender, RoutedEventArgs e)
        {
            AccountSettings accountSettings = new AccountSettings();
            accountSettings.Owner = this;
            accountSettings.Topmost = true;
            accountSettings.ShowDialog();
        }

        private void ComponentsInfo_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            ComponentsInfo componentsInfo = new ComponentsInfo();
            componentsInfo.Owner = this;
            componentsInfo.Topmost = true;
            componentsInfo.ShowDialog();
        }

        private void AddComponents_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            AddComponents addComponents = new AddComponents();
            addComponents.Owner = this;
            addComponents.Topmost = true;
            addComponents.ShowDialog();
        }

        private void AddEmployees_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            RegistrationWindow registrationWindow = new RegistrationWindow();
            registrationWindow.Title.Content = "Add employees";
            registrationWindow.Description.Content = "Fill the fields below to adding employee";
            registrationWindow.RegisterButton.Content = "ADD EMPLOYEE";
            registrationWindow.Account.Visibility = Visibility.Hidden;
            registrationWindow.SingInButton.Visibility = Visibility.Hidden;
            registrationWindow.successfulRegistration = "Adding employee is successful!";
            registrationWindow.Owner = this;
            registrationWindow.Topmost = true;
            registrationWindow.ShowDialog();
        }
    }
}