using BitDesk.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Navigation;
using System.Diagnostics;

namespace BitDesk.Views;

public sealed partial class XrpJpyPage : Page
{
    public PairViewModel? ViewModel { get; private set; }

    public XrpJpyPage()
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
