using Automation_of_accounting_of_MTZ_components.Data_validation;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Windows;
using System.Windows.Documents;
using System.Windows.Input;

namespace Automation_of_accounting_of_MTZ_components
{
    /// <summary>
    /// Логика взаимодействия для AddComponents.xaml
    /// </summary>
    public partial class AddComponentsWindow : Window
    {
        SqlConnection connectionString = new SqlConnection(@"Data Source=(local)\SQLEXPRESS; Initial Catalog=Automation_of_accounting_of_MTZ_components; Integrated Security=True");
        string description = string.Empty;
        string desc = string.Empty;

        public AddComponentsWindow()
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
            StreamReader codeFile = new StreamReader("ComponentCode.txt");
            string codeAvailability = codeFile.ReadLine();
            codeFile.Close();

            if (codeAvailability != null)
            {
                ChangeComponentsInfoWindow changeComponentsInfoWindow = new ChangeComponentsInfoWindow();
                changeComponentsInfoWindow.Show();
                File.WriteAllText(@"ComponentCode.txt", string.Empty);
                this.Close();
            }
            else
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
        }

        private void AddToExistingEntry(int code)
        {
            if (Convert.ToInt32(countField.Text) > 0)
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "UPDATE Component SET [componentCount] = [componentCount] + @count, [availabilityStatusCode] = (SELECT availabilityStatusCode FROM AvailabilityStatus WHERE availabilityStatusName = @status) WHERE [componentCode] = @code";
                cmd.Parameters.Add("@count", SqlDbType.Int).Value = int.Parse(countField.Text);
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
                cmd.Parameters.Add("@count", SqlDbType.Int).Value = int.Parse(countField.Text);
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
            cmd.Parameters.Add("@componentsCount", SqlDbType.Int).Value = int.Parse(countField.Text);
            cmd.Parameters.Add("@componentName", SqlDbType.VarChar).Value = componentNameField.Text;
            cmd.Parameters.Add("@componentWeight", SqlDbType.Float).Value = double.Parse(weightField.Text);
            cmd.Parameters.Add("@tractorBrandName", SqlDbType.VarChar).Value = tractorField.Text;
            cmd.Connection = connectionString;
            connectionString.Open();
            cmd.ExecuteNonQuery();
            connectionString.Close();
        }

