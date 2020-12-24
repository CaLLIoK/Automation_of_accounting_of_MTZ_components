using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Input;

namespace Automation_of_accounting_of_MTZ_components
{
    /// <summary>
    /// Логика взаимодействия для ConsignmentNoteReportWindow.xaml
    /// </summary>
    public partial class ConsignmentNoteReportWindow : Window
    {
        SqlConnection connectionString = new SqlConnection(@"Data Source=(local)\SQLEXPRESS; Initial Catalog=Automation_of_accounting_of_MTZ_components; Integrated Security=True");

        public ConsignmentNoteReportWindow()
        {
            InitializeComponent();

            connectionString.Open();

            List<string> consignmentNumbers = new List<string>();
            string query = @"SELECT DISTINCT consignmentNoteNumber FROM ConsignmentNote";
            SqlCommand sqlCommand = new SqlCommand(query, connectionString);
            SqlDataReader dataReader = sqlCommand.ExecuteReader();
            if (dataReader.HasRows)
            {
                while (dataReader.Read())
                {
                    consignmentNumbers.Add(dataReader["consignmentNoteNumber"].ToString());
                    var newList = from i in consignmentNumbers orderby i select i;
                    consignmentNoteNumbersField.ItemsSource = newList;
                }
            }
            dataReader.Close();
            connectionString.Close();
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            CreateReportWindow createReportWindow = new CreateReportWindow();
            createReportWindow.Show();
            this.Close();
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        [DllImport("user32")]
        static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId0);

        private void CreateReportButton_Click(object sender, RoutedEventArgs e)
        {
            if (consignmentNoteNumbersField.SelectedIndex == -1)
            {
                MessageBox.Show("Consignment note number not selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
            int i = 15;
            try
            {
                string adress = "Минский тракторный завод (МТЗ), Республика Беларусь, г. Минск ул. Долгобродская, д. 29, тел. +375(17)246-60-09";
                excelappworkbooks = excelapp.Workbooks;
                excelappworkbook = excelapp.Workbooks.Open(System.IO.Path.GetFullPath(@"ConsignmentNoteReport.xls"));
                excelsheets = excelappworkbook.Worksheets;
                excelworksheet = (Microsoft.Office.Interop.Excel.Worksheet)excelsheets.get_Item(1);
                excelcells = excelworksheet.get_Range("E3");
                excelcells.Value = consignmentNoteNumbersField.Text;
                excelcells = excelworksheet.get_Range("B6");
                excelcells.Value = DateTime.Now.ToShortDateString();
                excelcells = excelworksheet.get_Range("B9");
                excelcells.Value = adress;
                string infoQuery = "SELECT consumerName, consumerLegalAdress, consumerPhone, componentName, componntCount, componentWeight, componentCost, generalSum, employeeSurname, employeeName, employeePatronymic FROM ConsignmentNote " +
                                   "JOIN Component ON ConsignmentNote.componentCode = Component.componentCode " +
                                   "JOIN Employee ON ConsignmentNote.employeeCode = Employee.employeeCode " +
                                   "JOIN Consumer ON ConsignmentNote.consumerCode = Consumer.consumerCode " +
                                   "WHERE consignmentNoteNumber = '" + consignmentNoteNumbersField.Text + "'";
                using (SqlDataAdapter dataAdapter = new SqlDataAdapter(infoQuery, connectionString))
                {
                    int k = 3;
                    string ed = "шт.";
                    DataTable table = new DataTable();
                    dataAdapter.Fill(table);
                    if (table.Rows.Count > 0)
                    {
                        double generalSum = 0;
                        for (int rows = 0; rows < table.Rows.Count; ++rows)
                        {
                            for (int j = 2; j < 8; j++)
                            {
                                excelcells = (Microsoft.Office.Interop.Excel.Range)excelworksheet.Cells[i, j];
                                excelcells.Borders.ColorIndex = 0;
                                excelcells.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                                if (j == 3)
                                {
                                    excelcells.Value2 = ed.ToString();
                                }
                                else if (j == 7)
                                {
                                    excelcells.Value = table.Rows[rows][k].ToString();
                                    generalSum += double.Parse(table.Rows[rows][k].ToString());
                                    ++k;
                                }
                                else
                                {
                                    excelcells.Value = table.Rows[rows][k].ToString();
                                    ++k;
                                }
                            }
                            ++i;
                            k = 3;
                        }
                        adress = table.Rows[0]["consumerName"].ToString() + ", Республика Беларусь, г. Минск " + table.Rows[0]["consumerLegalAdress"].ToString() + ", тел. " + table.Rows[0]["consumerPhone"].ToString();
                        excelcells = excelworksheet.get_Range("B8");
                        excelcells.Value = adress;

                        for (int j = 2; j < 8; j++)
                        {
                            excelcells = (Microsoft.Office.Interop.Excel.Range)excelworksheet.Cells[i, j];
                            excelcells.Borders.ColorIndex = 0;
                            excelcells.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                            if (j == 2)
                            {
                                excelcells.HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlLeft;
                                excelcells.Value2 = "   ИТОГО";
                            }
                            else if (j == 3)
                            {
                                excelcells.Value2 = "х";
                            }
                            else if (j == 6)
                            {
                                excelcells.Value2 = "х";
                            }
                            else if (j == 7)
                            {
                                excelcells.Value2 = generalSum.ToString();
                            }
                        }

                        i = i + 2;
                        excelcells = (Microsoft.Office.Interop.Excel.Range)excelworksheet.Cells[i, 1];
                        excelcells.Value2 = "ФИО Сотрудника    ";
                        excelcells = (Microsoft.Office.Interop.Excel.Range)excelworksheet.Cells[i, 5];
                        excelcells.Value2 = "Подпись    ";
                        excelcells = (Microsoft.Office.Interop.Excel.Range)excelworksheet.Cells[i, 2];
                        excelcells.Font.Italic = true;
                        string employeeInfo = table.Rows[0]["employeeSurname"].ToString() + " " + table.Rows[0]["employeeName"].ToString() + " " + table.Rows[0]["employeePatronymic"].ToString();
                        excelcells.HorizontalAlignment = Microsoft.Office.Interop.Excel.Constants.xlLeft;
                        //excelcells.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlBordersIndex.xlDiagonalDown;
                        //excelcells.Borders.LineStyle = Microsoft.Office.Interop.Excel.XlLineStyle.xlContinuous;
                        excelcells.Value2 = employeeInfo;
                    }
                }
                path += @"\ConsignmentNoteReport.xls";
                excelappworkbooks = excelapp.Workbooks;
                excelappworkbook = excelappworkbooks[1];
                excelappworkbook.SaveAs(path);
                MessageBox.Show("The report has been successfully created.", "Notification", MessageBoxButton.OK, MessageBoxImage.Information);
                consignmentNoteNumbersField.SelectedIndex = -1;
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
    }
}
