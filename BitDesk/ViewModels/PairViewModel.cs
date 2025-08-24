using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Globalization;
using System.Security.Cryptography;
using System.Windows.Input;
using BitApps.Core.Helpers;
using BitApps.Core.Models;
using BitApps.Core.Models.APIClients;
using BitDesk.Contracts;
using BitDesk.Models;
using BitDesk.Models.APIClients;
using BitDesk.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.Kernel.Sketches;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using LiveChartsCore.SkiaSharpView.Painting.Effects;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.VisualBasic;
using SkiaSharp;
using static System.Net.WebRequestMethods;

namespace BitDesk.ViewModels;

public partial class PairViewModel : ObservableRecipient
{
    #region == Main properties ==

    private MainViewModel? MainViewModel { get; set; }

    private readonly PairCodes _p;
    public PairCodes PairCode => _p;

    // 表示用 通貨ペア名 "BTC/JPY";
    public string PairString => PairStrings[_p];

    private Dictionary<PairCodes, string> PairStrings
    {
        get; set;
    } = new Dictionary<PairCodes, string>()
    {
                {PairCodes.btc_jpy, "BTC/JPY"},
                {PairCodes.xrp_jpy, "XRP/JPY"},
                {PairCodes.eth_jpy, "ETH/JPY"},
                {PairCodes.ltc_jpy, "LTC/JPY"},
                {PairCodes.mona_jpy, "MONA/JPY"},
                {PairCodes.bcc_jpy, "BCC/JPY"},
                {PairCodes.xlm_jpy, "XLM/JPY"},
                {PairCodes.qtum_jpy, "QTUM/JPY"},
                {PairCodes.bat_jpy, "BAT/JPY"},
                {PairCodes.omg_jpy, "OMG/JPY"},
                {PairCodes.xym_jpy, "XYM/JPY"},
                {PairCodes.link_jpy, "LINK/JPY"},
                {PairCodes.mkr_jpy, "MKR/JPY"},
                {PairCodes.boba_jpy, "BOBA/JPY"},
                {PairCodes.enj_jpy, "ENJ/JPY"},
                {PairCodes.matic_jpy, "MATIC/JPY"},
                {PairCodes.dot_jpy, "DOT/JPY"},
                {PairCodes.doge_jpy, "DOGE/JPY"},
                {PairCodes.astr_jpy, "ASTR/JPY"},
                {PairCodes.ada_jpy, "ADA/JPY"},
                {PairCodes.avax_jpy, "AVAX/JPY"},
                {PairCodes.axs_jpy, "AXS/JPY"},
                {PairCodes.flr_jpy, "FLR/JPY"},
                {PairCodes.sand_jpy, "SAND/JPY"},
            };

    public Dictionary<string, PairCodes> GetPairCodes
    {
        get; set;
    } = new Dictionary<string, PairCodes>()
        {
            {"btc_jpy", PairCodes.btc_jpy},
            {"xrp_jpy", PairCodes.xrp_jpy},
            {"eth_jpy", PairCodes.eth_jpy},
            {"ltc_jpy", PairCodes.ltc_jpy},
            {"mona_jpy", PairCodes.mona_jpy},
            {"bcc_jpy", PairCodes.bcc_jpy},
            {"xlm_jpy", PairCodes.xlm_jpy},
            {"qtum_jpy", PairCodes.qtum_jpy},
            {"bat_jpy", PairCodes.bat_jpy},
            {"omg_jpy", PairCodes.omg_jpy},
            {"xym_jpy", PairCodes.xym_jpy},
            {"link_jpy", PairCodes.link_jpy},
            {"mkr_jpy", PairCodes.mkr_jpy},
            {"boba_jpy", PairCodes.boba_jpy},
            {"enj_jpy", PairCodes.enj_jpy},
            {"matic_jpy", PairCodes.matic_jpy},
            {"dot_jpy", PairCodes.dot_jpy},
            {"doge_jpy", PairCodes.doge_jpy},
            {"astr_jpy", PairCodes.astr_jpy},
            {"ada_jpy", PairCodes.ada_jpy},
            {"avax_jpy", PairCodes.avax_jpy},
            {"axs_jpy", PairCodes.axs_jpy},
            {"flr_jpy", PairCodes.flr_jpy},
            {"sand_jpy", PairCodes.sand_jpy},
        };

    public string CurrencyUnitString => CurrentPairCoin[_p];

    public Dictionary<PairCodes, string> CurrentPairCoin { get; set; } = new Dictionary<PairCodes, string>()
        {
            {PairCodes.btc_jpy, "BTC"},
            {PairCodes.xrp_jpy, "XRP"},
            //{Pairs.eth_btc, "ETH"},
            {PairCodes.eth_jpy, "ETH"},
            //{Pairs.ltc_btc, "LTC"},
            {PairCodes.ltc_jpy, "LTC"},
            {PairCodes.mona_jpy, "Mona"},
            //{Pairs.mona_btc, "Mona"},
            {PairCodes.bcc_jpy, "BCC"},
            //{Pairs.bcc_btc, "BCH"},
            {PairCodes.xlm_jpy, "XLM"},
            {PairCodes.qtum_jpy, "QTUM"},
            {PairCodes.bat_jpy, "BAT"},
            {PairCodes.omg_jpy, "OMG"},
            {PairCodes.xym_jpy, "XYM"},
            {PairCodes.link_jpy, "LINK"},
            {PairCodes.mkr_jpy, "MKR"},
            {PairCodes.boba_jpy, "BOBA"},
            {PairCodes.enj_jpy, "ENJ"},
            {PairCodes.matic_jpy, "MATIC"},
            {PairCodes.dot_jpy, "DOT"},
            {PairCodes.doge_jpy, "DOGE"},
            {PairCodes.astr_jpy, "ASTR"},
            {PairCodes.ada_jpy, "ADA"},
            {PairCodes.avax_jpy, "AVAX"},
            {PairCodes.axs_jpy, "AXS"},
            {PairCodes.flr_jpy, "FLR"},
            {PairCodes.sand_jpy, "SAND"},
        };

    private decimal _ltp;
    public decimal Ltp
    {
        get => _ltp;
        set
        {
            if (_ltp == value)
            {
                return;
            }

            _ltp = value;

            OnPropertyChanged(nameof(Ltp));
            OnPropertyChanged(nameof(LtpString));
            OnPropertyChanged(nameof(AssetCurrentEstimateAmount));
            OnPropertyChanged(nameof(AssetCurrentEstimateAmountText));
            /*
            if (_ltp > BasePrice)
            {
                BasePriceUpFlag = true;
            }
            else if (_ltp < BasePrice)
            {
                BasePriceUpFlag = false;
            }
            OnPropertyChanged("BasePriceIcon");

            if (_ltp > MiddleInitPrice)
            {
                MiddleInitPriceUpFlag = true;
            }
            else if (_ltp < MiddleInitPrice)
            {
                MiddleInitPriceUpFlag = false;
            }
            OnPropertyChanged("MiddleInitPriceIcon");

            if (_ltp > MiddleLast24Price)
            {
                MiddleLast24PriceUpFlag = true;
            }
            else if (_ltp < MiddleLast24Price)
            {
                MiddleLast24PriceUpFlag = false;
            }
            OnPropertyChanged("MiddleLast24PriceIcon");

            if (_ltp > AveragePrice)
            {
                AveragePriceUpFlag = true;
            }
            else if (_ltp < AveragePrice)
            {
                AveragePriceUpFlag = false;
            }
            OnPropertyChanged("AveragePriceIcon");
            */

            if (IsSelectedActive && IsEnabled) // 
            {
                App.CurrentDispatcherQueue?.TryEnqueue(() =>
                {
                    Sections[0].Yi = (double)_ltp;
                    Sections[0].Yj = (double)_ltp;
                });

                // a little hack to update Section...
                App.CurrentDispatcherQueue?.TryEnqueue(() =>
                {
                    if (Series[1].Values == null)
                    {
                        return;
                    }

                    if (Series[1].Values is ObservableCollection<FinancialPoint> oc)
                    {
                        if (oc.Count < 1)
                        {
                            return;
                        }

                        oc[0].Close =
                            oc[0].Close = (double)_ltp;
                    }
                });
            }
        }
    }
    public string LtpString
    {
        get
        {
            if (_ltp == 0)
            {
                return "";
            }
            else
            {
                return string.Format(_ltpFormstString, _ltp);

            }
        }
    }

    private string _ltpFormstString = "{0:#,0}";
    public string LtpFormstString
    {
        get => _ltpFormstString;
        set
        {
            if (_ltpFormstString == value)
            {
                return;
            }

            _ltpFormstString = value;
            OnPropertyChanged(nameof(LtpFormstString));
        }
    }

    private readonly string _currencyFormstString = "C";

    private bool _ltpUpFlag;
    public bool LtpUpFlag
    {
        get => _ltpUpFlag;
        set
        {
            if (_ltpUpFlag == value)
            {
                return;
            }

            _ltpUpFlag = value;
            OnPropertyChanged(nameof(LtpUpFlag));
        }
    }

    private readonly double _ltpFontSize = 28;
    public double LtpFontSize => _ltpFontSize;

    private decimal _bid;
    public decimal Bid
    {
        get => _bid;
        set
        {
            if (_bid == value)
            {
                return;
            }

            _bid = value;
            OnPropertyChanged(nameof(Bid));
            OnPropertyChanged(nameof(BidString));

        }
    }
    public string BidString => string.Format("{0:#,0}", _bid);

    private decimal _ask;
    public decimal Ask
    {
        get => _ask;
        set
        {
            if (_ask == value)
            {
                return;
            }

            _ask = value;
            OnPropertyChanged(nameof(Ask));
            OnPropertyChanged(nameof(AskString));

        }
    }
    public string AskString => string.Format("{0:#,0}", _ask);

    private DateTime _tickTimeStamp;
    public DateTime TickTimeStamp
    {
        get => _tickTimeStamp;
        set
        {
            if (_tickTimeStamp == value)
            {
                return;
            }

            _tickTimeStamp = value;
            OnPropertyChanged(nameof(TickTimeStamp));
            OnPropertyChanged(nameof(TickTimeStampString));

        }
    }
    public string TickTimeStampString => _tickTimeStamp.ToLocalTime().ToString("HH:mm:ss");//("G", System.Globalization.CultureInfo.CurrentUICulture);//"yyyy/MM/dd HH:mm:ss"

    private readonly ObservableCollection<TickHistory> _tickHistory = [];
    public ObservableCollection<TickHistory> TickHistories => _tickHistory;

    #endregion

    #region == Flags ==

    private bool _isEnabled = true;
    public bool IsEnabled
    {
        get => _isEnabled;
        set
        {
            if (_isEnabled == value)
            {
                return;
            }

            _isEnabled = value;
            OnPropertyChanged(nameof(IsEnabled));
        }
    }

    // Selected or not.
    private bool _isSelectedActive;
    public bool IsSelectedActive
    {
        get => _isSelectedActive;
        set
        {
            if (_isSelectedActive == value)
            {
                return;
            }

            _isSelectedActive = value;
            OnPropertyChanged(nameof(IsSelectedActive));
        }
    }

