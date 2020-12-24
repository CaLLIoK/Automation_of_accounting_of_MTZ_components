using System;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Input;
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
            StreamReader codeFile = new StreamReader("ConsCode.txt");
            string codeAvailability = codeFile.ReadLine();
            codeFile.Close();

            if (codeAvailability != null)
            {
                ChangeConsumersInfoWindow changeConsumersInfoWindow = new ChangeConsumersInfoWindow();
                changeConsumersInfoWindow.Show();
                File.WriteAllText(@"ConsCode.txt", string.Empty);
                this.Close();
            }
            else
            {
                MainWindow mainWindow = new MainWindow();
                mainWindow.Show();
                this.Close();
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (nameField.Text != ConsumersChecks.CheckConsumerName(nameField.Text, "Consumer name not entered.", "Consumer name contains invalid symbols.", "Allowed consumer name lenght is 3-80 symbols."))
            {
                MessageBox.Show(ConsumersChecks.CheckConsumerName(nameField.Text, "Consumer name not entered.", "Consumer name contains invalid symbols.", "Allowed consumer name lenght is 3-80 symbols."), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (phoneField.Text != ConsumersChecks.CheckConsumerPhone(phoneField.Text, "Consumer phone not entered.", "Consumer phone contains invalid symbols.", "The phone number should look like this: +375(17|25|29|33|44)###-##-##."))
            {
                MessageBox.Show(ConsumersChecks.CheckConsumerPhone(phoneField.Text, "Consumer phone not entered.", "Consumer phone contains invalid symbols.", "The phone number should look like this: +375(17|25|29|33|44)###-##-##."), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (streetField.Text != ConsumersChecks.CheckConsumerStreet(streetField.Text, "Street not entered.", "Street contains invalid symbols.", "Allowed consumer phone lenght is 17 symbols."))
            {
                MessageBox.Show(ConsumersChecks.CheckConsumerStreet(streetField.Text, "Street not entered.", "Street contains invalid symbols.", "Allowed street lenght is 3-60 symbols."), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (buildingFiled.Text != ConsumersChecks.CheckConsumerHouseOrOffice(buildingFiled.Text, "Building not entered.", "Building contains invalid symbols.", "Building can't be nagative."))
            {
                MessageBox.Show(ConsumersChecks.CheckConsumerHouseOrOffice(buildingFiled.Text, "Building not entered.", "Building contains invalid symbols.", "Building can't be nagative."), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (officeField.Text != ConsumersChecks.CheckConsumerHouseOrOffice(officeField.Text, "Office not entered.", "Office contains invalid symbols.", "Office can't be nagative."))
            {
                MessageBox.Show(ConsumersChecks.CheckConsumerHouseOrOffice(officeField.Text, "Office not entered.", "Office contains invalid symbols.", "Office can't be nagative."), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string selectConsumerPhoneQuery = "SELECT * FROM Consumer WHERE [consumerPhone] = '" + phoneField.Text + "'";
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(selectConsumerPhoneQuery, connectionString))
            {
                DataTable table = new DataTable();
                dataAdapter.Fill(table);
                if (table.Rows.Count > 0)
                {
                    MessageBox.Show("This phone is not available, please enter another one.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    MessageBox.Show("This consumer name is not available, please enter another one.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                    MessageBox.Show("Adding consumer is successful!", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                    nameField.Clear();
                    phoneField.Clear();
                    streetField.Clear();
                    buildingFiled.Clear();
                    officeField.Clear();
                }
            }
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (nameField.Text != ConsumersChecks.CheckConsumerName(nameField.Text, "Consumer name not entered.", "Consumer name contains invalid symbols.", "lAlowed consumer name lenght is 3-80 symbols."))
            {
                MessageBox.Show(ConsumersChecks.CheckConsumerName(nameField.Text, "Consumer name not entered.", "Consumer name contains invalid symbols.", "Allowed consumer name lenght is 3-80 symbols."), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (phoneField.Text != ConsumersChecks.CheckConsumerPhone(phoneField.Text, "Consumer phone not entered.", "Consumer phone contains invalid symbols.", "The phone number should look like this: +375(17|25|29|33|44)###-##-##."))
            {
                MessageBox.Show(ConsumersChecks.CheckConsumerPhone(phoneField.Text, "Consumer phone not entered.", "Consumer phone contains invalid symbols.", "The phone number should look like this: +375(17|25|29|33|44)###-##-##."), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (streetField.Text != ConsumersChecks.CheckConsumerStreet(streetField.Text, "Street not entered.", "Street contains invalid symbols.", "Allowed consumer phone lenght is 17 symbols."))
            {
                MessageBox.Show(ConsumersChecks.CheckConsumerStreet(streetField.Text, "Street not entered.", "Street contains invalid symbols.", "Allowed street lenght is 3-60 symbols."), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (buildingFiled.Text != ConsumersChecks.CheckConsumerHouseOrOffice(buildingFiled.Text, "Building not entered.", "Building contains invalid symbols.", "Building can't be nagative."))
            {
                MessageBox.Show(ConsumersChecks.CheckConsumerHouseOrOffice(buildingFiled.Text, "Building not entered.", "Building contains invalid symbols.", "Building can't be nagative."), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (officeField.Text != ConsumersChecks.CheckConsumerHouseOrOffice(officeField.Text, "Office not entered.", "Office contains invalid symbols.", "Office can't be nagative."))
            {
                MessageBox.Show(ConsumersChecks.CheckConsumerHouseOrOffice(officeField.Text, "Office not entered.", "Office contains invalid symbols.", "Office can't be nagative."), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            StreamReader file = new StreamReader("ConsCode.txt");
            int consumerCode = Convert.ToInt32(file.ReadLine());
            file.Close();

            string adress = "ул. " + streetField.Text + ", д. " + buildingFiled.Text + ", офис " + officeField.Text;       
            string selectConsumerInfoQuery = "SELECT * FROM Consumer WHERE [consumerCode] = '" + consumerCode + "'";
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(selectConsumerInfoQuery, connectionString))
            {
                DataTable table = new DataTable();
                dataAdapter.Fill(table);
                if (table.Rows.Count > 0 && (table.Rows[0]["consumerName"].ToString() != nameField.Text || table.Rows[0]["consumerPhone"].ToString() != phoneField.Text || table.Rows[0]["consumerLegalAdress"].ToString() != adress))
                {
                    string currentName = table.Rows[0]["consumerName"].ToString();
                    string currentPhone = table.Rows[0]["consumerPhone"].ToString();
                    string selectConsumerNameQuery = "SELECT * FROM Consumer WHERE [consumerName] = '" + nameField.Text + "'";
                    using (SqlDataAdapter dataAdapter1 = new SqlDataAdapter(selectConsumerNameQuery, connectionString))
                    {
                        DataTable table1 = new DataTable();
                        dataAdapter1.Fill(table1);
                        if (table1.Rows.Count > 0 && nameField.Text != currentName)
                        {
                            MessageBox.Show("This consumer name is not available, please enter another one.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }

                    string selectConsumerPhoneQuery = "SELECT * FROM Consumer WHERE [consumerPhone] = '" + phoneField.Text + "'";
                    using (SqlDataAdapter dataAdapter1 = new SqlDataAdapter(selectConsumerPhoneQuery, connectionString))
                    {
                        DataTable table1 = new DataTable();
                        dataAdapter1.Fill(table1);
                        if (table1.Rows.Count > 0 && phoneField.Text != currentPhone)
                        {
                            MessageBox.Show("This phone is not available, please enter another one.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                            return;
                        }
                    }

                    SqlCommand cmd = new SqlCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "UPDATE Consumer SET consumerName = @name, consumerPhone = @phone, consumerLegalAdress = @adress WHERE consumerCode = @code";
                    cmd.Parameters.Add("@name", SqlDbType.VarChar).Value = nameField.Text;
                    cmd.Parameters.Add("@code", SqlDbType.Int).Value = consumerCode;
                    cmd.Parameters.Add("@phone", SqlDbType.VarChar).Value = phoneField.Text;
                    cmd.Parameters.Add("@adress", SqlDbType.VarChar).Value = adress;
                    cmd.Connection = connectionString;
                    connectionString.Open();
                    cmd.ExecuteNonQuery();
                    connectionString.Close();
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