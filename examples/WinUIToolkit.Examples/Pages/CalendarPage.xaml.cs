using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using winUItoolkit.Tasks;

namespace WinUIToolkit.Examples.Pages
{
    public sealed partial class CalendarPage : UserControl
    {
        public CalendarPage()
        {
            InitializeComponent();
            DatePicker.Date = DateTimeOffset.Now.AddDays(1);
        }

        private async void Add_Click(object sender, RoutedEventArgs e)
        {
            var date = DatePicker.Date.DateTime;
            var time = TimePicker.Time;
            var start = date.Date.Add(time);
            var ok = await CalendarHelper.CreateAppointmentAsync(start, Subject.Text, allDay: false, Location.Text, details: "Created from the winUItoolkit Examples app.");
            Status.Text = ok ? "Appointment dialog dismissed (created or cancelled)." : "Failed to open appointment dialog.";
        }
    }
}
