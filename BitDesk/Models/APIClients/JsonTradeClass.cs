namespace BitDesk.Models.APIClients;

public class JsonTrade
{
    public ulong trade_id { get; set; }
    public string? pair { get; set; }
    public ulong order_id { get; set; }
    public string? side { get; set; }
    public string? type { get; set; }
    public string? amount { get; set; }
    public string? price { get; set; }
    public string? maker_taker { get; set; }
    public string? fee_amount_base { get; set; }
    public string? fee_amount_quote { get; set; }
    public long executed_at { get; set; }
}

public class TradeHistoryData
{
    public List<JsonTrade>? trades { get; set; }
}

public class JsonTradeHistoryObject
{
    public int success { get; set; }

    public TradeHistoryData? data { get; set; }
}
