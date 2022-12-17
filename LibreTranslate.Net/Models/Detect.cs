using Newtonsoft.Json;

namespace LibreTranslate.Net.Models;

/// <summary>
/// The model to send to the libre translate api
/// </summary>
public class Detect
{
    /// <summary>
    /// The text to be translated
    /// </summary>
    [JsonProperty("q")]
    public string Text { get; set; }

    /// <summary>
    /// The libre translate api key
    /// </summary>
    [JsonProperty("api_key")]
    public string? ApiKey { get; set; }
}