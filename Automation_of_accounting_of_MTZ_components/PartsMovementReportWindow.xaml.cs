using System;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

namespace Automation_of_accounting_of_MTZ_components
{
    /// <summary>
    /// Логика взаимодействия для PartsMovementReportWindow.xaml
    /// </summary>
    public partial class PartsMovementReportWindow : Window
    {
        string connectionString = @"Data Source=(local)\SQLEXPRESS; Initial Catalog=Automation_of_accounting_of_MTZ_components; Integrated Security=True";

        public PartsMovementReportWindow()
        {
            InitializeComponent();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        [DllImport("user32")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId0);

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CreateReportWindow createReportWindow = new CreateReportWindow();
            createReportWindow.Show();
            this.Close();
        }

        private void CreateReportButton_Click(object sender, RoutedEventArgs e)
        {
            if (Arrival.IsChecked == false && ArrivalAndSelling.IsChecked == false)
            {
                MessageBox.Show("You haven't selected the type of parts movement.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!FirstDate.SelectedDate.HasValue)
            {
                MessageBox.Show("You haven't specified the start of the period.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!LastDate.SelectedDate.HasValue)
            {
                MessageBox.Show("You haven't specified the end of the period.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (FirstDate.SelectedDate.Value.Date > LastDate.SelectedDate.Value.Date)
            {
                MessageBox.Show("The beginning of the period can't be greater than the end of the period.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (LastDate.SelectedDate.Value.Date > DateTime.Now.Date)
            {
                MessageBox.Show("You can't specify a date not yet due.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
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
            int i = 14;
            try
            {
                if (Arrival.IsChecked == true)
                {
                    excelappworkbooks = excelapp.Workbooks;
                    excelappworkbook = excelapp.Workbooks.Open(System.IO.Path.GetFullPath(@"ProductArrivalReportTemplate.xls"));
                    excelsheets = excelappworkbook.Worksheets;
                    excelworksheet = (Microsoft.Office.Interop.Excel.Worksheet)excelsheets.get_Item(1);
                    Random rnd = new Random();
                    excelcells = excelworksheet.get_Range("C3");
                    excelcells.Value = rnd.Next(10000);
                    excelcells = excelworksheet.get_Range("B6");
                    excelcells.Value = FirstDate.SelectedDate.Value.Date;
                    excelcells = excelworksheet.get_Range("D6");
                    excelcells.Value = LastDate.SelectedDate.Value.Date;
                    excelcells = excelworksheet.get_Range("B8");
                    excelcells.Value = "Минский тракторный завод (МТЗ)";
                    excelcells = excelworksheet.get_Range("B9");
                    excelcells.Value = "Минский тракторный завод (МТЗ)";
                    excelcells = excelworksheet.get_Range("F7");
                    excelcells.Value = DateTime.Now.ToShortDateString();
                    string infoQuery = "SELECT componentName, componentCost, componentsCount FROM Arrival JOIN Component ON Arrival.componentCode = Component.componentCode " +
                                       "WHERE arrivalDate BETWEEN '" + FirstDate.SelectedDate.Value.Date + "' AND '" + LastDate.SelectedDate.Value.Date + "'";
                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(infoQuery, connectionString))
                    {
                        int k = 0;
                        string ed = "шт.";
                        DataTable table = new DataTable();
                        dataAdapter.Fill(table);
                        if (table.Rows.Count > 0)
                        {
                            for (int rows = 0; rows < table.Rows.Count; ++rows)
                            {
                                for (int j = 1; j < 6; j++)
                                {
                                    excelcells = (Microsoft.Office.Interop.Excel.Range)excelworksheet.Cells[i, j];
                                    excelcells.Borders.ColorIndex = 0;
                                    excelcells.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlDouble;
                                    if (j == 2)
                                    {
                                        excelcells.Value2 = ed.ToString();
                                    }
                                    else if (j == 5)
                                    {
                                        excelcells.Value = double.Parse(table.Rows[rows]["componentCost"].ToString()) * int.Parse(table.Rows[rows]["componentsCount"].ToString());
                                    }
                                    else
                                    {
                                        excelcells.Value = table.Rows[rows][k].ToString();
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
                    path += @"\ArrivalReport.xls";
                    excelappworkbooks = excelapp.Workbooks;
                    excelappworkbook = excelappworkbooks[1];
                    excelappworkbook.SaveAs(path);
                    MessageBox.Show("The report has been successfully created.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                    Arrival.IsChecked = false;
                    ArrivalAndSelling.IsChecked = false;
                    FirstDate.SelectedDates.Clear();
                    LastDate.SelectedDates.Clear();
                }
                //else if (ArrivalAndSelling.IsChecked == true)
                //{
                //    excelappworkbooks = excelapp.Workbooks;
                //    excelappworkbook = excelapp.Workbooks.Open(System.IO.Path.GetFullPath(@"ProductMovementReportTemplate.xls"));
                //    excelsheets = excelappworkbook.Worksheets;
                //    excelworksheet = (Microsoft.Office.Interop.Excel.Worksheet)excelsheets.get_Item(1);
                //    Random rnd = new Random();
                //    excelcells = excelworksheet.get_Range("F3");
                //    excelcells.Value = rnd.Next(10000);
                //    excelcells = excelworksheet.get_Range("E6");
                //    excelcells.Value = FirstDate.SelectedDate.Value.Date;
                //    excelcells = excelworksheet.get_Range("G6");
                //    excelcells.Value = LastDate.SelectedDate.Value.Date;
                //    excelcells = excelworksheet.get_Range("B8");
                //    excelcells.Value = "Минский тракторный завод (МТЗ)";
                //    excelcells = excelworksheet.get_Range("B9");
                //    excelcells.Value = "Минский тракторный завод (МТЗ)";
                //    excelcells = excelworksheet.get_Range("I7");
                //    excelcells.Value = DateTime.Now.ToShortDateString();
                //    string infoQuery = "SELECT componentName, componentCost, componentsCount, componntCount, generalSum, componentCount FROM Component " +
                //                       "JOIN Arrival ON Component.componentCode = Arrival.componentCode " +
                //                       "JOIN ConsignmentNote ON Component.componentCode = ConsignmentNote.componentCode " +
                //                       "WHERE arrivalDate BETWEEN '" + FirstDate.SelectedDate.Value.Date + "' AND '" + LastDate.SelectedDate.Value.Date + "' AND issueDate BETWEEN '" + FirstDate.SelectedDate.Value.Date + "' AND '" + LastDate.SelectedDate.Value.Date + "'";
                //    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(infoQuery, connectionString))
                //    {
                //        int k = 0;
                //        string ed = "шт.";
                //        DataTable table = new DataTable();
                //        dataAdapter.Fill(table);
                //        if (table.Rows.Count > 0)
                //        {
                //            for (int rows = 0; rows < table.Rows.Count; ++rows)
                //            {
                //                for (int j = 1; j < 10; j++)
                //                {
                //                    excelcells = (Microsoft.Office.Interop.Excel.Range)excelworksheet.Cells[i, j];
                //                    excelcells.Borders.ColorIndex = 0;
                //                    excelcells.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlDouble;
                //                    if (j == 2)
                //                    {
                //                        excelcells.Value2 = ed.ToString();
                //                    }
                //                    else if (j == 5)
                //                    {
                //                        excelcells.Value = double.Parse(table.Rows[rows]["componentCost"].ToString()) * int.Parse(table.Rows[rows]["componentsCount"].ToString());
                //                    }
                //                    else if (j == 9)
                //                    {
                //                        excelcells.Value = double.Parse(table.Rows[rows]["componentCost"].ToString()) * int.Parse(table.Rows[rows]["componentCount"].ToString());
                //                    }
                //                    else
                //                    {
                //                        excelcells.Value = table.Rows[rows][k].ToString();
                //                        ++k;
                //                    }
                //                }
                //                ++i;
                //                k = 0;
                //            }
                //        }
                //        else
                //        {
                //            MessageBox.Show("Nothing was found for the specified date range.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
                //        }
                //    }
                //    path += @"\MovementReport.xls";
                //    excelappworkbooks = excelapp.Workbooks;
                //    excelappworkbook = excelappworkbooks[1];
                //    excelappworkbook.SaveAs(path);
                //    MessageBox.Show("The report has been successfully created.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                //    Arrival.IsChecked = false;
                //    ArrivalAndSelling.IsChecked = false;
                //    FirstDate.SelectedDates.Clear();
                //    LastDate.SelectedDates.Clear();
                //}
                else if (ArrivalAndSelling.IsChecked == true)
                {
                    excelappworkbooks = excelapp.Workbooks;
                    excelappworkbook = excelapp.Workbooks.Open(System.IO.Path.GetFullPath(@"ProductMovementReportTemplate1.xls"));
                    excelsheets = excelappworkbook.Worksheets;
                    excelworksheet = (Microsoft.Office.Interop.Excel.Worksheet)excelsheets.get_Item(1);
                    Random rnd = new Random();
                    excelcells = excelworksheet.get_Range("C3");
                    excelcells.Value = rnd.Next(10000);
                    excelcells = excelworksheet.get_Range("B6");
                    excelcells.Value = FirstDate.SelectedDate.Value.Date;
                    excelcells = excelworksheet.get_Range("D6");
                    excelcells.Value = LastDate.SelectedDate.Value.Date;
                    excelcells = excelworksheet.get_Range("B8");
                    excelcells.Value = "Минский тракторный завод (МТЗ)";
                    excelcells = excelworksheet.get_Range("B9");
                    excelcells.Value = "Минский тракторный завод (МТЗ)";
                    excelcells = excelworksheet.get_Range("F7");
                    excelcells.Value = DateTime.Now.ToShortDateString();
                    string infoQuery = "SELECT componentName, componentCost, componentsCount FROM Arrival " +
                                       "JOIN Component ON Arrival.componentCode = Component.componentCode " +
                                       "WHERE arrivalDate BETWEEN '" + FirstDate.SelectedDate.Value.Date + "' AND '" + LastDate.SelectedDate.Value.Date + "'";
                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(infoQuery, connectionString))
                    {
                        int k = 0;
                        string ed = "шт.";
                        DataTable table = new DataTable();
                        dataAdapter.Fill(table);
                        if (table.Rows.Count > 0)
                        {
                            for (int rows = 0; rows < table.Rows.Count; ++rows)
                            {
                                for (int j = 1; j < 6; j++)
                                {
                                    excelcells = (Microsoft.Office.Interop.Excel.Range)excelworksheet.Cells[i, j];
                                    excelcells.Borders.ColorIndex = 0;
                                    excelcells.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlDouble;
                                    if (j == 2)
                                    {
                                        excelcells.Value2 = ed.ToString();
                                    }
                                    else if (j == 5)
                                    {
                                        excelcells.Value = double.Parse(table.Rows[rows]["componentCost"].ToString()) * int.Parse(table.Rows[rows]["componentsCount"].ToString());
                                    }
                                    else
                                    {
                                        excelcells.Value = table.Rows[rows][k].ToString();
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
                    excelworksheet = (Microsoft.Office.Interop.Excel.Worksheet)excelsheets.get_Item(2);
                    excelcells = excelworksheet.get_Range("D3");
                    excelcells.Value = rnd.Next(10000);
                    excelcells = excelworksheet.get_Range("C6");
                    excelcells.Value = FirstDate.SelectedDate.Value.Date;
                    excelcells = excelworksheet.get_Range("E6");
                    excelcells.Value = LastDate.SelectedDate.Value.Date;
                    excelcells = excelworksheet.get_Range("B8");
                    excelcells.Value = "Минский тракторный завод (МТЗ)";
                    excelcells = excelworksheet.get_Range("B9");
                    excelcells.Value = "Минский тракторный завод (МТЗ)";
                    excelcells = excelworksheet.get_Range("G7");
                    excelcells.Value = DateTime.Now.ToShortDateString();
                    string info2Query = "SELECT componentName, componentCost, componntCount, generalSum, componentCount FROM ConsignmentNote " +
                                       "JOIN Component ON ConsignmentNote.componentCode = Component.componentCode " +
                                       "WHERE issueDate BETWEEN '" + FirstDate.SelectedDate.Value.Date + "' AND '" + LastDate.SelectedDate.Value.Date + "'";
                    i = 14;
                    using (SqlDataAdapter dataAdapter = new SqlDataAdapter(info2Query, connectionString))
                    {
                        int k = 0;
                        string ed = "шт.";
                        DataTable table = new DataTable();
                        dataAdapter.Fill(table);
                        if (table.Rows.Count > 0)
                        {
                            for (int rows = 0; rows < table.Rows.Count; ++rows)
                            {
                                for (int j = 1; j < 8; j++)
                                {
                                    excelcells = (Microsoft.Office.Interop.Excel.Range)excelworksheet.Cells[i, j];
                                    excelcells.Borders.ColorIndex = 0;
                                    excelcells.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlDouble;
                                    if (j == 2)
                                    {
                                        excelcells.Value2 = ed.ToString();
                                    }
                                    else if (j == 7)
                                    {
                                        excelcells.Value = double.Parse(table.Rows[rows]["componentCost"].ToString()) * int.Parse(table.Rows[rows]["componentCount"].ToString());
                                    }
                                    else
                                    {
                                        excelcells.Value = table.Rows[rows][k].ToString();
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
                    path += @"\MovementReport.xls";
                    excelappworkbooks = excelapp.Workbooks;
                    excelappworkbook = excelappworkbooks[1];
                    excelappworkbook.SaveAs(path);
                    MessageBox.Show("The report has been successfully created.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                    Arrival.IsChecked = false;
                    ArrivalAndSelling.IsChecked = false;
                    FirstDate.SelectedDates.Clear();
                    LastDate.SelectedDates.Clear();
                }
            }
            catch (Exception q)
            {
                MessageBox.Show(q.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                Process.GetProcessById((int)processId).Kill();
            }
        }

        private void Arrival_Checked(object sender, RoutedEventArgs e)
        {
            if (Arrival.IsChecked == true)
            {
                ArrivalAndSelling.IsChecked = false;
            }
        }

        private void ArrivalAndSelling_Checked(object sender, RoutedEventArgs e)
        {
            if (ArrivalAndSelling.IsChecked == true)
            {
                Arrival.IsChecked = false;
            }
        }
    }
}
