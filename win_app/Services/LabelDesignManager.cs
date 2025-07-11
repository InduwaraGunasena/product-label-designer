using System;
using System.IO;
using System.Text.Json;
using win_app.Models;

namespace win_app.Services
{
    public static class LabelDesignManager
    {
        private static readonly JsonSerializerOptions options = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        public static void SaveToFile(LabelDesign design, string filePath)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(design, options);
            File.WriteAllText(filePath, json);
        }

        public static LabelDesign? LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath)) return null;

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<LabelDesign>(json, options);
        }
    }
}
