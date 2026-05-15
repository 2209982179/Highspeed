using Newtonsoft.Json;

namespace highspeed.api.Filter
{
    public class ApiResult
    {
        public int? Code { get; set; }

        public string Message { get; set; }

        public object? Data { get; set; }

        public bool Success { get; set; }

        [JsonProperty(DefaultValueHandling = DefaultValueHandling.Ignore)]
        public Guid? ActionGuid { get; set; }
    }
}
