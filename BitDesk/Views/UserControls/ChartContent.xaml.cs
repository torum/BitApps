using BitDesk.ViewModels;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Automation.Peers;
using Microsoft.UI.Xaml.Automation.Provider;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using System.Diagnostics;

namespace BitDesk.Views.UserControls;

public sealed partial class ChartContent : UserControl
{
    public ChartContent()
    {
        InitializeComponent();
    }

    public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(PairViewModel),
      //typeof(PairViewModel),
      typeof(object),
      typeof(ChartContent),
      new PropertyMetadata(null, ValueChanged));

    public PairViewModel PairViewModel
    {
        get => (PairViewModel)GetValue(ViewModelProperty);
        set => SetValue(ViewModelProperty, value);
    }

    private static void ValueChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        ((ChartContent)sender).ValueChanged();
    }

    private void ValueChanged()
    {
        if (PairViewModel == null)
        {
            return;
        }

        PairViewModel.DepthScrollCenter += () => OnDepthScrollCenter();
    }

    private void OnDepthScrollCenter()
    {
        Task.Run(() => SetDepthListboxScrollPosition());
    }

    private void ListBoxDepth_SizeChanged(object sender, SizeChangedEventArgs e)
    {
        Task.Run(() => SetDepthListboxScrollPosition());
        //SetDepthListboxScrollPosition();
        //DoScroll();
    }

    private async void SetDepthListboxScrollPosition()
    {
        await Task.Delay(10);

        var uithread = App.CurrentDispatcherQueue?.HasThreadAccess;
        if (uithread != null)
        {
            if (uithread == true)
            {
                DoScroll();
            }
            else
            {
                App.CurrentDispatcherQueue?.TryEnqueue(() =>
                {
                    DoScroll();
                });
            }
        }
    }

    private void DoScroll()
    {
        if (DepthListBox.Items.Count > 0)
        {
            try
            {
                // ListBoxからAutomationPeerを取得
                var peer = ItemsControlAutomationPeer.CreatePeerForElement(DepthListBox);
                // GetPatternでIScrollProviderを取得

                if (peer.GetPattern(Microsoft.UI.Xaml.Automation.Peers.PatternInterface.Scroll) is IScrollProvider scrollProvider)
                {
                    if (scrollProvider.VerticallyScrollable)
                    {
                        try
                        {
                            // パーセントで位置を指定してスクロール
                            scrollProvider.SetScrollPercent(
                                // 水平スクロールは今の位置
                                scrollProvider.HorizontalScrollPercent,
                                // 垂直方向50%
                                50.0);
                        }
                        catch
                        {
                            Debug.WriteLine("■■■■■ SetDepthListboxScrollPosition scrollProvider null Error");
                        }
                    }
                }
            }
            catch
            {
                Debug.WriteLine("■■■■■ SetDepthListboxScrollPosition SetScrollPercent Error");
            }
        }
    }

    private async void AppBarButton_Click(object sender, RoutedEventArgs e)
    {
        if (PairViewModel == null)
        {
            return;
        }

        ContentDialogContent dialog = new();

        if (PairViewModel.AlarmPlus > 0)
        {
            dialog.HighPrice = PairViewModel.AlarmPlus;
        }

        if (PairViewModel.AlarmMinus > 0)
        {
            dialog.LowPrice = PairViewModel.AlarmMinus;
        }

        // XamlRoot must be set in the case of a ContentDialog running in a Desktop app
        dialog.XamlRoot = XamlRoot;
        dialog.Style = Application.Current.Resources["DefaultContentDialogStyle"] as Style;
        dialog.Title = "Notification";
        dialog.PrimaryButtonText = "Set";
        dialog.CloseButtonText = "Cancel";
        dialog.DefaultButton = ContentDialogButton.Primary;
        //dialog.Content = new ContentDialogContent();

        var result = await dialog.ShowAsync();

        if (result == ContentDialogResult.Primary)
        {
            PairViewModel.AlarmPlus = dialog.HighPrice;
            PairViewModel.AlarmMinus = dialog.LowPrice;
        }
        
    }

    private void DepthListBox_DoubleTapped(object sender, Microsoft.UI.Xaml.Input.DoubleTappedRoutedEventArgs e)
    {
        SetDepthListboxScrollPosition();
    }
}