        private void AddToNonExistingEntry()
        {
            for (int i = 0; i < description.Length - 2; i++)
                desc += description[i];

            if (Convert.ToInt32(countField.Text) == 0)
            {
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "INSERT Component (tractorBrandCode, componentName, componentDescription, componentWeight, componentCount, componentCost, availabilityStatusCode) VALUES ((SELECT tractorBrandCode FROM TractorBrand WHERE tractorBrandName = @tractorName), @componentName, @componentDescription, @componentWeight, @componentCount, @componentCost, (SELECT availabilityStatusCode FROM AvailabilityStatus WHERE availabilityStatusName = @status))";
                cmd.Parameters.Add("@tractorName", SqlDbType.VarChar).Value = tractorField.Text;
                cmd.Parameters.Add("@componentName", SqlDbType.VarChar).Value = componentNameField.Text;
                cmd.Parameters.Add("@componentDescription", SqlDbType.VarChar).Value = desc;
                cmd.Parameters.Add("@componentWeight", SqlDbType.Float).Value = double.Parse(weightField.Text);
                cmd.Parameters.Add("@componentCount", SqlDbType.Int).Value = int.Parse(countField.Text);
                cmd.Parameters.Add("@componentCost", SqlDbType.Float).Value = double.Parse(costField.Text);
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
                cmd.Parameters.Add("@componentDescription", SqlDbType.VarChar).Value = desc;
                cmd.Parameters.Add("@componentWeight", SqlDbType.Float).Value = double.Parse(weightField.Text);
                cmd.Parameters.Add("@componentCount", SqlDbType.Int).Value = int.Parse(countField.Text);
                cmd.Parameters.Add("@componentCost", SqlDbType.Float).Value = double.Parse(costField.Text);
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
            if (componentNameField.Text != ComponentsChecks.CheckComponentName(componentNameField.Text, "Component name not entered.", "Component name contains invalid symbols.", "Allowed component name length is 3-100 symbols.")) //checking component name for correctness
            {
                MessageBox.Show(ComponentsChecks.CheckComponentName(componentNameField.Text, "Component name not entered.", "Component name contains invalid symbols.", "Allowed component name length is 3-100 symbols."), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (weightField.Text != ComponentsChecks.CheckComponentWeightOrCost(weightField.Text, "Weight not entered.", "Weight contains invalid symbols.", "Weight has an incorrect value.")) //checking weight for correctness
            {
                MessageBox.Show(ComponentsChecks.CheckComponentWeightOrCost(weightField.Text, "Weight not entered.", "Weight contains invalid symbols.", "Weight has an incorrect value."), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (countField.Text != ComponentsChecks.CheckComponentCount(countField.Text, "Count not entered.", "Count contains invalid symbols.", "Count can't be negative.")) //checking count for correctness
            {
                MessageBox.Show(ComponentsChecks.CheckComponentCount(countField.Text, "Count not entered.", "Count contains invalid symbols.", "Count can't be negative."), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (costField.Text != ComponentsChecks.CheckComponentWeightOrCost(costField.Text, "Cost not entered.", "Cost contains invalid symbols.", "Cost has an incorrect value.")) //checking cost for correctness
            {
                MessageBox.Show(ComponentsChecks.CheckComponentWeightOrCost(costField.Text, "Cost not entered.", "Cost contains invalid symbols.", "Cost has an incorrect value."), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            description = new TextRange(descriptionField.Document.ContentStart, descriptionField.Document.ContentEnd).Text;

            if (description != ComponentsChecks.CheckComponentDescription(description, "Description contains invalid symbols.", "Description lenght more than 200 symbols.")) //checking description name for correctness
            {
                MessageBox.Show(ComponentsChecks.CheckComponentDescription(description, "Description contains invalid symbols.", "Description lenght more than 200 symbols."), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (tractorField.Text == string.Empty) //checking tractor name for emptiness
            {
                MessageBox.Show("Tractor not selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            SqlCommand command = new SqlCommand("SELECT componentCode FROM Component JOIN TractorBrand ON Component.tractorBrandCode = TractorBrand.tractorBrandCode WHERE [componentName] = @name " +
                                                "AND [tractorBrandName] = @tractorName AND [componentWeight] = @weight", connectionString);
            command.Parameters.AddWithValue("@name", componentNameField.Text);
            command.Parameters.AddWithValue("@tractorName", tractorField.Text);
            command.Parameters.AddWithValue("@weight", double.Parse(weightField.Text));
            connectionString.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    int componentCode = Convert.ToInt32(reader["componentCode"]);
                    connectionString.Close();
                    AddToExistingEntry(componentCode); //adding data to an existing record, if one already exists
                    if (Convert.ToInt32(countField.Text) != 0) InsertIntoArrival(); //adding data to the table about the arrival of parts  
                }
                else
                {
                    connectionString.Close();
                    AddToNonExistingEntry(); //adding new data to the table
                }
            }
            MessageBox.Show("Adding component is successful!", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
            componentNameField.Clear();
            weightField.Clear();
            countField.Clear();
            costField.Clear();
            tractorField.SelectedIndex = -1;
            descriptionField.Document.Blocks.Clear();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (componentNameField.Text != ComponentsChecks.CheckComponentName(componentNameField.Text, "Component name not entered.", "Component name contains invalid symbols.", "Allowed component name length is 3-100 symbols.")) //checking component name for correctness
            {
                MessageBox.Show(ComponentsChecks.CheckComponentName(componentNameField.Text, "Component name not entered.", "Component name contains invalid symbols.", "Allowed component name length is 3-100 symbols."), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (weightField.Text != ComponentsChecks.CheckComponentWeightOrCost(weightField.Text, "Weight not entered.", "Weight contains invalid symbols.", "Weight can't be negative.")) //checking weight for correctness
            {
                MessageBox.Show(ComponentsChecks.CheckComponentWeightOrCost(weightField.Text, "Weight not entered.", "Weight contains invalid symbols.", "Weight can't be negative."), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (countField.Text != ComponentsChecks.CheckComponentCount(countField.Text, "Count not entered.", "Count contains invalid symbols.", "Count can't be negative.")) //checking count for correctness
            {
                MessageBox.Show(ComponentsChecks.CheckComponentCount(countField.Text, "Count not entered.", "Count contains invalid symbols.", "Count can't be negative."), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (costField.Text != ComponentsChecks.CheckComponentWeightOrCost(costField.Text, "Cost not entered.", "Cost contains invalid symbols.", "Cost has an incorrect value.")) //checking cost for correctness
            {
                MessageBox.Show(ComponentsChecks.CheckComponentWeightOrCost(costField.Text, "Cost not entered.", "Cost contains invalid symbols.", "Cost has an incorrect value."), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            description = new TextRange(descriptionField.Document.ContentStart, descriptionField.Document.ContentEnd).Text;

            if (description != ComponentsChecks.CheckComponentDescription(description, "Description contains invalid symbols.", "Description lenght more than 200 symbols.")) //checking description name for correctness
            {
                MessageBox.Show(ComponentsChecks.CheckComponentDescription(description, "Description contains invalid symbols.", "Description lenght more than 200 symbols."), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (tractorField.Text == string.Empty) //checking tractor name for emptiness
            {
                MessageBox.Show("Tractor not selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            for (int i = 0; i < description.Length - 2; i++)
                desc += description[i];

            StreamReader codeFile = new StreamReader("ComponentCode.txt");
            int componentCode = int.Parse(codeFile.ReadLine());
            codeFile.Close();

            SqlCommand command = new SqlCommand("SELECT * FROM Component JOIN TractorBrand ON Component.tractorBrandCode = TractorBrand.tractorBrandCode WHERE [componentCode] = @code", connectionString);
            command.Parameters.AddWithValue("@code", componentCode);
            connectionString.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read() && (reader["componentName"].ToString() != componentNameField.Text || reader["tractorBrandName"].ToString() != tractorField.Text || double.Parse(reader["componentWeight"].ToString()) != double.Parse(weightField.Text) || 
                    int.Parse(reader["componentCount"].ToString()) != int.Parse(countField.Text) || double.Parse(reader["componentCost"].ToString()) != double.Parse(costField.Text) || reader["componentDescription"].ToString() != desc))
                {
                    string currentComponentName = reader["componentName"].ToString();
                    string currentTractorBrandName = reader["tractorBrandName"].ToString();
                    double currentWeight = double.Parse(reader["componentWeight"].ToString());
                    connectionString.Close();
                    SqlCommand newCommand = new SqlCommand("SELECT * FROM Component JOIN TractorBrand ON Component.tractorBrandCode = TractorBrand.tractorBrandCode WHERE [componentName] = @name AND [tractorBrandName] = @tractorName AND [componentWeight] = @weight", connectionString);
                    newCommand.Parameters.AddWithValue("@name", componentNameField.Text);
                    newCommand.Parameters.AddWithValue("@tractorName", tractorField.Text);
                    newCommand.Parameters.AddWithValue("@weight", double.Parse(weightField.Text));
                    connectionString.Open();
                    using (SqlDataReader newReader = newCommand.ExecuteReader())
                    {
                        if (newReader.Read() && newReader["componentName"].ToString() != currentComponentName && newReader["tractorBrandName"].ToString() != currentTractorBrandName && double.Parse(newReader["componentWeight"].ToString()) != currentWeight)
                        {
                            connectionString.Close();
                            MessageBox.Show("This entry is already on the list.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                        else
                        {
                            connectionString.Close();
                            if (int.Parse(countField.Text) == 0)
                            {
                                SqlCommand cmd = new SqlCommand();
                                cmd.CommandType = CommandType.Text;
                                cmd.CommandText = "UPDATE Component SET [componentName] = @componentName, [tractorBrandCode] = (SELECT tractorBrandCode FROM TractorBrand WHERE tractorBrandName = @tractorName), [componentWeight] = @weight, [componentCount] = @count, [componentCost] = @cost, [componentDescription] = @description, [availabilityStatusCode] = (SELECT availabilityStatusCode FROM AvailabilityStatus WHERE availabilityStatusName = @status) WHERE [componentCode] = @code";
                                cmd.Parameters.Add("@componentName", SqlDbType.VarChar).Value = componentNameField.Text;
                                cmd.Parameters.Add("@code", SqlDbType.Int).Value = componentCode;
                                cmd.Parameters.Add("@tractorName", SqlDbType.VarChar).Value = tractorField.Text;
                                cmd.Parameters.Add("@weight", SqlDbType.Float).Value = double.Parse(weightField.Text);
                                cmd.Parameters.Add("@count", SqlDbType.Int).Value = int.Parse(countField.Text);
                                cmd.Parameters.Add("@cost", SqlDbType.Int).Value = double.Parse(costField.Text);
                                cmd.Parameters.Add("@description", SqlDbType.VarChar).Value = desc;
                                cmd.Parameters.Add("@status", SqlDbType.VarChar).Value = "Нет в наличии";
                                cmd.Connection = connectionString;
                                connectionString.Open();
                                cmd.ExecuteNonQuery();
                                connectionString.Close();
                                MessageBox.Show("Changes saved.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                            else
                            {
                                SqlCommand cmd = new SqlCommand();
                                cmd.CommandType = CommandType.Text;
                                cmd.CommandText = "UPDATE Component SET [componentName] = @componentName, [tractorBrandCode] = (SELECT tractorBrandCode FROM TractorBrand WHERE tractorBrandName = @tractorName), [componentWeight] = @weight, [componentCount] = @count, [componentCost] = @cost, [componentDescription] = @description, [availabilityStatusCode] = (SELECT availabilityStatusCode FROM AvailabilityStatus WHERE availabilityStatusName = @status) WHERE [componentCode] = @code";
                                cmd.Parameters.Add("@componentName", SqlDbType.VarChar).Value = componentNameField.Text;
                                cmd.Parameters.Add("@code", SqlDbType.Int).Value = componentCode;
                                cmd.Parameters.Add("@tractorName", SqlDbType.VarChar).Value = tractorField.Text;
                                cmd.Parameters.Add("@weight", SqlDbType.Float).Value = double.Parse(weightField.Text);
                                cmd.Parameters.Add("@count", SqlDbType.Int).Value = int.Parse(countField.Text);
                                cmd.Parameters.Add("@cost", SqlDbType.Int).Value = double.Parse(costField.Text);
                                cmd.Parameters.Add("@description", SqlDbType.VarChar).Value = desc;
                                cmd.Parameters.Add("@status", SqlDbType.VarChar).Value = "Есть в наличии";
                                cmd.Connection = connectionString;
                                connectionString.Open();
                                cmd.ExecuteNonQuery();
                                connectionString.Close();
                                MessageBox.Show("Changes saved.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                            }
                        }
                    }
                }
                else
                {
                    connectionString.Close();
                    MessageBox.Show("You haven't made any changes.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
        }
    }
}