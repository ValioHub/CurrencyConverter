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
        SqlConnection sqlCon = new SqlConnection();
        SqlCommand sqlCmd = new SqlCommand();
        SqlDataAdapter sqlDa = new SqlDataAdapter();
        private int CurrencyID = 0;
        private double FromAmount = 0;
        private double ToAmount = 0;
        public MainWindow()
        {
            InitializeComponent();
            BindCurrency();
        }
        // CRUD - Create,Read,Update,Delete
        public void mycon() 
        {
            String Connection = ConfigurationManager.ConnectionStrings["ConnectionString"].ConnectionString;
            sqlCon = new SqlConnection(Connection);
            sqlCon.Open();
        }
        private void BindCurrency()
        {
            mycon();
            DataTable dt = new DataTable();
            // Query to get data from DB
            sqlCmd = new SqlCommand("SELECT Id, CurrencyName FROM Currency_Master", sqlCon);
            sqlCmd.CommandType = CommandType.Text;
            sqlDa = new SqlDataAdapter(sqlCmd);
            sqlDa.Fill(dt);
            // Object for DataRow
            DataRow newDataRow = dt.NewRow();
            // Give value to Id
            newDataRow["Id"] = 0;
            // Give value to CurrencyName
            newDataRow["CurrencyName"] = "Select";
            // Insert a new row in dt
            dt.Rows.InsertAt(newDataRow, 0);

            // If dt is not null and rows count > 0
            if (dt != null && dt.Rows.Count > 0) 
            {
                // Add DB data to FROM and TO comboboxes
                cmbFromCurrency.ItemsSource = dt.DefaultView;
                cmbToCurrency.ItemsSource = dt.DefaultView;
            }
            sqlCon.Close();


            cmbFromCurrency.DisplayMemberPath = "CurrencyName";
            cmbFromCurrency.SelectedValuePath = "Id";
            cmbFromCurrency.SelectedIndex = 0;
            cmbToCurrency.DisplayMemberPath = "CurrencyName";
            cmbToCurrency.SelectedValuePath = "Id";
            cmbToCurrency.SelectedIndex = 0;
        }
        private void Convert_Click(object sender, RoutedEventArgs e)
        {
            // Create variable with double datatype to store converted value
            double ConvertedValue;

            // Check if the textbox is Null or Empty
            if (txtCurrency.Text == null || txtCurrency.Text.Trim() == "")
            {
                // If is Null or Empty will show message
                MessageBox.Show("Please enter currency", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                // Set focus on textbox
                txtCurrency.Focus();
                return;
            }
            // If currency "FROM" is not selected or selected default text
            else if (cmbFromCurrency.SelectedValue == null || cmbFromCurrency.SelectedIndex == 0)
            {
                // Show message
                MessageBox.Show("Please select currency FROM", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                // Set focus on Combobox
                cmbFromCurrency.Focus();
                return;
            }
            // If currency "TO" is not selected or selected default text
            else if (cmbToCurrency.SelectedValue == null || cmbToCurrency.SelectedIndex == 0)
            {
                // Show message
                MessageBox.Show("Please select currency TO", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                // Set focus on Combobox
                cmbToCurrency.Focus();
                return;
            }

            // If "FROM" and "TO" have same values
            if (cmbFromCurrency.Text == cmbToCurrency.Text)
            {
                // Convert from string to double
                ConvertedValue = double.Parse(txtCurrency.Text);
                // Show converted currency in the label
                // N2 place 00 after dot(.)
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N2");
            }
            else
            {
                // "FROM" value multiply with value from TextBox and the total dividet with "TO" value
                ConvertedValue = (double.Parse(txtCurrency.Text))
                    / double.Parse(cmbFromCurrency.SelectedValue.ToString()) * double.Parse(cmbToCurrency.SelectedValue.ToString());
                // Show converted currency in the label
                // N2 place 00 after dot(.)
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N2");
            }
        }
        private void Clear_Click(object sender, RoutedEventArgs e)
        {
            ClearControls();
        }
        private void ClearControls()
        {
            txtCurrency.Text = string.Empty;
            if (cmbFromCurrency.Items.Count > 0)
            {
                cmbFromCurrency.SelectedIndex = 0;
            }
            if (cmbToCurrency.Items.Count > 0)
            {
                cmbToCurrency.SelectedIndex = 0;
            }
            lblCurrency.Content = "";
            txtCurrency.Focus();
        }
        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9]+");
            e.Handled = regex.IsMatch(e.Text);
        }

        private void Save_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (txtAmount.Text == null || txtAmount.Text.Trim() == "")
                {
                    MessageBox.Show("Please enter amount", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtAmount.Focus();
                    return;
                }
                else if (txtCurrencyName.Text == null || txtCurrencyName.Text.Trim() == "") 
                {
                    MessageBox.Show("Please enter currency name", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                    txtCurrencyName.Focus();
                    return;
                }
                else
                {
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
                MessageBox.Show(ex.Message,"Error",MessageBoxButton.OK,MessageBoxImage.Error);
            }
        }
        // Clear all input in Currency Master tab
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
        // Bind Data in DataGrid view
        private void GetData()
        {
            mycon();
            DataTable dt = new DataTable();
            sqlCmd = new SqlCommand("SELECT * FROM Currency_Master", sqlCon);
            sqlCmd.CommandType = CommandType.Text;
            sqlDa = new SqlDataAdapter(sqlCmd);
            sqlDa.Fill(dt);
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
    }
}
