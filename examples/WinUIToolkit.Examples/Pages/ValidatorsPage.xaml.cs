using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using winUItoolkit.Helpers;

namespace WinUIToolkit.Examples.Pages
{
    public sealed partial class ValidatorsPage : UserControl
    {
        public ValidatorsPage()
        {
            InitializeComponent();
        }

        private void Any_TextChanged(object sender, TextChangedEventArgs e)
        {
            Update(EmailBox, EmailResult, Validators.IsValidEmail(EmailBox.Text));
            Update(PhoneBox, PhoneResult, Validators.IsValidPhone(PhoneBox.Text));
            Update(UrlBox, UrlResult, Validators.IsValidUrl(UrlBox.Text));
            Update(PostalBox, PostalResult, Validators.IsValidPostalCode(PostalBox.Text));

            PasswordResult.Text = Validators.IsValidPassword(PasswordBox.Text)
                ? "OK"
                : "Needs 8+ chars with upper, lower, digit, and symbol.";
            PasswordResult.Foreground = new SolidColorBrush(
                Validators.IsValidPassword(PasswordBox.Text) ? Colors.Green : Colors.OrangeRed);
        }

        private static void Update(TextBox box, TextBlock result, bool ok)
        {
            result.Text = string.IsNullOrEmpty(box.Text) ? string.Empty : (ok ? "OK" : "Invalid");
            result.Foreground = new SolidColorBrush(ok ? Colors.Green : Colors.OrangeRed);
        }
    }
}
