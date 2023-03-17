using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Text;
using BitApps.Core.Models.APIClients;
using Newtonsoft.Json;
using BitDesk.Models;

namespace BitDesk.Models.APIClients;

public class PrivateAPIClient : BaseClient
{
    public Dictionary<long, string> ApiErrorCodesDictionary = new()
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

    private readonly Uri PrivateAPIUri = new("https://api.bitbank.cc/v1");

    public delegate void ClinetErrorEvent(PrivateAPIClient sender, ClientError err);

    public event ClinetErrorEvent? ErrorOccured;

    public PrivateAPIClient()
    {
        Client.BaseAddress = PrivateAPIUri;

        Client.DefaultRequestHeaders.Clear();
        Client.DefaultRequestHeaders.ConnectionClose = false;
        //Client.DefaultRequestHeaders.ConnectionClose = true;
        Client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
    }

    #region == メソッド ==

    // 資産残高取得メソッド
    public async Task<Assets?> GetAssetList(string _ApiKey, string _ApiSecret)
    {
        if (Client is null)
        {
            return null;
        }

        if (Client.BaseAddress is null)
        {
            return null;
        }

        var path = new Uri("/user/assets", UriKind.Relative);

        var json = await Send(path, _ApiKey, _ApiSecret, HttpMethod.Get);

        if (!string.IsNullOrEmpty(json))
        {
            //var deserialized = JsonConvert.DeserializeObject<JsonAssetObject>(json);
            var deserialized = JsonAssetObject.FromJson(json);

            if (deserialized.Success > 0)
            {
                var asts = new Assets();

                foreach (var ast in deserialized.Data.Assets)
                {
                    asts.AssetList.Add(new Asset
                    {
                        Name = ast.AssetAsset,
                        Amount = decimal.Parse(ast.OnhandAmount),
                        FreeAmount = decimal.Parse(ast.FreeAmount)
                    });
                }

                return asts;
            }
            else
            {
                var jsonResult = JsonConvert.DeserializeObject<JsonErrorObject>(json);

                System.Diagnostics.Debug.WriteLine("■■■■■ GetAssetList: API error code - " + jsonResult.data.code.ToString() + " ■■■■■");

                // エラーイベント発火
                var er = new ClientError();
                er.ErrType = "API";
                er.ErrCode = jsonResult.data.code;
                if (ApiErrorCodesDictionary.ContainsKey(jsonResult.data.code))
                {
                    er.ErrText = "「" + ApiErrorCodesDictionary[jsonResult.data.code] + "」";
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
            return null;
        }

    }

    // 注文発注メソッド
    public async Task<OrderResult> MakeOrder(string _ApiKey, string _ApiSecret, string pair, Decimal amount, Decimal price, string side, string type)
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
            var json = await Send(path, _ApiKey, _ApiSecret, HttpMethod.Post, body, null);

            if (!string.IsNullOrEmpty(json))
            {
                //System.Diagnostics.Debug.WriteLine("MakeOrder result: " + json);

                var deserialized = JsonConvert.DeserializeObject<JsonOrderObject>(json);

                var ord = new OrderResult();

                if (deserialized.success > 0)
                {
                    ord.OrderID = deserialized.data.order_id;
                    ord.Pair = deserialized.data.pair;
                    ord.Side = deserialized.data.side;
                    ord.Type = deserialized.data.type;

                    ord.StartAmount = decimal.Parse(deserialized.data.start_amount);
                    ord.RemainingAmount = decimal.Parse(deserialized.data.remaining_amount);
                    ord.ExecutedAmount = decimal.Parse(deserialized.data.executed_amount);
                    if (deserialized.data.price != null)
                    {
                        ord.Price = decimal.Parse(deserialized.data.price);
                    }
                    ord.AveragePrice = decimal.Parse(deserialized.data.average_price);

                    var start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                    var date = start.AddMilliseconds(deserialized.data.ordered_at).ToLocalTime();
                    ord.OrderedAt = date;

                    ord.Status = deserialized.data.status;

                    ord.IsSuccess = true;
                    ord.HasErrorInfo = false;

                    return ord;
                }
                else
                {
                    var jsonResult = JsonConvert.DeserializeObject<JsonErrorObject>(json);

                    ord.IsSuccess = false;
                    ord.ErrorCode = jsonResult.data.code;

                    System.Diagnostics.Debug.WriteLine("■■■■■ MakingOrder: API error code - " + jsonResult.data.code.ToString() + " ■■■■■");

                    // ユーザに表示するエラー情報
                    ord.HasErrorInfo = true;
                    ord.Err.ErrorCode = ord.ErrorCode;
                    ord.Err.ErrorTitle = "発注処理でエラーが返りました。";

                    if (ApiErrorCodesDictionary.ContainsKey(ord.ErrorCode))
                    {
                        ord.Err.ErrorDescription = ApiErrorCodesDictionary[ord.ErrorCode];
                    }
                    ord.Err.ErrorDescription = ord.Err.ErrorDescription + "(エラーコード：" + ord.ErrorCode.ToString() + ")";

                    // エラーイベント発火
                    var er = new ClientError();
                    er.ErrType = "API";
                    er.ErrCode = jsonResult.data.code;
                    if (ApiErrorCodesDictionary.ContainsKey(jsonResult.data.code))
                    {
                        er.ErrText = "「" + ApiErrorCodesDictionary[jsonResult.data.code] + "」";
                    }
                    er.ErrDatetime = DateTime.Now;
                    er.ErrPlace = path.ToString();

                    ErrorOccured?.Invoke(this, er);

                    return ord;
                }
            }
            else
            {
                System.Diagnostics.Debug.WriteLine("■■■■■ MakeOrder: Send returned NULL.");
                return null;
            }
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine("■■■■■ MakeOrder Exception: " + e + " ■■■■■");
            return null;
        }
    }

