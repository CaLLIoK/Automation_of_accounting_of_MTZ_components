using Automation_of_accounting_of_MTZ_components.Data_validation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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

namespace Automation_of_accounting_of_MTZ_components
{
    /// <summary>
    /// Логика взаимодействия для AccountSettings.xaml
    /// </summary>
    public partial class AccountSettings : Window
    {
        SqlConnection myConnectionString = new SqlConnection(@"Data Source=(local)\SQLEXPRESS; Initial Catalog=Automation_of_accounting_of_MTZ_components; Integrated Security=True");

        public AccountSettings()
        {
            InitializeComponent();

            int employeeCode = 0;
            int postCode = 0;

            StreamReader file = new StreamReader("UserLogin.txt");
            string employeeLogin = file.ReadLine();
            file.Close();

            string selectEmployeeLogin = "SELECT * FROM Employee WHERE [EmployeeLogin] = '" + employeeLogin + "'";
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(selectEmployeeLogin, myConnectionString))
            {
                DataTable table = new DataTable();
                dataAdapter.Fill(table);
                if (table.Rows.Count > 0)
                {
                    employeeCode = Convert.ToInt32(table.Rows[0]["employeeCode"].ToString());
                    loginField.Text = table.Rows[0]["employeeLogin"].ToString();
                    passwordField.Password = table.Rows[0]["employeePassword"].ToString();
                    nameField.Text = table.Rows[0]["employeeName"].ToString();
                    surnameField.Text = table.Rows[0]["employeeSurname"].ToString();
                    patronymicField.Text = table.Rows[0]["employeePatronymic"].ToString();
                    postCode = Convert.ToInt32(table.Rows[0]["postCode"].ToString());
                }
            }

            string selectPostNameQuery = "SELECT postName FROM Post WHERE [postCode] = " + postCode + "";
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(selectPostNameQuery, myConnectionString))
            {
                DataTable table = new DataTable();
                dataAdapter.Fill(table);
                if (table.Rows.Count > 0)
                {
                    postField.Text = table.Rows[0]["postName"].ToString();
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            int employeeCode = 0;

            StreamReader file = new StreamReader("UserLogin.txt");
            string employeeLogin = file.ReadLine();
            file.Close();

            string selectEmployeeCodeQuery = "SELECT employeeCode FROM Employee WHERE [employeeLogin] = '" + employeeLogin + "'";
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(selectEmployeeCodeQuery, myConnectionString))
            {
                DataTable table = new DataTable();
                dataAdapter.Fill(table);
                if (table.Rows.Count > 0)
                {
                    employeeCode = Convert.ToInt32(table.Rows[0]["employeeCode"].ToString());
                }
            }

            if (nameField.Text != EmployeeChecks.CheckEmployeeData(nameField.Text, "Name not entered.", "Name contains invalid symbols.", "Allowed name length is 2-30 symbols."))
            {
                MessageBox.Show(EmployeeChecks.CheckEmployeeData(nameField.Text, "Name not entered.", "Name contains invalid symbols.", "Allowed name length is 2-30 symbols."));
                return;
            }

            if (surnameField.Text != EmployeeChecks.CheckEmployeeData(surnameField.Text, "Surname not entered.", "Surname contains invalid symbols.", "Allowed surname length is 2-30 symbols."))
            {
                MessageBox.Show(EmployeeChecks.CheckEmployeeData(surnameField.Text, "Surname not entered.", "Surname contains invalid symbols.", "Allowed surname length is 2-30 symbols."));
                return;
            }

            if (patronymicField.Text != EmployeeChecks.CheckEmployeeData(patronymicField.Text, "Patronymic not entered.", "Patronymic contains invalid symbols.", "Allowed patronymic length is 2-30 symbols."))
            {
                MessageBox.Show(EmployeeChecks.CheckEmployeeData(patronymicField.Text, "Patronymic not entered.", "Patronymic contains invalid symbols.", "Allowed patronymic length is 2-30 symbols."));
                return;
            }

            if (loginField.Text != EmployeeChecks.CheckEmployeeLogin(loginField.Text, "Login not entered.", "Login contains invalid symbols.", "Allowed login length is 3-30 symbols."))
            {
                MessageBox.Show(EmployeeChecks.CheckEmployeeLogin(loginField.Text, "Login not entered.", "Login contains invalid symbols.", "Allowed login length is 3-30 symbols."));
                return;
            }

            string selectEmployeeLoginQuery = "SELECT * FROM Employee WHERE [employeeLogin] = '" + loginField.Text + "'";
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(selectEmployeeLoginQuery, myConnectionString))
            {
                DataTable table = new DataTable();
                dataAdapter.Fill(table);
                if (table.Rows.Count > 0 && table.Rows[0]["employeeLogin"].ToString() != employeeLogin)
                {
                    MessageBox.Show("This login is not available, please enter another one.");
                    return;
                }
            }

