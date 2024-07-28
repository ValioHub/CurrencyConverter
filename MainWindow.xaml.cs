using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace CurrencyConverter_Static
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        // SQL connection, command, and data adapter objects
        SqlConnection sqlCon = new SqlConnection();
        SqlCommand sqlCmd = new SqlCommand();
        SqlDataAdapter sqlDa = new SqlDataAdapter();

        // Variables to store selected currency ID and conversion amounts
        private int CurrencyID = 0;
        private double FromAmount = 0;
        private double ToAmount = 0;

        // Constructor for MainWindow
        public MainWindow()
        {
            InitializeComponent();
            BindCurrency(); // Load currency data into comboboxes
            BindCurrency(); // Load currency data into comboboxes
            GetData(); // Retrieve other necessary data
        }

        // Method to establish a connection to the database
        public void mycon() 
        {
            // Retrieve connection string from configuration file
            String Connection = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            sqlCon = new SqlConnection(Connection);
            sqlCon.Open(); // Open the SQL connection
        }

        // Method to bind currency data to comboboxes
        private void BindCurrency()
        {
            mycon(); // Establish the database connection
            DataTable dt = new DataTable(); // Create a DataTable to hold currency data
            
            // SQL query to get currency data from the database
            sqlCmd = new SqlCommand("SELECT Amount, CurrencyName FROM Currency_Master", sqlCon);
            sqlCmd.CommandType = CommandType.Text;
            sqlDa = new SqlDataAdapter(sqlCmd); 
            sqlDa.Fill(dt); // Fill the DataTable with data from the database

            // Create a new row for the default "Select" option
            DataRow newDataRow = dt.NewRow();
            // Give value to Id
            newDataRow["Amount"] = 0;
            // Give value to CurrencyName
            newDataRow["CurrencyName"] = "Select";
            // Insert a new row in dt
            dt.Rows.InsertAt(newDataRow, 0); // Insert the default row at the top of the DataTable

            // Check if DataTable has data
            if (dt != null && dt.Rows.Count > 0) 
            {
                // Bind the DataTable to the comboboxes
                cmbFromCurrency.ItemsSource = dt.DefaultView;
                cmbToCurrency.ItemsSource = dt.DefaultView;
            }
            sqlCon.Close(); // Close the SQL connection

            // Configure display and value paths for comboboxes
            cmbFromCurrency.DisplayMemberPath = "CurrencyName";
            cmbFromCurrency.SelectedValuePath = "Amount";
            cmbFromCurrency.SelectedIndex = 0; // Set default selected index

            cmbToCurrency.DisplayMemberPath = "CurrencyName";
            cmbToCurrency.SelectedValuePath = "Amount";
            cmbToCurrency.SelectedIndex = 0; // Set default selected index
        }

        // Event handler for the Convert button click event
        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            double ConvertedValue; // Variable to store the converted currency value

            // Check if the currency input textbox is empty
            if (txtCurrency.Text == null || txtCurrency.Text.Trim() == "")
            {
                MessageBox.Show("Please enter currency", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                txtCurrency.Focus(); // Set focus on the textbox
                return;
            }
            // Check if a "FROM" currency is selected
            else if (cmbFromCurrency.SelectedValue == null || cmbFromCurrency.SelectedIndex == 0)
            {
                MessageBox.Show("Please select currency FROM", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                cmbFromCurrency.Focus(); // Set focus on the combobox
                return;
            }
            // Check if a "TO" currency is selected
            else if (cmbToCurrency.SelectedValue == null || cmbToCurrency.SelectedIndex == 0)
            {
                MessageBox.Show("Please select currency TO", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                cmbToCurrency.Focus(); // Set focus on the combobox
                return;
            }

            // If the "FROM" and "TO" currencies are the same
            if (cmbFromCurrency.Text == cmbToCurrency.Text)
            {
                // Convert the input amount to double and display it
                ConvertedValue = double.Parse(txtCurrency.Text);
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N2");
            }
            else
            {
                // Convert the amount using the conversion rates
                ConvertedValue = (double.Parse(cmbFromCurrency.SelectedValue.ToString())
                    * double.Parse(txtCurrency.Text)) / double.Parse(cmbToCurrency.SelectedValue.ToString());
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N2");
            }
        }

        // Event handler for the Clear button click event
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearControls();
        }

        // Method to clear input fields and reset control states
        private void ClearControls()
        {
            txtCurrency.Text = string.Empty; // Clear text from the currency input field

            // Reset the 'From' currency combobox to the default selection
            if (cmbFromCurrency.Items.Count > 0)
            {
                cmbFromCurrency.SelectedIndex = 0;
            }

            // Reset the 'To' currency combobox to the default selection
            if (cmbToCurrency.Items.Count > 0)
            {
                cmbToCurrency.SelectedIndex = 0;
            }

            lblCurrency.Content = "";
            txtCurrency.Focus();
        }

        // Event handler for validating text input in currency fields (numeric only)
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            // Regular expression to allow only digits, commas, and periods
            Regex regex = new Regex("[^0-9,.]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        // Event handler for the Save button click event
        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                // Check if the amount input is empty
                if (txtAmount.Text == null || txtAmount.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter amount", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtAmount.Focus();
                    return;
                }
                // Check if the currency name input is empty
                else if (txtCurrencyName.Text == null || txtCurrencyName.Text.Trim() == "") 
                {
                    MessageBox.Show("Please enter currency name", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtCurrencyName.Focus();
                    return;
                }
                else
                {
                    // If CurrencyID is greater than 0, update existing record
                    if (CurrencyID > 0)
                    {
                        if (MessageBox.Show("Are you sure you want to update?", "Information", MessageBoxButton.YesNo,
                            MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            mycon();
                            DataTable dt = new DataTable();
                            sqlCmd = new SqlCommand("UPDATE Currency_Master SET Amount = @Amount, CurrencyName = @CurrencyName WHERE Id = @Id", sqlCon);
                            sqlCmd.CommandType = CommandType.Text;
                            sqlCmd.Parameters.AddWithValue("@Id", CurrencyID);
                            sqlCmd.Parameters.AddWithValue("@Amount", txtAmount.Text);
                            sqlCmd.Parameters.AddWithValue("@CurrencyName", txtCurrencyName.Text);
                            sqlCmd.ExecuteNonQuery();
                            sqlCon.Close();
                            MessageBox.Show("Data updated successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    // If CurrencyID is not greater than 0, insert a new record
                    else
                    {
                        if (MessageBox.Show("Are you sure you want to save?", "Information", MessageBoxButton.YesNo,
                            MessageBoxImage.Question) == MessageBoxResult.Yes)
                        {
                            mycon();
                            sqlCmd = new SqlCommand("INSERT INTO Currency_Master(Amount, CurrencyName) VALUES(@Amount, @CurrencyName)", sqlCon);
                            sqlCmd.CommandType = CommandType.Text;
                            sqlCmd.Parameters.AddWithValue("@Amount", txtAmount.Text);
                            sqlCmd.Parameters.AddWithValue("@CurrencyName", txtCurrencyName.Text);
                            sqlCmd.ExecuteNonQuery();
                            sqlCon.Close();
                            MessageBox.Show("Data saved successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                        }
                    }
                    CleanMaster();
                }
            }
            catch (Exception ex)
            {
                // Show error message if an exception occurs
                MessageBox.Show(ex.Message,"Error",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }

        // Method to clear all inputs and reset the form for Currency Master tab
        private void CleanMaster()
        {
            try
            {
                txtAmount.Text = string.Empty;
                txtCurrencyName.Text = string.Empty;
                btnSave.Content = "Save";
                GetData();
                CurrencyID = 0;
                BindCurrency();
                txtAmount.Focus();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Method to bind data to the DataGrid view
        private void GetData()
        {
            mycon();
            DataTable dt = new DataTable();

            // SQL query to select all records from the Currency_Master table
            sqlCmd = new SqlCommand("SELECT * FROM Currency_Master", sqlCon);
            sqlCmd.CommandType = CommandType.Text;
            sqlDa = new SqlDataAdapter(sqlCmd);
            sqlDa.Fill(dt);

            // Check if DataTable contains any data
            if (dt != null && dt.Rows.Count > 0)
            {
                dgvCurrency.ItemsSource = dt.DefaultView;
            }
            else
            {
                dgvCurrency.ItemsSource = null;
            }
            sqlCon.Close();
        }

        // Event handler for the Cancel button click event
        private void Cancel_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                CleanMaster();
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        // Event handler for changes in the selected cells of the DataGrid view
        private void dvgCurrency_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            try
            {
                DataGrid dtGrid = (DataGrid)sender;
                DataRowView dtRowSelected = dtGrid.CurrentItem as DataRowView;

                // Check if a row is selected
                if (dtRowSelected != null)
                {
                    // Check if the DataGrid has any items
                    if (dgvCurrency.Items.Count > 0)
                    {
                        // Check if any cells are selected
                        if (dtGrid.SelectedCells.Count > 0)
                        {
                            // Set the CurrencyID based on the selected row's ID
                            CurrencyID = Int32.Parse(dtRowSelected["Id"].ToString());

                            // Check which column is selected
                            if (dtGrid.SelectedCells[0].Column.DisplayIndex == 0)
                            {
                                // Populate the form fields with data from the selected row
                                txtAmount.Text = dtRowSelected["Amount"].ToString();
                                txtCurrencyName.Text = dtRowSelected["CurrencyName"].ToString();
                                btnSave.Content = "Update";
                            }
                            // Check if the selected column is for deleting
                            if (dtGrid.SelectedCells[0].Column.DisplayIndex == 1)
                            {
                                // Prompt user for confirmation before deleting
                                if (MessageBox.Show("Are you sure you want to delete?", "Information", MessageBoxButton.YesNo,
                                    MessageBoxImage.Question) == MessageBoxResult.Yes)
                                {
                                    mycon();
                                    DataTable dt = new DataTable();
                                    sqlCmd = new SqlCommand("DELETE FROM Currency_Master WHERE Id = @Id", sqlCon);
                                    sqlCmd.CommandType = CommandType.Text;
                                    sqlCmd.Parameters.AddWithValue("@Id", CurrencyID);
                                    sqlCmd.ExecuteNonQuery();
                                    sqlCon.Close();
                                    MessageBox.Show("Data deleted successfully", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                                    CleanMaster();
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
