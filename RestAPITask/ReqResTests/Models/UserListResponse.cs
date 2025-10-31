using Newtonsoft.Json;

namespace ReqResTests.Models
{
    public class UserListResponse
    {
        [JsonProperty("page")]
        public int Page { get; set; }

        [JsonProperty("per_page")]
        public int PerPage { get; set; }

        [JsonProperty("total")]
        public int Total { get; set; }

        [JsonProperty("total_pages")]
        public int TotalPages { get; set; }

        [JsonProperty("data")]
        public List<User> Data { get; set; } = new();

        [JsonProperty("support")]
        public Support? Support { get; set; }
    }
}