using System.Collections;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Text;
using System.Text.Json.Nodes;
using BitDesk.Contracts.Services;
using BitDesk.Services;
using BitDesk.ViewModels;
using BitDesk.Views;
using BitApps.Core.Helpers;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.UI.Xaml;
using Windows.Storage;

namespace BitDesk;

public partial class App : Application
{
    private static readonly string _appName = "BitDesk";
    private static readonly string _appDeveloper = "torum";
    private static readonly string _envDataFolder = System.Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
    public static string AppDataFolder { get; } = _envDataFolder + System.IO.Path.DirectorySeparatorChar + _appDeveloper + System.IO.Path.DirectorySeparatorChar + _appName;
    public static string AppConfigFilePath { get; } = AppDataFolder + System.IO.Path.DirectorySeparatorChar + _appName + "2.config";

    public bool IsSaveErrorLog = true;
    public string LogFilePath = System.Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + System.IO.Path.DirectorySeparatorChar + "BitDesk_errors.txt";
    private readonly StringBuilder Errortxt = new();

    public const string BackdropSettingsKey = "AppSystemBackdropOption";
    public const string ThemeSettingsKey = "AppBackgroundRequestedTheme";

    private static readonly Microsoft.UI.Dispatching.DispatcherQueue _currentDispatcherQueue = Microsoft.UI.Dispatching.DispatcherQueue.GetForCurrentThread();
    public static Microsoft.UI.Dispatching.DispatcherQueue CurrentDispatcherQueue => _currentDispatcherQueue;

    public IHost Host
    {
        get;
    }

    public static T GetService<T>()
        where T : class
    {
        if ((App.Current as App)!.Host.Services.GetService(typeof(T)) is not T service)
        {
            throw new ArgumentException($"{typeof(T)} needs to be registered in ConfigureServices within App.xaml.cs.");
        }

        return service;
    }

    public static WindowEx MainWindow { get; } = new MainWindow();

    public App()
    {
        InitializeComponent();

        Host = Microsoft.Extensions.Hosting.Host.
        CreateDefaultBuilder().
        UseContentRoot(AppContext.BaseDirectory).
        ConfigureServices((context, services) =>
        {
            // Services
            services.AddSingleton<IThemeSelectorService, ThemeSelectorService>();

            // Views and ViewModels
            services.AddSingleton<SettingsPage>();
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<ShellPage>();
        }).
        Build();

        UnhandledException += App_UnhandledException;
        TaskScheduler.UnobservedTaskException += TaskScheduler_UnobservedTaskException;
        AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
    }

    protected async override void OnLaunched(LaunchActivatedEventArgs args)
    {
        /*
         * https://learn.microsoft.com/en-us/windows/apps/windows-app-sdk/migrate-to-windows-app-sdk/guides/applifecycle
        */
        // If this is the first instance launched, then register it as the "main" instance.
        // If this isn't the first instance launched, then "main" will already be registered,
        // so retrieve it.
        var mainInstance = Microsoft.Windows.AppLifecycle.AppInstance.FindOrRegisterForKey("BitDeskMain");

        // If the instance that's executing the OnLaunched handler right now
        // isn't the "main" instance.
        if (!mainInstance.IsCurrent)
        {
            // Redirect the activation (and args) to the "main" instance, and exit.
            var activatedEventArgs = Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().GetActivatedEventArgs();
            await mainInstance.RedirectActivationToAsync(activatedEventArgs);

            System.Diagnostics.Process.GetCurrentProcess().Kill();
            return;
        }
        else
        {
            // Otherwise, register for activation redirection
            Microsoft.Windows.AppLifecycle.AppInstance.GetCurrent().Activated += App_Activated;
        }

        // WinUIEx Storage option.
        if (!RuntimeHelper.IsMSIX)
        {
            // Create if not exists.
            if (!Directory.Exists(AppDataFolder))
            {
                Directory.CreateDirectory(AppDataFolder);
            }

            WinUIEx.WindowManager.PersistenceStorage = new FilePersistence(Path.Combine(AppDataFolder, "WinUIExPersistence.json"));
        }

        base.OnLaunched(args);

        //var _viewModel = App.GetService<MainViewModel>();
        //var _shell = new ShellPage(_viewModel);
        var _shell = App.GetService<ShellPage>();//new ShellPage();
        MainWindow.Content = _shell;

        MainWindow?.Activate();
    }

    // Activated from other instance.
    private void App_Activated(object? sender, Microsoft.Windows.AppLifecycle.AppActivationArguments e)
    {
        CurrentDispatcherQueue?.TryEnqueue(() =>
        {
            MainWindow?.Activate();
            //MainWindow.BringToFront();
        });
    }


    #region == UnhandledException ==

