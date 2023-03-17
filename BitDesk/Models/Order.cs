using BitApps.Core.Models;

namespace BitDesk.Models;

// 注文情報クラス（JsonOrderClassから）
public class Order : ViewModelBase
{
    public ulong OrderID
    {
        get; set;
    }

    // btc_jpy, xrp_jpy, ltc_btc, eth_btc, mona_jpy, mona_btc, bcc_jpy, bcc_btc
    public string? Pair
    {
        get; set;
    }

    // buy または sell
    private string? _side;
    public string? Side
    {
        get => _side;
        set
        {
            if (_side == value)
            {
                return;
            }

            _side = value;
            NotifyPropertyChanged(nameof(Side));
            NotifyPropertyChanged(nameof(SideText));
        }
    }

    public string SideText
    {
        get
        {
            if (_side == "buy")
            {
                return "買";
            }
            else if (_side == "sell")
            {
                return "売";
            }
            else
            {
                return "";
            }
        }
    }

    // limit または market, 指値注文の場合はlimit、成行注文の場合はmarket
    private string? _type;
    public string? Type
    {
        get => _type;
        set
        {
            if (_type == value)
            {
                return;
            }

            _type = value;
            NotifyPropertyChanged(nameof(Type));
            NotifyPropertyChanged(nameof(TypeText));
        }
    }

    public string TypeText
    {
        get
        {
            if (_type == "market")
            {
                return "成行";
            }
            else if (_type == "limit")
            {
                return "指値";
            }
            else
            {
                return "";
            }
        }
    }

    // 注文時の数量
    public Decimal StartAmount
    {
        get; set;
    }

    // 未約定の数量
    private Decimal _remainingAmount;
    public Decimal RemainingAmount
    {
        get => _remainingAmount;
        set
        {
            if (_remainingAmount == value)
            {
                return;
            }

            _remainingAmount = value;
            NotifyPropertyChanged(nameof(RemainingAmount));
        }
    }

    // 約定済み数量
    private Decimal _executedAmount;
    public Decimal ExecutedAmount
    {
        get => _executedAmount;
        set
        {
            if (_executedAmount == value)
            {
                return;
            }

            _executedAmount = value;
            NotifyPropertyChanged(nameof(ExecutedAmount));
        }
    }

    // 注文価格
    private Decimal _price;
    public Decimal Price
    {
        get => _price;
        set
        {
            if (_price == value)
            {
                return;
            }

            _price = value;
            NotifyPropertyChanged(nameof(Price));
        }
    }

    // 平均約定価格
    private Decimal _averagePrice;
    public Decimal AveragePrice
    {
        get => _averagePrice;
        set
        {
            if (_averagePrice == value)
            {
                return;
            }

            _averagePrice = value;
            NotifyPropertyChanged(nameof(AveragePrice));
        }
    }

    // 注文日時(UnixTimeのミリ秒)
    private DateTime _orderedAt;
    public DateTime OrderedAt
    {
        get => _orderedAt;
        set
        {
            if (_orderedAt == value)
            {
                return;
            }

            _orderedAt = value;
            NotifyPropertyChanged(nameof(OrderedAt));
        }
    }

    // 注文ステータス  -  UNFILLED 注文中, PARTIALLY_FILLED 注文中(一部約定), FULLY_FILLED 約定済み, CANCELED_UNFILLED 取消済, CANCELED_PARTIALLY_FILLED 取消済(一部約定)
    private string? _status;
    public string? Status
    {
        get => _status;
        set
        {
            if (_status == value)
            {
                return;
            }

            _status = value;

            NotifyPropertyChanged(nameof(Status));
            NotifyPropertyChanged(nameof(StatusText));
            //NotifyPropertyChanged("IsCanceEnabled");
            NotifyPropertyChanged(nameof(OrderIsDone));
        }
    }

    public string StatusText
    {
        get
        {
            if (_status == "UNFILLED")
            {
                return "注文中";
            }
            else if (_status == "PARTIALLY_FILLED")
            {
                return "注文中(一部約定)";
            }
            else if (_status == "FULLY_FILLED")
            {
                return "約定済み";
            }
            else if (_status == "CANCELED_UNFILLED")
            {
                return "取消済";
            }
            else if (_status == "CANCELED_PARTIALLY_FILLED")
            {
                return "取消済(一部約定)";
            }
            else
            {
                return "";
            }
        }
    }

    // 現在値差額表示
    private Decimal _shushi;
    public Decimal Shushi
    {
        get => _shushi;
        set
        {
            if (_shushi == value)
            {
                return;
            }

            _shushi = value;
            NotifyPropertyChanged(nameof(Shushi));
        }
    }

    // 実質価格
    private Decimal _actualPrice;
    public Decimal ActualPrice
    {
        get => _actualPrice;
        set
        {
            if (_actualPrice == value)
            {
                return;
            }

            _actualPrice = value;
            NotifyPropertyChanged(nameof(ActualPrice));
        }
    }

    // キャンセルが効くかどうかのフラグ
    public bool IsCancelEnabled
    {
        get
        {
            if ((_status == "UNFILLED") || (_status == "PARTIALLY_FILLED"))
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    private ErrorInfo _err = new();
    public ErrorInfo Err
    {
        get => _err;
        set
        {
            if (_err == value)
            {
                return;
            }

            _err = value;
            NotifyPropertyChanged(nameof(Err));
        }
    }

    public bool _hasErrorInfo;
    public bool HasErrorInfo
    {
        get => _hasErrorInfo;
        set
        {
            if (_hasErrorInfo == value)
            {
                return;
            }

            _hasErrorInfo = value;
            NotifyPropertyChanged(nameof(HasErrorInfo));
        }
    }

    public bool OrderIsDone
    {
        get
        {
            if (_status == "UNFILLED")
            {
                return false;
            }
            else if (_status == "PARTIALLY_FILLED")
            {
                return false;
            }
            else if (_status == "FULLY_FILLED")
            {
                return true;
            }
            else if (_status == "CANCELED_UNFILLED")
            {
                return true;
            }
            else if (_status == "CANCELED_PARTIALLY_FILLED")
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    public Order()
    {

    }
}

public class Orders
{
    public List<Order> OrderList
    {
        get; set;
    }

    public Orders()
    {
        OrderList = new List<Order>();
    }
}

public class OrderResult : Order
{
    public bool IsSuccess
    {
        get; set;
    }
    public int ErrorCode
    {
        get; set;
    }
}
