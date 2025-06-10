using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using BitApps.Core.Models;
using BitApps.Core.Models.APIClients;
using BitDesk.Models;
using Newtonsoft.Json;

namespace BitDesk.Models.APIClients;

#region == クラス定義 ==

// TODO: 
public class ResponseBodyWrapper
{
    public string BodyText = string.Empty;
    public ClientError HTTPError = new();
}

// APIリクエストの結果に含めて返すエラー情報保持クラス
public partial class ErrorInfo : ViewModelBase
{
    private string _errorTitle = string.Empty;
    public string ErrorTitle
    {
        get => _errorTitle;
        set
        {
            if (_errorTitle == value)
            {
                return;
            }

            _errorTitle = value;
            NotifyPropertyChanged(nameof(ErrorTitle));
        }
    }

    private int _errorCode;
    public int ErrorCode
    {
        get => _errorCode;
        set
        {
            if (_errorCode == value)
            {
                return;
            }

            _errorCode = value;
            NotifyPropertyChanged(nameof(ErrorCode));
        }
    }

    private string _errorDescription = string.Empty;
    public string ErrorDescription
    {
        get => _errorDescription;
        set
        {
            if (_errorDescription == value)
            {
                return;
            }

            _errorDescription = value;
            NotifyPropertyChanged(nameof(ErrorDescription));
        }
    }

}

// 取引履歴クラス
public class Trade
{
    public ulong TradeID
    {
        get; set;
    }
    public required string Pair
    {
        get; set;
    }
    public ulong OrderID
    {
        get; set;
    }
    public required string Side
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
    public required string Type
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
}

public class TradeHistory
{
    public List<Trade> TradeList
    {
        get; set;
    }

    public TradeHistory()
    {
        TradeList = [];
    }
}

// 注文情報クラス（JsonOrderClassから）
public partial class Order : ViewModelBase
{
    public ulong OrderID
    {
        get; set;
    }
    public string? Pair
    {
        get; set;
    } // btc_jpy, xrp_jpy, ltc_btc, eth_btc, mona_jpy, mona_btc, bcc_jpy, bcc_btc

    private string _side = string.Empty;// buy または sell
    public string Side
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

    private string _type = string.Empty;// limit または market, 指値注文の場合はlimit、成行注文の場合はmarket
    public string Type
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

    public decimal StartAmount
    {
        get; set;
    } // 注文時の数量

    private decimal _remainingAmount;// 未約定の数量
    public decimal RemainingAmount
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

    private decimal _executedAmount;// 約定済み数量
    public decimal ExecutedAmount
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

    private decimal _price;// 注文価格
    public decimal Price
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

    private decimal _averagePrice;// 平均約定価格
    public decimal AveragePrice
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

    private DateTime _orderedAt;// 注文日時(UnixTimeのミリ秒)
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

    private string _status = string.Empty;// 注文ステータス  -  UNFILLED 注文中, PARTIALLY_FILLED 注文中(一部約定), FULLY_FILLED 約定済み, CANCELED_UNFILLED 取消済, CANCELED_PARTIALLY_FILLED 取消済(一部約定)
    public string Status
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
            NotifyPropertyChanged(nameof(IsCancelEnabled));
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
    private decimal _shushi;
    public decimal Shushi
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
    private decimal _actualPrice;
    public decimal ActualPrice
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

    private ErrorInfo _err;
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
        _err = new ErrorInfo();
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
        OrderList = [];
    }
}

public class OrderListResult : Orders
{
    public bool IsSuccess
    {
        get; set;
    }
    public int ApiErrorCode
    {
        get; set;
    }

    public ErrorInfo Err
    {
        get; set;
    }

    public OrderListResult()
    {
        Err = new ErrorInfo();
    }
}

public partial class OrderResult : Order
{
    public bool IsSuccess
    {
        get; set;
    }
    public int ApiErrorCode
    {
        get; set;
    }
}

// 発注時クエリパラメーター用クラス Jsonシリアライズ用
[JsonObject]
public class OrderParam(string pair, string amount, string price, string side, string type)
{
    [JsonProperty("pair")]
    public string Pair
    {
        get; set;
    } = pair;

    [JsonProperty("amount")]
    public string Amount
    {
        get; set;
    } = amount;

    [JsonProperty("price")]
    public string Price
    {
        get; set;
    } = price;

    [JsonProperty("side")]
    public string Side
    {
        get; set;
    } = side;

    [JsonProperty("type")]
    public string Type
    {
        get; set;
    } = type;
}

// パラメーター用クラス Jsonシリアライズ用
[JsonObject]
public class PairOrderIdParam(string pair, ulong orderID)
{
    [JsonProperty("pair")]
    public string Pair
    {
        get; set;
    } = pair;

    [JsonProperty("order_id")]
    public ulong Order_id
    {
        get; set;
    } = orderID;
}

// パラメーター用クラス Jsonシリアライズ用
[JsonObject]
public class PairOrderIdList(string pair, List<ulong> orderIds)
{
    [JsonProperty("pair")]
    public string Pair
    {
        get; set;
    } = pair;

    [JsonProperty("order_ids")]
    public List<ulong> OrderIds
    {
        get; set;
    } = orderIds;
}

#endregion

public class PrivateAPIClient : BaseClient
{
    #region == 変数宣言 ==

