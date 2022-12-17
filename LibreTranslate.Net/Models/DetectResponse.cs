using Newtonsoft.Json;

namespace LibreTranslate.Net.Models;

public class DetectResponse
{
    /// <summary>
    /// The confidence from the detected language
    /// </summary>
    [JsonProperty("confidence")]
    public double Confidence { get; set; }
    
    /// <summary>
    /// The detected language
    /// </summary>
    [JsonProperty("language")]
    public string Language { get; set; }
}