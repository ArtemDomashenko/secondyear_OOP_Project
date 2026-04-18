using System;
using System.IO;
using System.Text.Json;
using Project_WPF.Models;

namespace Project_WPF.Services
{
    public static class SettingsService
    {
        private const string FileName = "settings.json";

        public static AppSettings LoadOrCreate()
        {
            try
            {
                if (File.Exists(FileName))
                {
                    var text = File.ReadAllText(FileName);
                    var s = JsonSerializer.Deserialize<AppSettings>(text);
                    if (s != null) return s;
                }
            }
            catch { }

            var settings = new AppSettings();
            Save(settings);
            return settings;
        }

        public static void Save(AppSettings settings)
        {
            try
            {
                var json = JsonSerializer.Serialize(settings,
                    new JsonSerializerOptions { WriteIndented = true });
                File.WriteAllText(FileName, json);
            }
            catch (Exception) { }
        }
    }
}