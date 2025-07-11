using System.Text.Json.Serialization;

namespace win_app.Label
{
    /// <summary>
    /// Root object representing a complete label design:
    /// - Label metadata (size, DPI)
    /// - Items (fixed, variable, shapes)
    /// - Formats (styling/layout for each item)
    /// </summary>
    public class LabelDesign
    {
        [JsonPropertyName("label")]
        public LabelMetadata Label { get; set; } = new();

        [JsonPropertyName("items")]
        public LabelItems Items { get; set; } = new();

        [JsonPropertyName("formats")]
        public LabelFormats Formats { get; set; } = new();
    }
}
