using Newtonsoft.Json;

namespace PartnerNode.Models {
    public class JsonReq {

        [JsonProperty("jsonrpc")]
        public string Jsonrpc { get; set; } = "2.0";

        [JsonProperty("method")]
        public string Method { get; set; }


        [JsonProperty("params")]
        public object Params { get; set; }

        [JsonProperty("id")]
        public int Id { get; set; } = GenNewId();


        private static int GenId = 1;

        public static int GenNewId() {
            return GenId++;
        }

        public class JsonResp {
            [JsonProperty("jsonrpc")]
            public string Jsonrpc { get; set; } = "2.0";

            [JsonProperty("id")]
            public int Id { get; set; }

            [JsonProperty("result")]
            public object Result { get; set; }
        }
    }
}

