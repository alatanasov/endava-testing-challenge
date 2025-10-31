using Newtonsoft.Json;

namespace ReqResTests.Models
{
    public class SingleUserResponse
    {
        [JsonProperty("data")]
        public User Data { get; set; } = new();

        [JsonProperty("support")]
        public Support? Support { get; set; }
    }
}