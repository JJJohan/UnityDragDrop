#if UNITY_EDITOR_WIN || UNITY_STANDALONE_WIN
using System;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;

public class DragDropHandlerWindows : IDragDropHandler
{
    private static DragDropController _controller;
    private static IntPtr _windowHandle;
    private static IntPtr _oldWndProc;
    private static WndProcDelegate _oldWndProcDelegate;

    private const int SetWndProcMessage = -4;
    private const int DropFilesMessage = 0x0233;
    private const int MaxPath = 260;

    private delegate IntPtr WndProcDelegate(IntPtr hWnd, int message, IntPtr wParam, IntPtr lParam);

    [DllImport("user32.dll")]
    private static extern IntPtr GetForegroundWindow();

    [DllImport("Shell32.dll")]
    private static extern void DragAcceptFiles(IntPtr windowHandle, bool accept);

    [DllImport("Shell32.dll")]
    private static extern int DragQueryFile(IntPtr hDrop, uint fileIndex, [Out] StringBuilder fileName, uint fileSize);

    [DllImport("shell32.dll")]
    private static extern void DragFinish(IntPtr hDrop);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongW")]
    public static extern IntPtr SetWindowLongPtr32(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    [DllImport("user32.dll", EntryPoint = "SetWindowLongPtrW")]
    public static extern IntPtr SetWindowLongPtr64(IntPtr hWnd, int nIndex, IntPtr dwNewLong);

    public void Bind(DragDropController controller)
    {
        _controller = controller;
        _windowHandle = GetForegroundWindow();
    }

    public void Hook()
    {
        DragAcceptFiles(_windowHandle, true);
        _oldWndProc = SetWindowProc(_windowHandle, WndProc);
        if (_oldWndProc != IntPtr.Zero)
        {
            _oldWndProcDelegate = (WndProcDelegate)Marshal.GetDelegateForFunctionPointer(_oldWndProc, typeof(WndProcDelegate));
        }
    }

    public void Unhook()
    {
        if (_oldWndProc == IntPtr.Zero)
        {
            return;
        }

        if (IntPtr.Size == 4)
        {
            SetWindowLongPtr32(_windowHandle, SetWndProcMessage, _oldWndProc);
        }
        else
        {
            SetWindowLongPtr64(_windowHandle, SetWndProcMessage, _oldWndProc);
        }

        _oldWndProcDelegate = null;
    }

    private static IntPtr WndProc(IntPtr hwnd, int message, IntPtr wparam, IntPtr lparam)
    {
        if (message == DropFilesMessage)
        {
            int fileCount = DragQueryFile(wparam, 0xFFFFFFFF, null, 0);
            for (uint i = 0; i < fileCount; ++i)
            {
                int size = DragQueryFile(wparam, i, null, 0);

                StringBuilder fileNameBuilder = new StringBuilder(size + 1);
                DragQueryFile(wparam, i, fileNameBuilder, MaxPath);
                string fileName = fileNameBuilder.ToString();

                if (_controller.OnDropped != null)
                {
                    _controller.OnDropped(fileName, 0, 0);
                }

                if (_controller.OnDroppedData != null)
                {
                    byte[] data = File.ReadAllBytes(fileName);
                    _controller.OnDroppedData(fileName, 0, 0, data);
                }
            }

            DragFinish(wparam);
            return IntPtr.Zero;
        }

        if (_oldWndProc != IntPtr.Zero)
        {
            return _oldWndProcDelegate(hwnd, message, wparam, lparam);
        }

        return IntPtr.Zero;
    }

    private static IntPtr SetWindowProc(IntPtr hWnd, WndProcDelegate newWndProc)
    {
        IntPtr newWndProcPtr = Marshal.GetFunctionPointerForDelegate(newWndProc);
        IntPtr oldWndProcPtr;

        if (IntPtr.Size == 4)
        {
            oldWndProcPtr = SetWindowLongPtr32(hWnd, SetWndProcMessage, newWndProcPtr);
        }
        else
        {
            oldWndProcPtr = SetWindowLongPtr64(hWnd, SetWndProcMessage, newWndProcPtr);
        }

        return oldWndProcPtr;
    }
}
#endif