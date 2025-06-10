using System.Diagnostics;
using System.Reflection;
using BitApps.Core.Helpers;
using BitDesk.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Navigation;
using Windows.ApplicationModel;

namespace BitDesk.Views;

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
