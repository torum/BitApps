using Newtonsoft.Json;

namespace BitDesk.Models.APIClients;

// 発注時クエリパラメーター用クラス Jsonシリアライズ用
[JsonObject]
public class OrderParam
{
    [JsonProperty("pair")]
    public string Pair
    {
        get; set;
    }

    [JsonProperty("amount")]
    public string Amount
    {
        get; set;
    }

    [JsonProperty("price")]
    public string Price
    {
        get; set;
    }

    [JsonProperty("side")]
    public string Side
    {
        get; set;
    }

    [JsonProperty("type")]
    public string Type
    {
        get; set;
    }

    public OrderParam(string pair, string amount, string price, string side, string type)
    {
        Pair = pair;
        Amount = amount;
        Price = price;
        Side = side;
        Type = type;
    }
}

// パラメーター用クラス Jsonシリアライズ用
[JsonObject]
public class PairOrderIdParam
{
    [JsonProperty("pair")]
    public string pair
    {
        get; set;
    }

    [JsonProperty("order_id")]
    public ulong order_id
    {
        get; set;
    }

    public PairOrderIdParam(string pair, ulong orderID)
    {
        this.pair = pair;
        order_id = orderID;
    }
}

// パラメーター用クラス Jsonシリアライズ用
[JsonObject]
public class PairOrderIdList
{
    [JsonProperty("pair")]
    public string Pair
    {
        get; set;
    }

    [JsonProperty("order_ids")]
    public List<ulong> OrderIds
    {
        get; set;
    }

    public PairOrderIdList(string pair, List<ulong> orderIds)
    {
        Pair = pair;
        OrderIds = orderIds;
    }
}
