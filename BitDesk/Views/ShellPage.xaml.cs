using BitDesk.Contracts.Services;
using BitApps.Core.Helpers;
using BitDesk.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;
using Windows.System;
using BitApps.Core.Models;
using Microsoft.UI.Xaml.Media.Animation;
using System.Diagnostics;
using System.Xml.Linq;
using System.Xml;

namespace BitDesk.Views;

public sealed partial class ShellPage : Page
{
    public MainViewModel MainViewModel
    {
        get; private set;
    }

    private readonly List<(string Tag, Type Page)> _pages = new()
    {
        (PairCodes.btc_jpy.ToString(), typeof(BtcJpyPage)),
        (PairCodes.xrp_jpy.ToString(), typeof(XrpJpyPage)),
        (PairCodes.eth_jpy.ToString(), typeof(EthJpyPage)),
        (PairCodes.ltc_jpy.ToString(), typeof(LtcJpyPage)),
        (PairCodes.mona_jpy.ToString(), typeof(MonaJpyPage)),
        (PairCodes.bcc_jpy.ToString(), typeof(BccJpyPage)),
        (PairCodes.xlm_jpy.ToString(), typeof(XlmJpyPage)),
        (PairCodes.qtum_jpy.ToString(), typeof(QtumJpyPage)),
        (PairCodes.bat_jpy.ToString(), typeof(BatJpyPage)),
        (PairCodes.omg_jpy.ToString(), typeof(OmgJpyPage)),
        (PairCodes.xym_jpy.ToString(), typeof(XymJpyPage)),
        (PairCodes.link_jpy.ToString(), typeof(LinkJpyPage)),
        (PairCodes.mkr_jpy.ToString(), typeof(MkrJpyPage)),
        (PairCodes.boba_jpy.ToString(), typeof(BobaJpyPage)),
        (PairCodes.enj_jpy.ToString(), typeof(EnjJpyPage)),
        (PairCodes.matic_jpy.ToString(), typeof(MaticJpyPage)),
        (PairCodes.dot_jpy.ToString(), typeof(DotJpyPage)),
        (PairCodes.doge_jpy.ToString(), typeof(DogeJpyPage)),
        (PairCodes.astr_jpy.ToString(), typeof(AstrJpyPage)),
        (PairCodes.ada_jpy.ToString(), typeof(AdaJpyPage)),
        (PairCodes.avax_jpy.ToString(), typeof(AvaxJpyPage)),
        (PairCodes.axs_jpy.ToString(), typeof(AxsJpyPage)),
        (PairCodes.flr_jpy.ToString(), typeof(FlrJpyPage)),
        (PairCodes.sand_jpy.ToString(), typeof(SandJpyPage)),
        ("settings", typeof(SettingsPage)),
    };