    // プライベートAPI　エラーコード
    public Dictionary<int, string> ApiErrorCodesDictionary = new()
            {
                {10000, "URLが存在しません"},
                {10001, "システムエラーが発生しました。サポートにお問い合わせ下さい"},
                {10002, "不正なJSON形式です。送信内容をご確認下さい"},
                {10003, "システムエラーが発生しました。サポートにお問い合わせ下さい"},
                {10005, "タイムアウトエラーが発生しました。しばらく間をおいて再度実行して下さい"},

                // Undocumented
                {10007, "ただいまメンテナンスのため一時サービスを停止しております。 今しばらくお待ちください。"},

                {20001, "API認証に失敗しました"},
                {20002, "APIキーが不正です"},
                {20003, "APIキーが存在しません"},
                {20004, "API Nonceが存在しません"},
                {20005, "APIシグネチャが存在しません"},
                {20011, "２段階認証に失敗しました"},
                {20014, "SMS認証に失敗しました"},
                {30001, "注文数量を指定して下さい"},
                {30006, "注文IDを指定して下さい"},
                {30007, "注文ID配列を指定して下さい"},
                {30009, "銘柄を指定して下さい"},
                {30012, "注文価格を指定して下さい"},
                {30013, "売買どちらかを指定して下さい"},
                {30015, "注文タイプを指定して下さい"},
                {30016, "アセット名を指定して下さい"},
                {30019, "uuidを指定して下さい"},
                {30039, "出金額を指定して下さい"},
                {40001, "注文数量が不正です"},
                {40006, "count値が不正です"},
                {40007, "終了時期が不正です"},
                {40008, "end_id値が不正です"},
                {40009, "from_id値が不正です"},
                {40013, "注文IDが不正です"},
                {40014, "注文ID配列が不正です"},
                {40015, "指定された注文が多すぎます"},
                {40017, "銘柄名が不正です"},
                {40020, "注文価格が不正です"},
                {40021, "売買区分が不正です"},
                {40022, "開始時期が不正です"},
                {40024, "注文タイプが不正です"},
                {40025, "アセット名が不正です"},
                {40028, "uuidが不正です"},
                {40048, "出金額が不正です"},
                {50003, "現在、このアカウントはご指定の操作を実行できない状態となっております。サポートにお問い合わせ下さい"},
                {50004, "現在、このアカウントは仮登録の状態となっております。アカウント登録完了後、再度お試し下さい"},
                {50005, "現在、このアカウントはロックされております。サポートにお問い合わせ下さい"},
                {50006, "現在、このアカウントはロックされております。サポートにお問い合わせ下さい"},
                {50008, "ユーザの本人確認が完了していません"},
                {50009, "ご指定の注文は存在しません"},
                {50010, "ご指定の注文はキャンセルできません"},
                {50011, "APIが見つかりません"},

                {50026, "ご指定の注文は既にキャンセル済みです"},// added 2019-10-18
                {50027, "ご指定の注文は既に約定済みです"},// added 2019-10-18

                {60001, "保有数量が不足しています"},
                {60002, "成行買い注文の数量上限を上回っています"},
                {60003, "指定した数量が制限を超えています"},
                {60004, "指定した数量がしきい値を下回っています"},
                {60005, "指定した価格が上限を上回っています"},
                {60006, "指定した価格が下限を下回っています"},
                {60011, "同時発注制限件数(30件)を上回っています"},
                {70001, "システムエラーが発生しました。サポートにお問い合わせ下さい"},
                {70002, "システムエラーが発生しました。サポートにお問い合わせ下さい"},
                {70003, "システムエラーが発生しました。サポートにお問い合わせ下さい"},
                {70004, "現在取引停止中のため、注文を承ることができません"},
                {70005, "現在買注文停止中のため、注文を承ることができません"},
                {70006, "現在売注文停止中のため、注文を承ることができません"},
                {70009, "ただいま成行注文を一時的に制限しています。指値注文をご利用ください。"},
                {70010, "ただいまシステム負荷が高まっているため、最小注文数量を一時的に引き上げています。"},
                {70011, "ただいまリクエストが混雑してます。しばらく時間を空けてから再度リクエストをお願いします。"},

                {70012, "システムエラーが発生しました。サポートにお問い合わせ下さい。"},// added 2019-10-18
                {70013, "ただいまシステム負荷が高まっているため、注文および注文キャンセルを一時的に制限しています。"},// added 2019-10-18
                {70014, "ただいまシステム負荷が高まっているため、出金申請および出金申請キャンセルを一時的に制限しています。"},// added 2019-10-18
                {70015, "ただいまシステム負荷が高まっているため、貸出申請および貸出申請キャンセルを一時的に制限しています。"},// added 2019-10-18
                {70016, "貸出申請および貸出申請キャンセル停止中のため、リクエストを承ることができません。"},// added 2019-10-18
                {70017, "指定された銘柄は注文停止中のため、リクエストを承ることができません。"},// added 2019-10-18
                {70018, "指定された銘柄は注文およびキャンセル停止中のため、リクエストを承ることができません。"},// added 2019-10-18
                {70019, "注文はキャンセル中です。"},

            };

    // プライベートAPIのベースURL
    private readonly Uri PrivateAPIUri = new("https://api.bitbank.cc/v1");

    // デリゲート
    public delegate void ClinetErrorEvent(PrivateAPIClient sender, ClientError err);

    // エラーイベント
    public event ClinetErrorEvent? ErrorOccured;

    #endregion

    // コンストラクタ
    public PrivateAPIClient()
    {
        _HTTPConn.Client.BaseAddress = PrivateAPIUri;
    }

    #region == メソッド ==

    // 資産残高取得メソッド
    public async Task<List<Asset>?> GetAssetList(string _ApiKey, string _ApiSecret)
    {
        var path = new Uri("/user/assets", UriKind.Relative);

        //string json = await Send(path, _ApiKey, _ApiSecret, HttpMethod.Get);
        var resbo = await Send(path, _ApiKey, _ApiSecret, HttpMethod.Get);
        if (resbo == null)
        {
            System.Diagnostics.Debug.WriteLine("■■■■■ GetAssetList: Send returned NULL.");
            return null;
        }
        var json = resbo.BodyText;

        if (!string.IsNullOrEmpty(json))
        {
            var deserialized = JsonAssetResult.FromJson(json);

            if (deserialized?.Success > 0)
            {
                if (deserialized.Data?.Assets != null)
                {
                    List<Asset> asts = [];

                    foreach (var ast in deserialized.Data.Assets)
                    {
                        asts.Add(new Asset
                        {
                            Name = ast.AssetAsset ?? string.Empty,
                            Amount = decimal.Parse(ast.OnhandAmount ?? string.Empty),
                            FreeAmount = decimal.Parse(ast.FreeAmount ?? string.Empty)
                        });

                    }

                    return asts;
                }
                else
                {
                    return null;
                }

            }
            else
            {
                var jsonResult = JsonConvert.DeserializeObject<JsonErrorObject>(json);

                System.Diagnostics.Debug.WriteLine("■■■■■ GetAssetList: API error code - " + jsonResult?.Data?.Code.ToString() + " ■■■■■");

                // エラーイベント発火
                var er = new ClientError
                {
                    ErrType = "API",
                    ErrCode = jsonResult?.Data?.Code ?? 0
                };
                if (ApiErrorCodesDictionary.ContainsKey(jsonResult?.Data?.Code ?? 0))
                {
                    er.ErrText = "「" + ApiErrorCodesDictionary[jsonResult?.Data?.Code ?? 0] + "」";
                }
                er.ErrDatetime = DateTime.Now;
                er.ErrPlace = path.ToString();

                ErrorOccured?.Invoke(this, er);

                return null;
            }
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("■■■■■ GetAssetList: Send returned NULL.");

            // HTTPエラー情報を返しても使わないので、Nullを返す。

            return null;
        }

    }

