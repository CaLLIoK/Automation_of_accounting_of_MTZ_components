using System.Windows;
using System.Windows.Input;

namespace Automation_of_accounting_of_MTZ_components
{
    /// <summary>
    /// Логика взаимодействия для CreateReportWindow.xaml
    /// </summary>
    public partial class CreateReportWindow : Window
    {
        public CreateReportWindow()
        {
            InitializeComponent();
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

        private void PartsMovementButton_Click(object sender, RoutedEventArgs e)
        {
            PartsMovementReportWindow partsMovementReportWindow = new PartsMovementReportWindow();
            partsMovementReportWindow.Show();
            this.Close();
        }

        private void SpecificPartMovementButton_Click(object sender, RoutedEventArgs e)
        {
            SpecificPartMovementReport specificPartMovementReport = new SpecificPartMovementReport();
            specificPartMovementReport.Show();
            this.Close();
        }

        private void ConsignmentNoteButton_Click(object sender, RoutedEventArgs e)
        {
            ConsignmentNoteReportWindow consignmentNoteReportWindow = new ConsignmentNoteReportWindow();
            consignmentNoteReportWindow.Show();
            this.Close();
        }
    }
}