    public ShellPage(MainViewModel viewModel)
    {
        MainViewModel = viewModel;

        InitializeComponent();

        App.MainWindow.ExtendsContentIntoTitleBar = true;
        App.MainWindow.SetTitleBar(AppTitleBar);
        App.MainWindow.Activated += MainWindow_Activated;
        App.MainWindow.Closed += MainWindow_Closed;

        #region == Load settings ==

        double height = 640; 
        double width = 480;
        var navigationViewControl_IsPaneOpen = false;

        if (System.IO.File.Exists(App.AppConfigFilePath))
        {
            var xdoc = XDocument.Load(App.AppConfigFilePath);
            //Debug.WriteLine(xdoc.ToString());

            // Main window
            if (App.MainWindow != null && xdoc.Root != null)
            {
                // Main Window element
                var mainWindow = xdoc.Root.Element("MainWindow");
                if (mainWindow != null)
                {
                    /*
                    var hoge = mainWindow.Attribute("top");
                    if (hoge != null)
                    {
                        (sender as Window).Top = double.Parse(hoge.Value);
                    }
                    */
                    /*
                    hoge = mainWindow.Attribute("left");
                    if (hoge != null)
                    {
                        (sender as Window).Left = double.Parse(hoge.Value);
                    }
                    */
                    var hoge = mainWindow.Attribute("height");
                    if (hoge != null)
                    {
                        height = double.Parse(hoge.Value);
                    }

                    hoge = mainWindow.Attribute("width");
                    if (hoge != null)
                    {
                        width = double.Parse(hoge.Value);
                    }
                    /*
                    hoge = mainWindow.Attribute("state");
                    if (hoge != null)
                    {
                        if (hoge.Value == "Maximized")
                        {
                            (sender as Window).WindowState = WindowState.Maximized;
                        }
                        else if (hoge.Value == "Normal")
                        {
                            (sender as Window).WindowState = WindowState.Normal;
                        }
                        else if (hoge.Value == "Minimized")
                        {
                            (sender as Window).WindowState = WindowState.Normal;
                        }
                    }
                    */

                    var xNavView = mainWindow.Element("NavigationViewControl");
                    if (xNavView != null)
                    {
                        if (xNavView.Attribute("IsPaneOpen") != null)
                        {
                            var xvalue = xNavView.Attribute("IsPaneOpen")?.Value;
                            if (!string.IsNullOrEmpty(xvalue))
                            {
                                navigationViewControl_IsPaneOpen = xvalue == "True";
                            }
                        }
                    }
                }

                // Options
                var opts = xdoc.Root.Element("Opts");
                if (opts != null)
                {
                    var xvalue = opts.Attribute("IsChartTooltipVisible");
                    if (xvalue != null)
                    {
                        if (!string.IsNullOrEmpty(xvalue.Value))
                        {
                            MainViewModel.IsChartTooltipVisible = xvalue.Value == "True";
                        }
                    }

                    xvalue = opts.Attribute("IsDebugSaveLog");
                    if (xvalue != null)
                    {
                        if (!string.IsNullOrEmpty(xvalue.Value))
                        {
                            MainViewModel.IsDebugSaveLog = xvalue.Value == "True";
                        }
                    }
                }

                // Pairs
                var xPairs = xdoc.Root.Element("Pairs");
                if (xPairs != null)
                {
                    var pairList = xPairs.Elements("Pair");

                    foreach (var hoge in pairList)
                    {
                        var xvalue = hoge.Attribute("PairCode");
                        if (xvalue != null)
                        {
                            if (!string.IsNullOrEmpty(xvalue.Value))
                            {
                                try
                                {
                                    var pc = MainViewModel.GetPairs[xvalue.Value];

                                    var pair = MainViewModel.Pairs.FirstOrDefault(x => x.PairCode == pc);
                                    if (pair is not null)
                                    {
                                        // TODO:
                                        pair.IsChartTooltipVisible = MainViewModel.IsChartTooltipVisible;

                                        var attrv = hoge.Attribute("IsEnabled");
                                        if (attrv != null)
                                        {
                                            if (!string.IsNullOrEmpty(attrv.Value))
                                            {
                                                pair.IsEnabled = attrv.Value == "True";
                                            }
                                        }

                                        attrv = hoge.Attribute("IsPaneVisible");
                                        if (attrv != null)
                                        {
                                            if (!string.IsNullOrEmpty(attrv.Value))
                                            {
                                                pair.IsPaneVisible = attrv.Value == "True";
                                            }
                                        }

                                        attrv = hoge.Attribute("CandleType");
                                        if (attrv != null)
                                        {
                                            if (!string.IsNullOrEmpty(attrv.Value))
                                            {
                                                var candleTypeString = attrv.Value;

                                                if (candleTypeString == "OneMin")
                                                {
                                                    pair.SelectedCandleType = CandleTypes.OneMin;
                                                }
                                                else if (candleTypeString == "FiveMin")
                                                {
                                                    pair.SelectedCandleType = CandleTypes.FiveMin;
                                                }
                                                else if (candleTypeString == "FifteenMin")
                                                {
                                                    pair.SelectedCandleType = CandleTypes.FifteenMin;
                                                }
                                                else if (candleTypeString == "ThirtyMin")
                                                {
                                                    pair.SelectedCandleType = CandleTypes.ThirtyMin;
                                                }
                                                else if (candleTypeString == "OneHour")
                                                {
                                                    pair.SelectedCandleType = CandleTypes.OneHour;
                                                }
                                                else if (candleTypeString == "FourHour")
                                                {
                                                    pair.SelectedCandleType = CandleTypes.FourHour;
                                                }
                                                else if (candleTypeString == "EightHour")
                                                {
                                                    pair.SelectedCandleType = CandleTypes.EightHour;
                                                }
                                                else if (candleTypeString == "TwelveHour")
                                                {
                                                    pair.SelectedCandleType = CandleTypes.TwelveHour;
                                                }
                                                else if (candleTypeString == "OneDay")
                                                {
                                                    pair.SelectedCandleType = CandleTypes.OneDay;
                                                }
                                                else if (candleTypeString == "OneWeek")
                                                {
                                                    pair.SelectedCandleType = CandleTypes.OneWeek;
                                                }
                                                else if (candleTypeString == "OneMonth")
                                                {
                                                    pair.SelectedCandleType = CandleTypes.OneMonth;
                                                }
                                            }
                                        }

                                        // TODO: 個別設定が必要
                                        if (pair.PairCode == PairCodes.btc_jpy)
                                        {
                                            MainViewModel.IsOnBtcJpy = pair.IsEnabled;
                                        }
                                        else if (pair.PairCode == PairCodes.xrp_jpy)
                                        {
                                            MainViewModel.IsOnXrpJpy = pair.IsEnabled;
                                        }
                                        else if (pair.PairCode == PairCodes.eth_jpy)
                                        {
                                            MainViewModel.IsOnEthJpy = pair.IsEnabled;
                                        }
                                        else if (pair.PairCode == PairCodes.ltc_jpy)
                                        {
                                            MainViewModel.IsOnLtcJpy = pair.IsEnabled;
                                        }
                                        else if (pair.PairCode == PairCodes.bcc_jpy)
                                        {
                                            MainViewModel.IsOnBccJpy = pair.IsEnabled;
                                        }
                                        else if (pair.PairCode == PairCodes.mona_jpy)
                                        {
                                            MainViewModel.IsOnMonaJpy = pair.IsEnabled;
                                        }
                                        else if (pair.PairCode == PairCodes.xlm_jpy)
                                        {
                                            MainViewModel.IsOnXlmJpy = pair.IsEnabled;
                                        }
                                        else if (pair.PairCode == PairCodes.qtum_jpy)
                                        {
                                            MainViewModel.IsOnQtumJpy = pair.IsEnabled;
                                        }
                                        else if (pair.PairCode == PairCodes.bat_jpy)
                                        {
                                            MainViewModel.IsOnBatJpy = pair.IsEnabled;
                                        }
                                        else if (pair.PairCode == PairCodes.omg_jpy)
                                        {
                                            MainViewModel.IsOnOmgJpy = pair.IsEnabled;
                                        }
                                        else if (pair.PairCode == PairCodes.xym_jpy)
                                        {
                                            MainViewModel.IsOnXymJpy = pair.IsEnabled;
                                        }
                                        else if (pair.PairCode == PairCodes.link_jpy)
                                        {
                                            MainViewModel.IsOnLinkJpy = pair.IsEnabled;
                                        }
                                        else if (pair.PairCode == PairCodes.mkr_jpy)
                                        {
                                            MainViewModel.IsOnMkrJpy = pair.IsEnabled;
                                        }
                                        else if (pair.PairCode == PairCodes.boba_jpy)
                                        {
                                            MainViewModel.IsOnBobaJpy = pair.IsEnabled;
                                        }
                                        else if (pair.PairCode == PairCodes.enj_jpy)
                                        {
                                            MainViewModel.IsOnEnjJpy = pair.IsEnabled;
                                        }
                                        else if (pair.PairCode == PairCodes.matic_jpy)
                                        {
                                            MainViewModel.IsOnMaticJpy = pair.IsEnabled;
                                        }
                                        else if (pair.PairCode == PairCodes.dot_jpy)
                                        {
                                            MainViewModel.IsOnDotJpy = pair.IsEnabled;
                                        }
                                        else if (pair.PairCode == PairCodes.doge_jpy)
                                        {
                                            MainViewModel.IsOnDogeJpy = pair.IsEnabled;
                                        }
                                        else if (pair.PairCode == PairCodes.astr_jpy)
                                        {
                                            MainViewModel.IsOnAstrJpy = pair.IsEnabled;
                                        }
                                        else if (pair.PairCode == PairCodes.ada_jpy)
                                        {
                                            MainViewModel.IsOnAdaJpy = pair.IsEnabled;
                                        }
                                        else if (pair.PairCode == PairCodes.avax_jpy)
                                        {
                                            MainViewModel.IsOnAvaxJpy = pair.IsEnabled;
                                        }
                                        else if (pair.PairCode == PairCodes.axs_jpy)
                                        {
                                            MainViewModel.IsOnAxsJpy = pair.IsEnabled;
                                        }
                                        else if (pair.PairCode == PairCodes.flr_jpy)
                                        {
                                            MainViewModel.IsOnFlrJpy = pair.IsEnabled;
                                        }
                                        else if (pair.PairCode == PairCodes.sand_jpy)
                                        {
                                            MainViewModel.IsOnSandJpy = pair.IsEnabled;
                                        }
                                    }

                                }
                                catch { }
                            }
                        }
                    }
                }
            }
        }
        else
        {

        }

        var app = App.Current as App;
        if (app is not null)
        {
            app.IsSaveErrorLog = MainViewModel.IsDebugSaveLog;
        }

        #endregion
    }

