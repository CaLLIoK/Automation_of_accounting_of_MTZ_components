﻿using Automation_of_accounting_of_MTZ_components.Data_validation;
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
        string connectionString = @"Data Source=(local)\SQLEXPRESS; Initial Catalog=Automation_of_accounting_of_MTZ_components; Integrated Security=True";

        public ComponentsInfo()
        {
            InitializeComponent();

            FillDataGrid();
        }

        private void FillDataGrid()
        {
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

        private void SearchButton_Click(object sender, RoutedEventArgs e)
        {
            string componentsInfoQuery = string.Empty;

            if (tractorNameField.Text == string.Empty && componentNameField.Text == string.Empty && availabilityStatusNameField.Text == string.Empty)
            {
                MessageBox.Show("Search fields aren't filled.");
                return;
            }

            if (tractorNameField.Text != string.Empty && componentNameField.Text == string.Empty && availabilityStatusNameField.Text == string.Empty)
            {
                if (tractorNameField.Text != ComponentsChecks.CheckTractorBrandName(tractorNameField.Text, "Tractor name contains invalid symbols.", "Allowed tractor name length is 3-100 symbols."))
                {
                    MessageBox.Show(ComponentsChecks.CheckTractorBrandName(tractorNameField.Text, "Tractor name contains invalid symbols.", "Allowed tractor name length is 3-100 symbols."));
                    return;
                }
                componentsInfoQuery = "SELECT tractorBrandName, componentName, componentDescription, componentWeight, componentCount, componentCost, availabilityStatusName " +
                                             "FROM Component " +
                                             "JOIN TractorBrand ON Component.tractorBrandCode = TractorBrand.tractorBrandCode " +
                                             "JOIN AvailabilityStatus ON Component.availabilityStatusCode = AvailabilityStatus.availabilityStatusCode " +
                                             "WHERE tractorBrandName = '" + tractorNameField.Text + "'";
            }
            
            if (componentNameField.Text != string.Empty && tractorNameField.Text == string.Empty && availabilityStatusNameField.Text == string.Empty)
            {
                if (componentNameField.Text != ComponentsChecks.CheckComponentName(componentNameField.Text, "Component name contains invalid symbols.", "Allowed component name length is 3-100 symbols."))
                {
                    MessageBox.Show(ComponentsChecks.CheckComponentName(componentNameField.Text, "Component name contains invalid symbols.", "Allowed component name length is 3-100 symbols."));
                    return;
                }
                componentsInfoQuery = "SELECT tractorBrandName, componentName, componentDescription, componentWeight, componentCount, componentCost, availabilityStatusName " +
                                             "FROM Component " +
                                             "JOIN TractorBrand ON Component.tractorBrandCode = TractorBrand.tractorBrandCode " +
                                             "JOIN AvailabilityStatus ON Component.availabilityStatusCode = AvailabilityStatus.availabilityStatusCode " +
                                             "WHERE componentName = '" + componentNameField.Text + "'";
            }
            
            if (availabilityStatusNameField.Text != string.Empty && componentNameField.Text == string.Empty && tractorNameField.Text == string.Empty)
            {
                componentsInfoQuery = "SELECT tractorBrandName, componentName, componentDescription, componentWeight, componentCount, componentCost, availabilityStatusName " +
                                             "FROM Component " +
                                             "JOIN TractorBrand ON Component.tractorBrandCode = TractorBrand.tractorBrandCode " +
                                             "JOIN AvailabilityStatus ON Component.availabilityStatusCode = AvailabilityStatus.availabilityStatusCode " +
                                             "WHERE availabilityStatusName = '" + availabilityStatusNameField.Text + "'";
            }

            if (tractorNameField.Text != string.Empty && componentNameField.Text != string.Empty && availabilityStatusNameField.Text == string.Empty)
            {
                if (tractorNameField.Text != ComponentsChecks.CheckTractorBrandName(tractorNameField.Text, "Tractor name contains invalid symbols.", "Allowed tractor name length is 3-100 symbols.") && componentNameField.Text != ComponentsChecks.CheckComponentName(componentNameField.Text, "Component name contains invalid symbols.", "Allowed component name length is 3-100 symbols."))
                {
                    MessageBox.Show(ComponentsChecks.CheckTractorBrandName(tractorNameField.Text, "Tractor name contains invalid symbols.", "Allowed tractor name length is 3-100 symbols.") + "\n" + ComponentsChecks.CheckComponentName(componentNameField.Text, "Component name contains invalid symbols.", "Allowed component name length is 3-100 symbols."));
                    return;
                }
                componentsInfoQuery = "SELECT tractorBrandName, componentName, componentDescription, componentWeight, componentCount, componentCost, availabilityStatusName " +
                                             "FROM Component " +
                                             "JOIN TractorBrand ON Component.tractorBrandCode = TractorBrand.tractorBrandCode " +
                                             "JOIN AvailabilityStatus ON Component.availabilityStatusCode = AvailabilityStatus.availabilityStatusCode " +
                                             "WHERE tractorBrandName = '" + tractorNameField.Text + "' AND componentName = '" + componentNameField.Text + "'";
            }

            if (tractorNameField.Text != string.Empty && availabilityStatusNameField.Text != string.Empty && componentNameField.Text == string.Empty)
            {
                if (tractorNameField.Text != ComponentsChecks.CheckTractorBrandName(tractorNameField.Text, "Tractor name contains invalid symbols.", "Allowed tractor name length is 3-100 symbols."))
                {
                    MessageBox.Show(ComponentsChecks.CheckTractorBrandName(tractorNameField.Text, "Tractor name contains invalid symbols.", "Allowed tractor name length is 3-100 symbols."));
                    return;
                }
                componentsInfoQuery = "SELECT tractorBrandName, componentName, componentDescription, componentWeight, componentCount, componentCost, availabilityStatusName " +
                                             "FROM Component " +
                                             "JOIN TractorBrand ON Component.tractorBrandCode = TractorBrand.tractorBrandCode " +
                                             "JOIN AvailabilityStatus ON Component.availabilityStatusCode = AvailabilityStatus.availabilityStatusCode " +
                                             "WHERE tractorBrandName = '" + tractorNameField.Text + "' AND availabilityStatusName = '" + availabilityStatusNameField.Text + "'";
            }

            if (componentNameField.Text != string.Empty && availabilityStatusNameField.Text != string.Empty && tractorNameField.Text == string.Empty)
            {
                if (componentNameField.Text != ComponentsChecks.CheckComponentName(componentNameField.Text, "Component name contains invalid symbols.", "Allowed component name length is 3-100 symbols."))
                {
                    MessageBox.Show(ComponentsChecks.CheckComponentName(componentNameField.Text, "Component name contains invalid symbols.", "Allowed component name length is 3-100 symbols."));
                    return;
                }
                componentsInfoQuery = "SELECT tractorBrandName, componentName, componentDescription, componentWeight, componentCount, componentCost, availabilityStatusName " +
                                             "FROM Component " +
                                             "JOIN TractorBrand ON Component.tractorBrandCode = TractorBrand.tractorBrandCode " +
                                             "JOIN AvailabilityStatus ON Component.availabilityStatusCode = AvailabilityStatus.availabilityStatusCode " +
                                             "WHERE componentName = '" + componentNameField.Text + "' AND availabilityStatusName = '" + availabilityStatusNameField.Text + "'";
            }

            if (tractorNameField.Text != string.Empty && componentNameField.Text != string.Empty && availabilityStatusNameField.Text != string.Empty)
            {
                if (tractorNameField.Text != ComponentsChecks.CheckTractorBrandName(tractorNameField.Text, "Tractor name contains invalid symbols.", "Allowed tractor name length is 3-100 symbols.") && componentNameField.Text != ComponentsChecks.CheckComponentName(componentNameField.Text, "Component name contains invalid symbols.", "Allowed component name length is 3-100 symbols."))
                {
                    MessageBox.Show(ComponentsChecks.CheckTractorBrandName(tractorNameField.Text, "Tractor name contains invalid symbols.", "Allowed tractor name length is 3-100 symbols.") + "\n" + ComponentsChecks.CheckComponentName(componentNameField.Text, "Component name contains invalid symbols.", "Allowed component name length is 3-100 symbols."));
                    return;
                }
                componentsInfoQuery = "SELECT tractorBrandName, componentName, componentDescription, componentWeight, componentCount, componentCost, availabilityStatusName " +
                                             "FROM Component " +
                                             "JOIN TractorBrand ON Component.tractorBrandCode = TractorBrand.tractorBrandCode " +
                                             "JOIN AvailabilityStatus ON Component.availabilityStatusCode = AvailabilityStatus.availabilityStatusCode " +
                                             "WHERE tractorBrandName = '" + tractorNameField.Text + "' AND componentName = '" + componentNameField.Text + "' AND availabilityStatusName = '" + availabilityStatusNameField.Text + "'";
            }

            SqlConnection connection = new SqlConnection(connectionString);

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

        private void ClearButton_Click(object sender, RoutedEventArgs e)
        {
            tractorNameField.Clear();
            componentNameField.Clear();
            FillDataGrid();
            availabilityStatusNameField.SelectedIndex = -1;
        }
    }
}
