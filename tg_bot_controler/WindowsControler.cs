using Newtonsoft.Json.Bson;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace tg_bot_controler
{

    internal class WindowsControler
    {
        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        // Importing necessary functions from user32.dll
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool EnumWindows(EnumWindowsProc lpEnumFunc, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        const uint WM_CLOSE = 0x0010;

        static public string GetActiveProgramms()
        {
            List<string> windowTitles = new List<string>();

            // Callback function to be called for each open window
            EnumWindows((hWnd, lParam) =>
            {
                if (IsWindowVisible(hWnd))
                {
                    StringBuilder sb = new StringBuilder(256);
                    GetWindowText(hWnd, sb, sb.Capacity);

                    string title = sb.ToString();
                    if (!string.IsNullOrEmpty(title))
                    {
                        windowTitles.Add(title);
                    }
                }
                return true;
            }, IntPtr.Zero);

            string titles = "";

            // Output all window titles
            foreach (string title in windowTitles)
            {
                titles += windowTitles.IndexOf(title) + ": " + title + "\n" + "\n";
            }
            return titles;
        }

        static public void KillProgramm(int indexProgramm)
        {
            List<(IntPtr hWnd, string Title)> windows = new List<(IntPtr hWnd, string Title)>();

            // Callback function to be called for each open window
            EnumWindows((hWnd, lParam) =>
            {
                if (IsWindowVisible(hWnd))
                {
                    StringBuilder sb = new StringBuilder(256);
                    GetWindowText(hWnd, sb, sb.Capacity);

                    string title = sb.ToString();
                    if (!string.IsNullOrEmpty(title))
                    {
                        windows.Add((hWnd, title));
                    }
                }
                return true;
            }, IntPtr.Zero);

            // Output all window titles
            // Example of closing a specific window by title
            string titleToClose = windows[indexProgramm].Title; // Замените на название окна, которое нужно закрыть
            foreach (var window in windows)
            {
                if (window.Title.Contains(titleToClose))
                {
                    Console.WriteLine($"Closing window: {window.Title}");
                    PostMessage(window.hWnd, WM_CLOSE, IntPtr.Zero, IntPtr.Zero);
                }
            }
        }
    }
}
