using BitApps.Core.Helpers;
using Microsoft.UI.Composition.SystemBackdrops;
using Microsoft.UI.Xaml.Media;
using Windows.Storage;
using Windows.UI.ViewManagement;

namespace BitDesk;

public sealed partial class MainWindow : WindowEx
{
    public MainWindow()
    {
        InitializeComponent();

        AppWindow.SetIcon(Path.Combine(AppContext.BaseDirectory, "Assets/App.ico"));
        Content = null;
        //Title = "AppDisplayName".GetLocalized();

        settings = new UISettings();
        settings.ColorValuesChanged += Settings_ColorValuesChanged;

        // Need to be here in the code bihind.
        //ExtendsContentIntoTitleBar = true;

        // SystemBackdrop
        if (Microsoft.UI.Composition.SystemBackdrops.DesktopAcrylicController.IsSupported())
        {
            //manager.Backdrop = new WinUIEx.AcrylicSystemBackdrop();
            if (RuntimeHelper.IsMSIX)
            {
                // Load preference from localsetting.
                if (ApplicationData.Current.LocalSettings.Values.TryGetValue(App.BackdropSettingsKey, out var obj))
                {
                    var s = (string)obj;
                    if (s == SystemBackdropOption.Acrylic.ToString())
                    {
                        SystemBackdrop = new DesktopAcrylicBackdrop();
                    }
                    else if (s == SystemBackdropOption.Mica.ToString())
                    {
                        SystemBackdrop = new MicaBackdrop()
                        {
                            //Kind = MicaKind.Base
                            Kind = MicaKind.BaseAlt
                        };
                    }
                    else
                    {
                        SystemBackdrop = new DesktopAcrylicBackdrop();
                    }
                }
                else
                {
                    // default acrylic.
                    SystemBackdrop = new DesktopAcrylicBackdrop();
                }
            }
            else
            {
                // just for me.
                SystemBackdrop = new DesktopAcrylicBackdrop();
            }

        }
        else if (Microsoft.UI.Composition.SystemBackdrops.MicaController.IsSupported())
        {
            SystemBackdrop = new MicaBackdrop()
            {
                //Kind = MicaKind.Base
                Kind = MicaKind.BaseAlt
            };
        }
        else
        {
            // Memo: Without Backdrop, theme setting's theme is not gonna have any effect( "system default" will be used). So the setting is disabled.
        }

    }

    private readonly UISettings settings;

    // this handles updating the caption button colors correctly when Windows system theme is changed
    // while the app is open
    private void Settings_ColorValuesChanged(UISettings sender, object args)
    {
        /*
        // This calls comes off-thread, hence we will need to dispatch it to current app's thread
        App.CurrentDispatcherQueue.TryEnqueue(() =>
        {
            TitleBarHelper.ApplySystemThemeToCaptionButtons();
        });
        */
    }
}
