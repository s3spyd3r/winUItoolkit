using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using Windows.System;

namespace winUItoolkit.DesktopTasks
{
    public static class CalendarHelper
    {
        public static async Task CreateAppointmentAsync(DateTime start, string subject, bool allDay, string location, string details)
        {
            try
            {
                string ics = $@"
                BEGIN:VCALENDAR
                VERSION:2.0
                BEGIN:VEVENT
                SUMMARY:{subject}
                DESCRIPTION:{details}
                LOCATION:{location}
                DTSTART:{start:yyyyMMddTHHmmssZ}
                DTEND:{(allDay ? start.AddDays(1) : start.AddHours(1)):yyyyMMddTHHmmssZ}
                END:VEVENT
                END:VCALENDAR";

                string filePath = Path.Combine(Path.GetTempPath(), $"{Guid.NewGuid()}.ics");
                await File.WriteAllTextAsync(filePath, ics);

                await Launcher.LaunchUriAsync(new Uri(filePath));
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"[CalendarHelper] Error creating appointment: {ex}");
            }
        }
    }
}
