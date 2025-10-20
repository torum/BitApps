using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using BitDesk.Contracts;
using Microsoft.UI;
using Microsoft.UI.Windowing;
using Microsoft.UI.Xaml;
using WinRT.Interop;

namespace BitDesk.Services;

public partial class ModalDialogService : IModalDialogService
{
    public ModalDialogService()
    {
       
    }

    public void ShowOrderDialog(ViewModels.PairViewModel vm)
    {
        var mainWindow = App.MainWindow;

        var modalShell = new Views.Modal.ModalShell(vm)
        {
            DataContext = vm
        };
        var modalWindow = new Views.Modal.ModalWindow
        {
            Content = modalShell
        };

        //modalWindow.AppWindow.MoveAndResize(new Windows.Graphics.RectInt32(EditorWinLeft, EditorWinTop, EditorWinWidth, EditorWinHeight));

        var appWindow = modalWindow.AppWindow;
        if (appWindow is null)
        {
            return;
        }

        /*
        var hWndParent = WinRT.Interop.WindowNative.GetWindowHandle(mainWindow);
        var hWndDialog = WinRT.Interop.WindowNative.GetWindowHandle(modalWindow);
        SetWindow(hWndDialog, GWL_HWNDPARENT, hWndParent);
        */

        SetWindowOwner(mainWindow, modalWindow);

        /*
        if (appWindow.Presenter is OverlappedPresenter presenter)
        {
        }
        */
        var presenter = OverlappedPresenter.CreateForDialog();

        presenter.IsModal = true;

        appWindow.SetPresenter(presenter);


        modalWindow.Closed += (sender, e) =>
        {
            // This causes all sorts of problems. (as of WinAppSDK 1.7.25)
            //EnableWindow(hWndParent, true);

            mainWindow.Activate();
        };

        if (mainWindow != null)
        {
            mainWindow.Closed += (sender, e) =>
            {
                modalWindow.Close();
            };
        }

        // This causes all sorts of problems. (as of WinAppSDK 1.7.25)
        //EnableWindow(hWndParent, false);

        appWindow.Show(true);// Same as EnableWindow. This causes all sorts of problems. (as of WinAppSDK 1.7.25)

        //modalWindow.Activate();
        //modalWindow.Show();
    }

    #region == TEMP code for modal window(for setting an owner) ==


    [LibraryImport("User32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool EnableWindow(IntPtr hWnd, [MarshalAs(UnmanagedType.Bool)] bool bEnable);

    private const int GWL_HWNDPARENT = (-8);


    private static void SetWindowOwner(Window owner, Window child)
    {
        // Get the HWND (window handle) of the owner window (main window).
        var ownerHwnd = WindowNative.GetWindowHandle(owner);

        // Get the HWND of the AppWindow (modal window).
        var ownedHwnd = Win32Interop.GetWindowFromWindowId(child.AppWindow.Id);

        // Set the owner window using SetWindowLongPtr for 64-bit systems
        // or SetWindowLong for 32-bit systems.
        if (IntPtr.Size == 8) // Check if the system is 64-bit
        {
            SetWindowLongPtr(ownedHwnd, GWL_HWNDPARENT, ownerHwnd); // -8 = GWLP_HWNDPARENT
        }
        else // 32-bit system
        {
            SetWindowLong(ownedHwnd, GWL_HWNDPARENT, ownerHwnd); // -8 = GWL_HWNDPARENT
        }
    }

    // Import the Windows API function SetWindowLongPtr for modifying window properties on 64-bit systems.
    [LibraryImport("User32.dll", EntryPoint = "SetWindowLongPtrW")]
    private static partial IntPtr SetWindowLongPtr(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    // Import the Windows API function SetWindowLong for modifying window properties on 32-bit systems.
    [LibraryImport("User32.dll", EntryPoint = "SetWindowLongW")]
    private static partial IntPtr SetWindowLong(IntPtr hWnd, int nIndex, IntPtr dwNewLong);


    #endregion
}
