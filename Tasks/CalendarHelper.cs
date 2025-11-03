using System;
using System.Diagnostics;
using System.Threading.Tasks;
using Windows.ApplicationModel.Appointments;
using Windows.Foundation;
using Windows.UI.Popups;
using Microsoft.UI.Xaml;

namespace winUItoolkit.Tasks
{
    public static class CalendarHelper
    {
        public static async Task<bool> CreateAppointmentAsync(DateTime start, string subject, bool allDay, string location, string details, FrameworkElement? triggerElement = null)
        {
            try
            {
                var appointment = new Appointment
                {
                    StartTime = start,
                    Subject = subject,
                    Location = location ?? string.Empty,
                    Details = details ?? string.Empty,
                    AllDay = allDay,
                    Duration = allDay ? TimeSpan.FromDays(1) : TimeSpan.FromHours(1), // Adjust as needed
                    // Optional: BusyStatus = AppointmentBusyStatus.Busy, Reminder = TimeSpan.FromMinutes(15), etc.
                };

                // Get the selection Rect (area around which to show the UI)
                Rect selection;
                if (triggerElement != null)
                {
                    var transform = triggerElement.TransformToVisual(null);
                    var point = transform.TransformPoint(new Point());
                    selection = new Rect(point, triggerElement.RenderSize);
                }
                else
                {
                    selection = new Rect(0, 0, 0, 0); // Default: System decides placement
                }

                // Show the add-appointment UI
                string? appointmentId = await AppointmentManager.ShowAddAppointmentAsync(appointment, selection, Placement.Default);

                return !string.IsNullOrEmpty(appointmentId); // True if added successfully
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[CalendarHelper] Error creating appointment: {ex}");
                return false;
            }
        }
    }
}