    // 注文発注メソッド
    public async Task<OrderResult?> MakeOrder(string _ApiKey, string _ApiSecret, string pair, decimal amount, decimal price, string side, string type)
    {
        //パラメータ 
        // https://docs.bitbank.cc/#/Order
        /*
            { "pair", "btc_jpy" },//取引する通貨の種類
            { "amount", "0.01" },//ビットコインの注文量
            { "price", "100000" },//ビットコインのレート
            { "side", "buy" },//注文の売買の種類（買い:buy, 売り:sell）
            { "type", "limit" },//指値注文の場合はlimit、成行注文の場合はmarket）
        */
        //System.Diagnostics.Debug.WriteLine("MakingOrder...");

        /*
        ///////////////////////////////////
        // test data
        Order ord = new Order();
        ord.orderID = 1234;
        ord.pair = pair;
        ord.side = side;
        ord.type = type;
        ord.startAmount = amount;
        ord.remainingAmount = 0.001M;
        ord.executedAmount = 0.001M;
        ord.price = price;
        ord.averagePrice = 840001M;
        ord.orderedAt = DateTime.Now;
        ord.status = "UNFILLED";

        return ord;
        ///////////////////////////////////
        */

        var path = new Uri("/user/spot/order", UriKind.Relative);//APIの通信URL

        var orderParam = new OrderParam(pair, amount.ToString(), price.ToString(), side, type);

        var body = JsonConvert.SerializeObject(orderParam);

        //System.Diagnostics.Debug.WriteLine("MakingOrder... resquest body = " + body);

        try
        {
            //string json = await Send(path, _ApiKey, _ApiSecret, HttpMethod.Post, body, null);
            var resbo = await Send(path, _ApiKey, _ApiSecret, HttpMethod.Post, body);
            if (resbo == null)
            {
                System.Diagnostics.Debug.WriteLine("■■■■■ MakeOrder: Send returned NULL.");
                return null;
            }
            var json = resbo.BodyText;

            if (!string.IsNullOrEmpty(json))
            {
                //System.Diagnostics.Debug.WriteLine("MakeOrder result: " + json);

                var deserialized = JsonConvert.DeserializeObject<JsonOrderObject>(json);

                var ord = new OrderResult();

                if (deserialized?.Success > 0)
                {
                    if (deserialized.Data != null)
                    {
                        ord.OrderID = deserialized.Data.Order_id;
                        ord.Pair = deserialized.Data.Pair ?? string.Empty;
                        ord.Side = deserialized.Data.Side ?? string.Empty;
                        ord.Type = deserialized.Data.Type ?? string.Empty;

                        ord.StartAmount = decimal.Parse(deserialized.Data.Start_amount ?? "0");
                        ord.RemainingAmount = decimal.Parse(deserialized.Data.Remaining_amount ?? "0");
                        ord.ExecutedAmount = decimal.Parse(deserialized.Data.Executed_amount ?? "0");
                        if (deserialized.Data.Price != null)
                        {
                            ord.Price = decimal.Parse(deserialized.Data.Price);
                        }
                        ord.AveragePrice = decimal.Parse(deserialized.Data.Average_price ?? "0");

                        var start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                        var date = start.AddMilliseconds(deserialized.Data.Ordered_at).ToLocalTime();
                        ord.OrderedAt = date;

                        ord.Status = deserialized.Data.Status ?? string.Empty;

                        ord.IsSuccess = true;
                        ord.HasErrorInfo = false;
                    }

                    return ord;
                }
                else
                {
                    var jsonResult = JsonConvert.DeserializeObject<JsonErrorObject>(json);

                    ord.IsSuccess = false;
                    ord.ApiErrorCode = jsonResult?.Data?.Code ?? 0;

                    System.Diagnostics.Debug.WriteLine("■■■■■ MakingOrder: API error code - " + jsonResult?.Data?.Code.ToString() + " ■■■■■");

                    // ユーザに表示するエラー情報
                    ord.HasErrorInfo = true;
                    ord.Err.ErrorCode = ord.ApiErrorCode;
                    ord.Err.ErrorTitle = "発注処理でエラーが返りました。";

                    if (ApiErrorCodesDictionary.TryGetValue(ord.ApiErrorCode, out var value))
                    {
                        ord.Err.ErrorDescription = value;
                    }
                    ord.Err.ErrorDescription = ord.Err.ErrorDescription + "(エラーコード：" + ord.ApiErrorCode.ToString() + ")";

                    // エラーイベント発火
                    var er = new ClientError
                    {
                        ErrType = "API",
                        ErrCode = jsonResult?.Data?.Code ?? 0
                    };
                    if (ApiErrorCodesDictionary.ContainsKey(jsonResult?.Data?.Code ?? 0))
                    {
                        er.ErrText = "「" + ApiErrorCodesDictionary[jsonResult?.Data?.Code ?? 0] + "」";
                    }
                    er.ErrDatetime = DateTime.Now;
                    er.ErrPlace = path.ToString();

                    ErrorOccured?.Invoke(this, er);

                    return ord;
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("■■■■■ MakeOrder: Send returned Empty String...Could be HTTP error");

                var ord = new OrderResult
                {
                    IsSuccess = false,
                    ApiErrorCode = -1,
                    HasErrorInfo = true
                };

                ord.Err.ErrorTitle = "発注処理でHTTPエラーが返りました。";
                ord.Err.ErrorCode = resbo.HTTPError.ErrCode;
                ord.Err.ErrorDescription = "/user/spot/order への発注処理で、HTTPエラー（" + resbo.HTTPError.ErrCode.ToString() + "）が返りました。";

                return ord;
            }
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine("■■■■■ MakeOrder Exception: " + e + " ■■■■■");

            // TODO: これはClientErrorの方を発火すべきでは？
            var ord = new OrderResult
            {
                IsSuccess = false,
                ApiErrorCode = -1,
                HasErrorInfo = true
            };

            ord.Err.ErrorTitle = "発注処理で例外処理が発生しました。";
            ord.Err.ErrorCode = -1;
            ord.Err.ErrorDescription = "/user/spot/order への発注処理で、Exception（" + e + "）発生しました。";

            return ord;
        }
    }

    // 注文情報をIDから取得メソッド
    public async Task<OrderResult?> GetOrderByID(string _ApiKey, string _ApiSecret, string pair, ulong orderID)
    {
        var path = new Uri("/user/spot/order", UriKind.Relative);

        var param = new Dictionary<string, string>{
                { "pair", pair },//取引する通貨の種類
                { "order_id", orderID.ToString() },
            };

        //string json = await Send(path, _ApiKey, _ApiSecret, HttpMethod.Get, "", param);
        var resbo = await Send(path, _ApiKey, _ApiSecret, HttpMethod.Get, "", param);
        if (resbo == null)
        {
            System.Diagnostics.Debug.WriteLine("■■■■■ GetOrderByID: Send returned NULL.");
            return null;
        }
        var json = resbo.BodyText;

        if (!string.IsNullOrEmpty(json))
        {
            //System.Diagnostics.Debug.WriteLine("GetOrderByID: " + json);

            var deserialized = JsonConvert.DeserializeObject<JsonOrderObject>(json);

            if (deserialized?.Success > 0)
            {
                var ord = new OrderResult
                {
                    IsSuccess = true
                };

                if (deserialized.Data != null)
                {
                    ord.OrderID = deserialized.Data.Order_id;
                    ord.Pair = deserialized.Data.Pair;
                    ord.Side = deserialized.Data.Status ?? string.Empty;
                    ord.Type = deserialized.Data.Status ?? string.Empty;

                    ord.StartAmount = decimal.Parse(deserialized.Data.Start_amount ?? "0");
                    ord.RemainingAmount = decimal.Parse(deserialized.Data.Remaining_amount ?? "0");
                    ord.ExecutedAmount = decimal.Parse(deserialized.Data.Executed_amount ?? "0");
                    if (deserialized.Data.Price != null)
                    {
                        ord.Price = decimal.Parse(deserialized.Data.Price);
                    }
                    ord.AveragePrice = decimal.Parse(deserialized.Data.Average_price ?? "0");

                    var start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    var date = start.AddMilliseconds(deserialized.Data.Ordered_at).ToLocalTime();
                    ord.OrderedAt = date;

                    ord.Status = deserialized.Data.Status ?? string.Empty;
                }

                return ord;
            }
            else
            {
                var ord = new OrderResult();

                var jsonResult = JsonConvert.DeserializeObject<JsonErrorObject>(json);

                ord.IsSuccess = false;
                ord.ApiErrorCode = jsonResult?.Data?.Code ?? 0;

                System.Diagnostics.Debug.WriteLine("■■■■■ GetOrderByID: API error code - " + jsonResult?.Data?.Code.ToString() + " ■■■■■");

                // ユーザに表示するエラー情報
                ord.HasErrorInfo = true;
                ord.Err.ErrorCode = ord.ApiErrorCode;
                ord.Err.ErrorTitle = "注文情報を取得中にエラーが返りました。";

                if (ApiErrorCodesDictionary.TryGetValue(ord.ApiErrorCode, out var value))
                {
                    ord.Err.ErrorDescription = value;
                }
                ord.Err.ErrorDescription = ord.Err.ErrorDescription + "(エラーコード：" + ord.ApiErrorCode.ToString() + ")";

                // エラーイベント発火
                var er = new ClientError
                {
                    ErrType = "API",
                    ErrCode = jsonResult?.Data?.Code ?? 0
                };
                if (ApiErrorCodesDictionary.ContainsKey(jsonResult?.Data?.Code ?? 0))
                {
                    er.ErrText = "「" + ApiErrorCodesDictionary[jsonResult?.Data?.Code ?? 0] + "」";
                }
                er.ErrDatetime = DateTime.Now;
                er.ErrPlace = path.ToString();

                ErrorOccured?.Invoke(this, er);

                return ord;
            }
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("■■■■■ GetOrderByID: Send returned empty string. Could be a HTTP error.");

            var ord = new OrderResult
            {
                IsSuccess = false,
                ApiErrorCode = -1,
                HasErrorInfo = true
            };

            ord.Err.ErrorTitle = "注文情報を取得処理でHTTPエラーが返りました。";
            ord.Err.ErrorCode = resbo.HTTPError.ErrCode;
            ord.Err.ErrorDescription = "/user/spot/order への注文情報取得処理で、HTTPエラー（" + resbo.HTTPError.ErrCode.ToString() + "）が返りました。";

            return ord;
        }
    }

    // 注文情報を複数のIDから取得メソッド
    public async Task<OrderListResult?> GetOrderListByIDs(string _ApiKey, string _ApiSecret, string pair, List<ulong> orderIDs)
    {
        var path = new Uri("/user/spot/orders_info", UriKind.Relative);

        var idParam = new PairOrderIdList(pair, orderIDs);
        var body = JsonConvert.SerializeObject(idParam);

        //string json = await Send(path, _ApiKey, _ApiSecret, HttpMethod.Post, body);
        var resbo = await Send(path, _ApiKey, _ApiSecret, HttpMethod.Post, body);
        if (resbo == null)
        {
            System.Diagnostics.Debug.WriteLine("■■■■■ GetOrderListByIDs: Send returned NULL.");
            return null;
        }
        var json = resbo.BodyText;

        try
        {
            if (!string.IsNullOrEmpty(json))
            {
                //System.Diagnostics.Debug.WriteLine("GetOrderListByIDs: " + json);

                var deserialized = JsonConvert.DeserializeObject<JsonOrderInfoObject>(json);

                if (deserialized?.Success > 0)
                {
                    var ords = new OrderListResult
                    {
                        IsSuccess = true
                    };

                    if (deserialized.Data != null)
                    {
                        if (deserialized.Data.Orders != null)
                        {
                            foreach (var ord in deserialized.Data.Orders)
                            {
                                try
                                {
                                    var o = new Order
                                    {
                                        OrderID = ord.Order_id,
                                        Pair = ord.Pair,
                                        Side = ord.Side ?? string.Empty,
                                        Type = ord.Type ?? string.Empty,
                                        OrderedAt = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds(ord.Ordered_at).ToLocalTime(),
                                        Status = ord.Status ?? string.Empty
                                    };

                                    if (!string.IsNullOrEmpty(ord.Start_amount))
                                    {
                                        o.StartAmount = decimal.Parse(ord.Start_amount);
                                    }

                                    if (!string.IsNullOrEmpty(ord.Remaining_amount))
                                    {
                                        o.RemainingAmount = decimal.Parse(ord.Remaining_amount);
                                    }

                                    if (!string.IsNullOrEmpty(ord.Executed_amount))
                                    {
                                        o.ExecutedAmount = decimal.Parse(ord.Executed_amount);
                                    }

                                    if (!string.IsNullOrEmpty(ord.Price))
                                    {
                                        o.Price = decimal.Parse(ord.Price);
                                    }

                                    if (!string.IsNullOrEmpty(ord.Average_price))
                                    {
                                        o.AveragePrice = decimal.Parse(ord.Average_price);
                                    }

                                    ords.OrderList.Add(o);

                                    //System.Diagnostics.Debug.WriteLine("GetOrderListByIDs ord.status: " + ord.status);
                                }
                                catch
                                {
                                    System.Diagnostics.Debug.WriteLine("■■■■■ GGetOrderListByIDs - ords.OrderList.Add ■■■■■");
                                }
                            }
                        }
                    }

                    return ords;
                }
                else
                {
                    var jsonResult = JsonConvert.DeserializeObject<JsonErrorObject>(json);

                    System.Diagnostics.Debug.WriteLine("■■■■■ GetOrderListByIDs: API error code - " + jsonResult?.Data?.Code.ToString() + " ■■■■■");

                    // エラーイベント発火
                    var er = new ClientError
                    {
                        ErrType = "API",
                        ErrCode = jsonResult?.Data?.Code ?? 0
                    };
                    if (ApiErrorCodesDictionary.ContainsKey(jsonResult?.Data?.Code ?? 0))
                    {
                        er.ErrText = "「" + ApiErrorCodesDictionary[jsonResult?.Data?.Code ?? 0] + "」";
                    }
                    er.ErrDatetime = DateTime.Now;
                    er.ErrPlace = path.ToString();
                    er.ErrPlaceParent = "GetOrderListByIDs";
                    er.ErrEx = "注文情報を更新出来ませんでした";

                    ErrorOccured?.Invoke(this, er);

                    return null;
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("■■■■■ GetOrderListByIDs: Send returned empty string.");

                var ords = new OrderListResult
                {
                    IsSuccess = false,
                    ApiErrorCode = -1
                };

                ords.Err.ErrorTitle = "注文情報取得（複数）処理でHTTPエラーが返りました。";
                ords.Err.ErrorCode = resbo.HTTPError.ErrCode;
                ords.Err.ErrorDescription = "/user/spot/orders_info への発注処理で、HTTPエラー（" + resbo.HTTPError.ErrCode.ToString() + "）が返りました。";

                return ords;
            }
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine("■■■■■ GetOrderListByIDs: Exception " + e);

            // TODO: これはClientErrorの方を発火すべきでは？
            var ord = new OrderListResult
            {
                IsSuccess = false,
                ApiErrorCode = -1
            };

            ord.Err.ErrorTitle = "発注処理で例外処理が発生しました。";
            ord.Err.ErrorCode = -1;
            ord.Err.ErrorDescription = "/user/spot/orders_info への情報取得処理で、Exception（" + e + "）発生しました。";

            return ord;
        }
    }

    // 注文キャンセルメソッド
    public async Task<OrderResult?> CancelOrder(string _ApiKey, string _ApiSecret, string pair, ulong orderID)
    {
        var path = new Uri("/user/spot/cancel_order", UriKind.Relative);

        var cancelOrderParam = new PairOrderIdParam(pair, orderID);

        var body = JsonConvert.SerializeObject(cancelOrderParam);

        //string json = await Send(path, _ApiKey, _ApiSecret, HttpMethod.Post, body);
        var resbo = await Send(path, _ApiKey, _ApiSecret, HttpMethod.Post, body);
        if (resbo == null)
        {
            System.Diagnostics.Debug.WriteLine("■■■■■ CancelOrder: Send returned NULL.");
            return null;
        }
        var json = resbo.BodyText;

        if (!string.IsNullOrEmpty(json))
        {
            //System.Diagnostics.Debug.WriteLine("CancelOrder: " + json);

            var deserialized = JsonConvert.DeserializeObject<JsonOrderObject>(json);

            var ord = new OrderResult();

            if (deserialized?.Success > 0)
            {
                if (deserialized.Data != null)
                {
                    ord.OrderID = deserialized.Data.Order_id;
                    ord.Pair = deserialized.Data.Pair;
                    ord.Side = deserialized.Data.Side ?? string.Empty;
                    ord.Type = deserialized.Data.Type ?? string.Empty;

                    //TODO エラーハンドリング
                    ord.StartAmount = decimal.Parse(deserialized.Data.Start_amount ?? "0");
                    ord.RemainingAmount = decimal.Parse(deserialized.Data.Remaining_amount ?? "0");
                    ord.ExecutedAmount = decimal.Parse(deserialized.Data.Executed_amount ?? "0");
                    ord.Price = decimal.Parse(deserialized.Data.Price ?? "0");
                    ord.AveragePrice = decimal.Parse(deserialized.Data.Average_price ?? "0");

                    var start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    var date = start.AddMilliseconds(deserialized.Data.Ordered_at).ToLocalTime();
                    ord.OrderedAt = date;

                    ord.Status = deserialized.Data.Status ?? string.Empty;

                    ord.IsSuccess = true;
                    ord.HasErrorInfo = false;
                }

                return ord;
            }
            else
            {
                var jsonResult = JsonConvert.DeserializeObject<JsonErrorObject>(json);

                ord.IsSuccess = false;
                ord.ApiErrorCode = jsonResult?.Data?.Code ?? 0;

                // ユーザに表示するエラー情報
                ord.HasErrorInfo = true;
                ord.Err.ErrorCode = ord.ApiErrorCode;
                ord.Err.ErrorTitle = "注文キャンセル処理でエラーが返りました。";

                if (ApiErrorCodesDictionary.TryGetValue(ord.ApiErrorCode, out var value))
                {
                    ord.Err.ErrorDescription = value;
                }
                ord.Err.ErrorDescription = ord.Err.ErrorDescription + "(エラーコード：" + ord.ApiErrorCode.ToString() + ")";


                // エラーイベント発火
                var er = new ClientError
                {
                    ErrType = "API",
                    ErrCode = jsonResult?.Data?.Code ?? 0
                };
                if (ApiErrorCodesDictionary.ContainsKey(jsonResult?.Data?.Code ?? 0))
                {
                    er.ErrText = "「" + ApiErrorCodesDictionary[jsonResult?.Data?.Code ?? 0] + "」";
                }
                er.ErrDatetime = DateTime.Now;
                er.ErrPlace = path.ToString();

                ErrorOccured?.Invoke(this, er);

                System.Diagnostics.Debug.WriteLine("■■■■■ CancelOrder: API error code - " + jsonResult?.Data?.Code.ToString() + " ■■■■■");

                return ord;
            }
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("■■■■■ CancelOrder: Send returned NULL.");

            var ord = new OrderResult
            {
                IsSuccess = false,
                ApiErrorCode = -1,
                HasErrorInfo = true
            };

            ord.Err.ErrorTitle = "注文キャンセル処理でHTTPエラーが返りました。";
            ord.Err.ErrorCode = resbo.HTTPError.ErrCode;
            ord.Err.ErrorDescription = "/user/spot/cancel_order への注文キャンセル処理で、HTTPエラー（" + resbo.HTTPError.ErrCode.ToString() + "）が返りました。";

            return ord;
        }
    }

    // 文キャンセル（複数）メソッド
    public async Task<OrderListResult?> CancelOrders(string _ApiKey, string _ApiSecret, string pair, List<ulong> orderIDs)
    {
        var path = new Uri("/user/spot/cancel_orders", UriKind.Relative);

        var idParam = new PairOrderIdList(pair, orderIDs);
        var body = JsonConvert.SerializeObject(idParam);

        //string json = await Send(path, _ApiKey, _ApiSecret, HttpMethod.Post, body);
        var resbo = await Send(path, _ApiKey, _ApiSecret, HttpMethod.Post, body);
        if (resbo == null)
        {
            System.Diagnostics.Debug.WriteLine("■■■■■ CancelOrders: Send returned NULL.");
            return null;
        }
        var json = resbo.BodyText;

        try
        {
            if (!string.IsNullOrEmpty(json))
            {
                //System.Diagnostics.Debug.WriteLine("CancelOrders: " + json);

                var deserialized = JsonConvert.DeserializeObject<JsonOrderInfoObject>(json);

                if (deserialized?.Success > 0)
                {
                    var ords = new OrderListResult
                    {
                        IsSuccess = true
                    };

                    if (deserialized.Data != null)
                    {
                        if (deserialized.Data.Orders != null)
                        {
                            foreach (var ord in deserialized.Data.Orders)
                            {
                                try
                                {
                                    var o = new Order
                                    {
                                        OrderID = ord.Order_id,
                                        Pair = ord.Pair,
                                        Side = ord.Side ?? string.Empty,
                                        Type = ord.Type ?? string.Empty,
                                        OrderedAt = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds(ord.Ordered_at).ToLocalTime(),
                                        Status = ord.Status ?? string.Empty
                                    };

                                    if (!string.IsNullOrEmpty(ord.Start_amount))
                                    {
                                        o.StartAmount = decimal.Parse(ord.Start_amount);
                                    }

                                    if (!string.IsNullOrEmpty(ord.Remaining_amount))
                                    {
                                        o.RemainingAmount = decimal.Parse(ord.Remaining_amount);
                                    }

                                    if (!string.IsNullOrEmpty(ord.Executed_amount))
                                    {
                                        o.ExecutedAmount = decimal.Parse(ord.Executed_amount);
                                    }

                                    if (!string.IsNullOrEmpty(ord.Price))
                                    {
                                        o.Price = decimal.Parse(ord.Price);
                                    }

                                    if (!string.IsNullOrEmpty(ord.Average_price))
                                    {
                                        o.AveragePrice = decimal.Parse(ord.Average_price);
                                    }

                                    ords.OrderList.Add(o);

                                    //System.Diagnostics.Debug.WriteLine("CancelOrders ord.status: " + ord.status);
                                }
                                catch
                                {
                                    System.Diagnostics.Debug.WriteLine("■■■■■ CancelOrders - ords.OrderList.Add ■■■■■");
                                }
                            }
                        }
                    }

                    return ords;
                }
                else
                {
                    var jsonResult = JsonConvert.DeserializeObject<JsonErrorObject>(json);

                    System.Diagnostics.Debug.WriteLine("■■■■■ CancelOrders: API error code - " + jsonResult?.Data?.Code.ToString() + " ■■■■■");

                    // エラーイベント発火
                    var er = new ClientError
                    {
                        ErrType = "API",
                        ErrCode = jsonResult?.Data?.Code ?? 0
                    };
                    if (ApiErrorCodesDictionary.ContainsKey(jsonResult?.Data?.Code ?? 0))
                    {
                        er.ErrText = "「" + ApiErrorCodesDictionary[jsonResult?.Data?.Code ?? 0] + "」";
                    }
                    er.ErrDatetime = DateTime.Now;
                    er.ErrPlace = path.ToString();

                    ErrorOccured?.Invoke(this, er);

                    return null;
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("■■■■■ CancelOrders: Send returned NULL.");

                var ords = new OrderListResult
                {
                    IsSuccess = false,
                    ApiErrorCode = -1
                };

                ords.Err.ErrorTitle = "注文キャンセル処理（複数）でHTTPエラーが返りました。";
                ords.Err.ErrorCode = resbo.HTTPError.ErrCode;
                ords.Err.ErrorDescription = "/user/spot/cancel_orders への処理で、HTTPエラー（" + resbo.HTTPError.ErrCode.ToString() + "）が返りました。";

                return ords;
            }
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine("■■■■■ CancelOrders: Exception " + e);

            // TODO: これはClientErrorの方を発火すべきでは？
            var ord = new OrderListResult
            {
                IsSuccess = false,
                ApiErrorCode = -1
            };

            ord.Err.ErrorTitle = "注文キャンセル処理（複数）で例外処理が発生しました。";
            ord.Err.ErrorCode = -1;
            ord.Err.ErrorDescription = "/user/spot/cancel_orders への情報取得処理で、Exception（" + e + "）発生しました。";

            return ord;
        }
    }

    // 注文リスト取得メソッド
    public async Task<OrderListResult?> GetOrderList(string _ApiKey, string _ApiSecret, string pair)
    {
        var path = new Uri("/user/spot/active_orders", UriKind.Relative);

        var param = new Dictionary<string, string>{
                { "pair", pair },//取引する通貨の種類
                //{ "count", "10" },//取得する注文数（double）
                //{ "from_id", "100000" },//取得開始注文ID
                //{ "end_id", "100000" },//取得終了注文ID
                //{ "since", "100000" },//開始UNIXタイムスタンプ
                //{ "end", "0.01" },//終了UNIXタイムスタンプ
            };

        //string json = await Send(path, _ApiKey, _ApiSecret, HttpMethod.Get,"", param);
        var resbo = await Send(path, _ApiKey, _ApiSecret, HttpMethod.Get, "", param);
        if (resbo == null)
        {
            System.Diagnostics.Debug.WriteLine("■■■■■ GetOrderList: Send returned NULL.");
            return null;
        }
        var json = resbo.BodyText;

        if (!string.IsNullOrEmpty(json))
        {
            //System.Diagnostics.Debug.WriteLine("■■■■■ ■■■■■ ■■■■■ GetOrderList: " + json);

            var deserialized = JsonConvert.DeserializeObject<JsonOrderInfoObject>(json);

            if (deserialized?.Success > 0)
            {
                var ords = new OrderListResult
                {
                    IsSuccess = true
                };

                if (deserialized?.Data?.Orders != null)
                {
                    foreach (var ord in deserialized.Data.Orders)
                    {
                        var o = new Order
                        {
                            OrderID = ord.Order_id,
                            Pair = ord.Pair,
                            Side = ord.Side ?? string.Empty,
                            Type = ord.Type ?? string.Empty,
                            OrderedAt = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds(ord.Ordered_at).ToLocalTime(),
                            Status = ord.Status ?? string.Empty
                        };

                        if (!string.IsNullOrEmpty(ord.Start_amount))
                        {
                            o.StartAmount = decimal.Parse(ord.Start_amount);
                        }

                        if (!string.IsNullOrEmpty(ord.Remaining_amount))
                        {
                            o.RemainingAmount = decimal.Parse(ord.Remaining_amount);
                        }

                        if (!string.IsNullOrEmpty(ord.Executed_amount))
                        {
                            o.ExecutedAmount = decimal.Parse(ord.Executed_amount);
                        }

                        if (!string.IsNullOrEmpty(ord.Price)) { o.Price = decimal.Parse(ord.Price); } else { o.Price = 0; }
                        if (!string.IsNullOrEmpty(ord.Average_price))
                        {
                            o.AveragePrice = decimal.Parse(ord.Average_price);
                        }

                        ords.OrderList.Add(o);
                    }
                }
                return ords;
            }
            else
            {
                var jsonResult = JsonConvert.DeserializeObject<JsonErrorObject>(json);

                System.Diagnostics.Debug.WriteLine("■■■■■ GetOrderList: API error code - " + jsonResult?.Data?.Code.ToString() + " ■■■■■");

                // エラーイベント発火
                var er = new ClientError
                {
                    ErrType = "API",
                    ErrCode = jsonResult?.Data?.Code ?? 0
                };
                if (ApiErrorCodesDictionary.ContainsKey(jsonResult?.Data?.Code ?? 0))
                {
                    er.ErrText = "「" + ApiErrorCodesDictionary[jsonResult?.Data?.Code ?? 0] + "」";
                }
                er.ErrDatetime = DateTime.Now;
                er.ErrPlace = path.ToString();

                ErrorOccured?.Invoke(this, er);

                return null;
            }
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("■■■■■ GetOrderList: Send returned NULL.");

            var ords = new OrderListResult
            {
                IsSuccess = false,
                ApiErrorCode = -1
            };

            ords.Err.ErrorTitle = "注文リスト取得でHTTPエラーが返りました。";
            ords.Err.ErrorCode = resbo.HTTPError.ErrCode;
            ords.Err.ErrorDescription = "/user/spot/active_orders への注文リスト取得処理で、HTTPエラー（" + resbo.HTTPError.ErrCode.ToString() + "）が返りました。";

            return ords;
        }
    }

    // 取引履歴取得メソッド
    public async Task<TradeHistory?> GetTradeHistory(string _ApiKey, string _ApiSecret, string pair)
    {

        var path = new Uri("/user/spot/trade_history", UriKind.Relative);

        var param = new Dictionary<string, string>{
                { "pair", pair },//取得する約定数
                { "count", "10" },//取得する注文数（double）
                //{ "order_id", "100000" },//注文ID
                //{ "since", "100000" },//開始UNIXタイムスタンプ
                //{ "end", "0.01" },//終了UNIXタイムスタンプ
                //{ "order", "asc" },//約定時刻順序(asc: 昇順、desc: 降順、デフォルト降順)
            };

        //string json = await Send(path, _ApiKey, _ApiSecret, HttpMethod.Get, "", param);
        var resbo = await Send(path, _ApiKey, _ApiSecret, HttpMethod.Get, "", param);
        if (resbo == null)
        {
            System.Diagnostics.Debug.WriteLine("■■■■■ GetTradeHistory: Send returned NULL.");
            return null;
        }
        var json = resbo.BodyText;

        if (!string.IsNullOrEmpty(json))
        {
            //System.Diagnostics.Debug.WriteLine("GetTradeHistory: " + json);

            var deserialized = JsonConvert.DeserializeObject<JsonTradeHistoryObject>(json);

            if (deserialized?.Success > 0)
            {
                var history = new TradeHistory();

                if (deserialized?.Data != null)
                {
                    if (deserialized.Data.Trades != null)
                    {
                        foreach (var trd in deserialized.Data.Trades)
                        {
                            var o = new Trade
                            {
                                TradeID = trd.Trade_id,
                                OrderID = trd.Order_id,
                                Pair = trd.Pair ?? string.Empty,
                                Side = trd.Side ?? string.Empty,
                                Type = trd.Type ?? string.Empty,
                                MakerTaker = trd.Maker_taker,
                                ExecutedAt = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds(trd.Executed_at).ToLocalTime(),

                            };

                            if (!string.IsNullOrEmpty(trd.Amount))
                            {
                                o.Amount = decimal.Parse(trd.Amount);
                            }

                            if (!string.IsNullOrEmpty(trd.Price)) { o.Price = decimal.Parse(trd.Price); } else { o.Price = 0; }

                            if (!string.IsNullOrEmpty(trd.Amount))
                            {
                                o.FeeAmountBase = decimal.Parse(trd.Fee_amount_base ?? "0");
                            }

                            if (!string.IsNullOrEmpty(trd.Amount))
                            {
                                o.FeeAmountQuote = decimal.Parse(trd.Fee_amount_quote ?? "0");
                            }

                            history.TradeList.Add(o);
                        }
                    }
                }

                return history;
            }
            else
            {
                var jsonResult = JsonConvert.DeserializeObject<JsonErrorObject>(json);

                System.Diagnostics.Debug.WriteLine("■■■■■ GetTradeHistory: API error code - " + jsonResult?.Data?.Code.ToString() + " ■■■■■");

                // エラーイベント発火
                var er = new ClientError
                {
                    ErrType = "API",
                    ErrCode = jsonResult?.Data?.Code ?? 0
                };
                if (ApiErrorCodesDictionary.ContainsKey(jsonResult?.Data?.Code ?? 0))
                {
                    er.ErrText = "「" + ApiErrorCodesDictionary[jsonResult?.Data?.Code ?? 0] + "」";
                }
                er.ErrDatetime = DateTime.Now;
                er.ErrPlace = path.ToString();

                ErrorOccured?.Invoke(this, er);

                return null;
            }

        }
        else
        {
            System.Diagnostics.Debug.WriteLine("■■■■■ GetTradeHistory: Send returned NULL.");

            // TradeHistryの場合はords返しても使わないので

            return null;
        }

    }

    // HTTPリクエスト送信メソッド
    internal async Task<ResponseBodyWrapper?> Send(Uri path, string apiKey, string secret, HttpMethod method, string body = "", Dictionary<string, string>? queries = null)
    {
        try
        {
            //ACCESS-NONCE
            var _accessNonce = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds().ToString(); //NONCE=unixtime

            // メッセージを作成
            var message = "";
            if (method == HttpMethod.Get)
            {
                if (queries != null)
                {
                    // パラメータ文字列を作成
                    var pms = new FormUrlEncodedContent(queries);
                    var param = await pms.ReadAsStringAsync();

                    message = _accessNonce + "/v1" + path.ToString() + "?" + param;
                }
                else
                {
                    message = _accessNonce + "/v1" + path.ToString();
                }
            }
            else if (method == HttpMethod.Post)
            {
                message = _accessNonce + body;
            }

            // メッセージをHMACSHA256で署名
            var hash = new HMACSHA256(Encoding.UTF8.GetBytes(secret)).ComputeHash(Encoding.UTF8.GetBytes(message));
            //ACCESS-SIGNATURE
            var _accessSignature = BitConverter.ToString(hash).ToLower().Replace("-", "");//バイト配列をを16進文字列へ

            //System.Diagnostics.Debug.WriteLine("Sending..." + Environment.NewLine + _HTTPConn.Client.DefaultRequestHeaders.ToString());

            HttpResponseMessage res;
            var resbowrap = new ResponseBodyWrapper();
            try
            {
                if (method == HttpMethod.Post)
                {
                    var content = new StringContent(body, Encoding.UTF8, "application/json");

                    content.Headers.Add("ACCESS-KEY", apiKey);
                    content.Headers.Add("ACCESS-NONCE", _accessNonce);
                    content.Headers.Add("ACCESS-SIGNATURE", _accessSignature);

                    res = await _HTTPConn.Client.PostAsync(_HTTPConn.Client.BaseAddress?.ToString() + path.ToString(), content);
                }
                else if (method == HttpMethod.Get)
                {

                    if (queries == null)
                    {
                        //
                        var requestMessage = new HttpRequestMessage(HttpMethod.Get, _HTTPConn.Client.BaseAddress?.ToString() + path.ToString());
                        requestMessage.Headers.Add("ACCESS-KEY", apiKey);
                        requestMessage.Headers.Add("ACCESS-NONCE", _accessNonce);
                        requestMessage.Headers.Add("ACCESS-SIGNATURE", _accessSignature);

                        res = await _HTTPConn.Client.SendAsync(requestMessage);
                    }
                    else
                    {
                        var pms = new FormUrlEncodedContent(queries);
                        var param = await pms.ReadAsStringAsync();

                        //
                        var requestMessage = new HttpRequestMessage(HttpMethod.Get, _HTTPConn.Client.BaseAddress?.ToString() + path.ToString() + "?" + param);
                        requestMessage.Headers.Add("ACCESS-KEY", apiKey);
                        requestMessage.Headers.Add("ACCESS-NONCE", _accessNonce);
                        requestMessage.Headers.Add("ACCESS-SIGNATURE", _accessSignature);

                        res = await _HTTPConn.Client.SendAsync(requestMessage);
                    }
                }
                else
                {
                    throw new ArgumentException("method は POST か GET を指定してください。", method.ToString());
                }

                //返答内容を取得
                var text = await res.Content.ReadAsStringAsync();

                //通信上の失敗
                if (!res.IsSuccessStatusCode)
                {
                    //System.Diagnostics.Debug.WriteLine("■■■■■■■■ Send: HTTP Error " + res.StatusCode.ToString() + " " + method + " " + _HTTPConn.Client.BaseAddress.ToString() + path.ToString() + " ■■■■■■■");

                    var er = new ClientError
                    {
                        ErrType = "HTTP " + method,
                        ErrCode = (int)res.StatusCode,
                        ErrText = res.StatusCode.ToString(),
                        ErrDatetime = DateTime.Now,
                        ErrPlace = path.ToString()
                    };
                    // イベント発火
                    ErrorOccured?.Invoke(this, er);

                    //ラッパー
                    resbowrap.BodyText = "";
                    resbowrap.HTTPError = er;

                    return resbowrap;
                    //return "";
                }

                resbowrap.BodyText = text;
                return resbowrap;
                //return text;
            }
            catch (System.Net.Sockets.SocketException ex)
            {
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("HTTP Send: SocketException - " + ex.Message + " + 内部例外: " + ex.InnerException.Message);
                }
                else
                {

                    System.Diagnostics.Debug.WriteLine("HTTP Send: SocketException - " + ex.Message);
                }
                return null;
            }
            catch (System.IO.IOException ex)
            {
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("HTTP Send: IOException - " + ex.Message + " + 内部例外: " + ex.InnerException.Message);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("HTTP Send: IOException - " + ex.Message);
                }
                    return null;
            }
            catch (HttpRequestException ex)
            {
                if (ex.InnerException != null)
                {
                    System.Diagnostics.Debug.WriteLine("HTTP Send: HttpRequestException - " + ex.Message + " + 内部例外: " + ex.InnerException.Message);
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("HTTP Send: HttpRequestException - " + ex.Message);

                }
                return null;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("■■■■■ HTTP GET/POST Error - Exception : " + ex.Message);
            return null;
        }
        finally
        {
            //_httpClientIsBusy = false;
        }

    }

    #endregion
}


