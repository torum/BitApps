using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace BitApps.Core.Models;

[JsonSerializable(typeof(JsonDepthData))]
[JsonSerializable(typeof(JsonDepthObject))]
[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
internal partial class DepthJsonSerializerContext : JsonSerializerContext
{
}

public partial class JsonDepthObject
{
    //[JsonProperty("success")]
    public long Success { get; set; }

    //[JsonProperty("data")]
    public JsonDepthData? Data { get; set; }
}

public partial class JsonDepthData
{
    //[JsonProperty("asks")]
    public List<List<string>>? Asks { get; set; }

    //[JsonProperty("bids")]
    public List<List<string>>? Bids { get; set; }

    //[JsonProperty("timestamp")]
    public long Timestamp { get; set; }
}
