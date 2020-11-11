using Automation_of_accounting_of_MTZ_components.Data_validation;
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

namespace Automation_of_accounting_of_MTZ_components
{
    /// <summary>
    /// Логика взаимодействия для AddComponents.xaml
    /// </summary>
    public partial class AddComponents : Window
    {
        SqlConnection connectionString = new SqlConnection(@"Data Source=(local)\SQLEXPRESS; Initial Catalog=Automation_of_accounting_of_MTZ_components; Integrated Security=True");
        string description = string.Empty;

        public AddComponents()
        {
            InitializeComponent();

            List<string> tractorBrandNames = new List<string>();
            connectionString.Open();
            string query = @"SELECT tractorBrandName FROM TractorBrand";
            SqlCommand sqlCommand = new SqlCommand(query, connectionString);
            SqlDataReader dataReader = sqlCommand.ExecuteReader();
            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    tractorBrandNames.Add(dataReader["tractorBrandName"].ToString());
                    var newList = from i in tractorBrandNames orderby i select i;
                    tractorField.ItemsSource = newList;
                }
            }
            dataReader.Close();
            connectionString.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void AddToExistingEntry(int code)
        {
            if (Convert.ToInt32(countField.Text) > 0)
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "UPDATE Component SET [componentCount] = [componentCount] + @count, [availabilityStatusCode] = (SELECT availabilityStatusCode FROM AvailabilityStatus WHERE availabilityStatusName = @status) WHERE [componentCode] = @code";
                cmd.Parameters.Add("@count", SqlDbType.Int).Value = Convert.ToInt32(countField.Text);
                cmd.Parameters.Add("@code", SqlDbType.Int).Value = code;
                cmd.Parameters.Add("@status", SqlDbType.VarChar).Value = "Есть в наличии";
                cmd.Connection = connectionString;
                connectionString.Open();
                cmd.ExecuteNonQuery();
                connectionString.Close();
            }
            else
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "UPDATE Component SET [componentCount] = [componentCount] + @count WHERE [componentCode] = @code";
                cmd.Parameters.Add("@count", SqlDbType.Int).Value = Convert.ToInt32(countField.Text);
                cmd.Parameters.Add("@code", SqlDbType.Int).Value = code;
                cmd.Connection = connectionString;
                connectionString.Open();
                cmd.ExecuteNonQuery();
                connectionString.Close();
            }
        }

        private void InsertIntoArrival()
        {
            SqlCommand cmd = new SqlCommand();
            cmd.CommandType = CommandType.Text;
            cmd.CommandText = "INSERT Arrival (arrivalDate, componentsCount, componentCode) VALUES (@date, @componentsCount, (SELECT componentCode FROM Component WHERE componentName = @componentName AND componentWeight = @componentWeight AND tractorBrandCode = (SELECT tractorBrandCode FROM TractorBrand WHERE tractorBrandName = @tractorBrandName)))";
            cmd.Parameters.Add("@date", SqlDbType.Date).Value = DateTime.Now.Date;
            cmd.Parameters.Add("@componentsCount", SqlDbType.Int).Value = Convert.ToInt32(countField.Text);
            cmd.Parameters.Add("@componentName", SqlDbType.VarChar).Value = componentNameField.Text;
            cmd.Parameters.Add("@componentWeight", SqlDbType.Float).Value = Convert.ToDouble(weightField.Text);
            cmd.Parameters.Add("@tractorBrandName", SqlDbType.VarChar).Value = tractorField.Text;
            cmd.Connection = connectionString;
            connectionString.Open();
            cmd.ExecuteNonQuery();
            connectionString.Close();
        }

        private void AddToNonExistingEntry()
        {
            if (Convert.ToInt32(countField.Text) == 0)
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT Component (tractorBrandCode, componentName, componentDescription, componentWeight, componentCount, componentCost, availabilityStatusCode) VALUES ((SELECT tractorBrandCode FROM TractorBrand WHERE tractorBrandName = @tractorName), @componentName, @componentDescription, @componentWeight, @componentCount, @componentCost, (SELECT availabilityStatusCode FROM AvailabilityStatus WHERE availabilityStatusName = @status))";
                cmd.Parameters.Add("@tractorName", SqlDbType.VarChar).Value = tractorField.Text;
                cmd.Parameters.Add("@componentName", SqlDbType.VarChar).Value = componentNameField.Text;
                cmd.Parameters.Add("@componentDescription", SqlDbType.VarChar).Value = description;
                cmd.Parameters.Add("@componentWeight", SqlDbType.Float).Value = Convert.ToDouble(weightField.Text);
                cmd.Parameters.Add("@componentCount", SqlDbType.Int).Value = Convert.ToInt32(countField.Text);
                cmd.Parameters.Add("@componentCost", SqlDbType.Float).Value = Convert.ToDouble(costField.Text);
                cmd.Parameters.Add("@status", SqlDbType.VarChar).Value = "Нет в наличии";
                cmd.Connection = connectionString;
                connectionString.Open();
                cmd.ExecuteNonQuery();
                connectionString.Close();
            }
            else
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT Component (tractorBrandCode, componentName, componentDescription, componentWeight, componentCount, componentCost, availabilityStatusCode) VALUES ((SELECT tractorBrandCode FROM TractorBrand WHERE tractorBrandName = @tractorName), @componentName, @componentDescription, @componentWeight, @componentCount, @componentCost, (SELECT availabilityStatusCode FROM AvailabilityStatus WHERE availabilityStatusName = @status))";
                cmd.Parameters.Add("@tractorName", SqlDbType.VarChar).Value = tractorField.Text;
                cmd.Parameters.Add("@componentName", SqlDbType.VarChar).Value = componentNameField.Text;
                cmd.Parameters.Add("@componentDescription", SqlDbType.VarChar).Value = description;
                cmd.Parameters.Add("@componentWeight", SqlDbType.Float).Value = Convert.ToDouble(weightField.Text);
                cmd.Parameters.Add("@componentCount", SqlDbType.Int).Value = Convert.ToInt32(countField.Text);
                cmd.Parameters.Add("@componentCost", SqlDbType.Float).Value = Convert.ToDouble(costField.Text);
                cmd.Parameters.Add("@status", SqlDbType.VarChar).Value = "Есть в наличии";
                cmd.Connection = connectionString;
                connectionString.Open();
                cmd.ExecuteNonQuery();
                connectionString.Close();

                InsertIntoArrival();
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (componentNameField.Text != ComponentsChecks.CheckComponentName(componentNameField.Text, "Component name not entered.", "Component name contains invalid symbols.", "Allowed component name length is 3-100 symbols."))
            {
                MessageBox.Show(ComponentsChecks.CheckComponentName(componentNameField.Text, "Component name not entered.", "Component name contains invalid symbols.", "Allowed component name length is 3-100 symbols."));
                return;
            }

            if (weightField.Text != ComponentsChecks.CheckComponentWeightOrCount(weightField.Text, "Weight not entered.", "Weight contains invalid symbols.", "Weight can't be negative."))
            {
                MessageBox.Show(ComponentsChecks.CheckComponentWeightOrCount(weightField.Text, "Weight not entered.", "Weight contains invalid symbols.", "Weight can't be negative."));
                return;
            }

            if (countField.Text != ComponentsChecks.CheckComponentWeightOrCount(countField.Text, "Count not entered.", "Count contains invalid symbols.", "Count can't be negative."))
            {
                MessageBox.Show(ComponentsChecks.CheckComponentWeightOrCount(countField.Text, "Count not entered.", "Count contains invalid symbols.", "Count can't be negative."));
                return;
            }

            if (costField.Text != ComponentsChecks.CheckComponentCost(costField.Text, "Cost not entered.", "Cost contains invalid symbols.", "Cost has an incorrect value."))
            {
                MessageBox.Show(ComponentsChecks.CheckComponentCost(costField.Text, "Cost not entered.", "Cost contains invalid symbols.", "Cost has an incorrect value."));
                return;
            }

            description = new TextRange(descriptionField.Document.ContentStart, descriptionField.Document.ContentEnd).Text;

            if (description != ComponentsChecks.CheckComponentDescription(description, "Description contains invalid symbols.", "Description lenght less than 200 symbols."))
            {
                MessageBox.Show(ComponentsChecks.CheckComponentDescription(description, "Description contains invalid symbols.", "Description lenght less than 200 symbols."));
                return;
            }

            if (tractorField.Text == string.Empty)
            {
                MessageBox.Show("Tractor not selected.");
                return;
            }

            string selectEmployeeLoginQuery = "SELECT * FROM Component JOIN TractorBrand ON Component.tractorBrandCode = TractorBrand.tractorBrandCode WHERE [componentName] = '" + componentNameField.Text + "' AND [tractorBrandName] = '" + tractorField.Text + "' AND [componentWeight] = " + Convert.ToDouble(weightField.Text) + "";
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(selectEmployeeLoginQuery, connectionString))
            {
                DataTable table = new DataTable();
                dataAdapter.Fill(table);
                if (table.Rows.Count > 0)   
                {
                    int componentCode = Convert.ToInt32(table.Rows[0]["componentCode"].ToString());
                    AddToExistingEntry(componentCode);
                    if (Convert.ToInt32(countField.Text) != 0) InsertIntoArrival();
                }
                else if (table.Rows.Count == 0)
                {
                    AddToNonExistingEntry();
                }
            }
        }
    }
}