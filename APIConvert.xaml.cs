using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Http;
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
    /// Interaction logic for APIConvert.xaml
    /// </summary>
    public partial class APIConvert : UserControl
    {
        Root val = new Root();
        public APIConvert()
        {
            InitializeComponent();
            GetValue();
        }
        public static async Task<Root> GetData<T>(string url)
        {
            var myRoot = new Root();
            try
            {
                // Sending/Receiving HTTP requests
                using (var client = new HttpClient())
                {
                    // To wait before request times out
                    client.Timeout = TimeSpan.FromMinutes(1);
                    HttpResponseMessage response = await client.GetAsync(url);
                    // Check API response status code
                    if (response.StatusCode == System.Net.HttpStatusCode.OK)
                    {
                        // HTTP content to a string
                        var ResponceString = await response.Content.ReadAsStringAsync();
                        var ResponceObject = JsonConvert.DeserializeObject<Root>(ResponceString);

                        //MessageBox.Show("License: " + ResponceObject.license, "Information", MessageBoxButton.OK, MessageBoxImage.Information);

                        // Return API responce
                        return ResponceObject;
                    }
                    return myRoot;
                }
            }
            catch
            {
                return myRoot;
            }
        }
        private async void GetValue()
        {
            val = await GetData<Root>("https://openexchangerates.org/api/latest.json?app_id=d3b8982f1d6e46dc81856656a1107b7f");
            BindCurrency();
        }
        public void BindCurrency()
        {
            // Create DataTable
            DataTable dt = new DataTable();
            // Add Text Columns
            dt.Columns.Add("Text");
            // Add Value Columns
            dt.Columns.Add("Value");

            // Add rows in the Datatable
            dt.Rows.Add("Select", 0);
            dt.Rows.Add("CAD", val.rates.CAD);
            dt.Rows.Add("CZK", val.rates.CZK);
            dt.Rows.Add("DKK", val.rates.DKK);
            dt.Rows.Add("EUR", val.rates.EUR);
            dt.Rows.Add("INR", val.rates.INR);
            dt.Rows.Add("ISK", val.rates.ISK);
            dt.Rows.Add("JPY", val.rates.JPY);
            dt.Rows.Add("NZD", val.rates.NZD);
            dt.Rows.Add("PHP", val.rates.PHP);
            dt.Rows.Add("USD", val.rates.USD);

            //Datatable data assigned from the currency comboboxes
            cmbFromCurrency.ItemsSource = dt.DefaultView;
            cmbToCurrency.ItemsSource = dt.DefaultView;

            // Display data in comboboxes
            cmbFromCurrency.DisplayMemberPath = "Text";
            cmbToCurrency.DisplayMemberPath = "Text";
            // Display data in comboboxes
            cmbFromCurrency.SelectedValuePath = "Value";
            cmbToCurrency.SelectedValuePath = "Value";

            // Default selected item
            cmbFromCurrency.SelectedIndex = 0;
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
                ConvertedValue = (double.Parse(cmbToCurrency.SelectedValue.ToString())
                    * double.Parse(txtCurrency.Text)) / double.Parse(cmbFromCurrency.SelectedValue.ToString());
                // Show converted currency in the label
                // N2 place 00 after dot(.)
                lblCurrency.Content = cmbToCurrency.Text + " " + ConvertedValue.ToString("N2");
            }
        }

        private void NumberValidationTextBox(object sender, TextCompositionEventArgs e)
        {
            Regex regex = new Regex("[^0-9,.]+");
            e.Handled = regex.IsMatch(e.Text);
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
    }
}
