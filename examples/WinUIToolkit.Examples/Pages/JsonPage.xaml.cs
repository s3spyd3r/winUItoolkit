using System.Collections.Generic;
using System.Linq;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using winUItoolkit.IO;

namespace WinUIToolkit.Examples.Pages
{
    public sealed partial class JsonPage : UserControl
    {
        private string? _lastJson;

        public JsonPage()
        {
            InitializeComponent();
        }

        private record Person(string Name, int Year, IReadOnlyList<string> Languages);

        private async void Serialize_Click(object sender, RoutedEventArgs e)
        {
            var person = new Person(
                NameBox.Text,
                (int)YearBox.Value,
                LanguagesBox.Text.Split(',', System.StringSplitOptions.RemoveEmptyEntries | System.StringSplitOptions.TrimEntries).ToList());
            _lastJson = await JsonStorage.SerializeAsync(person);
            JsonOut.Text = _lastJson;
        }

        private async void Deserialize_Click(object sender, RoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(_lastJson))
            {
                JsonOut.Text = "Click Serialize first.";
                return;
            }

            var p = await JsonStorage.DeserializeAsync<Person>(_lastJson);
            if (p == null)
            {
                JsonOut.Text = "Deserialization returned null.";
                return;
            }

            JsonOut.Text = $"Name={p.Name}\nYear={p.Year}\nLanguages={string.Join(", ", p.Languages)}";
        }
    }
}
