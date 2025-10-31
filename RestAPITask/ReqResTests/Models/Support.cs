using Newtonsoft.Json;

namespace ReqResTests.Models
{
    public class Support
    {
        [JsonProperty("url")]
        public string Url { get; set; } = string.Empty;

        [JsonProperty("text")]
        public string Text { get; set; } = string.Empty;
    }
}