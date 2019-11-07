using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;

namespace MyConverter.Models
{
    public class ApiResponse
    {
        [JsonProperty(PropertyName = "disclaimer")]
        public string Disclaimer { get; set; }

        [JsonProperty(PropertyName = "license")]
        public string License { get; set; }

        [JsonProperty(PropertyName = "timestamp")]
        public long Timestamp { get; set; }

        [JsonProperty(PropertyName = "base")]
        public string Base { get; set; }

        [JsonProperty(PropertyName = "rates")]
        public IDictionary<string, decimal> Rates { get; set; } 
    }
}
