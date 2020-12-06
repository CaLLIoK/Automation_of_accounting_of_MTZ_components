using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Automation_of_accounting_of_MTZ_components
{
    /// <summary>
    /// Логика взаимодействия для ChangeComponentsInfoWindow.xaml
    /// </summary>
    public partial class ChangeComponentsInfoWindow : Window
    {
        SqlConnection connectionString = new SqlConnection(@"Data Source=(local)\SQLEXPRESS; Initial Catalog=Automation_of_accounting_of_MTZ_components; Integrated Security=True");

        public ChangeComponentsInfoWindow()
        {
            InitializeComponent();

            connectionString.Open();
            FillDataGrid();
            connectionString.Close();
        }

        private void FillDataGrid()
        {
            string componentsInfoQuery = "SELECT tractorBrandName, componentName, componentDescription, componentWeight, componentCount, componentCost, availabilityStatusName " +
                                         "FROM Component " +
                                         "JOIN TractorBrand ON Component.tractorBrandCode = TractorBrand.tractorBrandCode " +
                                         "JOIN AvailabilityStatus ON Component.availabilityStatusCode = AvailabilityStatus.availabilityStatusCode";

            DataTable table = new DataTable();
            using (SqlCommand cmd = new SqlCommand(componentsInfoQuery, connectionString))
            {
                using (IDataReader rdr = cmd.ExecuteReader())
                {
                    table.Load(rdr);
                }
            }
            ComponentsInfoGrid.ItemsSource = table.DefaultView;
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
            if (ComponentsInfoGrid.SelectedItem == null)
            {
                MessageBox.Show("Can't delete the blank entry.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                DataRowView componentInfo = (DataRowView)ComponentsInfoGrid.SelectedItems[0];
                SqlCommand cmd = new SqlCommand();
                cmd.CommandType = CommandType.Text;
                cmd.CommandText = "DELETE FROM Component WHERE [tractorBrandCode] = (SELECT tractorBrandCode FROM TractorBrand WHERE tractorBrandName = @tractorBrandName) AND [componentName] = @componentName AND [componentWeight] = @componentWeight";
                cmd.Parameters.Add("@tractorBrandName", SqlDbType.VarChar).Value = componentInfo["tractorBrandName"].ToString();
                cmd.Parameters.Add("@componentName", SqlDbType.VarChar).Value = componentInfo["componentName"].ToString();
                cmd.Parameters.Add("@componentWeight", SqlDbType.Float).Value = double.Parse(componentInfo["componentWeight"].ToString());
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
            if (ComponentsInfoGrid.SelectedItem == null)
            {
                MessageBox.Show("Can't change the blank entry.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                DataRowView componentInfo = (DataRowView)ComponentsInfoGrid.SelectedItems[0];
                SqlCommand command = new SqlCommand("SELECT componentCode FROM Component JOIN TractorBrand ON Component.tractorBrandCode = TractorBrand.tractorBrandCode WHERE [componentName] = @name " +
                                                "AND [tractorBrandName] = @tractorName AND [componentWeight] = @weight", connectionString);
                command.Parameters.AddWithValue("@name", componentInfo["componentName"].ToString());
                command.Parameters.AddWithValue("@tractorName", componentInfo["tractorBrandName"].ToString());
                command.Parameters.AddWithValue("@weight", double.Parse(componentInfo["componentWeight"].ToString()));
                connectionString.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.Read())
                    {
                        StreamWriter componentCode = new StreamWriter("ComponentCode.txt");
                        componentCode.Write(reader["componentCode"]);
                        componentCode.Close(); 
                        connectionString.Close();
                    }
                }
                AddComponentsWindow addComponentsWindow = new AddComponentsWindow();
                addComponentsWindow.componentNameField.Text = componentInfo["componentName"].ToString();
                addComponentsWindow.weightField.Text = componentInfo["componentWeight"].ToString();
                addComponentsWindow.countField.Text = componentInfo["componentCount"].ToString();
                addComponentsWindow.costField.Text = componentInfo["componentCost"].ToString();
                addComponentsWindow.tractorField.Text = componentInfo["tractorBrandName"].ToString();
                addComponentsWindow.descriptionField.AppendText(componentInfo["componentDescription"].ToString());
                addComponentsWindow.Title.Content = "Change component info";
                addComponentsWindow.Description.Content = "Change the field that you need";
                addComponentsWindow.AddButton.Visibility = Visibility.Hidden;
                addComponentsWindow.SaveButton.Visibility = Visibility.Visible;
                addComponentsWindow.Show();
                this.Close();
            }
        }
    }
}
