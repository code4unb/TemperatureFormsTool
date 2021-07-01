using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace TemperatureFormsTool
{
    public class Mapping
    {
        [JsonProperty("input_id")]
        public string InputId { get; set; }

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("value")]
        public string Value { get; set; }
    }

    public class InputMapping
    {
        [JsonProperty("form_id")]
        public string FormId { get; set; }

        [JsonProperty("mappings")]
        public List<Mapping> Mappings { get; set; }
    }
}
