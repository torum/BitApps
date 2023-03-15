using BitWares.Core.Helpers;

namespace BitDesk;

public sealed partial class MainWindow : WindowEx
{
    public MainWindow()
    {
        InitializeComponent();

        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/App.ico"));
        Content = null;
        //Title = "AppDisplayName".GetLocalized();
    }
}