    private void App_UnhandledException(object sender, Microsoft.UI.Xaml.UnhandledExceptionEventArgs e)
    {
        Debug.WriteLine("App_UnhandledException", e.Message);
        Debug.WriteLine($"StackTrace: {e.Exception.StackTrace}, Source: {e.Exception.Source}");
        AppendErrorLog("App_UnhandledException", e.Message + System.Environment.NewLine + $"StackTrace: {e.Exception.StackTrace}, Source: {e.Exception.Source}");

        SaveErrorLog();
    }

    private void TaskScheduler_UnobservedTaskException(object? sender, UnobservedTaskExceptionEventArgs e)
    {
        if (e.Exception.InnerException is not Exception exception)
        {
            return;
        }

        Debug.WriteLine("TaskScheduler_UnobservedTaskException: " + exception.Message);
        AppendErrorLog("TaskScheduler_UnobservedTaskException", exception.Message);
        SaveErrorLog();

        e.SetObserved();
    }

    private void CurrentDomain_UnhandledException(object sender, System.UnhandledExceptionEventArgs e)
    {
        if (e.ExceptionObject is not Exception exception)
        {
            return;
        }

        if (exception is TaskCanceledException)
        {
            // can ignore.
            Debug.WriteLine("CurrentDomain_UnhandledException (TaskCanceledException): " + exception.Message);
            AppendErrorLog("CurrentDomain_UnhandledException (TaskCanceledException)", exception.Message);
        }
        else
        {
            Debug.WriteLine("CurrentDomain_UnhandledException: " + exception.Message);
            AppendErrorLog("CurrentDomain_UnhandledException", exception.Message);
            SaveErrorLog();
        }
    }

    public void AppendErrorLog(string kindTxt, string errorTxt)
    {
        Errortxt.AppendLine(kindTxt + ": " + errorTxt);
        var dt = DateTime.Now;
        Errortxt.AppendLine($"Occured at {dt.ToString("yyyy/MM/dd HH:mm:ss")}");
        Errortxt.AppendLine("");
    }

    public void SaveErrorLog()
    {
        if (!IsSaveErrorLog)
        {
            return;
        }

        if (string.IsNullOrEmpty(LogFilePath))
        {
            return;
        }

        if (Errortxt.Length > 0)
        {
            Errortxt.AppendLine("");
            var dt = DateTime.Now;
            Errortxt.AppendLine($"Saved at {dt.ToString("yyyy/MM/dd HH:mm:ss")}");

            var s = Errortxt.ToString();
            if (!string.IsNullOrEmpty(s))
            {
                File.WriteAllText(LogFilePath, s);
            }
        }
    }

    #endregion

    #region == FilePersistence for WinUIEx ==

    private partial class FilePersistence : IDictionary<string, object>
    {
        private readonly Dictionary<string, object> _data = [];
        private readonly string _file;

        public FilePersistence(string filename)
        {
            _file = filename;
            try
            {
                if (File.Exists(filename))
                {
                    if (JsonNode.Parse(File.ReadAllText(filename)) is JsonObject jo)
                    {
                        foreach (var node in jo)
                        {
                            if (node.Value is JsonValue jvalue && jvalue.TryGetValue<string>(out var value))
                            {
                                _data[node.Key] = value;
                            }
                        }
                    }
                }
            }
            catch { }
        }
        private void Save()
        {
            var jo = new JsonObject();
            foreach (var item in _data)
            {
                if (item.Value is string s) // In this case we only need string support. TODO: Support other types
                {
                    jo.Add(item.Key, s);
                }
            }
            File.WriteAllText(_file, jo.ToJsonString());
        }
        public object this[string key] { get => _data[key]; set { _data[key] = value; Save(); } }

        public ICollection<string> Keys => _data.Keys;

        public ICollection<object> Values => _data.Values;

        public int Count => _data.Count;

        public bool IsReadOnly => false;

        public void Add(string key, object value)
        {
            _data.Add(key, value); Save();
        }

        public void Add(KeyValuePair<string, object> item)
        {
            _data.Add(item.Key, item.Value); Save();
        }

        public void Clear()
        {
            _data.Clear(); Save();
        }

        public bool Contains(KeyValuePair<string, object> item) => _data.Contains(item);

        public bool ContainsKey(string key) => _data.ContainsKey(key);

        public void CopyTo(KeyValuePair<string, object>[] array, int arrayIndex) => throw new NotImplementedException();

        public IEnumerator<KeyValuePair<string, object>> GetEnumerator() => throw new NotImplementedException();

        public bool Remove(string key) => throw new NotImplementedException();

        public bool Remove(KeyValuePair<string, object> item) => throw new NotImplementedException();

        public bool TryGetValue(string key, [MaybeNullWhen(false)] out object value) => throw new NotImplementedException();

        IEnumerator IEnumerable.GetEnumerator() => throw new NotImplementedException();
    }

    #endregion
}
