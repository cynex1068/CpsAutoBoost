using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.InteropServices;

namespace WindowsCpsTool;

enum MouseButton
{
    Left,
    Right
}

static class AutoClickGuard
{
    private static int activeSenders;

    public static bool IsActive => Volatile.Read(ref activeSenders) > 0;

    public static void Enter()
    {
        Interlocked.Increment(ref activeSenders);
    }

    public static void Exit()
    {
        Interlocked.Decrement(ref activeSenders);
    }
}

sealed class GlobalMouseHook : IDisposable
{
    private const int WH_MOUSE_LL = 14;
    private const int WM_LBUTTONDOWN = 0x0201;
    private const int WM_RBUTTONDOWN = 0x0204;
    private const uint LLKHF_INJECTED = 0x00000001;
    private const long AUTO_CLICK_MARKER = 0x435053544F4F4C;
    private HookProc? hookProc;
    private nint hookId;

    public event Action<MouseButton>? MouseButtonDown;

    public void Install()
    {
        if (!OperatingSystem.IsWindows())
        {
            throw new PlatformNotSupportedException("Windows only");
        }
        hookProc = HookCallback;
        using var process = Process.GetCurrentProcess();
        using var module = process.MainModule!;
        hookId = SetWindowsHookEx(WH_MOUSE_LL, hookProc, GetModuleHandle(module.ModuleName), 0);
        if (hookId == 0)
        {
            throw new Win32Exception(Marshal.GetLastWin32Error(), "Unable to install mouse hook");
        }
    }

    private nint HookCallback(int nCode, nint wParam, nint lParam)
    {
        if (nCode >= 0)
        {
            var data = Marshal.PtrToStructure<MSLLHOOKSTRUCT>(lParam);
            var injected = (data.flags & LLKHF_INJECTED) != 0;
            var isOurAutoClick = data.dwExtraInfo == (nint)AUTO_CLICK_MARKER;
            if (!injected && !isOurAutoClick && !AutoClickGuard.IsActive)
            {
                if (wParam == WM_LBUTTONDOWN)
                {
                    MouseButtonDown?.Invoke(MouseButton.Left);
                }
                else if (wParam == WM_RBUTTONDOWN)
                {
                    MouseButtonDown?.Invoke(MouseButton.Right);
                }
            }
        }
        return CallNextHookEx(hookId, nCode, wParam, lParam);
    }

    public void Dispose()
    {
        if (hookId != 0)
        {
            UnhookWindowsHookEx(hookId);
            hookId = 0;
        }
        GC.SuppressFinalize(this);
    }

    private delegate nint HookProc(int nCode, nint wParam, nint lParam);

    [StructLayout(LayoutKind.Sequential)]
    private struct MSLLHOOKSTRUCT
    {
        public PointStruct point;
        public uint mouseData;
        public uint flags;
        public uint time;
        public nint dwExtraInfo;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct PointStruct
    {
        public int x;
        public int y;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern nint SetWindowsHookEx(int idHook, HookProc lpfn, nint hMod, uint dwThreadId);

    [DllImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static extern bool UnhookWindowsHookEx(nint hhk);

    [DllImport("user32.dll")]
    private static extern nint CallNextHookEx(nint hhk, int nCode, nint wParam, nint lParam);

    [DllImport("kernel32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
    private static extern nint GetModuleHandle(string? lpModuleName);
}
