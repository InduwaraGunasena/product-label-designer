using System;
using System.IO;
using System.Text.Json;
using win_app.Label;

namespace win_app.Services
{
    // Handles saving and loading LabelDesign data as JSON files.
    // Use this to persist full designs to disk and read them back.
    public static class LabelDesignManager
    {
        // Global serializer options
        private static readonly JsonSerializerOptions options = new()
        {
            WriteIndented = true,
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        };

        // Save the full label design to a JSON file.
        public static void SaveToFile(LabelDesign design, string filePath)
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(design, options);
            File.WriteAllText(filePath, json);
        }

        // Load a label design from a JSON file. Returns null if file doesn't exist.
        public static LabelDesign? LoadFromFile(string filePath)
        {
            if (!File.Exists(filePath)) return null;

            string json = File.ReadAllText(filePath);
            return JsonSerializer.Deserialize<LabelDesign>(json, options);
        }
    }
}
