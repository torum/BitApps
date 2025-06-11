using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitDesk.Models;

public class JsonTrade
{
    public ulong Trade_id { get; set; }
    public string? Pair { get; set; }
    public ulong Order_id { get; set; }
    public string? Side { get; set; }
    public string? Type { get; set; }
    public string? Amount { get; set; }
    public string? Price { get; set; }
    public string? Maker_taker { get; set; }
    //public string? Fee_amount_base { get; set; }
    public string? Fee_amount_quote { get; set; }
    public string? Fee_occurred_amount_quote
    {
        get; set;
    }
    public long Executed_at { get; set; }
}

public class TradeHistoryData
{
    public List<JsonTrade>? Trades { get; set; }
}

public class JsonTradeHistoryObject
{
    public int Success { get; set; }
    public TradeHistoryData? Data { get; set; }
}
