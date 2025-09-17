using System.Diagnostics;
using BitWallpaper.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Navigation;

namespace BitWallpaper.Views;

public sealed partial class SettingsPage : Page
{
    public MainViewModel? ViewModel
    {
        get; private set;
    }

    public SettingsPage()
    {
        InitializeComponent();
    }

    protected override void OnNavigatedTo(NavigationEventArgs e)
    {
        //base.OnNavigatedTo(e);

        if (e.Parameter is MainViewModel mvm)
        {
            ViewModel = mvm;
        }

        base.OnNavigatedTo(e);
    }

}
