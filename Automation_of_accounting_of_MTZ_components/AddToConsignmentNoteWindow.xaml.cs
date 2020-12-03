using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
    /// Логика взаимодействия для AddToConsignmentNoteWindow.xaml
    /// </summary>
    public partial class AddToConsignmentNoteWindow : Window
    {
        SqlConnection connectionString = new SqlConnection(@"Data Source=(local)\SQLEXPRESS; Initial Catalog=Automation_of_accounting_of_MTZ_components; Integrated Security=True");
        string existingEntry = string.Empty;
        string[] componentsInfo;

        public AddToConsignmentNoteWindow()
        {
            InitializeComponent();
            
            connectionString.Open();
            FillDataGrid();

            List<string> consumersNames = new List<string>();
            string query = @"SELECT consumerName FROM Consumer";
            SqlCommand sqlCommand = new SqlCommand(query, connectionString);
            SqlDataReader dataReader = sqlCommand.ExecuteReader();
            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    consumersNames.Add(dataReader["consumerName"].ToString());
                    var newList = from i in consumersNames orderby i select i;
                    consumerField.ItemsSource = newList;
                }
            }
            dataReader.Close();
            connectionString.Close();
        }

        private void FillDataGrid()
        {
            string componentsInfoQuery = "SELECT tractorBrandName, componentName, componentWeight, componentCount, componentCost, availabilityStatusName " +
                                         "FROM Component " +
                                         "JOIN TractorBrand ON Component.tractorBrandCode = TractorBrand.tractorBrandCode " +
                                         "JOIN AvailabilityStatus ON Component.availabilityStatusCode = AvailabilityStatus.availabilityStatusCode " +
                                         "WHERE availabilityStatusName = 'Есть в наличии'";

            DataTable table = new DataTable();
            using (SqlCommand cmd = new SqlCommand(componentsInfoQuery, connectionString))
            {
                using (IDataReader rdr = cmd.ExecuteReader())
                {
                    table.Load(rdr);
                }
            }
            componentsGrid.ItemsSource = table.DefaultView;
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

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (componentsGrid.SelectedIndex == -1)
            {
                MessageBox.Show("You haven't selected a component for adding to basket.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            int currentCount = 0;
            DataRowView componentInfo = (DataRowView)componentsGrid.SelectedItems[0];

            SqlCommand command = new SqlCommand("SELECT componentCode, componentCount FROM Component JOIN TractorBrand ON Component.tractorBrandCode = TractorBrand.tractorBrandCode WHERE [componentName] = @name " +
                                                "AND [tractorBrandName] = @tractorName AND [componentWeight] = @weight", connectionString);
            command.Parameters.AddWithValue("@name", componentInfo["componentName"].ToString());
            command.Parameters.AddWithValue("@tractorName", componentInfo["tractorBrandName"].ToString());
            command.Parameters.AddWithValue("@weight", double.Parse(componentInfo["componentWeight"].ToString()));
            connectionString.Open();
            using (SqlDataReader reader = command.ExecuteReader())
            {
                if (reader.Read())
                {
                    StreamWriter componentFile = new StreamWriter("ComponentCode.txt");
                    componentFile.Write(reader["componentCode"].ToString());
                    componentFile.Close();
                    currentCount = int.Parse(reader["componentCount"].ToString());
                }
            }
            connectionString.Close();

            for (int i = 0; i < basket.Items.Count; i++)
            {
                existingEntry = basket.Items[i].ToString();
                existingEntry = existingEntry.Replace("\t-\t", "*");
                componentsInfo = existingEntry.Split('*');
                if (componentsInfo[0] == componentInfo["tractorBrandName"].ToString() && componentsInfo[1] == componentInfo["componentName"].ToString() && double.Parse(componentsInfo[2]) == double.Parse(componentInfo["componentWeight"].ToString()))
                {
                    MessageBox.Show("This item has already been added to the basket.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }
            }
            File.WriteAllText(@"ComponentsCount.txt", string.Empty);
            SelectCountOfComponentsWindow selectCountOfComponentsWindow = new SelectCountOfComponentsWindow();
            selectCountOfComponentsWindow.ShowDialog();
            int count = 0;
            StreamReader countFile = new StreamReader("ComponentsCount.txt");
            if (countFile == null)
            {           
                countFile.Close();
                return;
            }
            else
            {
                string checkContent = countFile.ReadLine();
                if (!String.IsNullOrEmpty(checkContent))
                {
                    int.TryParse(checkContent, out count);
                    countFile.Close();
                }
                else
                {
                    countFile.Close();
                    return;
                }
            }
            string newComponent = componentInfo["tractorBrandName"].ToString() + "\t-\t" + componentInfo["componentName"].ToString() + "\t-\t" + componentInfo["componentWeight"].ToString() + "\t-\t" + count;
            basket.Items.Add(newComponent);
            componentsGrid.SelectedIndex = -1;
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            if (basket.SelectedIndex == -1)
            {
                MessageBox.Show("You haven't selected a component for deleting to basket.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                basket.Items.RemoveAt(basket.SelectedIndex);
            }
        }

        private string CheckCongignmentNoteNumber(string str, string notEntered, string invalidSymbols, string allowedLenght, string notAvailable)
        {
            if (str == string.Empty) return notEntered;
            else
            {
                if (str.Length == 9)
                {
                    if (!Regex.IsMatch(str, @"\d{5}\-\d{3}")) return invalidSymbols;
                }
                else return allowedLenght;
            }
            string selectConsumerNameQuery = "SELECT * FROM ConsignmentNote WHERE [consignmentNoteNumber] = '" + str + "'";
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(selectConsumerNameQuery, connectionString))
            {
                DataTable table = new DataTable();
                dataAdapter.Fill(table);
                if (table.Rows.Count > 0)
                {
                    return notAvailable;
                }
            }
            return str;
        }

        private void FillButton_Click(object sender, RoutedEventArgs e)
        {
            if (numberField.Text != CheckCongignmentNoteNumber(numberField.Text, "Consignment note number not entered.", "Consignment note number contains invalid symbols.", "The consignment note number should look like this: #####-###. \nExample: 12345-123.", "This consignment note already exists, please enter another one.")) //checking consignment note number name for correctness
            {
                MessageBox.Show(CheckCongignmentNoteNumber(numberField.Text, "Consignment note number not entered.", "Consignment note number contains invalid symbols.", "The consignment note number should look like this: #####-###. \nExample: 12345-123.", "This consignment note already exists, please enter another one."), "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (consumerField.Text == string.Empty) //checking consumer name for emptiness
            {
                MessageBox.Show("Consumer not selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (basket.Items.Count == 0)
            {
                MessageBox.Show("You haven't added any details.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            double cost = 0;
            double generalSum = 0;

            StreamReader file = new StreamReader("UserLogin.txt");
            string employeeLogin = file.ReadLine();
            file.Close();

            int componentCode = 0;
            int currentCount = 0;
            for (int i = 0; i < basket.Items.Count; i++)
            {
                existingEntry = basket.Items[i].ToString();
                existingEntry = existingEntry.Replace("\t-\t", "*");
                componentsInfo = existingEntry.Split('*');
                SqlCommand command = new SqlCommand("SELECT componentCode, componentCost, componentCount FROM Component JOIN TractorBrand ON Component.tractorBrandCode = TractorBrand.tractorBrandCode WHERE [componentName] = @name " +
                                                    "AND [tractorBrandName] = @tractorName AND [componentWeight] = @weight", connectionString);
                command.Parameters.AddWithValue("@name", componentsInfo[1]);
                command.Parameters.AddWithValue("@tractorName", componentsInfo[0]);
                command.Parameters.AddWithValue("@weight", double.Parse(componentsInfo[2]));
                connectionString.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        cost = double.Parse(reader["componentCost"].ToString());
                        componentCode = Convert.ToInt32(reader["componentCode"]);
                        currentCount = Convert.ToInt32(reader["componentCount"]);
                        connectionString.Close();
                    }
                    generalSum = double.Parse(componentsInfo[3]) * cost;
                    connectionString.Open();
                    SqlCommand cmd = new SqlCommand();
                    if (currentCount == int.Parse(componentsInfo[3]))
                    {
                        cmd = new SqlCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "UPDATE Component SET [componentCount] = [componentCount] - @count, [availabilityStatusCode] = (SELECT availabilityStatusCode FROM AvailabilityStatus WHERE availabilityStatusName = @status) WHERE [componentCode] = @code";
                        cmd.Parameters.Add("@count", SqlDbType.Int).Value = int.Parse(componentsInfo[3]);
                        cmd.Parameters.Add("@code", SqlDbType.Int).Value = componentCode;
                        cmd.Parameters.Add("@status", SqlDbType.VarChar).Value = "Нет в наличии";
                        cmd.Connection = connectionString;
                        cmd.ExecuteNonQuery();
                    }
                    else
                    {
                        cmd = new SqlCommand();
                        cmd.CommandType = CommandType.Text;
                        cmd.CommandText = "UPDATE Component SET [componentCount] = [componentCount] - @count, [availabilityStatusCode] = (SELECT availabilityStatusCode FROM AvailabilityStatus WHERE availabilityStatusName = @status) WHERE [componentCode] = @code";
                        cmd.Parameters.Add("@count", SqlDbType.Int).Value = int.Parse(componentsInfo[3]);
                        cmd.Parameters.Add("@code", SqlDbType.Int).Value = componentCode;
                        cmd.Parameters.Add("@status", SqlDbType.VarChar).Value = "Есть в наличии";
                        cmd.Connection = connectionString;
                        cmd.ExecuteNonQuery();
                    }
                    cmd = new SqlCommand();
                    cmd.CommandType = CommandType.Text;
                    cmd.CommandText = "INSERT ConsignmentNote (consignmentNoteNumber, componentCount, issueDate, generalSum, componentCode, consumerCode, employeeCode) VALUES (@number, @count, @date, @sum, @componentCode, (SELECT consumerCode FROM Consumer WHERE consumerName = @consumerName), (SELECT employeeCode FROM Employee WHERE employeeLogin = @login))";
                    cmd.Parameters.Add("@number", SqlDbType.VarChar).Value = numberField.Text;
                    cmd.Parameters.Add("@count", SqlDbType.Int).Value = int.Parse(componentsInfo[3]);
                    cmd.Parameters.Add("@date", SqlDbType.DateTime).Value = DateTime.Now.Date;
                    cmd.Parameters.Add("@sum", SqlDbType.Float).Value = generalSum;
                    cmd.Parameters.Add("@componentCode", SqlDbType.Int).Value = componentCode;
                    cmd.Parameters.Add("@consumerName", SqlDbType.VarChar).Value = consumerField.Text;
                    cmd.Parameters.Add("@login", SqlDbType.VarChar).Value = employeeLogin;
                    cmd.Connection = connectionString;
                    cmd.ExecuteNonQuery();
                    connectionString.Close();
                }
                connectionString.Close();
            }
            MessageBox.Show("Consignment note has been successfully generated.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
            consumerField.SelectedIndex = -1;
            numberField.Clear();
            basket.Items.Clear();
        }
    }
}
