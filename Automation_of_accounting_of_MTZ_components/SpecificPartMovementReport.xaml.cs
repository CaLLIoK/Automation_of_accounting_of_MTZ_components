using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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
    /// Логика взаимодействия для SpecificPartMovementReport.xaml
    /// </summary>
    public partial class SpecificPartMovementReport : Window
    {
        SqlConnection connectionString = new SqlConnection(@"Data Source=(local)\SQLEXPRESS; Initial Catalog=Automation_of_accounting_of_MTZ_components; Integrated Security=True");

        public SpecificPartMovementReport()
        {
            InitializeComponent();

            connectionString.Open();
            FillDataGrid();
            connectionString.Close();
        }

        private void FillDataGrid()
        {
            string componentsInfoQuery = "SELECT tractorBrandName, componentName, componentWeight " +
                                         "FROM Component " +
                                         "JOIN TractorBrand ON Component.tractorBrandCode = TractorBrand.tractorBrandCode ";

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
            CreateReportWindow createReportWindow = new CreateReportWindow();
            createReportWindow.Show();
            this.Close();
        }

        [DllImport("user32")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId0);

        private void CreateReportButton_Click(object sender, RoutedEventArgs e)
        {
            if (ComponentsInfoGrid.SelectedItem == null)
            {
                MessageBox.Show("You haven't selected a component on the basis of which the report will be generated.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            DataRowView componentInfo = (DataRowView)ComponentsInfoGrid.SelectedItems[0];

            Microsoft.Office.Interop.Excel.Workbooks excelappworkbooks;
            Microsoft.Office.Interop.Excel.Workbook excelappworkbook;
            Microsoft.Office.Interop.Excel.Sheets excelsheets;
            Microsoft.Office.Interop.Excel.Worksheet excelworksheet;
            Microsoft.Office.Interop.Excel.Range excelcells;
            string path = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            Microsoft.Office.Interop.Excel.Application excelapp = new Microsoft.Office.Interop.Excel.Application();
            excelapp.Interactive = false;
            uint processId;
            GetWindowThreadProcessId((IntPtr)excelapp.Hwnd, out processId);
            int i = 12;
            try
            {
                excelappworkbooks = excelapp.Workbooks;
                excelappworkbook = excelapp.Workbooks.Open(System.IO.Path.GetFullPath(@"SpecialProductMovementReportTemplate.xls"));
                excelsheets = excelappworkbook.Worksheets;
                excelworksheet = (Microsoft.Office.Interop.Excel.Worksheet)excelsheets.get_Item(1);
                Random rnd = new Random();
                excelcells = excelworksheet.get_Range("D3");
                excelcells.Value = rnd.Next(10000);
                excelcells = excelworksheet.get_Range("B6");
                excelcells.Value = "(" + componentInfo["tractorBrandName"].ToString() + ") " + componentInfo["componentName"].ToString();
                excelcells = excelworksheet.get_Range("B7");
                excelcells.Value = "единица детали с весом " + componentInfo["componentWeight"].ToString() + " кг";
                excelcells = excelworksheet.get_Range("F5");
                excelcells.Value = DateTime.Now.ToShortDateString();

                SqlCommand command = new SqlCommand("SELECT componentName, arrivalDate, componentCost, componentsCount FROM Arrival " +
                                                    "JOIN Component ON Arrival.componentCode = Component.componentCode " +
                                                    "JOIN TractorBrand ON Component.tractorBrandCode = TractorBrand.tractorBrandCode " +
                                                    "WHERE componentName = @name AND tractorBrandName = @tractorName AND componentWeight = @weight", connectionString);
                command.Parameters.AddWithValue("@name", componentInfo["componentName"].ToString());
                command.Parameters.AddWithValue("@tractorName", componentInfo["tractorBrandName"].ToString());
                command.Parameters.AddWithValue("@weight", double.Parse(componentInfo["componentWeight"].ToString()));
                connectionString.Open();
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        int k = 0;
                        while (reader.Read())
                        {
                            for (int j = 1; j < 6; j++)
                            {
                                excelcells = (Microsoft.Office.Interop.Excel.Range)excelworksheet.Cells[i, j];
                                excelcells.Borders.ColorIndex = 0;
                                excelcells.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlDouble;
                                if (j == 2)
                                {
                                    excelcells.Value = DateTime.Parse(reader[k].ToString()).ToShortDateString();
                                    ++k;
                                }
                                else if (j == 5)
                                {
                                    excelcells.Value = double.Parse(reader["componentCost"].ToString()) * int.Parse(reader["componentsCount"].ToString());
                                }
                                else
                                {
                                    excelcells.Value = reader[k].ToString();
                                    ++k;
                                }
                            }
                            ++i;
                            k = 0;
                        }

                    }
                    else
                    {
                        MessageBox.Show("Nothing was found for the specified date range.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                connectionString.Close();
                excelworksheet = (Microsoft.Office.Interop.Excel.Worksheet)excelsheets.get_Item(2);
                excelcells = excelworksheet.get_Range("D2");
                excelcells.Value = rnd.Next(10000);
                excelcells = excelworksheet.get_Range("B5");
                excelcells.Value = "(" + componentInfo["tractorBrandName"].ToString() + ") " + componentInfo["componentName"].ToString();
                excelcells = excelworksheet.get_Range("B6");
                excelcells.Value = "единица детали с весом " + componentInfo["componentWeight"].ToString() + " кг";
                excelcells = excelworksheet.get_Range("F4");
                excelcells.Value = DateTime.Now.ToShortDateString();

                SqlCommand command1 = new SqlCommand("SELECT componentName, issueDate, componentCost, componntCount, generalSum, componentCount FROM ConsignmentNote " +
                                                    "JOIN Component ON ConsignmentNote.componentCode = Component.componentCode " +
                                                    "JOIN TractorBrand ON Component.tractorBrandCode = TractorBrand.tractorBrandCode " +
                                                    "WHERE componentName = @name AND tractorBrandName = @tractorName AND componentWeight = @weight", connectionString);
                command1.Parameters.AddWithValue("@name", componentInfo["componentName"].ToString());
                command1.Parameters.AddWithValue("@tractorName", componentInfo["tractorBrandName"].ToString());
                command1.Parameters.AddWithValue("@weight", double.Parse(componentInfo["componentWeight"].ToString()));
                connectionString.Open();
                using (SqlDataReader reader = command1.ExecuteReader())
                {
                    if (reader.HasRows)
                    {
                        i = 11;
                        int k = 0;
                        while (reader.Read())
                        {
                            for (int j = 1; j < 8; j++)
                            {
                                excelcells = (Microsoft.Office.Interop.Excel.Range)excelworksheet.Cells[i, j];
                                excelcells.Borders.ColorIndex = 0;
                                excelcells.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlDouble;
                                if (j == 2)
                                {
                                    excelcells.Value = DateTime.Parse(reader[k].ToString()).ToShortDateString();
                                    ++k;
                                }
                                else if (j == 7)
                                {
                                    excelcells.Value = double.Parse(reader["componentCost"].ToString()) * int.Parse(reader["componentCount"].ToString());
                                }
                                else
                                {
                                    excelcells.Value = reader[k].ToString();
                                    ++k;
                                }
                            }
                            ++i;
                            k = 0;
                        }

                    }
                    else
                    {
                        MessageBox.Show("Nothing was found for the specified date range.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                    }
                }
                connectionString.Close();
                path += @"\SpecificComponentMovementReport.xls";
                excelappworkbooks = excelapp.Workbooks;
                excelappworkbook = excelappworkbooks[1];
                excelappworkbook.SaveAs(path);
                MessageBox.Show("The report has been successfully created.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                connectionString.Close();
            }
            catch (Exception q)
            {
                MessageBox.Show(q.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                connectionString.Close();
            }
            finally
            {
                Process.GetProcessById((int)processId).Kill();
            }
        }
    }
}
