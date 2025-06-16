using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;
using Microsoft.UI.Xaml.Navigation;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using Windows.System;
using Windows.UI.Core;
using Microsoft.UI.Xaml.Markup;
using BitDesk.ViewModels;

namespace BitDesk.Views.Modal;

public sealed partial class ModalShell : Page
{
    public ModalShell(ViewModels.PairViewModel vm)
    {
        InitializeComponent();

        DataContext = vm;
    }
}