    // 注文情報をIDから取得メソッド
    public async Task<OrderResult> GetOrderByID(string _ApiKey, string _ApiSecret, string pair, ulong orderID)
    {

        var path = new Uri("/user/spot/order", UriKind.Relative);

        var param = new Dictionary<string, string>{
                { "pair", pair },//取引する通貨の種類
                { "order_id", orderID.ToString() },
            };

        var json = await Send(path, _ApiKey, _ApiSecret, HttpMethod.Get, "", param);

        if (!string.IsNullOrEmpty(json))
        {
            //System.Diagnostics.Debug.WriteLine("GetOrderByID: " + json);

            var deserialized = JsonConvert.DeserializeObject<JsonOrderObject>(json);

            if (deserialized.success > 0)
            {
                var ord = new OrderResult();
                ord.IsSuccess = true;

                ord.OrderID = deserialized.data.order_id;
                ord.Pair = deserialized.data.pair;
                ord.Side = deserialized.data.side;
                ord.Type = deserialized.data.type;

                ord.StartAmount = decimal.Parse(deserialized.data.start_amount);
                ord.RemainingAmount = decimal.Parse(deserialized.data.remaining_amount);
                ord.ExecutedAmount = decimal.Parse(deserialized.data.executed_amount);
                if (deserialized.data.price != null)
                {
                    ord.Price = decimal.Parse(deserialized.data.price);
                }
                ord.AveragePrice = decimal.Parse(deserialized.data.average_price);

                var start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                var date = start.AddMilliseconds(deserialized.data.ordered_at).ToLocalTime();
                ord.OrderedAt = date;

                ord.Status = deserialized.data.status;

                return ord;
            }
            else
            {
                var ord = new OrderResult();

                var jsonResult = JsonConvert.DeserializeObject<JsonErrorObject>(json);

                ord.IsSuccess = false;
                ord.ErrorCode = jsonResult.data.code;

                System.Diagnostics.Debug.WriteLine("■■■■■ GetOrderByID: API error code - " + jsonResult.data.code.ToString() + " ■■■■■");

                // ユーザに表示するエラー情報
                ord.HasErrorInfo = true;
                ord.Err.ErrorCode = ord.ErrorCode;
                ord.Err.ErrorTitle = "注文情報を取得中にエラーが返りました。";

                if (ApiErrorCodesDictionary.ContainsKey(ord.ErrorCode))
                {
                    ord.Err.ErrorDescription = ApiErrorCodesDictionary[ord.ErrorCode];
                }
                ord.Err.ErrorDescription = ord.Err.ErrorDescription + "(エラーコード：" + ord.ErrorCode.ToString() + ")";

                // エラーイベント発火
                var er = new ClientError();
                er.ErrType = "API";
                er.ErrCode = jsonResult.data.code;
                if (ApiErrorCodesDictionary.ContainsKey(jsonResult.data.code))
                {
                    er.ErrText = "「" + ApiErrorCodesDictionary[jsonResult.data.code] + "」";
                }
                er.ErrDatetime = DateTime.Now;
                er.ErrPlace = path.ToString();

                ErrorOccured?.Invoke(this, er);

                return ord;
            }
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("■■■■■ GetOrderByID: Send returned NULL.");
            return null;
        }


    }

