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
    /// Логика взаимодействия для ComponentsInfo.xaml
    /// </summary>
    public partial class ComponentsInfo : Window
    {
        public ComponentsInfo()
        {
            InitializeComponent();

            string connectionString = @"Data Source=(local)\SQLEXPRESS; Initial Catalog=Automation_of_accounting_of_MTZ_components; Integrated Security=True";

            SqlConnection connection = new SqlConnection(connectionString);

            string componentsInfoQuery = "SELECT tractorBrandName, componentName, componentDescription, componentWeight, componentCount, componentCost, availabilityStatusName " +
                                         "FROM Component " +
                                         "JOIN TractorBrand ON Component.tractorBrandCode = TractorBrand.tractorBrandCode " +
                                         "JOIN AvailabilityStatus ON Component.availabilityStatusCode = AvailabilityStatus.availabilityStatusCode";

            connection.Open();
            DataTable table = new DataTable();
            using (SqlCommand cmd = new SqlCommand(componentsInfoQuery, connection))
            {
                using (IDataReader rdr = cmd.ExecuteReader())
                {
                    table.Load(rdr);
                }
            }
            ComponentsGrid.ItemsSource = table.DefaultView;
            connection.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
