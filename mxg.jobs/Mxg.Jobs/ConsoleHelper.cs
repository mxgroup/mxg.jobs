using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Security;

namespace Mxg.Jobs
{
    public class ConsoleHelper
    {
        static Int32 WM_SYSCOMMAND = 0x0112;
        static Int32 SC_MINIMIZE = 0x0F020;

        public static int Create()
        {
            if (AllocConsole())
            {
                SendMessage(Process.GetCurrentProcess().MainWindowHandle, WM_SYSCOMMAND, SC_MINIMIZE, 0);

                return 0;
            }
            return Marshal.GetLastWin32Error();
        }

        public static int Attach()
        {
            if (!AttachConsole(-1))
            { // Attach to an parent process console
                return Create();
            }
            return Marshal.GetLastWin32Error();
        }

        public static int Destroy()
        {
            if (FreeConsole())
            {
                return 0;
            }
            return Marshal.GetLastWin32Error();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage"), SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AllocConsole();


        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2118:ReviewSuppressUnmanagedCodeSecurityUsage"), SuppressUnmanagedCodeSecurity]
        [DllImport("kernel32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool FreeConsole();

        [DllImport("kernel32.dll")]
        static extern bool AttachConsole(int pid);

        [DllImport("user32.dll")]
        internal static extern bool SendMessage(IntPtr hWnd, Int32 msg, Int32 wParam, Int32 lParam);
    }
}
