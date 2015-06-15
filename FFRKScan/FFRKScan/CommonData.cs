using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FFRKScan
{
    public class SimpleSuccessData
    {
        [JsonProperty("success")]
        public bool IsSuccess { get; set; }
    }

    public class SimpleIdData
    {
        [JsonProperty("id")]
        public ulong Id { get; set; }
    }
    public class SimpleStringIdData
    {
        [JsonProperty("id")]
        public string Id { get; set; }
    }
}
