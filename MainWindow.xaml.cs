using System;
using System.Collections.Generic;
using System.Data;
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
        public MainWindow()
        {
            InitializeComponent();
            BindCurrency();
        }
        private void BindCurrency()
        {
            DataTable dtCurrency = new DataTable();
            dtCurrency.Columns.Add("Text");
            dtCurrency.Columns.Add("Value");

            // Add rows in the DataTable'
            dtCurrency.Rows.Add("Select", 0);
            dtCurrency.Rows.Add("USD", 1);
            dtCurrency.Rows.Add("EUR", 0.910247);
            dtCurrency.Rows.Add("GBP", 0.783139);
            dtCurrency.Rows.Add("INR", 81.938793);
            dtCurrency.Rows.Add("AUD", 1.471434);
            dtCurrency.Rows.Add("CAD", 1.316428);
            dtCurrency.Rows.Add("SGD", 1.340416);

            cmbFromCurrency.ItemsSource = dtCurrency.DefaultView;
            cmbFromCurrency.DisplayMemberPath = "Text";
            cmbFromCurrency.SelectedValuePath = "Value";
            cmbFromCurrency.SelectedIndex = 0;

            cmbToCurrency.ItemsSource = dtCurrency.DefaultView;
            cmbToCurrency.DisplayMemberPath = "Text";
            cmbToCurrency.SelectedValuePath = "Value";
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
    }
}
