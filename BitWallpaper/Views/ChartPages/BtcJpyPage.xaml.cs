using System.Diagnostics;
using BitWallpaper.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Navigation;

namespace BitWallpaper.Views;

public sealed partial class BtcJpyPage : Page
{
    public PairViewModel? ViewModel
    {
        get; private set;
    }

    public BtcJpyPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        //base.OnNavigatedTo(e);

        if (e.Parameter is PairViewModel pvm)
        {
            ViewModel = pvm;
        }

        base.OnNavigatedTo(e);
    }
}
