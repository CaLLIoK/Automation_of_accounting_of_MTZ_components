using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Automation_of_accounting_of_MTZ_components
{
    /// <summary>
    /// Логика взаимодействия для ChangeEmployeesInfoWindow.xaml
    /// </summary>
    public partial class ChangeEmployeesInfoWindow : Window
    {
        SqlConnection connectionString = new SqlConnection(@"Data Source=(local)\SQLEXPRESS; Initial Catalog=Automation_of_accounting_of_MTZ_components; Integrated Security=True");

        public ChangeEmployeesInfoWindow()
        {
            InitializeComponent();

            connectionString.Open();
            FillDataGrid();
            connectionString.Close();
        }

        private void FillDataGrid()
        {
            StreamReader file = new StreamReader("UserLogin.txt");
            string login = file.ReadLine();
            file.Close();

            string componentsInfoQuery = "SELECT employeeName, employeeSurname, employeePatronymic, employeeLogin, employeePassword, postName " +
                                         "FROM Employee " +
                                         "JOIN Post ON Employee.postCode = Post.postCode WHERE employeeLogin != '" + login + "'";

            DataTable table = new DataTable();
            using (SqlCommand cmd = new SqlCommand(componentsInfoQuery, connectionString))
            {
                using (IDataReader rdr = cmd.ExecuteReader())
                {
                    table.Load(rdr);
                }
            }
            EmployeesInfoGrid.ItemsSource = table.DefaultView;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeesInfoGrid.SelectedItem == null)
            {
                MessageBox.Show("Can't delete the blank entry.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                DataRowView employeeInfo = (DataRowView)EmployeesInfoGrid.SelectedItems[0];
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "DELETE FROM Employee WHERE [employeeLogin] = @login";
                cmd.Parameters.Add("@login", SqlDbType.VarChar).Value = employeeInfo["employeeLogin"].ToString();
                cmd.Connection = connectionString;
                connectionString.Open();
                cmd.ExecuteNonQuery();
                FillDataGrid();
                connectionString.Close();
                MessageBox.Show("Deletion completed successfully.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
            }
        }

        private void ChangeButton_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeesInfoGrid.SelectedItem == null)
            {
                MessageBox.Show("Can't change the blank entry.");
                return;
            }
            else
            {
                DataRowView employeeInfo = (DataRowView)EmployeesInfoGrid.SelectedItems[0];
                RegistrationWindow registrationWindow = new RegistrationWindow();
                string selectEmployeeInfoQuery = "SELECT employeeCode FROM Employee JOIN Post ON Employee.postCode = Post.postCode " +
                                                 "WHERE [employeeName] = '" + employeeInfo["employeeName"].ToString() + "' AND [employeeSurname] = '" + employeeInfo["employeeSurname"].ToString() + "' AND [employeePatronymic] = '" + employeeInfo["employeePatronymic"].ToString() + "' " +
                                                 "AND [employeeLogin] = '" + employeeInfo["employeeLogin"].ToString() + "' AND [employeePassword] = '" + employeeInfo["employeePassword"].ToString() + "' AND [postName] = '" + employeeInfo["postName"].ToString() + "'";
                using (SqlDataAdapter dataAdapter = new SqlDataAdapter(selectEmployeeInfoQuery, connectionString))
                {
                    DataTable table = new DataTable();
                    dataAdapter.Fill(table);
                    if (table.Rows.Count > 0)
                    {
                        StreamWriter employeeCode = new StreamWriter("EmpCode.txt");
                        employeeCode.Write(table.Rows[0]["employeeCode"].ToString());
                        employeeCode.Close();
                    }
                }
                registrationWindow.Title.Content = "Change employee information";
                registrationWindow.Description.Content = "Change the fields that you need";
                registrationWindow.RegisterButton.Visibility = Visibility.Hidden;
                registrationWindow.SaveButton.Visibility = Visibility.Visible;
                registrationWindow.SingInButton.Visibility = Visibility.Hidden;
                registrationWindow.Account.Visibility = Visibility.Hidden;
                registrationWindow.nameField.Text = employeeInfo["employeeName"].ToString();
                registrationWindow.surnameField.Text = employeeInfo["employeeSurname"].ToString();
                registrationWindow.patronymicField.Text = employeeInfo["employeePatronymic"].ToString();
                registrationWindow.loginField.Text = employeeInfo["employeeLogin"].ToString();
                registrationWindow.passwordField.Password = employeeInfo["employeePassword"].ToString();
                registrationWindow.postField.Text = employeeInfo["postName"].ToString();
                registrationWindow.Show();
                this.Close();
            }
        }
    }
}
