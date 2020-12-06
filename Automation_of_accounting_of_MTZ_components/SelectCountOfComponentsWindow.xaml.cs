using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Windows;
using System.Windows.Input;

namespace Automation_of_accounting_of_MTZ_components
{
    /// <summary>
    /// Логика взаимодействия для SelectCountOfComponentsWindow.xaml
    /// </summary>
    public partial class SelectCountOfComponentsWindow : Window
    {
        SqlConnection myConnectionString = new SqlConnection(@"Data Source=(local)\SQLEXPRESS; Initial Catalog=Automation_of_accounting_of_MTZ_components; Integrated Security=True");

        public SelectCountOfComponentsWindow()
        {
            InitializeComponent();
            int code = OpenCodeFile();
            int count = 0;
            string selectComponentInfoQuery = "SELECT componentCount FROM Component WHERE componentCode = '" + code + "'";
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(selectComponentInfoQuery, myConnectionString))
            {
                DataTable table = new DataTable();
                dataAdapter.Fill(table);
                if (table.Rows.Count > 0)
                {
                    count = int.Parse(table.Rows[0]["componentCount"].ToString());
                }
            }
            if (int.Parse(countField.Text) == count)
            {
                EnlargeButton.Visibility = Visibility.Hidden;
                ReduceButton.Visibility = Visibility.Hidden;
            }
 
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            File.WriteAllText(@"ComponentCode.txt", string.Empty);
            this.Close();
        }

        private int OpenCodeFile()
        {
            StreamReader codeFile = new StreamReader("ComponentCode.txt");
            int componentCode = int.Parse(codeFile.ReadLine());
            codeFile.Close();
            return componentCode;
        }

        private void Window_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            this.DragMove();
        }

        private void EnlargeButton_Click(object sender, RoutedEventArgs e)
        {
            int code = OpenCodeFile();
            int count = 0;
            string selectComponentInfoQuery = "SELECT componentCount FROM Component WHERE componentCode = '" + code + "'";
            using (SqlDataAdapter dataAdapter = new SqlDataAdapter(selectComponentInfoQuery, myConnectionString))
            {
                DataTable table = new DataTable();
                dataAdapter.Fill(table);
                if (table.Rows.Count > 0)
                {
                    count = int.Parse(table.Rows[0]["componentCount"].ToString());
                }
            }

            countField.Text = (int.Parse(countField.Text) + 1).ToString();

            if (int.Parse(countField.Text) == 0 || int.Parse(countField.Text) < count)
            {
                EnlargeButton.Visibility = Visibility.Visible;
                ReduceButton.Visibility = Visibility.Visible;
            }

            if (int.Parse(countField.Text) == count)
            {
                EnlargeButton.Visibility = Visibility.Hidden;
            }
        }

        private void AddButton_Click(object sender, RoutedEventArgs e)
        {
            if (int.Parse(countField.Text) == 0)
            {
                MessageBox.Show("You can't specify a quantity equal to zero.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else
            {
                File.WriteAllText(@"ComponentCode.txt", string.Empty);
                StreamWriter componentFile = new StreamWriter("ComponentsCount.txt");
                componentFile.Write(countField.Text);
                componentFile.Close();
                this.Close();
            }
        }

        private void ReduceButton_Click(object sender, RoutedEventArgs e)
        {
            countField.Text = (int.Parse(countField.Text) - 1).ToString();

            if (int.Parse(countField.Text) > 0)
            {
                EnlargeButton.Visibility = Visibility.Visible;
                ReduceButton.Visibility = Visibility.Visible;
            }

            if (int.Parse(countField.Text) == 0)
            {
                ReduceButton.Visibility = Visibility.Hidden;
            }
        }
    }
}
