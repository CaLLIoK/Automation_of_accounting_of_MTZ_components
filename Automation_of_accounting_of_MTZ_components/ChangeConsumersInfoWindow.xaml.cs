using System.Windows;
using System.Windows.Input;
using System.Data.SqlClient;
using System.Data;
using System.IO;

namespace Automation_of_accounting_of_MTZ_components
{
    /// <summary>
    /// Логика взаимодействия для ChangeConsumersInfoWindow.xaml
    /// </summary>
    public partial class ChangeConsumersInfoWindow : Window
    {
        SqlConnection connectionString = new SqlConnection(@"Data Source=(local)\SQLEXPRESS; Initial Catalog=Automation_of_accounting_of_MTZ_components; Integrated Security=True");

        public ChangeConsumersInfoWindow()
        {
            InitializeComponent();

            connectionString.Open();
            FillDataGrid();
            connectionString.Close();
        }

        private void FillDataGrid()
        {
            string componentsInfoQuery = "SELECT consumerName, consumerLegalAdress, consumerPhone FROM Consumer";

            DataTable table = new DataTable();
            using (SqlCommand cmd = new SqlCommand(componentsInfoQuery, connectionString))
            {
                using (IDataReader rdr = cmd.ExecuteReader())
                {
                    table.Load(rdr);
                }
            }
            ConsumersInfoGrid.ItemsSource = table.DefaultView;
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
            if (ConsumersInfoGrid.SelectedItem == null) //check for string selection
            {
                MessageBox.Show("Can't delete the blank entry.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                DataRowView consumerInfo = (DataRowView)ConsumersInfoGrid.SelectedItems[0]; //creating a variable with the data of the selected string
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "DELETE FROM Consumer WHERE consumerName = @consumerName AND consumerPhone = @consumerPhone"; //data deletion request
                cmd.Parameters.Add("@consumerName", SqlDbType.VarChar).Value = consumerInfo["consumerName"].ToString();
                cmd.Parameters.Add("@consumerPhone", SqlDbType.VarChar).Value = consumerInfo["consumerPhone"].ToString();
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
            if (ConsumersInfoGrid.SelectedItem == null) //check for string selection
            {
                MessageBox.Show("Can't change the blank entry.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                DataRowView consumerInfo = (DataRowView)ConsumersInfoGrid.SelectedItems[0]; //creating a variable with the data of the selected string
                AddConsumersWindow addConsumersWindow = new AddConsumersWindow();
                string selectConsumerInfoQuery = "SELECT consumerCode FROM Consumer WHERE [consumerName] = '" + consumerInfo["consumerName"].ToString() + "' AND [consumerLegalAdress] = '" + consumerInfo["consumerLegalAdress"].ToString() + "' AND [consumerPhone] = '" + consumerInfo["consumerPhone"].ToString() + "'";
                using (SqlDataAdapter dataAdapter = new SqlDataAdapter(selectConsumerInfoQuery, connectionString))
                {
                    DataTable table = new DataTable();
                    dataAdapter.Fill(table);
                    if (table.Rows.Count > 0)
                    {
                        StreamWriter consumerCode = new StreamWriter("ConsCode.txt");
                        consumerCode.Write(table.Rows[0]["consumerCode"].ToString());
                        consumerCode.Close();
                    }
                }
                addConsumersWindow.Title.Content = "Change consumer information";
                addConsumersWindow.Description.Content = "Change the fields that you need";
                addConsumersWindow.nameField.Text = consumerInfo["consumerName"].ToString();
                addConsumersWindow.phoneField.Text = consumerInfo["consumerPhone"].ToString();
                string adress = consumerInfo["consumerLegalAdress"].ToString();
                adress = adress.Replace("ул. ", "");
                adress = adress.Replace(", д. ", "*");
                adress = adress.Replace(", офис ", "*");
                string[] newAdress = adress.Split('*');
                string street = newAdress[0];
                string building = newAdress[1];
                string office = newAdress[2];
                addConsumersWindow.streetField.Text = street;
                addConsumersWindow.buildingFiled.Text = building;
                addConsumersWindow.officeField.Text = office;
                addConsumersWindow.AddButton.Visibility = Visibility.Hidden;
                addConsumersWindow.SaveButton.Visibility = Visibility.Visible;
                addConsumersWindow.Show();
                this.Close();
            }
        }
    }
}
