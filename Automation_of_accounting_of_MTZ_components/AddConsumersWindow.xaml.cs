using System;
using System.Collections.Generic;
using System.Data;
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
using Automation_of_accounting_of_MTZ_components.Data_validation;

namespace Automation_of_accounting_of_MTZ_components
{
    /// <summary>
    /// Логика взаимодействия для AddConsumersWindow.xaml
    /// </summary>
    public partial class AddConsumersWindow : Window
    {
        SqlConnection connectionString = new SqlConnection(@"Data Source=(local)\SQLEXPRESS; Initial Catalog=Automation_of_accounting_of_MTZ_components; Integrated Security=True");

        public AddConsumersWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (nameField.Text != ConsumersChecks.CheckConsumerName(nameField.Text, "Consumer name not entered.", "Consumer name contains invalid symbols.", "Alowed consumer name lenght is 3-80 symbols."))
            {
                MessageBox.Show(ConsumersChecks.CheckConsumerName(nameField.Text, "Consumer name not entered.", "Consumer name contains invalid symbols.", "Alowed consumer name lenght is 3-80 symbols."));
                return;
            }

            if (phoneField.Text != ConsumersChecks.CheckConsumerPhone(phoneField.Text, "Consumer phone not entered.", "Consumer phone contains invalid symbols.", "Allowed consumer phone lenght is 17 symbols."))
            {
                MessageBox.Show(ConsumersChecks.CheckConsumerPhone(phoneField.Text, "Consumer phone not entered.", "Consumer phone contains invalid symbols.", "Allowed consumer phone lenght is 17 symbols."));
                return;
            }

            if (streetField.Text != ConsumersChecks.CheckConsumerStreet(streetField.Text, "Street not entered.", "Street contains invalid symbols.", "Allowed consumer phone lenght is 17 symbols."))
            {
                MessageBox.Show(ConsumersChecks.CheckConsumerStreet(streetField.Text, "Street not entered.", "Street contains invalid symbols.", "Allowed street lenght is 3-60 symbols."));
                return;
            }

            if (buildingFiled.Text != ConsumersChecks.CheckConsumerHouseOrOffice(buildingFiled.Text, "Building not entered.", "Building contains invalid symbols.", "Building can't be nagative."))
            {
                MessageBox.Show(ConsumersChecks.CheckConsumerHouseOrOffice(buildingFiled.Text, "Building not entered.", "Building contains invalid symbols.", "Building can't be nagative."));
                return;
            }

            if (officeField.Text != ConsumersChecks.CheckConsumerHouseOrOffice(officeField.Text, "Office not entered.", "Office contains invalid symbols.", "Office can't be nagative."))
            {
                MessageBox.Show(ConsumersChecks.CheckConsumerHouseOrOffice(officeField.Text, "Office not entered.", "Office contains invalid symbols.", "Office can't be nagative."));
                return;
            }

            string selectConsumerPhoneQuery = "SELECT * FROM Consumer WHERE [consumerPhone] = '" + phoneField.Text + "'";
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(selectConsumerPhoneQuery, connectionString))
            {
                DataTable table = new DataTable();
                dataAdapter.Fill(table);
                if (table.Rows.Count > 0)
                {
                    MessageBox.Show("This phone is not available, please enter another one.");
                    return;
                }
            }

            string adress = "ул. " + streetField.Text + ", д. " + buildingFiled.Text + ", офис " + officeField.Text;

            string selectConsumerNameQuery = "SELECT * FROM Consumer WHERE [consumerName] = '" + nameField.Text + "'";
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(selectConsumerNameQuery, connectionString))
            {
                DataTable table = new DataTable();
                dataAdapter.Fill(table);
                if (table.Rows.Count > 0)
                {
                    MessageBox.Show("This consumer name is not available, please enter another one.");
                    return;
                }
                else if (table.Rows.Count == 0)
                {
                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "INSERT Consumer (consumerName, consumerLegalAdress, consumerPhone) VALUES (@name, @adress, @phone)";
                    cmd.Parameters.Add("@name", SqlDbType.VarChar).Value = nameField.Text;
                    cmd.Parameters.Add("@adress", SqlDbType.VarChar).Value = adress;
                    cmd.Parameters.Add("@phone", SqlDbType.VarChar).Value = phoneField.Text;
                    cmd.Connection = connectionString;
                    connectionString.Open();
                    cmd.ExecuteNonQuery();
                    connectionString.Close();
                }
            }
            MessageBox.Show("Adding consumer is successful!");
        }
    }
}