    private bool _isChartInitAndLoaded;
    public bool IsChartInitAndLoaded
    {
        get => _isChartInitAndLoaded;
        set
        {
            if (_isChartInitAndLoaded == value)
            {
                return;
            }

            _isChartInitAndLoaded = value;
            OnPropertyChanged(nameof(IsChartInitAndLoaded));
        }
    }

    #endregion

    #region == Options ==

    private bool _isPaneVisible = true;
    public bool IsPaneVisible
    {
        get => _isPaneVisible;
        set
        {
            if (_isPaneVisible == value)
            {
                return;
            }

            _isPaneVisible = value;
            OnPropertyChanged(nameof(IsPaneVisible));
        }
    }

    private bool _isChartTooltipVisible = true;
    public bool IsChartTooltipVisible
    {
        get => _isChartTooltipVisible;
        set
        {
            if (_isChartTooltipVisible == value)
            {
                return;
            }

            _isChartTooltipVisible = value;
            OnPropertyChanged(nameof(IsChartTooltipVisible));

            if (_isChartTooltipVisible)
            {
                IsChartTooltipVisibleTemp = LiveChartsCore.Measure.TooltipPosition.Center;
            }
            else
            {
                IsChartTooltipVisibleTemp = LiveChartsCore.Measure.TooltipPosition.Hidden;
            }
        }
    }

    private LiveChartsCore.Measure.TooltipPosition _isChartTooltipVisibleTemp = LiveChartsCore.Measure.TooltipPosition.Center;
    public LiveChartsCore.Measure.TooltipPosition IsChartTooltipVisibleTemp
    {
        get => _isChartTooltipVisibleTemp;
        set
        {
            if (_isChartTooltipVisibleTemp == value)
            {
                return;
            }

            _isChartTooltipVisibleTemp = value;
            OnPropertyChanged(nameof(IsChartTooltipVisibleTemp));
        }
    }

    #endregion

    #region == Alarm ==

    private decimal _alarmPlus;
    public decimal AlarmPlus
    {
        get => _alarmPlus;
        set
        {
            if (_alarmPlus == value)
            {
                return;
            }

            if (value != 0)
            {
                if (value <= _ltp)
                {
                    return;
                }
            }

            _alarmPlus = value;
            OnPropertyChanged(nameof(AlarmPlus));
            OnPropertyChanged(nameof(AlarmPlusString));
            OnPropertyChanged(nameof(AlarmLabel));
        }
    }
    public string AlarmPlusString => string.Format(_ltpFormstString, AlarmPlus);

    private decimal _alarmMinus;
    public decimal AlarmMinus
    {
        get => _alarmMinus;
        set
        {
            if (_alarmMinus == value)
            {
                return;
            }

            if (value != 0)
            {
                if (value >= _ltp)
                {
                    return;
                }
            }

            _alarmMinus = value;
            OnPropertyChanged(nameof(AlarmMinus));
            OnPropertyChanged(nameof(AlarmMinusString));
            OnPropertyChanged(nameof(AlarmLabel));
        }
    }
    public string AlarmMinusString => string.Format(_ltpFormstString, AlarmMinus);

    public string AlarmLabel
    {
        get
        {
            if ((AlarmPlus > 0) || (AlarmMinus > 0))
            {
                return "set";
            }
            else
            {
                return "none";
            }
        }
    }

    // 起動後　最安値　最高値　アラーム情報表示
    private string _highLowInfoText = "";
    public string HighLowInfoText
    {
        get => _highLowInfoText;
        set
        {
            if (_highLowInfoText == value)
            {
                return;
            }

            _highLowInfoText = value;
            OnPropertyChanged(nameof(HighLowInfoText));
        }
    }

    private bool _highLowInfoTextColorFlag;
    public bool HighLowInfoTextColorFlag
    {
        get => _highLowInfoTextColorFlag;
        set
        {
            if (_highLowInfoTextColorFlag == value)
            {
                return;
            }

            _highLowInfoTextColorFlag = value;
            OnPropertyChanged(nameof(HighLowInfoTextColorFlag));
        }
    }

    // アラーム音
    // 起動後
    private bool _playSoundLowest = false;
    public bool PlaySoundLowest
    {
        get => _playSoundLowest;
        set
        {
            if (_playSoundLowest == value)
            {
                return;
            }

            _playSoundLowest = value;
            OnPropertyChanged(nameof(PlaySoundLowest));
        }
    }

    private bool _playSoundHighest = false;
    public bool PlaySoundHighest
    {
        get => _playSoundHighest;
        set
        {
            if (_playSoundHighest == value)
            {
                return;
            }

            _playSoundHighest = value;
            OnPropertyChanged(nameof(PlaySoundHighest));
        }
    }

    // last 24h
    private bool _playSoundLowest24h = false;
    public bool PlaySoundLowest24h
    {
        get => _playSoundLowest24h;
        set
        {
            if (_playSoundLowest24h == value)
            {
                return;
            }

            _playSoundLowest24h = value;
            OnPropertyChanged(nameof(PlaySoundLowest24h));
        }
    }

    private bool _playSoundHighest24h = false;
    public bool PlaySoundHighest24h
    {
        get => _playSoundHighest24h;
        set
        {
            if (_playSoundHighest24h == value)
            {
                return;
            }

            _playSoundHighest24h = value;
            OnPropertyChanged(nameof(PlaySoundHighest24h));
        }
    }

    #endregion

    #region == Stat ==

    // Initial price at startup.
    private decimal _basePrice = 0;
    public decimal BasePrice
    {
        get => _basePrice;
        set
        {
            if (_basePrice == value)
            {
                return;
            }

            _basePrice = value;

            OnPropertyChanged(nameof(BasePrice));
            OnPropertyChanged(nameof(BasePriceIcon));
            OnPropertyChanged(nameof(BasePriceString));

        }
    }

    public string BasePriceString => string.Format(_ltpFormstString, BasePrice);

    public string BasePriceIcon
    {
        get
        {
            if (_ltp > BasePrice)
            {
                return "▲";
            }
            else if (_ltp < BasePrice)
            {
                return "▼";
            }
            else
            {
                return "＝";
            }
        }
    }

    private bool _basePriceUpFlag;
    public bool BasePriceUpFlag
    {
        get => _basePriceUpFlag;
        set
        {
            if (_basePriceUpFlag == value)
            {
                return;
            }

            _basePriceUpFlag = value;
            OnPropertyChanged(nameof(BasePriceUpFlag));
        }
    }

