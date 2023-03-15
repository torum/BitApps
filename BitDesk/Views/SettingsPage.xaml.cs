﻿using System.Diagnostics;
using BitDesk.ViewModels;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Markup;
using Microsoft.UI.Xaml.Navigation;

namespace BitDesk.Views;

public sealed partial class SettingsPage : Page
{
    public MainViewModel? ViewModel
    {
        get; private set;
    }

    public SettingsPage()
    {
        try
        {
            InitializeComponent();
        }
        catch (XamlParseException parseException)
        {
            Debug.WriteLine($"Unhandled XamlParseException in SettingsPage: {parseException.Message}");
            foreach (var key in parseException.Data.Keys)
            {
                Debug.WriteLine("{Key}:{Value}", key.ToString(), parseException.Data[key]?.ToString());
            }
            throw;
        }
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
