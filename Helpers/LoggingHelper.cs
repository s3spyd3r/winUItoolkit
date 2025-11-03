using System;
using System.Diagnostics;
using System.IO;

namespace winUItoolkit.Helpers
{
    public static class LoggingHelper
    {
        private static readonly object _lock = new object();
        private static string? _logFilePath;

        public static void Init(string? logFilePath = null)
        {
            if (string.IsNullOrWhiteSpace(logFilePath))
            {
                var folder = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
                Directory.CreateDirectory(Path.Combine(folder, "winUItoolkit"));
                _logFilePath = Path.Combine(folder, "winUItoolkit", "log.txt");
            }
            else
            {
                _logFilePath = logFilePath;
            }
        }

        public static void Info(string message) => Log("INFO", message);
        public static void Warn(string message) => Log("WARN", message);
        public static void Error(string message) => Log("ERROR", message);

        private static void Log(string level, string message)
        {
            var line = $"[{DateTime.UtcNow:O}] {level}: {message}";
            Debug.WriteLine(line);
            if (!string.IsNullOrWhiteSpace(_logFilePath))
            {
                try
                {
                    lock (_lock)
                    {
                        File.AppendAllText(_logFilePath, line + Environment.NewLine);
                    }
                }
                catch { }
            }
        }
    }
}