    // 過去1分間の平均値
    private decimal _averagePrice;
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
            OnPropertyChanged(nameof(AveragePrice));
            OnPropertyChanged(nameof(AveragePriceIcon));
            //this.OnPropertyChanged("AveragePriceIconColor");
            OnPropertyChanged(nameof(AveragePriceString));
        }
    }

    public string AveragePriceString
    {
        get
        {
            return string.Format(_ltpFormstString, _averagePrice); ;
        }
    }

    public string AveragePriceIcon
    {
        get
        {
            if (_ltp > _averagePrice)
            {
                return "▲";
            }
            else if (_ltp < _averagePrice)
            {
                return "▼";
            }
            else
            {
                return "＝";
            }
        }
    }

    private bool _averagePriceUpFlag;
    public bool AveragePriceUpFlag
    {
        get => _averagePriceUpFlag;
        set
        {
            if (_averagePriceUpFlag == value)
            {
                return;
            }

            _averagePriceUpFlag = value;
            OnPropertyChanged(nameof(AveragePriceUpFlag));
        }
    }

    // 過去２４時間の中央値
    public decimal MiddleLast24Price => (_lowestIn24Price + _highestIn24Price) / 2;
    public string MiddleLast24PriceString
    {
        get
        {
            return string.Format(_ltpFormstString, MiddleLast24Price); ;
        }
    }

    public string MiddleLast24PriceIcon
    {
        get
        {
            if (_ltp > MiddleLast24Price)
            {
                return "▲";
            }
            else if (_ltp < MiddleLast24Price)
            {
                return "▼";
            }
            else
            {
                return "＝";
            }
        }
    }

    private bool _middleLast24PriceUpFlag;
    public bool MiddleLast24PriceUpFlag
    {
        get => _middleLast24PriceUpFlag;
        set
        {
            if (_middleLast24PriceUpFlag == value)
            {
                return;
            }

            _middleLast24PriceUpFlag = value;
            OnPropertyChanged(nameof(MiddleLast24PriceUpFlag));
        }
    }

    // 起動後の中央値
    public decimal MiddleInitPrice => (_lowestPrice + _highestPrice) / 2;
    public string MiddleInitPriceString
    {
        get
        {
            return string.Format(_ltpFormstString, MiddleInitPrice); ;
        }
    }

    public string MiddleInitPriceIcon
    {
        get
        {
            if (_ltp > MiddleInitPrice)
            {
                return "▲";
            }
            else if (_ltp < MiddleInitPrice)
            {
                return "▼";
            }
            else
            {
                return "＝";
            }
        }
    }

    private bool _middleInitPriceUpFlag;
    public bool MiddleInitPriceUpFlag
    {
        get => _middleInitPriceUpFlag;
        set
        {
            if (_middleInitPriceUpFlag == value)
            {
                return;
            }

            _middleInitPriceUpFlag = value;
            OnPropertyChanged(nameof(MiddleInitPriceUpFlag));
        }
    }

    // 24時間の最高値 
    private decimal _highestIn24Price;
    public decimal HighestIn24Price
    {
        get => _highestIn24Price;
        set
        {
            if (_highestIn24Price == value)
            {
                return;
            }

            _highestIn24Price = value;
            OnPropertyChanged(nameof(HighestIn24Price));
            OnPropertyChanged(nameof(High24String));

            OnPropertyChanged(nameof(MiddleLast24Price));
            OnPropertyChanged(nameof(MiddleLast24PriceString));

            //if (MinMode) return;
            // チャートの最高値をセット
            //ChartAxisY[0].MaxValue = (double)_highestIn24Price + 3000;
            // チャートの２４最高値ポイントを更新
            //(ChartSeries[1].Values[0] as ObservableValue).Value = (double)_highestIn24Price;

        }
    }
    public string High24String => string.Format(_ltpFormstString, _highestIn24Price);

    // 24時間の最高値 アラートOn/Offフラグ
    private bool _highestIn24PriceAlart;
    public bool HighestIn24PriceAlart
    {
        get => _highestIn24PriceAlart;
        set
        {
            if (_highestIn24PriceAlart == value)
            {
                return;
            }

            _highestIn24PriceAlart = value;
            OnPropertyChanged(nameof(HighestIn24PriceAlart));
            OnPropertyChanged(nameof(PriceAlart));
        }
    }

    // 24時間の最安値 
    private decimal _lowestIn24Price;
    public decimal LowestIn24Price
    {
        get => _lowestIn24Price;
        set
        {
            if (_lowestIn24Price == value)
            {
                return;
            }

            _lowestIn24Price = value;
            OnPropertyChanged(nameof(LowestIn24Price));
            OnPropertyChanged(nameof(Low24String));

            OnPropertyChanged(nameof(MiddleLast24Price));
            OnPropertyChanged(nameof(MiddleLast24PriceString));

            //if (MinMode) return;
            // チャートの最低値をセット
            //ChartAxisY[0].MinValue = (double)_lowestIn24Price - 3000;
            // チャートの２４最低値ポイントを更新
            //(ChartSeries[2].Values[0] as ObservableValue).Value = (double)_lowestIn24Price;
        }
    }
    public string Low24String => string.Format(_ltpFormstString, _lowestIn24Price);

    // 24時間の最安値 アラートOn/Offフラグ
    private bool _lowestIn24PriceAlart;
    public bool LowestIn24PriceAlart
    {
        get => _lowestIn24PriceAlart;
        set
        {
            if (_lowestIn24PriceAlart == value)
            {
                return;
            }

            _lowestIn24PriceAlart = value;
            OnPropertyChanged(nameof(LowestIn24PriceAlart));
            OnPropertyChanged(nameof(PriceAlart));
        }
    }

    // 起動後 最高値
    private decimal _highestPrice;
    public decimal HighestPrice
    {
        get => _highestPrice;
        set
        {
            if (_highestPrice == value)
            {
                return;
            }

            _highestPrice = value;
            OnPropertyChanged(nameof(HighestPrice));
            OnPropertyChanged(nameof(HighestPriceString));

            OnPropertyChanged(nameof(MiddleInitPrice));
            OnPropertyChanged(nameof(MiddleInitPriceString));
            OnPropertyChanged(nameof(MiddleInitPriceIcon));

            //if (MinMode) return;
            //(ChartSeries[1].Values[0] as ObservableValue).Value = (double)_highestPrice;
        }
    }
    public string HighestPriceString
    {
        get
        {
            return string.Format(_ltpFormstString, _highestPrice); ;
        }
    }

    // 起動後 最高値 アラートOn/Offフラグ
    private bool _highestPriceAlart;
    public bool HighestPriceAlart
    {
        get => _highestPriceAlart;
        set
        {
            if (_highestPriceAlart == value)
            {
                return;
            }

            _highestPriceAlart = value;
            OnPropertyChanged(nameof(HighestPriceAlart));
            OnPropertyChanged(nameof(PriceAlart));
        }
    }

    // 起動後 最低値
    private decimal _lowestPrice;
    public decimal LowestPrice
    {
        get => _lowestPrice;
        set
        {
            if (_lowestPrice == value)
            {
                return;
            }

            _lowestPrice = value;
            OnPropertyChanged(nameof(LowestPrice));
            OnPropertyChanged(nameof(LowestPriceString));

            OnPropertyChanged(nameof(MiddleInitPrice));
            OnPropertyChanged(nameof(MiddleInitPriceString));
            OnPropertyChanged(nameof(MiddleInitPriceIcon));

            //if (MinMode) return;
            // (ChartSeries[2].Values[0] as ObservableValue).Value = (double)_lowestPrice;
        }
    }
    public string LowestPriceString
    {
        get
        {
            return string.Format(_ltpFormstString, _lowestPrice); ;
        }
    }

    // 起動後 最低値アラートOn/Offフラグ
    private bool _lowestPriceAlart;
    public bool LowestPriceAlart
    {
        get => _lowestPriceAlart;
        set
        {
            if (_lowestPriceAlart == value)
            {
                return;
            }

            _lowestPriceAlart = value;
            OnPropertyChanged(nameof(LowestPriceAlart));
            OnPropertyChanged(nameof(PriceAlart));
        }
    }

    // タブの点滅用
    public bool PriceAlart
    {
        get
        {
            if (LowestPriceAlart || HighestPriceAlart || LowestIn24PriceAlart || HighestIn24PriceAlart)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    #endregion

    #region == Assets ==

    // 現在の資産名
    private string _assetCurrentName = string.Empty;
    public string AssetCurrentName
    {
        get => _assetCurrentName;
        set
        {
            if (_assetCurrentName == value)
            {
                return;
            }

            _assetCurrentName = value;
            OnPropertyChanged(nameof(AssetCurrentName));
        }
    }

    // 現在の通貨の資産
    private decimal _assetCurrentAmount;
    public decimal AssetCurrentAmount
    {
        get =>
            //return Math.Floor((_assetBTCAmount * 10000M)) / 10000M;
            _assetCurrentAmount;
        set
        {
            if (_assetCurrentAmount == value)
            {
                return;
            }

            _assetCurrentAmount = value;
            OnPropertyChanged(nameof(AssetCurrentAmount));
            OnPropertyChanged(nameof(AssetCurrentAmountText));
            OnPropertyChanged(nameof(AssetCurrentEstimateAmount));
            OnPropertyChanged(nameof(AssetCurrentEstimateAmountText));
        }
    }

    public string AssetCurrentAmountText => AssetCurrentAmount.ToString();

    // 現在の通貨の利用可能資産額
    private decimal _assetCurrentFreeAmount;
    public decimal AssetCurrentFreeAmount
    {
        get =>
            // 売買出来ない桁は、切り捨てで。
            //return _assetBTCFreeAmount;
            Math.Floor((_assetCurrentFreeAmount * 10000M)) / 10000M;
        set
        {
            if (_assetCurrentFreeAmount == value)
            {
                return;
            }

            _assetCurrentFreeAmount = value;
            OnPropertyChanged(nameof(AssetCurrentFreeAmount));
            OnPropertyChanged(nameof(AssetCurrentFreeAmountText));
        }
    }

    public string AssetCurrentFreeAmountText => AssetCurrentFreeAmount.ToString();

    // 現在の通貨の時価評価額 (ティックから更新される)
    private decimal _assetCurrentEstimateAmount;
    public decimal AssetCurrentEstimateAmount
    {
        get
        {
            _assetCurrentEstimateAmount = _assetCurrentAmount * _ltp;
            return Math.Floor((_assetCurrentEstimateAmount * 10000M)) / 10000M;
        }
        set
        {
            if (_assetCurrentEstimateAmount == value)
            {
                return;
            }

            _assetCurrentEstimateAmount = value;
            OnPropertyChanged(nameof(AssetCurrentEstimateAmount));
            OnPropertyChanged(nameof(AssetCurrentEstimateAmountText));
        }
    }

    public string AssetCurrentEstimateAmountText => AssetCurrentEstimateAmount.ToString("C0")+"";

    // 円資産名
    private string _assetJPYName = string.Empty;
    public string AssetJPYName
    {
        get => _assetJPYName;
        set
        {
            if (_assetJPYName == value)
            {
                return;
            }

            _assetJPYName = value;
            OnPropertyChanged(nameof(AssetJPYName));
        }
    }

    // 円総資産額
    private decimal _assetJPYAmount;
    public decimal AssetJPYAmount
    {
        get => _assetJPYAmount;
        set
        {
            if (_assetJPYAmount == value)
            {
                return;
            }

            _assetJPYAmount = value;
            OnPropertyChanged(nameof(AssetJPYAmount));
            //OnPropertyChanged(nameof(AssetAllEstimateAmountString));
            OnPropertyChanged(nameof(AssetJPYAmountText));
        }
    }

    public string AssetJPYAmountText => _assetJPYAmount.ToString("C4");

    // 円利用可能資産額
    private decimal _assetJPYFreeAmount;
    public decimal AssetJPYFreeAmount
    {
        get =>
            // 利用可能額は小数点以下を切り捨て（読みやすいように）
            //return _assetJPYFreeAmount;
            Math.Floor(_assetJPYFreeAmount);
        set
        {
            if (_assetJPYFreeAmount == value)
            {
                return;
            }

            _assetJPYFreeAmount = value;
            OnPropertyChanged(nameof(AssetJPYFreeAmount));
            OnPropertyChanged(nameof(AssetJPYFreeAmountText));
        }
    }

    public string AssetJPYFreeAmountText => AssetJPYFreeAmount.ToString("C0");



    #endregion

    #region == Orders, TradeHistory ==

    private ObservableCollection<Order> _orders = [];
    public ObservableCollection<Order> Orders
    {
        get => _orders;
        set
        {
            if (_orders== value)
            {
                return;
            }

            _orders = value;
            OnPropertyChanged(nameof(Orders));
        }
    }

    private ObservableCollection<Trade> _tradeHistories = [];
    public ObservableCollection<Trade> TradeHistories
    {
        get => _tradeHistories;
        set
        {
            if (_tradeHistories == value)
            {
                return;
            }

            _tradeHistories = value;
            OnPropertyChanged(nameof(TradeHistories));
        }
    }

    #endregion

    #region == Transactions ==

    private ObservableCollection<Transaction> _transactions = [];
    public ObservableCollection<Transaction> Transactions
    {
        get => _transactions;
        set
        {
            if (_transactions == value)
            {
                return;
            }

            _transactions = value;
            OnPropertyChanged(nameof(Transactions));
        }
    }

    #endregion

    #region == Depth and Transaction ==

    private ObservableCollection<Depth> _depth = [];
    public ObservableCollection<Depth> Depth
    {
        get => _depth;
        set
        {
            if (_depth == value)
            {
                return;
            }

            _depth = value;
            OnPropertyChanged(nameof(Depth));
        }
    }

    private bool _isDeepthGroupingChanged;
    public bool IsDepthGroupingChanged
    {
        get => _isDeepthGroupingChanged;
        set
        {
            if (_isDeepthGroupingChanged == value)
            {
                return;
            }

            _isDeepthGroupingChanged = value;

            OnPropertyChanged(nameof(IsDepthGroupingChanged));
        }
    }

    private decimal _depthGrouping = 0;
    public decimal DepthGrouping
    {
        get => _depthGrouping;
        set
        {
            if (_depthGrouping == value)
            {
                return;
            }

            _depthGrouping = value;

            if (DepthGrouping100 == _depthGrouping)
            {
                IsDepthGrouping100 = true;
                IsDepthGrouping1000 = false;
                IsDepthGroupingOff = false;
            }
            if (DepthGrouping1000 == _depthGrouping)
            {
                IsDepthGrouping100 = false;
                IsDepthGrouping1000 = true;
                IsDepthGroupingOff = false;
            }

            if (0 == _depthGrouping)
            {
                IsDepthGrouping100 = false;
                IsDepthGrouping1000 = false;
                IsDepthGroupingOff = true;
            }

            OnPropertyChanged(nameof(DepthGrouping));

        }
    }

    private bool _isDepthGroupingOff = true;
    public bool IsDepthGroupingOff
    {
        get => _isDepthGroupingOff;
        set
        {
            if (_isDepthGroupingOff == value)
            {
                return;
            }

            _isDepthGroupingOff = value;
            OnPropertyChanged(nameof(IsDepthGroupingOff));
        }
    }

    private decimal _depthGrouping100 = 100;
    public decimal DepthGrouping100
    {
        get => _depthGrouping100;
        set
        {
            if (_depthGrouping100 == value)
            {
                return;
            }

            _depthGrouping100 = value;
            OnPropertyChanged(nameof(DepthGrouping100));

        }
    }

    private bool _isDepthGrouping100;
    public bool IsDepthGrouping100
    {
        get => _isDepthGrouping100;
        set
        {
            if (_isDepthGrouping100 == value)
            {
                return;
            }

            _isDepthGrouping100 = value;
            OnPropertyChanged(nameof(IsDepthGrouping100));
        }
    }

    private decimal _depthGrouping1000 = 1000;
    public decimal DepthGrouping1000
    {
        get => _depthGrouping1000;
        set
        {
            if (_depthGrouping1000 == value)
            {
                return;
            }

            _depthGrouping1000 = value;
            OnPropertyChanged(nameof(DepthGrouping1000));

        }
    }

    private bool _isDepthGrouping1000;
    public bool IsDepthGrouping1000
    {
        get => _isDepthGrouping1000;
        set
        {
            if (_isDepthGrouping1000 == value)
            {
                return;
            }

            _isDepthGrouping1000 = value;
            OnPropertyChanged(nameof(IsDepthGrouping1000));
        }
    }

    #endregion

    #region == Charts ==

    public Section<LiveChartsCore.SkiaSharpView.Drawing.SkiaSharpDrawingContext>[] Sections { get; set; } =
    [
        new RectangularSection
        {
            Yi = 1,
            Yj = 1,
            ScalesYAt = 1,
            Stroke = new SolidColorPaint
            {
                Color = SKColors.Silver.WithAlpha(80),
                StrokeThickness = 1,
                //PathEffect = new DashEffect(new float[] { 6, 6 })
            }
        }
    ];

    public ICartesianAxis[] XAxes {get; set;} =
    [
        new Axis()
        {
            TextSize = 12.5,
            LabelsRotation = -13,
            LabelsPaint = new SolidColorPaint(SKColors.Gray.WithAlpha(80)),
            Labeler = value => new DateTime((long) value).ToString("M/d"), //TODO: localize aware
            //UnitWidth = TimeSpan.FromHours(0.5).Ticks,
            UnitWidth = TimeSpan.FromDays(1).Ticks,
            //MinStep = TimeSpan.FromDays(1).Ticks,
            MaxLimit = null,
            MinLimit= DateTime.Now.Ticks - TimeSpan.FromDays(2.8).Ticks,
            SeparatorsPaint = new SolidColorPaint(SKColors.LightSlateGray.WithAlpha(80)) { StrokeThickness = 1,PathEffect = new DashEffect([3, 3]) }
        }
    ];

    public ICartesianAxis[] YAxes
    {
        get; set;

    } =
        [
        new Axis()
            {
                LabelsRotation = 0,
                //LabelsPaint = new SolidColorPaint(SKColors.Wheat),
                //LabelsPaint = new SolidColorPaint(SKColors.Gray.WithAlpha(80)),
                IsVisible = false,
                Position = LiveChartsCore.Measure.AxisPosition.Start,
                ShowSeparatorLines = false,
                MinLimit=0,
            },
        new Axis()
            {
                TextSize = 12.5,
                LabelsRotation = 0,
                LabelsPaint = new SolidColorPaint(SKColors.Gray.WithAlpha(80)),
                Position = LiveChartsCore.Measure.AxisPosition.End,
                SeparatorsPaint = new SolidColorPaint(SKColors.LightSlateGray.WithAlpha(80))
                            {
                                StrokeThickness = 1,
                                PathEffect = new DashEffect([3, 3])
                            },
                //Labeler = Labelers.Currency,
                //Labeler = (value) => value.ToString("C", new System.Globalization.CultureInfo("ja-Jp")),
                Labeler = (value) => value.ToString("N", new CultureInfo("ja-Jp")),
            }
    ];

    private ISeries[] _series =
    [
        new ColumnSeries<DateTimePoint>
        {
            Name = "Depth",
            ScalesYAt = 0,
            //Stroke = new SolidColorPaint((new SKColor(198, 167, 0)), 0),
            Fill =  new SolidColorPaint(new SKColor(127, 127, 127).WithAlpha(80), 1),
            //XToolTipLabelFormatter = (chartPoint) =>
                //$"Depth, {new DateTime((long) chartPoint.SecondaryValue):yyy/MM/dd HH}: {chartPoint.PrimaryValue}",
            Values = new ObservableCollection<DateTimePoint>
            {
                //new DateTimePoint(DateTime.Now, 1)
            }
        },
        new CandlesticksSeries<FinancialPoint>
        {
            Name = "Price",
            ScalesYAt = 1,
            //TooltipLabelFormatter = (chartPoint) => $"Price: {new DateTime((long) chartPoint.SecondaryValue):yyy/MM/dd HH}: {chartPoint.PrimaryValue}",
            Values = new ObservableCollection<FinancialPoint>
            {
                //new(DateTime.Now, 100, 0, 0, 0)
            }
        }
    ];
    public ISeries[] Series
    {
        get => _series;
        set
        {
            if (_series == value)
            {
                return;
            }

            _series = value;
            OnPropertyChanged(nameof(Series));
        }
    }

    private TimeSpan chartUpdateInterval = new(1, 0, 0);

    private CandleTypes _selectedCandleType = CandleTypes.OneHour;
    public CandleTypes SelectedCandleType
    {
        get => _selectedCandleType;
        set
        {
            if (_selectedCandleType == value)
            {
                return;
            }

            _selectedCandleType = value;
            OnPropertyChanged(nameof(SelectedCandleType));
            OnPropertyChanged(nameof(SelectedCandleTypeLabelString));

            if (_selectedCandleType == CandleTypes.OneMin)
            {
                chartUpdateInterval = new TimeSpan(0, 1, 0);
            }
            else if (_selectedCandleType == CandleTypes.FiveMin)
            {
                chartUpdateInterval = new TimeSpan(0, 5, 0);
            }
            else if (_selectedCandleType == CandleTypes.FifteenMin)
            {
                chartUpdateInterval = new TimeSpan(0, 15, 0);
            }
            else if (_selectedCandleType == CandleTypes.ThirtyMin)
            {
                chartUpdateInterval = new TimeSpan(0, 30, 0);
            }
            else if (_selectedCandleType == CandleTypes.OneHour)
            {
                chartUpdateInterval = new TimeSpan(1, 0, 0);
            }
            else if (_selectedCandleType == CandleTypes.FourHour)
            {
                chartUpdateInterval = new TimeSpan(4, 0, 0);
            }
            else if (_selectedCandleType == CandleTypes.EightHour)
            {
                chartUpdateInterval = new TimeSpan(8, 0, 0);
            }
            else if (_selectedCandleType == CandleTypes.TwelveHour)
            {
                chartUpdateInterval = new TimeSpan(12, 0, 0);
            }
            else if (_selectedCandleType == CandleTypes.OneDay)
            {
                chartUpdateInterval = new TimeSpan(24, 0, 0);
            }
            else if (_selectedCandleType == CandleTypes.OneWeek)
            {
                chartUpdateInterval = new TimeSpan(168, 0, 0);
            }
            else if (_selectedCandleType == CandleTypes.OneMonth)
            {
                chartUpdateInterval = new TimeSpan(720, 0, 0);
            }

            _dispatcherTimerChart.Stop();
            _dispatcherTimerChart.Interval = chartUpdateInterval;
            _dispatcherTimerChart.Start();
        }
    }

    public string SelectedCandleTypeLabelString
    {
        get
        {
            var ct = _selectedCandleType;
            var candleTypeText = "";
            if (ct == CandleTypes.OneMin)
            {
                candleTypeText = "1 min";//"１分";
            }
            else if (ct == CandleTypes.FiveMin)
            {
                candleTypeText = "5 min";//"５分";
            }
            else if (ct == CandleTypes.FifteenMin)
            {
                candleTypeText = "15 min";//"１５分";
            }
            else if (ct == CandleTypes.ThirtyMin)
            {
                candleTypeText = "30 min";//"３０分";
            }
            else if (ct == CandleTypes.OneHour)
            {
                candleTypeText = "1 hour";//"１時間";
            }
            else if (ct == CandleTypes.FourHour)
            {
                candleTypeText = "4 hours";//"４時間";
            }
            else if (ct == CandleTypes.EightHour)
            {
                candleTypeText = "8 hours";//"８時間";
            }
            else if (ct == CandleTypes.TwelveHour)
            {
                candleTypeText = "12 hours";//"１２時間";
            }
            else if (ct == CandleTypes.OneDay)
            {
                candleTypeText = "1 day";//"１日";
            }
            else if (ct == CandleTypes.OneWeek)
            {
                candleTypeText = "1 week";//"１週間";
            }
            else if (ct == CandleTypes.OneMonth)
            {
                candleTypeText = "1 month";//"１ヵ月";
            }

            return candleTypeText;
            //return $"ロウソク足の選択（{candleTypeText}）";
        }
    }

    private DateTime lastChartLoadedDateTime = DateTime.MinValue;

    #endregion

    #region == Order ==

    private OrderSides _orderSide = OrderSides.buy;
    public OrderSides OrderSide
    {
        get => _orderSide;
        set
        {
            if (_orderSide == value)
            {
                return;
            }

            _orderSide = value;
            OnPropertyChanged(nameof(OrderSide));
        }
    }

    private int _orderSideSelectedIndex; // 0=Buy, 1=Sell
    public int OrderSideSelectedIndex
    {
        get => _orderSideSelectedIndex;
        set
        {
            if (_orderSideSelectedIndex == value)
            {
                return;
            }

            _orderSideSelectedIndex = value;
            OnPropertyChanged(nameof(OrderSideSelectedIndex));

            if (_orderSideSelectedIndex == 0)
            {
                OrderSide = OrderSides.buy;
            }
            else
            {
                OrderSide = OrderSides.sell;
            }
        }
    }

    public ObservableCollection<OrderTypeSelectionForComboBox> OrderTypesForComboBox =
    [
        new OrderTypeSelectionForComboBox(OrderTypes.limit, "指値"),
        new OrderTypeSelectionForComboBox(OrderTypes.market, "成行")
    ];

    private OrderTypeSelectionForComboBox? _selectedOrderTypeSelectionForComboBox;
    public OrderTypeSelectionForComboBox? SelectedOrderTypeForComboBox
    {
        get => _selectedOrderTypeSelectionForComboBox;
        set
        {
            if (_selectedOrderTypeSelectionForComboBox == value)
            {
                return;
            }

             _selectedOrderTypeSelectionForComboBox = value;
            OnPropertyChanged(nameof(SelectedOrderTypeForComboBox));

            if (_selectedOrderTypeSelectionForComboBox?.Key == OrderTypes.limit)
            {
                IsOrderPriceVisible = true;
            }
            else if (_selectedOrderTypeSelectionForComboBox?.Key == OrderTypes.market)
            {
                IsOrderPriceVisible = false;
            }
        }
    }

    private decimal _orderAmount;// = 0.001M; // 通貨別デフォ指定 TODO
    public string OrderAmount
    {
        get => _orderAmount.ToString();
        set
        {
            decimal decimalValue;
            if (decimal.TryParse(value, out decimalValue))
            {
                if (_orderAmount == decimalValue)
                {
                    return;
                }
                _orderAmount = decimalValue;
                OnPropertyChanged(nameof(OrderAmount));
                OnPropertyChanged(nameof(OrderEstimatePrice));
            }
        }
    }

    private decimal _orderPrice; // 通貨別デフォ指定 TODO
    public string OrderPrice
    {
        get => _orderPrice.ToString();
        set
        {
            decimal decimalValue;
            if (decimal.TryParse(value, out decimalValue))
            {
                if (_orderPrice == decimalValue)
                {
                    return;
                }
                _orderPrice = decimalValue;
                OnPropertyChanged(nameof(OrderPrice));
                OnPropertyChanged(nameof(OrderEstimatePrice));
            }
        }
    }

    private bool _isOrderPriceVisible = true;
    public bool IsOrderPriceVisible
    {
        get => _isOrderPriceVisible;
        set
        {
            if (_isOrderPriceVisible == value)
            {
                return;
            }

            _isOrderPriceVisible = value;
            OnPropertyChanged(nameof(IsOrderPriceVisible));
        }
    }

    public string OrderEstimatePrice
    {
        get
        {
            decimal decimalValue;
            // TODO:
            if (_selectedOrderTypeSelectionForComboBox?.Key == OrderTypes.market)
            {
                decimalValue = _orderAmount * Bid;//_bid;
            }
            else
            {
                decimalValue = _orderAmount * _orderPrice;
            }
            return decimalValue.ToString();
        }
    }

    private bool _isOrderPostOnly = true;
    public bool IsOrderPostOnly
    {
        get => _isOrderPostOnly;
        set
        {
            if (_isOrderPostOnly == value)
            {
                return;
            }

            _isOrderPostOnly = value;
            OnPropertyChanged(nameof(IsOrderPostOnly));
        }
    }

    private string _makeOrderResultText = string.Empty;
    public string MakeOrderRresultText
    {
        get => _makeOrderResultText;
        set
        {
            if (_makeOrderResultText == value)
            {
                return;
            }

            _makeOrderResultText = value;
            OnPropertyChanged(nameof(MakeOrderRresultText));
        }
    }

    #endregion

    #region == HTTP Clients ==

    // HTTP Clients
    //private readonly PublicAPIClient _pubCandlestickApi = new();
    //private readonly PublicAPIClient _pubTransactionsApi = new();
    //private readonly PublicAPIClient _pubDepthApi = new();

    #endregion

    #region == Timers ==
    // Timer
    private readonly DispatcherTimer _dispatcherTimerChart = new();
    private readonly DispatcherTimer _dispatcherTimerDepth = new();
    private readonly DispatcherTimer _dispatcherTimerTransaction = new();
    private readonly DispatcherTimer _dispatcherTimerOrders = new(); 
    private readonly DispatcherTimer _dispatcherTimerTradeHistory = new(); 
    #endregion

    #region == Events ==

    public delegate void DepthCenterEventHandler();
    public event DepthCenterEventHandler? DepthScrollCenter;

    //public event EventHandler? EventShowNewModal;

    #endregion

    private readonly IModalDialogService _dlg;

    // 
    public PairViewModel(PairCodes p, double fontSize, string ltpFormstString, string currencyFormstString, decimal grouping100, decimal grouping1000)
    {
        _dlg = App.GetService<IModalDialogService>();

        _p = p;
        _ltpFontSize = fontSize;
        _ltpFormstString = ltpFormstString;
        _currencyFormstString = currencyFormstString;

        _depthGrouping100 = grouping100;
        _depthGrouping1000 = grouping1000;

        SelectedOrderTypeForComboBox = OrderTypesForComboBox[0];


        #region == RelayCommands ==

        ChangeCandleTypeCommand = new GenericRelayCommand<CandleTypes>(param => ChangeCandleTypeCommand_Execute(param), param => ChangeCandleTypeCommand_CanExecute());
        CancelOrderCommand = new GenericRelayCommand<IList<object>>(param => CancelOrderCommand_Execute(param), param => CancelOrderCommand_CanExecute());

        #endregion

        #region == Timers ==

        // Depth update timer
        _dispatcherTimerDepth.Tick += TickerTimerDepth;
        _dispatcherTimerDepth.Interval = new TimeSpan(0, 0, 5);
        _dispatcherTimerDepth.Start();

        // Transaction update timer
        _dispatcherTimerTransaction.Tick += TickerTimerTransaction;
        _dispatcherTimerTransaction.Interval = new TimeSpan(0, 0, 8);
        _dispatcherTimerTransaction.Start();

        // Chart update timer
        _dispatcherTimerChart.Tick += TickerTimerChart;
        _dispatcherTimerChart.Interval = chartUpdateInterval;
        _dispatcherTimerChart.Start();

        // Orders update timer
        _dispatcherTimerOrders.Tick += TickerTimerOrders;
        _dispatcherTimerOrders.Interval = new TimeSpan(0, 0, 20);
        _dispatcherTimerOrders.Start();

        // TradeHistory update timer
        _dispatcherTimerTradeHistory.Tick += TickerTimerTradeHistory;
        _dispatcherTimerTradeHistory.Interval = new TimeSpan(0, 0, 20);
        _dispatcherTimerTradeHistory.Start();

        #endregion

    }

    // Called from MainViewModel.
    public void InitializeAndLoad(MainViewModel vm)
    {
        MainViewModel = vm;

        MainViewModel.PriAssetsApi.ErrorOccured += new PrivateAPIClient.ClinetErrorEvent(OnClientError);
        MainViewModel.PriOrdersApi.ErrorOccured += new PrivateAPIClient.ClinetErrorEvent(OnClientError);

        if (IsChartInitAndLoaded) 
        {
            if (lastChartLoadedDateTime.Add(chartUpdateInterval) < DateTime.Now)
            {
                LoadChart(SelectedCandleType);
                _dispatcherTimerChart.Stop();
                _dispatcherTimerChart.Start();
            }
        }
        else
        {
            Sections[0].Yi = 0;
            Sections[0].Yj = 0;

            // Little hack to init. This is required after upgrading Livechart2 to 2.0 rc.
            // clear chart data.
            Series[0].Values = new ObservableCollection<DateTimePoint>
            {
                new(DateTime.Now, 1)
            };
            Series[1].Values = new ObservableCollection<FinancialPoint>
            {
                new(DateTime.Now, 100, 0, 0, 0)
            };

            LoadChart(SelectedCandleType);
        }

        Task.Run(() => GetOrders());
        Task.Run(() => GetTradeHistory());
        Task.Run(() => GetDepth());
    }

    public void CleanUp()
    {
        try
        {
            _dispatcherTimerChart.Stop();
            _dispatcherTimerDepth.Stop();
            _dispatcherTimerOrders.Stop();
            _dispatcherTimerTransaction.Stop();
            _dispatcherTimerTradeHistory.Start();

            //_pubCandlestickApi.Dispose();
            //_pubTransactionsApi.Dispose();
            //_pubDepthApi.Dispose();
        }
        catch(Exception ex)
        {
            Debug.WriteLine("Error while Shutdown() : " + ex);
        }
    }

    #region == Chart ==

    private void TickerTimerChart(object? source, object e)
    {
        UpdateChart();
    }

    private void UpdateChart()
    {
        if (!IsChartInitAndLoaded)
        {
            return;
        }

        if (!IsEnabled)
        {
            return;
        }

        if (!IsSelectedActive)
        {
            return;
        }

        /*
        Task.Run(async () =>
        {

        });
        */
        /*
        List<Ohlcv> res = await GetCandlesticks(this.PairCode, SelectedCandleType);

        if (res == null) return;

        if (res.Count > 0)
        {
            LoadChart(res, SelectedCandleType);

            Sections[0].Yi = (double)_ltp;
            Sections[0].Yj = (double)_ltp;
        }
        */

        LoadChart(SelectedCandleType);
    }

    private void ChangeCandleType(CandleTypes candleType)
    {
        if (candleType == SelectedCandleType)
        {
            return;
        }

        // set new candle type
        SelectedCandleType = candleType;

        // clear chart data.
        Series[0].Values = new ObservableCollection<DateTimePoint>
        {
            //new DateTimePoint(DateTime.Now, 1)
        };
        Series[1].Values = new ObservableCollection<FinancialPoint>
        {
            //new(DateTime.Now, 100, 0, 0, 0)
        };

        LoadChart(SelectedCandleType);
    }

    private async void LoadChart(CandleTypes ct)
    {        
        // gets new data.
        var res = await GetCandlesticks(PairCode, ct);

        if (res == null)
        {
            return;
        }

        if (res.Count > 0)
        {
            DoLoadChart(res, ct);

            Sections[0].Yi = (double)_ltp;
            Sections[0].Yj = (double)_ltp;
        }
    }

    private void DoLoadChart(List<Ohlcv> list, CandleTypes ct)
    {
        //Debug.WriteLine("DoLoadChart: " + this.PairCode + ", " + ct.ToString());

        // Need to be here. not static and all.
        if (_currencyFormstString.Equals("C3"))
        {
            YAxes[1].Labeler = (value) => value.ToString("C3", new CultureInfo("ja-JP"));
        }
        else if (_currencyFormstString.Equals("C2"))
        {
            YAxes[1].Labeler = (value) => value.ToString("C2", new CultureInfo("ja-JP"));
        }
        else if (_currencyFormstString.Equals("C1"))
        {
            YAxes[1].Labeler = (value) => value.ToString("C1", new CultureInfo("ja-JP"));
        }
        else
        {
            YAxes[1].Labeler = (value) => value.ToString("C", new CultureInfo("ja-JP"));
        }

        //TODO: localize aware
        // キャンドルタイプにあわせてチャートXAxes[0]表示tweak
        if (ct == CandleTypes.OneMin)
        {
            XAxes[0].Labeler = value => new DateTime((long)value).ToString("H:mm");
            XAxes[0].UnitWidth = TimeSpan.FromMinutes(0.4).Ticks;
            //XAxes[0].MinStep = TimeSpan.FromMinutes(10).Ticks;
            XAxes[0].MinLimit = DateTime.Now.Ticks - TimeSpan.FromMinutes(60).Ticks;
            XAxes[0].MaxLimit = null;
        }
        else if (ct == CandleTypes.FiveMin)
        {
            XAxes[0].Labeler = value => new DateTime((long)value).ToString("H:mm");
            XAxes[0].UnitWidth = TimeSpan.FromMinutes(2.5).Ticks;
            //XAxes[0].MinStep = TimeSpan.FromMinutes(10).Ticks;
            XAxes[0].MinLimit = DateTime.Now.Ticks - TimeSpan.FromMinutes(300).Ticks;
            XAxes[0].MaxLimit = null;
        }
        else if (ct == CandleTypes.FifteenMin)
        {
            XAxes[0].Labeler = value => new DateTime((long)value).ToString("M/d H:mm");
            XAxes[0].UnitWidth = TimeSpan.FromMinutes(7).Ticks;
            //XAxes[0].MinStep = TimeSpan.FromMinutes(15).Ticks;
            XAxes[0].MinLimit = DateTime.Now.Ticks - TimeSpan.FromMinutes(750).Ticks;
            XAxes[0].MaxLimit = null;
        }
        else if (ct == CandleTypes.ThirtyMin)
        {
            XAxes[0].Labeler = value => new DateTime((long)value).ToString("M/d H:mm");
            XAxes[0].UnitWidth = TimeSpan.FromMinutes(15).Ticks;
            //XAxes[0].MinStep = TimeSpan.FromMinutes(30).Ticks;
            XAxes[0].MinLimit = DateTime.Now.Ticks - TimeSpan.FromMinutes(1500).Ticks;
            XAxes[0].MaxLimit = null;
        }
        else if (ct== CandleTypes.OneHour)
        {
            XAxes[0].Labeler = value => new DateTime((long)value).ToString("M/d H:mm");
            XAxes[0].UnitWidth = TimeSpan.FromHours(0.5).Ticks;
            //XAxes[0].MinStep = TimeSpan.FromDays(1).Ticks;
            XAxes[0].MinLimit = DateTime.Now.Ticks - TimeSpan.FromDays(3).Ticks;
            XAxes[0].MaxLimit = null;
        }
        else if (ct == CandleTypes.FourHour)
        {
            XAxes[0].Labeler = value => new DateTime((long)value).ToString("M/d HH");
            XAxes[0].UnitWidth = TimeSpan.FromHours(2).Ticks;
            //XAxes[0].MinStep = TimeSpan.FromHours(4).Ticks;
            XAxes[0].MinLimit = DateTime.Now.Ticks - TimeSpan.FromDays(6).Ticks;
            XAxes[0].MaxLimit = null;
        }
        else if (ct == CandleTypes.EightHour)
        {
            XAxes[0].Labeler = value => new DateTime((long)value).ToString("M/d HH");
            XAxes[0].UnitWidth = TimeSpan.FromHours(4).Ticks;
            //XAxes[0].MinStep = TimeSpan.FromHours(8).Ticks;
            XAxes[0].MinLimit = DateTime.Now.Ticks - TimeSpan.FromDays(12).Ticks;
            XAxes[0].MaxLimit = null;
        }
        else if (ct == CandleTypes.TwelveHour)
        {
            XAxes[0].Labeler = value => new DateTime((long)value).ToString("yyyy M/d");
            XAxes[0].UnitWidth = TimeSpan.FromHours(6).Ticks;
            //XAxes[0].MinStep = TimeSpan.FromDays(0.5).Ticks;
            XAxes[0].MinLimit = DateTime.Now.Ticks - TimeSpan.FromDays(24).Ticks;
            XAxes[0].MaxLimit = null;
        }
        else if (ct == CandleTypes.OneDay)
        {
            XAxes[0].Labeler = value => new DateTime((long)value).ToString("yyyy M/d");
            XAxes[0].UnitWidth = TimeSpan.FromDays(0.4).Ticks;
            //XAxes[0].MinStep = TimeSpan.FromDays(1).Ticks;
            XAxes[0].MinLimit = DateTime.Now.Ticks - TimeSpan.FromDays(90).Ticks;
            XAxes[0].MaxLimit = null;
        }
        else if (ct == CandleTypes.OneWeek)
        {
            XAxes[0].Labeler = value => new DateTime((long)value).ToString("yyyy M/d");
            XAxes[0].UnitWidth = TimeSpan.FromDays(2).Ticks;
            //XAxes[0].MinStep = TimeSpan.FromDays(1).Ticks;
            XAxes[0].MinLimit = DateTime.Now.Ticks - TimeSpan.FromDays(300).Ticks;
            XAxes[0].MaxLimit = null;
        }
        else if (ct == CandleTypes.OneMonth)
        {
            XAxes[0].Labeler = value => new DateTime((long)value).ToString("yyyy/M");
            XAxes[0].UnitWidth = TimeSpan.FromDays(21).Ticks;
            //XAxes[0].MinStep = TimeSpan.FromDays(30).Ticks;
            XAxes[0].MinLimit = DateTime.Now.Ticks - TimeSpan.FromDays(360*3).Ticks;
            XAxes[0].MaxLimit = null;
        }

        var ohlcs = new ObservableCollection<FinancialPoint>();
        var vols = new ObservableCollection<DateTimePoint>();

        foreach (var hoge in list)
        {
            vols.Add(new DateTimePoint(hoge.TimeStamp, (double)hoge.Volume));

            ohlcs.Add(new FinancialPoint(hoge.TimeStamp, (double)hoge.High, (double)hoge.Open, (double)hoge.Close, (double)hoge.Low));
        }

        Series[0].Values = vols;
        Series[1].Values = ohlcs;

        IsChartInitAndLoaded = true;

        lastChartLoadedDateTime = DateTime.Now;
    }

    private async Task<List<Ohlcv>?> GetCandlesticks(PairCodes pair, CandleTypes ct)
    {
        List<Ohlcv>? OhlcvList =  [];

        //Debug.WriteLine("チャートデータを取得中.... " + pair.ToString());

        // 今日の日付セット。UTCで。
        var dtToday = DateTime.Now.ToUniversalTime();

        // データは、ローカルタイムで、朝9:00 から翌8:59分まで。8:59分までしか取れないので、 9:00過ぎていたら 最新のデータとるには日付を１日追加する

        var daysOrYearsCountToGetData = 0;

        var isYearly = false;

        if (ct == CandleTypes.OneMin)
        {
            daysOrYearsCountToGetData = 2;
        }
        else if (ct == CandleTypes.FiveMin)
        {
            daysOrYearsCountToGetData = 2;
        }
        else if (ct == CandleTypes.FifteenMin)
        {
            daysOrYearsCountToGetData = 4;
        }
        else if (ct == CandleTypes.ThirtyMin)
        {
            daysOrYearsCountToGetData = 4;
        }
        else if (ct == CandleTypes.OneHour)
        {
            daysOrYearsCountToGetData = 6;
        }
        else if (ct == CandleTypes.FourHour)
        {
            isYearly = true;
            daysOrYearsCountToGetData = 2;
        }
        else if (ct == CandleTypes.EightHour)
        {
            isYearly = true;
            daysOrYearsCountToGetData = 2;
        }
        else if (ct == CandleTypes.TwelveHour)
        {
            isYearly = true;
            daysOrYearsCountToGetData = 2;
        }
        else if (ct == CandleTypes.OneDay)
        {
            isYearly = true;
            daysOrYearsCountToGetData = 3;
        }
        else if (ct == CandleTypes.OneWeek)
        {
            isYearly = true;
            daysOrYearsCountToGetData = 5;
        }
        else if (ct == CandleTypes.OneMonth)
        {
            isYearly = true;
            daysOrYearsCountToGetData = 10;
        }

        int i;

        for (i = 0; i < daysOrYearsCountToGetData;)
        {
            if (i <= 0)
            {
                OhlcvList = await GetCandlestick(pair, ct, dtToday);

                if (OhlcvList != null)
                {
                    OhlcvList.Reverse();
                }
                else
                {
                    Debug.WriteLine("failed to get candlestic.");
                    //return OhlcvList  = new List<Ohlcv>(); 
                }
            }
            else
            {
                DateTime dateTarget;

                if (isYearly)
                {
                    dateTarget = dtToday.AddDays(-(i * 365));
                }
                else
                {
                    dateTarget = dtToday.AddDays(-i);
                }

                var responseOhlcvList = await GetCandlestick(pair, ct, dateTarget);

                if (responseOhlcvList != null)
                {
                    responseOhlcvList.Reverse();

                    foreach (var r in responseOhlcvList)
                    {
                        OhlcvList ??= [];
                        OhlcvList.Add(r);
                    }
                }
                else
                {
                    Debug.WriteLine("failed to get candlestic.");
                    //OhlcvList.Clear();
                    //return OhlcvList;
                }
            }

            await Task.Delay(100);

            i++;
        }

        return OhlcvList;
    }

    private async Task<List<Ohlcv>?> GetCandlestick(PairCodes pair, CandleTypes ct, DateTime dtTarget)
    {
        string ctString;
        string dtString;

        if (ct == CandleTypes.OneMin)
        {
            ctString = "1min";
            dtString = dtTarget.ToString("yyyyMMdd");
        }
        else if (ct == CandleTypes.FiveMin)
        {
            ctString = "5min";
            dtString = dtTarget.ToString("yyyyMMdd");
        }
        else if (ct == CandleTypes.FifteenMin)
        {
            ctString = "15min";
            dtString = dtTarget.ToString("yyyyMMdd");
        }
        else if (ct == CandleTypes.ThirtyMin)
        {
            ctString = "30min";
            dtString = dtTarget.ToString("yyyyMMdd");
        }
        else if (ct == CandleTypes.OneHour)
        {
            ctString = "1hour";
            dtString = dtTarget.ToString("yyyyMMdd");
        }
        else if (ct == CandleTypes.FourHour)
        {
            ctString = "4hour";
            dtString = dtTarget.ToString("yyyy");
        }
        else if (ct == CandleTypes.EightHour)
        {
            ctString = "8hour";
            dtString = dtTarget.ToString("yyyy");
        }
        else if (ct == CandleTypes.TwelveHour)
        {
            ctString = "12hour";
            dtString = dtTarget.ToString("yyyy");
        }
        else if (ct == CandleTypes.OneDay)
        {
            ctString = "1day";
            dtString = dtTarget.ToString("yyyy");
        }
        else if (ct == CandleTypes.OneWeek)
        {
            ctString = "1week";
            dtString = dtTarget.ToString("yyyy");
        }
        else if (ct == CandleTypes.OneMonth)
        {
            ctString = "1month";
            dtString = dtTarget.ToString("yyyy");
        }
        else
        {
            throw new InvalidOperationException("Unsupported.");
            //return null;
        }

        var csr = await PublicAPIClient.GetCandlestick(pair.ToString(), ctString, dtString);

        if (!IsEnabled)
        {
            return null;
        }

        if (csr != null)
        {
            if (csr.IsSuccess == true)
            {
                if (csr.Candlesticks.Count > 0)
                {
                    // ロウソク足タイプが同じかどうか一応確認
                    if (csr.CandleType.ToString() == ct.ToString())
                    {
                        return csr.Candlesticks;
                    }
                }
            }
            else
            {
                Debug.WriteLine("GetCandlestick: GetCandlestick returned error");
            }
        }
        else
        {
            Debug.WriteLine("GetCandlestick: GetCandlestick returned null");
        }

        return null;
    }

    #endregion

    #region == TradeHistory ==

    private void TickerTimerTradeHistory(object? source, object e)
    {
        if (!IsEnabled)
        {
            return;
        }

        if (!IsSelectedActive)
        {
            return;
        }

        //Task.Run(() => GetTradeHistory());
        GetTradeHistory();
    }

    private void GetTradeHistory()
    {
        if (MainViewModel == null)
        {
            return;
        }

        if (MainViewModel.TradeHistoryApiKeyIsSet == false)
        {
            // TODO show message?
            System.Diagnostics.Debug.WriteLine("■■■■■ GetOrders: (TradeHistoryApiKeyIsSet == false)");
            return;
        }

        App.CurrentDispatcherQueue?.TryEnqueue(async () =>
        {
            try
            {

                var trd = await MainViewModel.PriOrdersApi.GetTradeHistory(MainViewModel.TradeHistoryApiKey, MainViewModel.TradeHistorySecret, PairCode.ToString());

                if (trd != null)
                {
                    // 逆順にする
                    trd.TradeList.Reverse();

                    App.CurrentDispatcherQueue?.TryEnqueue(() =>
                    {
                        foreach (var tr in trd.TradeList)
                        {
                            var found = TradeHistories.FirstOrDefault(x => x.TradeID == tr.TradeID);
                            if (found == null)
                            {
                                // "btc_jpy" を "BTC/JPY"に。
                                if (GetPairCodes.TryGetValue(tr.Pair, out var value))
                                {
                                    tr.Pair = PairStrings[value];
                                }

                                found = new Trade
                                {
                                    TradeID = tr.TradeID,
                                    OrderID = tr.OrderID,
                                    Pair = tr.Pair,
                                    Price = tr.Price,
                                    ExecutedAt = tr.ExecutedAt,
                                    Amount = tr.Amount,
                                    Side = tr.Side,
                                    Type = tr.Type,
                                    //FeeAmountBase = tr.FeeAmountBase,
                                    FeeOccurredAmountQuote = tr.FeeOccurredAmountQuote,
                                    FeeAmountQuote = tr.FeeAmountQuote,
                                    MakerTaker = tr.MakerTaker,
                                };

                                TradeHistories.Insert(0, found);
                            }

                        }

                    });

                    return;
                }
                else
                {
                    // TODO:
                    /*
                    _tradeHistories = -1;
                    NotifyPropertyChanged(nameof(TradeHistoryTitle));

                    APIResultTradeHistory = "<<取得失敗>>";
                    */

                    System.Diagnostics.Debug.WriteLine("■■■■■ GetTradeHistory is null.");

                    return;
                }

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine("■■■■■ GetTradeHistory Exception: " + e);

                return;
            }
        });
    }

    #endregion

    #region == Orders ==

    private void TickerTimerOrders(object? source, object e)
    {
        if (!IsEnabled)
        {
            return;
        }

        if (!IsSelectedActive)
        {
            return;
        }

        //Task.Run(() => GetOrders());
        /*
        App.CurrentDispatcherQueue?.TryEnqueue(async () =>
        {
            await GetOrders();
        });
        */
        GetOrders();
    }

    private void GetOrders()
    {
        if (MainViewModel == null)
        {
            return;
        }

        if (MainViewModel.OrdersApiKeyIsSet == false)
        {
            // TODO show message?
            Debug.WriteLine("■■■■■ GetOrders: (OrdersApiKeyIsSet == false)");
            return;
        }

        App.CurrentDispatcherQueue?.TryEnqueue(async () =>
        {
            var ords = await MainViewModel.PriOrdersApi.GetOrderList(MainViewModel.OrdersApiKey, MainViewModel.OrdersSecret, PairCode.ToString());
            if (ords == null)
            {
                return;
            }
            if (ords.IsSuccess == false)
            {
                return;
            }

            try
            {
                // 逆順にする
                ords.OrderList.Reverse();

                foreach (var ord in ords.OrderList)
                {
                    var found = Orders.FirstOrDefault(x => x.OrderID == ord.OrderID);

                    if (found != null)
                    {
                        found.AveragePrice = ord.AveragePrice;
                        found.OrderedAt = ord.OrderedAt;
                        found.Pair = ord.Pair;
                        found.Price = ord.Price;
                        found.RemainingAmount = ord.RemainingAmount;
                        found.ExecutedAmount = ord.ExecutedAmount;
                        found.Side = ord.Side;
                        found.StartAmount = ord.StartAmount;
                        found.Type = ord.Type;
                        found.Status = ord.Status;

                        // 現在値のセット
                        // 投資金額
                        if (found.Type == "limit")
                        {
                            found.ActualPrice = (ord.Price * ord.StartAmount);
                            // 一部約定の時は考えない？
                        }
                        else
                        {
                            found.ActualPrice = (ord.AveragePrice * ord.StartAmount);
                        }

                        // 現在値との差額
                        if ((found.Status == "UNFILLED") || (found.Status == "PARTIALLY_FILLED"))
                        {
                            found.Shushi = (ord.Price - _ltp);
                        }
                        else
                        {
                            // 約定済みなので
                            found.Shushi = 0;
                        }
                    }
                    else
                    {
                        // 現在値のセット
                        // 投資金額
                        if (ord.Type == "limit")
                        {
                            ord.ActualPrice = (ord.Price * ord.StartAmount);
                        }
                        else
                        {
                            ord.ActualPrice = (ord.AveragePrice * ord.StartAmount);
                        }

                        // 現在値との差額
                        if ((ord.Status == "UNFILLED") || (ord.Status == "PARTIALLY_FILLED"))
                        {
                            //ord.Shushi = ((_ltp - ord.Price));
                            ord.Shushi = (ord.Price - _ltp);

                            // リスト追加
                            Orders.Insert(0, ord);
                        }
                        else
                        {
                            // 約定済みなので
                            //ord.Shushi = 0;

                            // 追加しない。
                        }

                    }
                }

                // 返ってきた注文リストに存在しない注文リスト
                var lst = new List<ulong>();

                // 返ってきた注文リストに存在しない注文を抽出
                foreach (var ors in Orders)
                {
                    var found = ords.OrderList.FirstOrDefault(x => x.OrderID == ors.OrderID);
                    if (found == null)
                    {
                        if (string.IsNullOrEmpty(ors.Status) || (ors.Status == "UNFILLED") || (ors.Status == "PARTIALLY_FILLED"))
                        {
                            //if ( (ors.Status != "FULLY_FILLED") || (ors.Status != "CANCELED_UNFILLED") || (ors.Status != "CANCELED_PARTIALLY_FILLED")) { 
                            lst.Add(ors.OrderID);
                        }
                    }

                }

                // 注文リスト更新
                if (lst.Count > 0)
                {
                    // リストのリスト（小分けにして分割取得用）
                    var ListOfList = new List<List<ulong>>();

                    // GetOrderListByIDs 40015 数が多いとエラーになるので、小分けにして。
                    var temp = new List<ulong>();
                    var c = 0;

                    for (var i = 0; i < lst.Count; i++)
                    {

                        temp.Add(lst[c]);

                        if (temp.Count == 5)
                        {
                            ListOfList.Add(temp);

                            temp = [];
                        }

                        if (c == lst.Count - 1)
                        {
                            if (temp.Count > 0)
                            {
                                ListOfList.Add(temp);
                            }

                            break;
                        }

                        c += 1;
                    }

                    foreach (var list in ListOfList)
                    {
                        // 最新の注文情報をゲット
                        Orders? oup = await MainViewModel.PriOrdersApi.GetOrderListByIDs(MainViewModel.OrdersApiKey, MainViewModel.OrdersSecret, PairCode.ToString(), list);
                        if (oup != null)
                        {
                            // 注文をアップデート
                            foreach (var ord in oup.OrderList)
                            {
                                try
                                {
                                    var found = Orders.FirstOrDefault(x => x.OrderID == ord.OrderID);
                                    if (found != null)
                                    {
                                        var i = Orders.IndexOf(found);
                                        if (i > -1)
                                        {
                                            Orders[i].Status = ord.Status;
                                            Orders[i].AveragePrice = ord.AveragePrice;
                                            Orders[i].OrderedAt = ord.OrderedAt;
                                            Orders[i].Type = ord.Type;
                                            Orders[i].StartAmount = ord.StartAmount;
                                            Orders[i].RemainingAmount = ord.RemainingAmount;
                                            Orders[i].ExecutedAmount = ord.ExecutedAmount;
                                            Orders[i].Price = ord.Price;
                                            Orders[i].AveragePrice = ord.AveragePrice;


                                            // 現在値のセット
                                            // 投資金額
                                            if (Orders[i].Type == "limit")
                                            {
                                                Orders[i].ActualPrice = (ord.Price * ord.StartAmount);
                                            }
                                            else
                                            {
                                                Orders[i].ActualPrice = (ord.AveragePrice * ord.StartAmount);
                                            }

                                            // 現在値との差額
                                            if ((Orders[i].Status == "UNFILLED") || (Orders[i].Status == "PARTIALLY_FILLED"))
                                            {
                                                Orders[i].Shushi = ((_ltp - ord.Price));
                                            }
                                            else
                                            {
                                                // 約定済みなので
                                                //orders[i].Shushi = 0;

                                                // 約定済みは単に削除
                                                Orders.Remove(Orders[i]);

                                            }

                                        }
                                    }
                                }
                                catch (Exception ex)
                                {
                                    Debug.WriteLine("■■■■■ Order oup: Exception - " + ex.Message);

                                }

                                await Task.Delay(400);
                            }
                        }
                    }
                }

            }
            catch (Exception e)
            {
                Debug.WriteLine("■■■■■ GetOrders Exception: " + e);

                return;
            }

        });

    }

    #endregion

    #region == Depth ==

    private void TickerTimerDepth(object? source, object e)
    {
        if (!IsEnabled)
        {
            return;
        }

        if (!IsSelectedActive)
        {
            return;
        }

        UpdateDepth();
    }

    private void UpdateDepth()
    {
        // timer ver
        if (IsSelectedActive && IsPaneVisible && IsEnabled)
        {
            GetDepth();
        }
    }

    private void GetDepth()
    {
        var pair = PairCode;

        // まとめグルーピング単位 
        var unit = DepthGrouping;

        // リスト数 （基本 上売り200、下買い200）
        var half = 200;
        var listCount = (half * 2) + 1;

        if (_depth == null)
        {
            return;
        }


        App.CurrentDispatcherQueue?.TryEnqueue(async () =>
        {


            // 初期化
            if ((_depth.Count == 0) || (_depth.Count < listCount))
            {
                //_depth.Clear();
                for (var i = 0; i < listCount; i++)
                {
                    Depth dd = new(_ltpFormstString)
                    {
                        DepthPrice = i,
                        DepthBid = 0,
                        DepthAsk = 0
                    };
                    //if (i == (half-1)) dd.IsLTP = true;
                    _depth.Add(dd);
                }

                // scroll center
                App.CurrentDispatcherQueue?.TryEnqueue(() =>
                {
                    DepthScrollCenter?.Invoke();
                });
            }
            else
            {
                if (IsDepthGroupingChanged)
                {
                    //グルーピング単位が変わったので、一旦クリアする。
                    for (var i = 0; i < _depth.Count - 1; i++)
                    {
                        _depth[i].DepthPrice = 0;
                        _depth[i].DepthBid = 0;
                        _depth[i].DepthAsk = 0;
                    }

                    IsDepthGroupingChanged = false;
                }
            }

            // LTP を追加
            _depth[half].DepthPrice = Ltp;
            _depth[half].IsLTP = true;

            var dpr = await PublicAPIClient.GetDepth(pair.ToString());

            if (!IsEnabled)
            {
                return;
            }

            if (dpr != null)
            {
                if (_depth == null)
                {
                    return;
                }


                if (_depth.Count != 0)
                {
                    var i = 1;

                    // 100円単位でまとめる
                    // まとめた時の価格
                    decimal c2 = 0;
                    // 100単位ごとにまとめたAsk数量を保持
                    decimal t = 0;
                    // 先送りするAsk
                    decimal d = 0;
                    // 先送りする価格
                    decimal e = 0;

                    // ask をループ
                    foreach (var dp in dpr.DepthAskList)
                    {
                        // まとめ表示On
                        if (unit > 0)
                        {

                            if (c2 == 0)
                            {
                                c2 = Math.Ceiling(dp.DepthPrice / unit);
                            }

                            // 100円単位でまとめる
                            if (Math.Ceiling(dp.DepthPrice / unit) == c2)
                            {
                                t += dp.DepthAsk;
                            }
                            else
                            {
                                //Debug.WriteLine(System.Math.Ceiling(dp.DepthPrice / unit).ToString() + " " + System.Math.Ceiling(c / unit).ToString());

                                // 一時保存
                                e = dp.DepthPrice;
                                dp.DepthPrice = c2 * unit;

                                // 一時保存
                                d = dp.DepthAsk;
                                dp.DepthAsk = t;

                                _depth[half - i].DepthAsk = dp.DepthAsk;
                                _depth[half - i].DepthBid = dp.DepthBid;
                                _depth[half - i].DepthPrice = dp.DepthPrice;
                                _depth[half - i].PriceFormat = _ltpFormstString;

                                // 今回のAskは先送り
                                t = d;
                                // 今回のPriceが基準になる
                                c2 = Math.Ceiling(e / unit);

                                i++;
                            }

                        }
                        else
                        {
                            //dp.PriceFormat = this._ltpFormstString;
                            //_depth[half - i] = dp;
                            _depth[half - i].DepthPrice = dp.DepthPrice;
                            _depth[half - i].DepthBid = dp.DepthBid;
                            _depth[half - i].DepthAsk = dp.DepthAsk;
                            _depth[half - i].PriceFormat = _ltpFormstString;

                            i++;
                        }
                    }

                    _depth[half - 1].IsAskBest = true;

                    i = half + 1;

                    // 100円単位でまとめる
                    // まとめた時の価格
                    decimal c = 0;
                    // 100単位ごとにまとめた数量を保持
                    t = 0;
                    // 先送りするBid
                    d = 0;
                    // 先送りする価格
                    e = 0;

                    // bid をループ
                    foreach (var dp in dpr.DepthBidList)
                    {
                        if (unit > 0)
                        {

                            if (c == 0)
                            {
                                c = Math.Ceiling(dp.DepthPrice / unit);
                            }

                            // 100円単位でまとめる
                            if (Math.Ceiling(dp.DepthPrice / unit) == c)
                            {
                                t += dp.DepthBid;
                            }
                            else
                            {
                                // 一時保存
                                e = dp.DepthPrice;
                                dp.DepthPrice = c * unit;

                                // 一時保存
                                d = dp.DepthBid;
                                dp.DepthBid = t;

                                // 追加
                                _depth[i].DepthAsk = dp.DepthAsk;
                                _depth[i].DepthBid = dp.DepthBid;
                                _depth[i].DepthPrice = dp.DepthPrice;

                                // 今回のBidは先送り
                                t = d;
                                // 今回のPriceが基準になる
                                c = Math.Ceiling(e / unit);

                                i++;
                            }
                        }
                        else
                        {
                            //dp.PriceFormat = this._ltpFormstString;
                            //_depth[i] = dp;

                            _depth[i].DepthPrice = dp.DepthPrice;
                            _depth[i].DepthBid = dp.DepthBid;
                            _depth[i].DepthAsk = dp.DepthAsk;
                            _depth[i].PriceFormat = _ltpFormstString;
                            i++;
                        }
                    }

                    _depth[half + 1].IsBidBest = true;
                }

                return;
            }
            else
            {
                return;
            }

        });

    }

    #endregion

    #region == Transaction ==

    private void TickerTimerTransaction(object? source, object e)
    {
        if (!IsEnabled)
        {
            return;
        }

        if (!IsSelectedActive)
        {
            return;
        }

        UpdateTransactions();
    }

    private async void UpdateTransactions()
    {
        // timer ver
        if (IsSelectedActive && IsPaneVisible && IsEnabled)
        {
            await GetTransactions(PairCode);
        }
        /*
while (true)
{
   if ((IsSelectedActive == false) || (IsPaneVisible == false) || (IsEnabled == false))
   {
       await Task.Delay(2000);
       continue;
   }
   else
   {
       try
       {
           await GetTransactions(this.PairCode);
       }
       catch (Exception e)
       {
           Debug.WriteLine("■■■■■ UpdateTransactions Exception: " + e);
       }
   }

   await Task.Delay(2000);
}
*/
    }

    private async Task<bool> GetTransactions(PairCodes pair)
    {
        if (!IsEnabled)
        {
            return false;
        }

        var trs = await PublicAPIClient.GetTransactions(pair.ToString());

        if (trs != null)
        {
            App.CurrentDispatcherQueue?.TryEnqueue(() =>
            {
                if (_transactions == null)
                {
                    return;
                }

                if (_transactions.Count == 0)
                {
                    // 60 で初期化
                    for (var i = 0; i < 60; i++)
                    {
                        Transaction dd = new(_ltpFormstString);

                        _transactions.Add(dd);
                    }
                }

                if (trs.Trans != null)
                {
                    var v = 0;
                    foreach (var tr in trs.Trans)
                    {
                        _transactions[v].Amount = tr.Amount;
                        _transactions[v].ExecutedAt = tr.ExecutedAt;
                        _transactions[v].Price = tr.Price;
                        _transactions[v].Side = tr.Side;
                        _transactions[v].TransactionId = tr.TransactionId;

                        v++;

                        if (v >= 60)
                        {
                            break;
                        }
                    }
                }
            });

            return true;
        }
        else
        {
            Debug.WriteLine("■■■■■ GetTransactions returned null");
            return false;
        }
    }

    #endregion

    #region == Error ==

    private void OnClientError(BaseClient sender, ClientError err)
    {
        if (err == null) { return; }
        /*
        // TODO
        err.ErrPlaceParent = "";

        _errors.Insert(0, err);

        if (Application.Current == null) { return; }
        Application.Current.Dispatcher.Invoke(() =>
        {
            // タブの「エラー（＊）」を更新
            ErrorsCount = _errors.Count;

        });

        // ついでにAPI client からのエラーもログ保存の方に追加する。
        mylogs.AddMyErrorLogs("ClientError: " + "Type " + err.ErrType + ", Error code " + err.ErrCode + ", Error path " + err.ErrPlace + ", Error text " + err.ErrText + ", Error description " + err.ErrEx + " - " + err.ErrDatetime.ToString());
    */
    }

    #endregion

    #region == Commands ==

    private RelayCommand? togglePaneVisibilityCommand;
    public IRelayCommand TogglePaneVisibilityCommand => togglePaneVisibilityCommand ??= new RelayCommand(TogglePaneVisibility);
    public void TogglePaneVisibility()
    {
        IsPaneVisible = !IsPaneVisible;
    }

    public ICommand ChangeCandleTypeCommand { get; set; }
    public static bool ChangeCandleTypeCommand_CanExecute()
    {
        return true;
    }
    public void ChangeCandleTypeCommand_Execute(CandleTypes candleType)
    {
        ChangeCandleType(candleType);
    }

    // Show New Modal window command
    private RelayCommand? showNewModalCommand;
    public IRelayCommand ShowNewModalCommand => showNewModalCommand ??= new RelayCommand(ShowNewModal);
    private void ShowNewModal()
    {
        _dlg.ShowOrderDialog(this);
    }

    private RelayCommand? makeOrderCommand;
    public IRelayCommand MakeOrderCommand => makeOrderCommand ??= new RelayCommand(MakeOrder);
    private void MakeOrder()
    {
        MakeOrderRresultText = string.Empty;

        if (MainViewModel == null)
        {
            return;
        }

        if (!MainViewModel.MakeOrderApiKeyIsSet)
        {
            // TODO:  show msg;
            MakeOrderRresultText = "(MakeOrderApiKeyIsSet == false)";
            Debug.WriteLine("■■■■■ MakeOrder: (MakeOrderApiKeyIsSet == false)");
            return;
        }

        //Debug.WriteLine("MakeOrderCommand");

        var orderSide = OrderSide;
        var orderType = SelectedOrderTypeForComboBox?.Key ?? OrderTypes.limit;
        var amount = _orderAmount;
        var price = _orderPrice;
        var isPostOnly = IsOrderPostOnly;

        if (orderType == OrderTypes.limit)
        {
            if (amount <= 0)
            {
                // TODO:  show msg;
                MakeOrderRresultText = "(amount <= 0)";
                return;
            }
        }
        if (price <= 0)
        {
            // TODO:  show msg;
            MakeOrderRresultText = "(price <= 0)";
            return;
        }

        App.CurrentDispatcherQueue?.TryEnqueue(async () =>
        {
            var ord = await MainViewModel.PriOrdersApi.MakeOrder(MainViewModel.MakeOrderApiKey, MainViewModel.MakeOrderSecret, PairCode.ToString(),amount,price, orderSide.ToString(), orderType.ToString(), isPostOnly);
            if (ord == null)
            {
                // TODO:  show msg;
                MakeOrderRresultText = "Fail:(ord == null)";
                return;
            }
            if (ord.IsSuccess == false)
            {
                // TODO:  show msg;
                MakeOrderRresultText = "Fail:ord.IsSuccess == false";
                return;
            }

            MakeOrderRresultText = "Success";

        });
    }


    public ICommand CancelOrderCommand
    {
        get; set;
    }
    public static bool CancelOrderCommand_CanExecute()
    {
        return true;
    }
    public void CancelOrderCommand_Execute(IList<object> selectedOrders)
    {
        MakeOrderRresultText = string.Empty;

        if (MainViewModel == null)
        {
            return;
        }

        if (!MainViewModel.MakeOrderApiKeyIsSet)
        {
            // TODO:  show msg;
            MakeOrderRresultText = "(MakeOrderApiKeyIsSet == false)";
            Debug.WriteLine("■■■■■ CancelOrders: (MakeOrderApiKeyIsSet == false)");
            return;
        }

        if (selectedOrders is null)
        {
            return;
        }

        if (selectedOrders.Count == 0)
        {
            return;
        }

        var orderIDs = new List<ulong>();

        foreach (var o in selectedOrders) 
        {
            if (o is Order ord)
            {
                Debug.WriteLine("CancelOrder: "+ ord.OrderID);
                orderIDs.Add(ord.OrderID);
            }
        }

        //CancelOrders
        App.CurrentDispatcherQueue?.TryEnqueue(async () =>
        {
            var ord = await MainViewModel.PriOrdersApi.CancelOrders(MainViewModel.MakeOrderApiKey, MainViewModel.MakeOrderSecret, PairCode.ToString(), orderIDs);

            // Update indivistual order status.
            if (ord is null)
            {
                // show err or rewrite api 
                return;
            }

            if (!ord.IsSuccess)
            {
                // show err 
                return;
            }

            foreach (var order in ord.OrderList)
            {
                var pos = Orders.FirstOrDefault(x => x.OrderID == order.OrderID);
                if (pos is null)
                {
                    continue;
                }

                pos.Status = order.Status;

            }

        });
    }

    #endregion

}
