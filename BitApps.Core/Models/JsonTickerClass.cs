using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace BitApps.Core.Models;

[JsonSerializable(typeof(JsonTickerObject))]
[JsonSerializable(typeof(JsonTickerData))]
[JsonSourceGenerationOptions(PropertyNameCaseInsensitive = true)]
internal partial class TickerJsonSerializerContext : JsonSerializerContext
{
}

public class JsonTickerData
{
    public string? Sell
    {
        get; set;
    }
    public string? Buy
    {
        get; set;
    }
    public string? High
    {
        get; set;
    }
    public string? Low
    {
        get; set;
    }
    public string? Last
    {
        get; set;
    }
    public string? Vol
    {
        get; set;
    }
    public long Timestamp
    {
        get; set;
    }
}

public class JsonTickerObject
{
    public int Success
    {
        get; set;
    }
    public JsonTickerData? Data
    {
        get; set;
    }
}