            if (passwordField.Password.ToString() != EmployeeChecks.CheckEmployeePassword(passwordField.Password.ToString()))
            {
                MessageBox.Show(EmployeeChecks.CheckEmployeePassword(passwordField.Password.ToString()));
                return;
            }

            //if (postField.Text == string.Empty)
            //{
            //    MessageBox.Show("Post not selected.");
            //    return;
            //}

            //int postCode = 0;
            //string selectPostCodeQuery = "SELECT postCode FROM Post WHERE [postName] = '" + postField.Text + "'";

            //using (SqlDataAdapter dataAdapter = new SqlDataAdapter(selectPostCodeQuery, myConnectionString))
            //{
            //    DataTable table = new DataTable();
            //    dataAdapter.Fill(table);
            //    if (table.Rows.Count > 0)
            //    {
            //        postCode = Convert.ToInt32(table.Rows[0]["postCode"].ToString());
            //    }
            //}

            string alterationQuery = "SELECT * FROM Employee WHERE [employeeCode] = '" + employeeCode + "'";
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(alterationQuery, myConnectionString))
            {
                DataTable table = new DataTable();
                dataAdapter.Fill(table);
                if (table.Rows.Count > 0 && (table.Rows[0]["employeeLogin"].ToString() != loginField.Text || table.Rows[0]["employeeName"].ToString() != nameField.Text || table.Rows[0]["employeeSurname"].ToString() != surnameField.Text || table.Rows[0]["employeePatronymic"].ToString() != patronymicField.Text || table.Rows[0]["employeePassword"].ToString() != passwordField.Password.ToString()))
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "UPDATE Employee SET employeeLogin = @login, employeePassword = @password, employeeName = @name, employeeSurname = @surname, employeePatronymic = @patronymic WHERE employeeCode = @code"; //question?
                    cmd.Parameters.Add("@login", SqlDbType.VarChar).Value = loginField.Text;
                    cmd.Parameters.Add("@code", SqlDbType.Int).Value = employeeCode;
                    cmd.Parameters.Add("@password", SqlDbType.VarChar).Value = passwordField.Password.ToString();
                    cmd.Parameters.Add("@name", SqlDbType.VarChar).Value = nameField.Text;
                    cmd.Parameters.Add("@surname", SqlDbType.VarChar).Value = surnameField.Text;
                    cmd.Parameters.Add("@patronymic", SqlDbType.VarChar).Value = patronymicField.Text;
                    cmd.Connection = myConnectionString;
                    myConnectionString.Open();
                    cmd.ExecuteNonQuery();
                    myConnectionString.Close();

                    MessageBox.Show("Changed saved.");

                    StreamWriter writeFile = new StreamWriter("UserLogin.txt");
                    writeFile.Write(loginField.Text);
                    writeFile.Close();
                    
                    MainWindow mainWindow = new MainWindow();
                    mainWindow.login.Text = loginField.Text;
                    mainWindow.Close();
                }
                else
                {
                    MessageBox.Show("You haven't made any changes.");
                    return;
                }
            }
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            MainWindow mainWindow = new MainWindow();
            mainWindow.Show();
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }
    }
}
