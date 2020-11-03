using Automation_of_accounting_of_MTZ_components.EmployeeFolder;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
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

namespace Automation_of_accounting_of_MTZ_components
{
    /// <summary>
    /// Логика взаимодействия для RegistrationWindow.xaml
    /// </summary>
    public partial class RegistrationWindow : Window
    {
        //EmployeeContext db;
        public RegistrationWindow()
        {
            InitializeComponent();
        }

        private void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            int postCode = 0;
            SqlConnection myConnectionString = new SqlConnection(@"Data Source=(local)\SQLEXPRESS; Initial Catalog=Automation_of_accounting_of_MTZ_components; Integrated Security=True");
            string selectPostCodeQuery = "SELECT postCode FROM Post WHERE [postName] = '" + postField.Text + "'";

            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(selectPostCodeQuery, myConnectionString))
            {
                DataTable table = new DataTable();
                dataAdapter.Fill(table);
                if (table.Rows.Count > 0)
                {
                    postCode = Convert.ToInt32(table.Rows[0]["postCode"].ToString());
                }
            }

            //db = new EmployeeContext();
            //var employees = new List<Employee>();
            //Employee employee = new Employee(loginField.Text, passwordField.Password.ToString(), nameField.Text, surnameField.Text, patronymicField.Text, postCode);
            //employees.Add(employee);
            //db.Employee.AddRange(employees);
            //db.SaveChanges();

            string selectEmployeeLogin = "SELECT * FROM Employee WHERE [EmployeeLogin] = '" + loginField.Text + "'";
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(selectEmployeeLogin, myConnectionString))
            {
                DataTable table = new DataTable();
                dataAdapter.Fill(table);
                if (table.Rows.Count > 0)
                {
                    MessageBox.Show("This login is not available, please enter another one.");
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
                    MessageBox.Show("Registration is successful!");
                }
            }
        }

        private void Grid_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void SingInButton_Click(object sender, RoutedEventArgs e)
        {
            AutorizationWindow autorizationWindow = new AutorizationWindow();
            autorizationWindow.Show();
            this.Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Application.Current.Shutdown();
        }
    }
}
