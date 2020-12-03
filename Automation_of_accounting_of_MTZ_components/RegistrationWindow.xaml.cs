using Automation_of_accounting_of_MTZ_components.Data_validation;
using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Automation_of_accounting_of_MTZ_components
{
    /// <summary>
    /// Логика взаимодействия для RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        SqlConnection myConnectionString = new SqlConnection(@"Data Source=(local)\SQLEXPRESS; Initial Catalog=Automation_of_accounting_of_MTZ_components; Integrated Security=True");
        internal string successfulRegistration = "Registration is successful!";

        public RegistrationWindow()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            if (nameField.Text != EmployeeChecks.CheckEmployeeData(nameField.Text, "Name not entered.", "Name contains invalid symbols.", "Allowed name length is 2-30 symbols."))
            {
                MessageBox.Show(EmployeeChecks.CheckEmployeeData(nameField.Text, "Name not entered.", "Name contains invalid symbols.", "Allowed name length is 2-30 symbols."), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (surnameField.Text != EmployeeChecks.CheckEmployeeData(surnameField.Text, "Surname not entered.", "Surname contains invalid symbols.", "Allowed surname length is 2-30 symbols."))
            {
                MessageBox.Show(EmployeeChecks.CheckEmployeeData(surnameField.Text, "Surname not entered.", "Surname contains invalid symbols.", "Allowed surname length is 2-30 symbols."), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (patronymicField.Text != EmployeeChecks.CheckEmployeeData(patronymicField.Text, "Patronymic not entered.", "Patronymic contains invalid symbols.", "Allowed patronymic length is 2-30 symbols."))
            {
                MessageBox.Show(EmployeeChecks.CheckEmployeeData(patronymicField.Text, "Patronymic not entered.", "Patronymic contains invalid symbols.", "Allowed patronymic length is 2-30 symbols."), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (loginField.Text != EmployeeChecks.CheckEmployeeLogin(loginField.Text, "Login not entered.", "Login contains invalid symbols.", "Allowed login length is 3-30 symbols."))
            {
                MessageBox.Show(EmployeeChecks.CheckEmployeeLogin(loginField.Text, "Login not entered.", "Login contains invalid symbols.", "Allowed login length is 3-30 symbols."), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (passwordField.Password.ToString() != EmployeeChecks.CheckEmployeePassword(passwordField.Password.ToString()))
            {
                MessageBox.Show(EmployeeChecks.CheckEmployeePassword(passwordField.Password.ToString()), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (postField.Text == string.Empty)
            {
                MessageBox.Show("Post not selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int postCode = 0;
            string selectPostCodeQuery = "SELECT postCode FROM Post WHERE [postName] = '" + postField.Text + "'";

            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(selectPostCodeQuery, myConnectionString))
            {
                DataTable table = new DataTable();
                dataAdapter.Fill(table);
                if (table.Rows.Count > 0)
                {
                    postCode = int.Parse(table.Rows[0]["postCode"].ToString());
                }
            }

            string selectEmployeeLoginQuery = "SELECT * FROM Employee WHERE [employeeLogin] = '" + loginField.Text + "'";
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(selectEmployeeLoginQuery, myConnectionString))
            {
                DataTable table = new DataTable();
                dataAdapter.Fill(table);
                if (table.Rows.Count > 0)
                {
                    MessageBox.Show("This login is not available, please enter another one.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
                else if (table.Rows.Count == 0)
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "INSERT Employee (EmployeeLogin,EmployeePassword, EmployeeName, EmployeeSurname, EmployeePatronymic, PostCode) VALUES (@login, @password, @name, @surname, @patronymic, @code)";
                    cmd.Parameters.Add("@login", SqlDbType.VarChar).Value = loginField.Text;
                    cmd.Parameters.Add("@password", SqlDbType.VarChar).Value = passwordField.Password.ToString();
                    cmd.Parameters.Add("@name", SqlDbType.VarChar).Value = nameField.Text;
                    cmd.Parameters.Add("@surname", SqlDbType.VarChar).Value = surnameField.Text;
                    cmd.Parameters.Add("@patronymic", SqlDbType.VarChar).Value = patronymicField.Text;
                    cmd.Parameters.Add("@code", SqlDbType.VarChar).Value = postCode;
                    cmd.Connection = myConnectionString;
                    myConnectionString.Open();
                    cmd.ExecuteNonQuery();
                    myConnectionString.Close();
                    MessageBox.Show(successfulRegistration, "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                    loginField.Clear();
                    passwordField.Clear();
                    nameField.Clear();
                    surnameField.Clear();
                    patronymicField.Clear();
                    postField.SelectedIndex = -1;
                }
            }
        }

        private void SingInButton_Click(object sender, RoutedEventArgs e)
        {
            AutorizationWindow autorizationWindow = new AutorizationWindow();
            autorizationWindow.Show();
            this.Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            StreamReader statusFile = new StreamReader("AutorizationStatus.txt");
            string autorizationStatus = statusFile.ReadLine();
            statusFile.Close();

            StreamReader codeFile = new StreamReader("EmpCode.txt");
            string codeAvailability = codeFile.ReadLine();
            codeFile.Close();

            if (autorizationStatus == "Autorized" && codeAvailability != null)
            {
                ChangeEmployeesInfoWindow changeEmployeesInfoWindow = new ChangeEmployeesInfoWindow();
                changeEmployeesInfoWindow.Show();
                File.WriteAllText(@"EmpCode.txt", string.Empty);
                this.Close();
            }
            else if (autorizationStatus == "Autorized")
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
            else
            {
                this.Close();
            }
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (nameField.Text != EmployeeChecks.CheckEmployeeData(nameField.Text, "Name not entered.", "Name contains invalid symbols.", "Allowed name length is 2-30 symbols."))
            {
                MessageBox.Show(EmployeeChecks.CheckEmployeeData(nameField.Text, "Name not entered.", "Name contains invalid symbols.", "Allowed name length is 2-30 symbols."), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (surnameField.Text != EmployeeChecks.CheckEmployeeData(surnameField.Text, "Surname not entered.", "Surname contains invalid symbols.", "Allowed surname length is 2-30 symbols."))
            {
                MessageBox.Show(EmployeeChecks.CheckEmployeeData(surnameField.Text, "Surname not entered.", "Surname contains invalid symbols.", "Allowed surname length is 2-30 symbols."), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (patronymicField.Text != EmployeeChecks.CheckEmployeeData(patronymicField.Text, "Patronymic not entered.", "Patronymic contains invalid symbols.", "Allowed patronymic length is 2-30 symbols."))
            {
                MessageBox.Show(EmployeeChecks.CheckEmployeeData(patronymicField.Text, "Patronymic not entered.", "Patronymic contains invalid symbols.", "Allowed patronymic length is 2-30 symbols."), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (loginField.Text != EmployeeChecks.CheckEmployeeLogin(loginField.Text, "Login not entered.", "Login contains invalid symbols.", "Allowed login length is 3-30 symbols."))
            {
                MessageBox.Show(EmployeeChecks.CheckEmployeeLogin(loginField.Text, "Login not entered.", "Login contains invalid symbols.", "Allowed login length is 3-30 symbols."), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (passwordField.Password.ToString() != EmployeeChecks.CheckEmployeePassword(passwordField.Password.ToString()))
            {
                MessageBox.Show(EmployeeChecks.CheckEmployeePassword(passwordField.Password.ToString()), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int postCode = 0;
            string selectPostCodeQuery = "SELECT postCode FROM Post WHERE [postName] = '" + postField.Text + "'";

            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(selectPostCodeQuery, myConnectionString)) //choice of employee post code
            {
                DataTable table = new DataTable();
                dataAdapter.Fill(table);
                if (table.Rows.Count > 0)
                {
                    postCode = int.Parse(table.Rows[0]["postCode"].ToString());
                }
            }

            StreamReader file = new StreamReader("EmpCode.txt");
            int employeeCode = Convert.ToInt32(file.ReadLine()); //code of the selected employee
            file.Close();

            string selectEmployeeInfoQuery = "SELECT * FROM Employee WHERE [employeeCode] = '" + employeeCode + "'"; //select all information about employee
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(selectEmployeeInfoQuery, myConnectionString))
            {
                DataTable table = new DataTable();
                dataAdapter.Fill(table); //filling the table with an appropriate data
                if (table.Rows.Count > 0 && (table.Rows[0]["employeeLogin"].ToString() != loginField.Text || table.Rows[0]["employeeName"].ToString() != nameField.Text || table.Rows[0]["employeeSurname"].ToString() != surnameField.Text || table.Rows[0]["employeePatronymic"].ToString() != patronymicField.Text || table.Rows[0]["employeePassword"].ToString() != passwordField.Password.ToString() || table.Rows[0]["postCode"].ToString() != postCode.ToString()))
                {
                    string currentLogin = table.Rows[0]["employeeLogin"].ToString();
                    string selectEmployeeLoginQuery = "SELECT * FROM Employee WHERE [employeeLogin] = '" + loginField.Text + "'";
                    using (SqlDataAdapter dataAdapter1 = new SqlDataAdapter(selectEmployeeLoginQuery, myConnectionString))
                    {
                        DataTable table1 = new DataTable();
                        dataAdapter1.Fill(table1);
                        if (table1.Rows.Count > 0 && loginField.Text != currentLogin) //checking login for availability
                        {
                            MessageBox.Show("This login is not available, please enter another one.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }

                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "UPDATE Employee SET employeeLogin = @login, employeePassword = @password, employeeName = @name, employeeSurname = @surname, employeePatronymic = @patronymic, postCode = @post WHERE employeeCode = @code"; //update selected employee information
                    cmd.Parameters.Add("@login", SqlDbType.VarChar).Value = loginField.Text;
                    cmd.Parameters.Add("@code", SqlDbType.Int).Value = employeeCode;
                    cmd.Parameters.Add("@post", SqlDbType.Int).Value = postCode;
                    cmd.Parameters.Add("@password", SqlDbType.VarChar).Value = passwordField.Password.ToString();
                    cmd.Parameters.Add("@name", SqlDbType.VarChar).Value = nameField.Text;
                    cmd.Parameters.Add("@surname", SqlDbType.VarChar).Value = surnameField.Text;
                    cmd.Parameters.Add("@patronymic", SqlDbType.VarChar).Value = patronymicField.Text;
                    cmd.Connection = myConnectionString;
                    myConnectionString.Open();
                    cmd.ExecuteNonQuery();
                    myConnectionString.Close();
                    MessageBox.Show("Changes saved.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                }
                else
                {
                    MessageBox.Show("You haven't made any changes.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
        }
    }
}