    // 注文情報を複数のIDから取得メソッド
    public async Task<Orders> GetOrderListByIDs(string _ApiKey, string _ApiSecret, string pair, List<ulong> orderIDs)
    {

        var path = new Uri("/user/spot/orders_info", UriKind.Relative);

        var idParam = new PairOrderIdList(pair, orderIDs);
        var body = JsonConvert.SerializeObject(idParam);

        var json = await Send(path, _ApiKey, _ApiSecret, HttpMethod.Post, body);

        try
        {
            if (!string.IsNullOrEmpty(json))
            {
                //System.Diagnostics.Debug.WriteLine("GetOrderListByIDs: " + json);

                var deserialized = JsonConvert.DeserializeObject<JsonOrderInfoObject>(json);

                if (deserialized.success > 0)
                {
                    var ords = new Orders();

                    foreach (var ord in deserialized.data.orders)
                    {
                        try
                        {
                            var o = new Order
                            {
                                OrderID = ord.order_id,
                                Pair = ord.pair,
                                Side = ord.side,
                                Type = ord.type,
                                OrderedAt = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds(ord.ordered_at).ToLocalTime(),
                                Status = ord.status
                            };

                            if (!string.IsNullOrEmpty(ord.start_amount))
                                o.StartAmount = decimal.Parse(ord.start_amount);
                            if (!string.IsNullOrEmpty(ord.remaining_amount))
                                o.RemainingAmount = decimal.Parse(ord.remaining_amount);
                            if (!string.IsNullOrEmpty(ord.executed_amount))
                                o.ExecutedAmount = decimal.Parse(ord.executed_amount);
                            if (!string.IsNullOrEmpty(ord.price))
                                o.Price = decimal.Parse(ord.price);
                            if (!string.IsNullOrEmpty(ord.average_price))
                                o.AveragePrice = decimal.Parse(ord.average_price);

                            ords.OrderList.Add(o);

                            //System.Diagnostics.Debug.WriteLine("GetOrderListByIDs ord.status: " + ord.status);
                        }
                        catch
                        {
                            System.Diagnostics.Debug.WriteLine("■■■■■ GGetOrderListByIDs - ords.OrderList.Add ■■■■■");
                        }
                    }

                    return ords;
                }
                else
                {
                    var jsonResult = JsonConvert.DeserializeObject<JsonErrorObject>(json);

                    System.Diagnostics.Debug.WriteLine("■■■■■ GetOrderListByIDs: API error code - " + jsonResult.data.code.ToString() + " ■■■■■");

                    // エラーイベント発火
                    var er = new ClientError();
                    er.ErrType = "API";
                    er.ErrCode = jsonResult.data.code;
                    if (ApiErrorCodesDictionary.ContainsKey(jsonResult.data.code))
                    {
                        er.ErrText = "「" + ApiErrorCodesDictionary[jsonResult.data.code] + "」";
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
                System.Diagnostics.Debug.WriteLine("■■■■■ GetOrderListByIDs: Send returned NULL.");
                return null;
            }
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine("■■■■■ GetOrderListByIDs: Exception " + e);
            return null;
        }

    }

    // 注文キャンセルメソッド
    public async Task<OrderResult> CancelOrder(string _ApiKey, string _ApiSecret, string pair, ulong orderID)
    {

        var path = new Uri("/user/spot/cancel_order", UriKind.Relative);

        var cancelOrderParam = new PairOrderIdParam(pair, orderID);

        var body = JsonConvert.SerializeObject(cancelOrderParam);

        var json = await Send(path, _ApiKey, _ApiSecret, HttpMethod.Post, body);

        if (!string.IsNullOrEmpty(json))
        {
            //System.Diagnostics.Debug.WriteLine("CancelOrder: " + json);

            var deserialized = JsonConvert.DeserializeObject<JsonOrderObject>(json);

            var ord = new OrderResult();

            if (deserialized.success > 0)
            {

                ord.OrderID = deserialized.data.order_id;
                ord.Pair = deserialized.data.pair;
                ord.Side = deserialized.data.side;
                ord.Type = deserialized.data.type;

                //TODO エラーハンドリング
                ord.StartAmount = decimal.Parse(deserialized.data.start_amount);
                ord.RemainingAmount = decimal.Parse(deserialized.data.remaining_amount);
                ord.ExecutedAmount = decimal.Parse(deserialized.data.executed_amount);
                ord.Price = decimal.Parse(deserialized.data.price);
                ord.AveragePrice = decimal.Parse(deserialized.data.average_price);

                var start = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);
                var date = start.AddMilliseconds(deserialized.data.ordered_at).ToLocalTime();
                ord.OrderedAt = date;

                ord.Status = deserialized.data.status;

                ord.IsSuccess = true;
                ord.HasErrorInfo = false;

                return ord;
            }
            else
            {
                var jsonResult = JsonConvert.DeserializeObject<JsonErrorObject>(json);

                ord.IsSuccess = false;
                ord.ErrorCode = jsonResult.data.code;

                // ユーザに表示するエラー情報
                ord.HasErrorInfo = true;
                ord.Err.ErrorCode = ord.ErrorCode;
                ord.Err.ErrorTitle = "注文キャンセル処理でエラーが返りました。";

                if (ApiErrorCodesDictionary.ContainsKey(ord.ErrorCode))
                {
                    ord.Err.ErrorDescription = ApiErrorCodesDictionary[ord.ErrorCode];
                }
                ord.Err.ErrorDescription = ord.Err.ErrorDescription + "(エラーコード：" + ord.ErrorCode.ToString() + ")";


                // エラーイベント発火
                var er = new ClientError();
                er.ErrType = "API";
                er.ErrCode = jsonResult.data.code;
                if (ApiErrorCodesDictionary.ContainsKey(jsonResult.data.code))
                {
                    er.ErrText = "「" + ApiErrorCodesDictionary[jsonResult.data.code] + "」";
                }
                er.ErrDatetime = DateTime.Now;
                er.ErrPlace = path.ToString();

                ErrorOccured?.Invoke(this, er);

                System.Diagnostics.Debug.WriteLine("■■■■■ CancelOrder: API error code - " + jsonResult.data.code.ToString() + " ■■■■■");

                return ord;
            }
        }
        else
        {
            System.Diagnostics.Debug.WriteLine("■■■■■ CancelOrder: Send returned NULL.");
            return null;
        }

    }

    // 文キャンセル（複数）メソッド
    public async Task<Orders> CancelOrders(string _ApiKey, string _ApiSecret, string pair, List<ulong> orderIDs)
    {

        var path = new Uri("/user/spot/cancel_orders", UriKind.Relative);

        var idParam = new PairOrderIdList(pair, orderIDs);
        var body = JsonConvert.SerializeObject(idParam);

        var json = await Send(path, _ApiKey, _ApiSecret, HttpMethod.Post, body);

        try
        {
            if (!string.IsNullOrEmpty(json))
            {
                //System.Diagnostics.Debug.WriteLine("GetOrderListByIDs: " + json);

                var deserialized = JsonConvert.DeserializeObject<JsonOrderInfoObject>(json);

                if (deserialized.success > 0)
                {
                    var ords = new Orders();

                    foreach (var ord in deserialized.data.orders)
                    {
                        try
                        {
                            var o = new Order
                            {
                                OrderID = ord.order_id,
                                Pair = ord.pair,
                                Side = ord.side,
                                Type = ord.type,
                                OrderedAt = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds(ord.ordered_at).ToLocalTime(),
                                Status = ord.status
                            };

                            if (!string.IsNullOrEmpty(ord.start_amount))
                                o.StartAmount = decimal.Parse(ord.start_amount);
                            if (!string.IsNullOrEmpty(ord.remaining_amount))
                                o.RemainingAmount = decimal.Parse(ord.remaining_amount);
                            if (!string.IsNullOrEmpty(ord.executed_amount))
                                o.ExecutedAmount = decimal.Parse(ord.executed_amount);
                            if (!string.IsNullOrEmpty(ord.price))
                                o.Price = decimal.Parse(ord.price);
                            if (!string.IsNullOrEmpty(ord.average_price))
                                o.AveragePrice = decimal.Parse(ord.average_price);

                            ords.OrderList.Add(o);

                            //System.Diagnostics.Debug.WriteLine("GetOrderListByIDs ord.status: " + ord.status);
                        }
                        catch
                        {
                            System.Diagnostics.Debug.WriteLine("■■■■■ CancelOrders - ords.OrderList.Add ■■■■■");
                        }


                    }

                    return ords;
                }
                else
                {
                    var jsonResult = JsonConvert.DeserializeObject<JsonErrorObject>(json);

                    System.Diagnostics.Debug.WriteLine("■■■■■ CancelOrders: API error code - " + jsonResult.data.code.ToString() + " ■■■■■");

                    // エラーイベント発火
                    var er = new ClientError();
                    er.ErrType = "API";
                    er.ErrCode = jsonResult.data.code;
                    if (ApiErrorCodesDictionary.ContainsKey(jsonResult.data.code))
                    {
                        er.ErrText = "「" + ApiErrorCodesDictionary[jsonResult.data.code] + "」";
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
                return null;
            }
        }
        catch (Exception e)
        {
            System.Diagnostics.Debug.WriteLine("■■■■■ CancelOrders: Exception " + e);
            return null;
        }

    }

    // 注文リスト取得メソッド
    public async Task<Orders> GetOrderList(string _ApiKey, string _ApiSecret, string pair)
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

        var json = await Send(path, _ApiKey, _ApiSecret, HttpMethod.Get, "", param);

        if (!string.IsNullOrEmpty(json))
        {
            //System.Diagnostics.Debug.WriteLine("■■■■■ ■■■■■ ■■■■■ GetOrderList: " + json);

            var deserialized = JsonConvert.DeserializeObject<JsonOrderInfoObject>(json);

            if (deserialized.success > 0)
            {
                var ords = new Orders();

                foreach (var ord in deserialized.data.orders)
                {

                    var o = new Order
                    {
                        OrderID = ord.order_id,
                        Pair = ord.pair,
                        Side = ord.side,
                        Type = ord.type,
                        OrderedAt = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds(ord.ordered_at).ToLocalTime(),
                        Status = ord.status
                    };

                    if (!string.IsNullOrEmpty(ord.start_amount)) o.StartAmount = decimal.Parse(ord.start_amount);
                    if (!string.IsNullOrEmpty(ord.remaining_amount)) o.RemainingAmount = decimal.Parse(ord.remaining_amount);
                    if (!string.IsNullOrEmpty(ord.executed_amount)) o.ExecutedAmount = decimal.Parse(ord.executed_amount);
                    if (!string.IsNullOrEmpty(ord.price)) { o.Price = decimal.Parse(ord.price); } else { o.Price = 0; }
                    if (!string.IsNullOrEmpty(ord.average_price)) o.AveragePrice = decimal.Parse(ord.average_price);

                    ords.OrderList.Add(o);

                }

                return ords;
            }
            else
            {
                var jsonResult = JsonConvert.DeserializeObject<JsonErrorObject>(json);

                System.Diagnostics.Debug.WriteLine("■■■■■ GetOrderList: API error code - " + jsonResult.data.code.ToString() + " ■■■■■");

                // エラーイベント発火
                var er = new ClientError();
                er.ErrType = "API";
                er.ErrCode = jsonResult.data.code;
                if (ApiErrorCodesDictionary.ContainsKey(jsonResult.data.code))
                {
                    er.ErrText = "「" + ApiErrorCodesDictionary[jsonResult.data.code] + "」";
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
            return null;
        }
    }

    // 取引履歴取得メソッド
    public async Task<TradeHistory> GetTradeHistory(string _ApiKey, string _ApiSecret, string pair)
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

        var json = await Send(path, _ApiKey, _ApiSecret, HttpMethod.Get, "", param);

        if (!string.IsNullOrEmpty(json))
        {
            //System.Diagnostics.Debug.WriteLine("GetTradeHistory: " + json);

            var deserialized = JsonConvert.DeserializeObject<JsonTradeHistoryObject>(json);

            if (deserialized.success > 0)
            {
                var history = new TradeHistory();

                foreach (var trd in deserialized.data.trades)
                {
                    var o = new Trade
                    {
                        TradeID = trd.trade_id,
                        OrderID = trd.order_id,
                        Pair = trd.pair,
                        Side = trd.side,
                        Type = trd.type,
                        MakerTaker = trd.maker_taker,
                        ExecutedAt = (new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).AddMilliseconds(trd.executed_at).ToLocalTime(),

                    };

                    if (!string.IsNullOrEmpty(trd.amount)) o.Amount = decimal.Parse(trd.amount);
                    if (!string.IsNullOrEmpty(trd.price)) { o.Price = decimal.Parse(trd.price); } else { o.Price = 0; }

                    if (!string.IsNullOrEmpty(trd.amount)) o.FeeAmountBase = decimal.Parse(trd.fee_amount_base);
                    if (!string.IsNullOrEmpty(trd.amount)) o.FeeAmountQuote = decimal.Parse(trd.fee_amount_quote);

                    history.TradeList.Add(o);

                }

                return history;
            }
            else
            {
                var jsonResult = JsonConvert.DeserializeObject<JsonErrorObject>(json);

                System.Diagnostics.Debug.WriteLine("■■■■■ GetTradeHistory: API error code - " + jsonResult.data.code.ToString() + " ■■■■■");

                // エラーイベント発火
                var er = new ClientError();
                er.ErrType = "API";
                er.ErrCode = jsonResult.data.code;
                if (ApiErrorCodesDictionary.ContainsKey(jsonResult.data.code))
                {
                    er.ErrText = "「" + ApiErrorCodesDictionary[jsonResult.data.code] + "」";
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
            return null;
        }

    }

    // HTTPリクエスト送信メソッド
    internal async Task<string> Send(Uri path, string apiKey, string secret, HttpMethod method, string body = "", Dictionary<string, string> queries = null)
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
            if (method == HttpMethod.Post)
            {
                var content = new StringContent(body, Encoding.UTF8, "application/json");

                content.Headers.Add("ACCESS-KEY", apiKey);
                content.Headers.Add("ACCESS-NONCE", _accessNonce);
                content.Headers.Add("ACCESS-SIGNATURE", _accessSignature);

                res = await Client.PostAsync(Client.BaseAddress.ToString() + path.ToString(), content);
            }
            else if (method == HttpMethod.Get)
            {

                if (queries == null)
                {
                    //
                    var requestMessage = new HttpRequestMessage(HttpMethod.Get, Client.BaseAddress.ToString() + path.ToString());
                    requestMessage.Headers.Add("ACCESS-KEY", apiKey);
                    requestMessage.Headers.Add("ACCESS-NONCE", _accessNonce);
                    requestMessage.Headers.Add("ACCESS-SIGNATURE", _accessSignature);

                    res = await Client.SendAsync(requestMessage);
                }
                else
                {
                    var pms = new FormUrlEncodedContent(queries);
                    var param = await pms.ReadAsStringAsync();

                    //
                    var requestMessage = new HttpRequestMessage(HttpMethod.Get, Client.BaseAddress.ToString() + path.ToString() + "?" + param);
                    requestMessage.Headers.Add("ACCESS-KEY", apiKey);
                    requestMessage.Headers.Add("ACCESS-NONCE", _accessNonce);
                    requestMessage.Headers.Add("ACCESS-SIGNATURE", _accessSignature);

                    res = await Client.SendAsync(requestMessage);
                }
            }
            else
            {
                throw new ArgumentException("method は POST か GET を指定してください。", method.ToString());
            }


            try
            {
                //返答内容を取得
                var text = await res.Content.ReadAsStringAsync();

                //通信上の失敗
                if (!res.IsSuccessStatusCode)
                {
                    //System.Diagnostics.Debug.WriteLine("■■■■■■■■ Send: HTTP Error " + res.StatusCode.ToString() + " " + method + " " + _HTTPConn.Client.BaseAddress.ToString() + path.ToString() + " ■■■■■■■");

                    var er = new ClientError();
                    er.ErrType = "HTTP " + method;
                    er.ErrCode = (int)res.StatusCode;
                    er.ErrText = res.StatusCode.ToString();
                    er.ErrDatetime = DateTime.Now;
                    er.ErrPlace = path.ToString();

                    ErrorOccured?.Invoke(this, er);

                    return "";
                }

                return text;
            }
            catch (HttpRequestException ex)
            {
                System.Diagnostics.Debug.WriteLine("HTTP Send: HttpRequestException - " + ex.Message + " + 内部例外: " + ex.InnerException.Message);

                return "";
            }


        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine("■■■■■ HTTP GET/POST Error - Exception : " + ex.Message);
            return "";
        }
        finally
        {
            //_httpClientIsBusy = false;
        }



    }

    #endregion
}

