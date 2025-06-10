using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BitDesk.Models;

// /user/spot/order
public class JsonOrderData
{
    public ulong Order_id { get; set; }
    public string? Pair { get; set; }
    public string? Side { get; set; }
    public string? Type { get; set; }
    public string? Start_amount { get; set; }
    public string? Remaining_amount { get; set; }
    public string? Executed_amount { get; set; }
    public string? Price { get; set; }
    public string? Average_price { get; set; }
    public long Ordered_at { get; set; }
    public string? Status { get; set; }
}

public class JsonOrderObject
{
    public int Success { get; set; }
    public JsonOrderData? Data { get; set; }
}

// /user/spot/active_orders
public class OrderInfoData
{
    public List<JsonOrderData>? Orders { get; set; }
}

public class JsonOrderInfoObject
{
    public int Success { get; set; }
    public OrderInfoData? Data { get; set; }
}
