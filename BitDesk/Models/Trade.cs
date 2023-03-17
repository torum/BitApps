namespace BitDesk.Models;

// 取引履歴クラス
public class Trade
{
    public ulong TradeID
    {
        get; set;
    }
    public string? Pair
    {
        get; set;
    }
    public ulong OrderID
    {
        get; set;
    }
    public string? Side
    {
        get; set;
    }
    public string SideText
    {
        get
        {
            if (Side == "buy")
            {
                return "買";
            }
            else if (Side == "sell")
            {
                return "売";
            }
            else
            {
                return "";
            }
        }
    }
    public string? Type
    {
        get; set;
    }
    public string TypeText
    {
        get
        {
            if (Type == "market")
            {
                return "成行";
            }
            else if (Type == "limit")
            {
                return "指値";
            }
            else
            {
                return "";
            }
        }
    }
    public decimal Amount
    {
        get; set;
    }
    public decimal Price
    {
        get; set;
    }
    public string? MakerTaker
    {
        get; set;
    }
    public decimal FeeAmountBase
    {
        get; set;
    }
    public decimal FeeAmountQuote
    {
        get; set;
    }
    public DateTime ExecutedAt
    {
        get; set;
    }

    public Trade()
    {

    }
}

public class TradeHistory
{
    public List<Trade> TradeList
    {
        get; set;
    }

    public TradeHistory()
    {
        TradeList = new List<Trade>();
    }
}