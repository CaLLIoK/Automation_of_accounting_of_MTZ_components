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
using System.Windows.Shapes;

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
            string componentsInfoQuery = "SELECT employeeName, employeeSurname, employeePatronymic, employeeLogin, employeePassword, postName " +
                                         "FROM Employee " +
                                         "JOIN Post ON Employee.postCode = Post.postCode";

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
            this.Close();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (EmployeesInfoGrid.SelectedItem == null)
            {
                MessageBox.Show("Can't delete the blank entry.");
                return;
            }
            else
            {
                StreamReader file = new StreamReader("UserLogin.txt");
                string employeeLogin = file.ReadLine();
                file.Close();

                DataRowView employeeInfo = (DataRowView)EmployeesInfoGrid.SelectedItems[0];
                if (employeeInfo["employeeLogin"].ToString() == employeeLogin)
                {
                    MessageBox.Show("You can't delete your own account.");
                    return;
                }
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "DELETE FROM Employee WHERE [employeeLogin] = @login";
                cmd.Parameters.Add("@login", SqlDbType.VarChar).Value = employeeInfo["employeeLogin"].ToString();
                cmd.Connection = connectionString;
                connectionString.Open();
                cmd.ExecuteNonQuery();
                FillDataGrid();
                connectionString.Close();
                MessageBox.Show("Deletion completed successfully.");
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