    private void OnLoaded(object sender, Microsoft.UI.Xaml.RoutedEventArgs e)
    {
        KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.Left, VirtualKeyModifiers.Menu));
        KeyboardAccelerators.Add(BuildKeyboardAccelerator(VirtualKey.GoBack));
    }

    private void MainWindow_Activated(object sender, WindowActivatedEventArgs args)
    {
        var resource = args.WindowActivationState == WindowActivationState.Deactivated ? "WindowCaptionForegroundDisabled" : "WindowCaptionForeground";

        AppTitleBarText.Foreground = (SolidColorBrush)App.Current.Resources[resource];
    }

    private void MainWindow_Closed(object sender, WindowEventArgs args)
    {
        #region == Save setting ==

        XmlDocument doc = new();
        var xmlDeclaration = doc.CreateXmlDeclaration("1.0", "UTF-8", null);
        doc.InsertBefore(xmlDeclaration, doc.DocumentElement);

        // Root Document Element
        var root = doc.CreateElement(string.Empty, "App", string.Empty);
        doc.AppendChild(root);

        //XmlAttribute attrs = doc.CreateAttribute("Version");
        //attrs.Value = _appVer;
        //root.SetAttributeNode(attrs);
        XmlAttribute attrs;

        // Main window
        if (App.MainWindow != null)
        {
            // Main Window element
            var mainWindow = doc.CreateElement(string.Empty, "MainWindow", string.Empty);

            // Main Window attributes
            attrs = doc.CreateAttribute("width");
            /*
            if ((sender as Window).WindowState == WindowState.Maximized)
            {
                attrs.Value = (sender as Window).RestoreBounds.Width.ToString();
            }
            else
            {
                attrs.Value = (sender as Window).Width.ToString();
            }
            */
            attrs.Value = App.MainWindow.AppWindow.Size.Width.ToString();//App.MainWindow.GetAppWindow().Size.Width.ToString();
            mainWindow.SetAttributeNode(attrs);

            attrs = doc.CreateAttribute("height");
            /*
            if ((sender as Window).WindowState == WindowState.Maximized)
            {
                attrs.Value = (sender as Window).RestoreBounds.Height.ToString();
            }
            else
            {
                attrs.Value = (sender as Window).Height.ToString();
            }
            */
            attrs.Value = App.MainWindow.AppWindow.Size.Height.ToString();//App.MainWindow.GetAppWindow().Size.Height.ToString();
            mainWindow.SetAttributeNode(attrs);

            /*
            attrs = doc.CreateAttribute("top");
            if ((sender as Window).WindowState == WindowState.Maximized)
            {
                attrs.Value = (sender as Window).RestoreBounds.Top.ToString();
            }
            else
            {
                attrs.Value = (sender as Window).Top.ToString();
            }
            mainWindow.SetAttributeNode(attrs);
            */
            /*
            attrs = doc.CreateAttribute("left");
            if ((sender as Window).WindowState == WindowState.Maximized)
            {
                attrs.Value = (sender as Window).RestoreBounds.Left.ToString();
            }
            else
            {
                attrs.Value = (sender as Window).Left.ToString();
            }
            mainWindow.SetAttributeNode(attrs);
            */
            /*
            attrs = doc.CreateAttribute("state");
            if ((sender as Window).WindowState == WindowState.Maximized)
            {
                attrs.Value = "Maximized";
            }
            else if ((sender as Window).WindowState == WindowState.Normal)
            {
                attrs.Value = "Normal";

            }
            else if ((sender as Window).WindowState == WindowState.Minimized)
            {
                attrs.Value = "Minimized";
            }
            mainWindow.SetAttributeNode(attrs);
            */


            var xNavigationViewControl = doc.CreateElement(string.Empty, "NavigationViewControl", string.Empty);

            var xAttrs = doc.CreateAttribute("IsPaneOpen");
            xAttrs.Value = MainViewModel.NavigationViewControl_IsPaneOpen.ToString();
            xNavigationViewControl.SetAttributeNode(xAttrs);

            mainWindow.AppendChild(xNavigationViewControl);

            // set Main Window element to root.
            root.AppendChild(mainWindow);

        }

        // Options
        var xOpts = doc.CreateElement(string.Empty, "Opts", string.Empty);

        attrs = doc.CreateAttribute("IsChartTooltipVisible");
        attrs.Value = MainViewModel.IsChartTooltipVisible.ToString();
        xOpts.SetAttributeNode(attrs);

        attrs = doc.CreateAttribute("IsDebugSaveLog");
        attrs.Value = MainViewModel.IsDebugSaveLog.ToString();
        xOpts.SetAttributeNode(attrs);

        root.AppendChild(xOpts);

        // Each pairs
        var xPairs = doc.CreateElement(string.Empty, "Pairs", string.Empty);
        foreach (var hoge in MainViewModel.Pairs)
        {
            var xPair = doc.CreateElement(string.Empty, "Pair", string.Empty);

            var xAttrs = doc.CreateAttribute("PairCode");
            xAttrs.Value = hoge.PairCode.ToString();
            xPair.SetAttributeNode(xAttrs);

            xAttrs = doc.CreateAttribute("IsEnabled");
            xAttrs.Value = hoge.IsEnabled.ToString();
            xPair.SetAttributeNode(xAttrs);

            xAttrs = doc.CreateAttribute("IsPaneVisible");
            xAttrs.Value = hoge.IsPaneVisible.ToString();
            xPair.SetAttributeNode(xAttrs);

            xAttrs = doc.CreateAttribute("CandleType");
            xAttrs.Value = hoge.SelectedCandleType.ToString();
            xPair.SetAttributeNode(xAttrs);


            xAttrs = doc.CreateAttribute("IsChartTooltipVisible");
            // TODO:
            //xAttrs.Value = hoge.IsChartTooltipVisible.ToString();
            //xAttrs.Value = MainVM.IsChartTooltipVisible.ToString();
            xPair.SetAttributeNode(xAttrs);


            xPairs.AppendChild(xPair);

            // Since we are here, we might as well clean up.
            hoge.IsEnabled = false;
            hoge.CleanUp();
        }
        root.AppendChild(xPairs);

        try
        {
            doc.Save(App.AppConfigFilePath);
        }
        //catch (System.IO.FileNotFoundException) { }
        catch (Exception ex)
        {
            Debug.WriteLine("MainWindow_Closed: " + ex + " while saving : " + App.AppConfigFilePath);
        }

        #endregion

        MainViewModel.CleanUp();

        // error logs.
        var app = App.Current as App;
        app?.SaveErrorLog();
    }

    private void NavigationViewControl_DisplayModeChanged(NavigationView sender, NavigationViewDisplayModeChangedEventArgs args)
    {
        /*
        AppTitleBar.Margin = new Thickness()
        {
            Left = sender.CompactPaneLength * (sender.DisplayMode == NavigationViewDisplayMode.Minimal ? 2 : 1),
            Top = AppTitleBar.Margin.Top,
            Right = AppTitleBar.Margin.Right,
            Bottom = AppTitleBar.Margin.Bottom
        };
        */
    }

    private static KeyboardAccelerator BuildKeyboardAccelerator(VirtualKey key, VirtualKeyModifiers? modifiers = null)
    {
        var keyboardAccelerator = new KeyboardAccelerator() { Key = key };

        if (modifiers.HasValue)
        {
            keyboardAccelerator.Modifiers = modifiers.Value;
        }

        keyboardAccelerator.Invoked += OnKeyboardAcceleratorInvoked;

        return keyboardAccelerator;
    }

    private static void OnKeyboardAcceleratorInvoked(KeyboardAccelerator sender, KeyboardAcceleratorInvokedEventArgs args)
    {
        /*
        var navigationService = App.GetService<INavigationService>();

        var result = navigationService.GoBack();

        args.Handled = result;
        */
    }

    private void NavigationViewControl_Loaded(object sender, RoutedEventArgs e)
    {
        //await Task.Delay(100);
        if (NavigationViewControl.MenuItems.Count > 0)
        {
            // Since we use ItemInvoked, we set selecteditem manually
            NavigationViewControl.SelectedItem = NavigationViewControl.MenuItems.OfType<NavigationViewItem>().Where(x => x.Visibility == Visibility.Visible).FirstOrDefault();//.First();

            if (NavigationViewControl.SelectedItem is null)
            {
                // pass mainviewmodel for setting page.
                //NavigationFrame.Navigate(typeof(SettingsPage), MainViewModel, new Microsoft.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo());

                return;
            }

            // Pass vm to destination Frame when navigate.
            //var pairViewModel = MainVM.Pairs.FirstOrDefault(x => x.PairCode == PairCodes.btc_jpy);
            //NavigationFrame.Navigate(typeof(BtcJpyPage), pairViewModel, new Microsoft.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo());
            var pairViewModel = MainViewModel.Pairs.FirstOrDefault(x => x.IsEnabled == true);


            if (pairViewModel != null)
            {
                var item = _pages.FirstOrDefault(p => p.Tag.Equals(pairViewModel.PairCode.ToString()));
                var _page = item.Page;

                if (((NavigationViewItem)NavigationViewControl.SelectedItem).Tag.ToString() == pairViewModel.PairCode.ToString())
                {
                    NavigationFrame.Navigate(_page, pairViewModel, new Microsoft.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo());
                }
            }
        }


        // Listen to the window directly so the app responds to accelerator keys regardless of which element has focus.
        //Window.Current.CoreWindow.Dispatcher.AcceleratorKeyActivated +=  CoreDispatcher_AcceleratorKeyActivated;

        //Window.Current.CoreWindow.PointerPressed += CoreWindow_PointerPressed;

        //SystemNavigationManager.GetForCurrentView().BackRequested += System_BackRequested;


        // Any other way?
        //var settings = (Microsoft.UI.Xaml.Controls.NavigationViewItem)NavigationViewControl.SettingsItem;
        //settings.Content = "";//"Setting".GetLocalized();
    }

    private void NavigationViewControl_ItemInvoked(NavigationView sender, NavigationViewItemInvokedEventArgs args)
    {
        if (args.IsSettingsInvoked == true)
        {
            // pass mainviewmodel for setting page.
            NavigationFrame.Navigate(typeof(SettingsPage), MainViewModel, new Microsoft.UI.Xaml.Media.Animation.EntranceNavigationTransitionInfo());
            //NavigationFrame.Navigate(typeof(SettingsPage), ViewModel, args.RecommendedNavigationTransitionInfo);
        }
        else if (args.InvokedItemContainer != null && (args.InvokedItemContainer?.Tag != null))
        {
            if (_pages is null)
            {
                return;
            }

            var item = _pages.FirstOrDefault(p => p.Tag.Equals(args.InvokedItemContainer.Tag.ToString()));

            var _page = item.Page;

            if (_page is null)
            {
                return;
            }

            // Pass pairviewmodel when navigate to each pair page.
            PairViewModel? vm;
            if (_page == typeof(BtcJpyPage))
            {
                vm = MainViewModel.Pairs.FirstOrDefault(x => x.PairCode == PairCodes.btc_jpy);
            }
            else if (_page == typeof(XrpJpyPage))
            {
                vm = MainViewModel.Pairs.FirstOrDefault(x => x.PairCode == PairCodes.xrp_jpy);
            }
            else if (_page == typeof(EthJpyPage))
            {
                vm = MainViewModel.Pairs.FirstOrDefault(x => x.PairCode == PairCodes.eth_jpy);
            }
            else if (_page == typeof(LtcJpyPage))
            {
                vm = MainViewModel.Pairs.FirstOrDefault(x => x.PairCode == PairCodes.ltc_jpy);
            }
            else if (_page == typeof(MonaJpyPage))
            {
                vm = MainViewModel.Pairs.FirstOrDefault(x => x.PairCode == PairCodes.mona_jpy);
            }
            else if (_page == typeof(BccJpyPage))
            {
                vm = MainViewModel.Pairs.FirstOrDefault(x => x.PairCode == PairCodes.bcc_jpy);
            }
            else if (_page == typeof(XlmJpyPage))
            {
                vm = MainViewModel.Pairs.FirstOrDefault(x => x.PairCode == PairCodes.xlm_jpy);
            }
            else if (_page == typeof(QtumJpyPage))
            {
                vm = MainViewModel.Pairs.FirstOrDefault(x => x.PairCode == PairCodes.qtum_jpy);
            }
            else if (_page == typeof(BatJpyPage))
            {
                vm = MainViewModel.Pairs.FirstOrDefault(x => x.PairCode == PairCodes.bat_jpy);
            }
            else if (_page == typeof(OmgJpyPage))
            {
                vm = MainViewModel.Pairs.FirstOrDefault(x => x.PairCode == PairCodes.omg_jpy);
            }
            else if (_page == typeof(XymJpyPage))
            {
                vm = MainViewModel.Pairs.FirstOrDefault(x => x.PairCode == PairCodes.xym_jpy);
            }
            else if (_page == typeof(LinkJpyPage))
            {
                vm = MainViewModel.Pairs.FirstOrDefault(x => x.PairCode == PairCodes.link_jpy);
            }
            else if (_page == typeof(MkrJpyPage))
            {
                vm = MainViewModel.Pairs.FirstOrDefault(x => x.PairCode == PairCodes.mkr_jpy);
            }
            else if (_page == typeof(BobaJpyPage))
            {
                vm = MainViewModel.Pairs.FirstOrDefault(x => x.PairCode == PairCodes.boba_jpy);
            }
            else if (_page == typeof(EnjJpyPage))
            {
                vm = MainViewModel.Pairs.FirstOrDefault(x => x.PairCode == PairCodes.enj_jpy);
            }
            else if (_page == typeof(MaticJpyPage))
            {
                vm = MainViewModel.Pairs.FirstOrDefault(x => x.PairCode == PairCodes.matic_jpy);
            }
            else if (_page == typeof(DotJpyPage))
            {
                vm = MainViewModel.Pairs.FirstOrDefault(x => x.PairCode == PairCodes.dot_jpy);
            }
            else if (_page == typeof(DogeJpyPage))
            {
                vm = MainViewModel.Pairs.FirstOrDefault(x => x.PairCode == PairCodes.doge_jpy);
            }
            else if (_page == typeof(AstrJpyPage))
            {
                vm = MainViewModel.Pairs.FirstOrDefault(x => x.PairCode == PairCodes.astr_jpy);
            }
            else if (_page == typeof(AdaJpyPage))
            {
                vm = MainViewModel.Pairs.FirstOrDefault(x => x.PairCode == PairCodes.ada_jpy);
            }
            else if (_page == typeof(AvaxJpyPage))
            {
                vm = MainViewModel.Pairs.FirstOrDefault(x => x.PairCode == PairCodes.avax_jpy);
            }
            else if (_page == typeof(AxsJpyPage))
            {
                vm = MainViewModel.Pairs.FirstOrDefault(x => x.PairCode == PairCodes.axs_jpy);
            }
            else if (_page == typeof(FlrJpyPage))
            {
                vm = MainViewModel.Pairs.FirstOrDefault(x => x.PairCode == PairCodes.flr_jpy);
            }
            else if (_page == typeof(SandJpyPage))
            {
                vm = MainViewModel.Pairs.FirstOrDefault(x => x.PairCode == PairCodes.sand_jpy);
            }
            else
            {
                Debug.WriteLine($"NavigationViewControl_ItemInvoked: SourcePageType {_page.Name} is not recognized.");
                throw new NotImplementedException();
            }

            NavigationFrame.Navigate(_page, vm, new SlideNavigationTransitionInfo() { Effect = SlideNavigationTransitionEffect.FromLeft });
            //NavigationFrame.Navigate(_page, vm, args.RecommendedNavigationTransitionInfo);
            //NavigationFrame.Navigate(_page, vm, new SuppressNavigationTransitionInfo());
        }

    }

    private void NavigationFrame_Navigated(object sender, Microsoft.UI.Xaml.Navigation.NavigationEventArgs e)
    {

        if (NavigationFrame.SourcePageType == typeof(SettingsPage))
        {
            // SettingsItem is not part of NavView.MenuItems, and doesn't have a Tag.
            //NavigationViewControl.SelectedItem = (NavigationViewItem)NavigationViewControl.SettingsItem;
            //NavigationViewControl.Header = "設定";
            return;
        }
        else if (NavigationFrame.SourcePageType != null)
        {
            //NavigationViewControl.Header = null;

            //var item = _pages.FirstOrDefault(p => p.Page == e.SourcePageType);

            //NavigationViewControl.SelectedItem = NavigationViewControl.MenuItems.OfType<NavigationViewItem>().First(n => n.Tag.Equals(item.Tag));

            //NavigationViewControl.Header = ((NavigationViewItem)NavigationViewControl.SelectedItem)?.Content?.ToString();

            // Do nothing.
        }

        if (e.SourcePageType == typeof(SettingsPage))
        {
            //Selected = NavigationViewService.SettingsItem;
            //_activePair = null;
            return;
        }

        PairCodes _activePair;

        if (e.SourcePageType == typeof(BtcJpyPage))
        {
            _activePair = PairCodes.btc_jpy;
        }
        else if (e.SourcePageType == typeof(XrpJpyPage))
        {
            _activePair = PairCodes.xrp_jpy;
        }
        else if (e.SourcePageType == typeof(EthJpyPage))
        {
            _activePair = PairCodes.eth_jpy;
        }
        else if (e.SourcePageType == typeof(LtcJpyPage))
        {
            _activePair = PairCodes.ltc_jpy;
        }
        else if (e.SourcePageType == typeof(MonaJpyPage))
        {
            _activePair = PairCodes.mona_jpy;
        }
        else if (e.SourcePageType == typeof(BccJpyPage))
        {
            _activePair = PairCodes.bcc_jpy;
        }
        else if (e.SourcePageType == typeof(XlmJpyPage))
        {
            _activePair = PairCodes.xlm_jpy;
        }
        else if (e.SourcePageType == typeof(QtumJpyPage))
        {
            _activePair = PairCodes.qtum_jpy;
        }
        else if (e.SourcePageType == typeof(BatJpyPage))
        {
            _activePair = PairCodes.bat_jpy;
        }
        else if (e.SourcePageType == typeof(OmgJpyPage))
        {
            _activePair = PairCodes.omg_jpy;
        }
        else if (e.SourcePageType == typeof(XymJpyPage))
        {
            _activePair = PairCodes.xym_jpy;
        }
        else if (e.SourcePageType == typeof(LinkJpyPage))
        {
            _activePair = PairCodes.link_jpy;
        }
        else if (e.SourcePageType == typeof(MkrJpyPage))
        {
            _activePair = PairCodes.mkr_jpy;
        }
        else if (e.SourcePageType == typeof(BobaJpyPage))
        {
            _activePair = PairCodes.boba_jpy;
        }
        else if (e.SourcePageType == typeof(EnjJpyPage))
        {
            _activePair = PairCodes.enj_jpy;
        }
        else if (e.SourcePageType == typeof(MaticJpyPage))
        {
            _activePair = PairCodes.matic_jpy;
        }
        else if (e.SourcePageType == typeof(DotJpyPage))
        {
            _activePair = PairCodes.dot_jpy;
        }
        else if (e.SourcePageType == typeof(DogeJpyPage))
        {
            _activePair = PairCodes.doge_jpy;
        }
        else if (e.SourcePageType == typeof(AstrJpyPage))
        {
            _activePair = PairCodes.astr_jpy;
        }
        else if (e.SourcePageType == typeof(AdaJpyPage))
        {
            _activePair = PairCodes.ada_jpy;
        }
        else if (e.SourcePageType == typeof(AvaxJpyPage))
        {
            _activePair = PairCodes.avax_jpy;
        }
        else if (e.SourcePageType == typeof(AxsJpyPage))
        {
            _activePair = PairCodes.axs_jpy;
        }
        else if (e.SourcePageType == typeof(FlrJpyPage))
        {
            _activePair = PairCodes.flr_jpy;
        }
        else if (e.SourcePageType == typeof(SandJpyPage))
        {
            _activePair = PairCodes.sand_jpy;
        }
        else
        {
            Debug.WriteLine($"NavigationFrame_Navigated: SourcePageType {e.SourcePageType} is not recognized.");
            throw new NotImplementedException();
        }

        // Set IsActive and Init & Start.
        MainViewModel.SetSelectedPairFromCode(_activePair);
    }

    private void NavigationFrame_NavigationFailed(object sender, Microsoft.UI.Xaml.Navigation.NavigationFailedEventArgs e)
    {
        Debug.WriteLine("NavigationFrame_NavigationFailed: " + e.Exception.Message + " - " + e.Exception.StackTrace);

        (App.Current as App)?.AppendErrorLog("NavigationFrame_NavigationFailed", e.Exception.Message + ", StackTrace: " + e.Exception.StackTrace);

        e.Handled = true;
    }
}
