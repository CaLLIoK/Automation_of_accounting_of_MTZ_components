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
            this.Close();
        }

        private void DeleteButton_Click(object sender, RoutedEventArgs e)
        {
            if (ComponentsInfoGrid.SelectedItem == null)
            {
                MessageBox.Show("Can't delete the blank entry.");
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
                cmd.Parameters.Add("@componentWeight", SqlDbType.Float).Value = Convert.ToDouble(componentInfo["componentWeight"].ToString());
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
