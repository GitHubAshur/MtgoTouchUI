using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WindowsInput;

namespace MtgoTouchUI
{
    internal class ProcessProvider : IDisposable
    {
        private Action action;
        private bool loop = true;
        private IntPtr hWndLast = IntPtr.Zero;
        
        public delegate Exception ActiveWindowChanged(bool active, IntPtr hWnd);
        private delegate bool EnumThreadDelegate(IntPtr hWnd, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern bool EnumThreadWindows(int dwThreadId, EnumThreadDelegate lpfn, IntPtr lParam);

        [DllImport("user32.dll", SetLastError = true, CharSet = CharSet.Auto)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        internal ProcessProvider(ActiveWindowChanged callback, string processName)
        {
            action = () =>
            {
                while (loop)
                {
                    var processes = Process.GetProcessesByName(processName);
                    var foregroundWindow = GetForegroundWindow();
                    var windowFound = false;

                    foreach (var window in processes.Select(EnumerateProcessWindowHandles).SelectMany(windows =>
                    {
                        var intPtrs = windows as IntPtr[] ?? windows.ToArray();
                        return intPtrs;
                    }).Where(window => foregroundWindow != hWndLast))
                    {
                        hWndLast = window;

                        if (foregroundWindow != Process.GetCurrentProcess().MainWindowHandle)
                        {
                            Console.WriteLine(foregroundWindow);
                            Console.WriteLine(Process.GetCurrentProcess().MainWindowHandle);
                            Console.WriteLine(foregroundWindow == window);
                            Console.WriteLine("***");
                            callback(foregroundWindow == window, window);
                        }
                    }

                    Thread.Sleep(150);
                }
            };

            Task.Factory.StartNew(action);
        }

        public void Dispose()
        {
            loop = false;
        }

        private static IEnumerable<IntPtr> EnumerateProcessWindowHandles(Process process)
        {
            var handles = new List<IntPtr>();

            foreach (var processThread in process.Threads.OfType<ProcessThread>())
            {
                EnumThreadWindows(processThread.Id,
                    (hWnd, lParam) => { handles.Add(hWnd); return true; }, IntPtr.Zero);
            }

            return handles;
        }

        public string GetLastWindowText()
        {
            var capacity = GetWindowTextLength(hWndLast) + 1;
            var sb = new StringBuilder(capacity);
            var result = GetWindowText(hWndLast, sb, capacity) + 1;

            return result == capacity ? sb.ToString() : null;
        }

        public void SendMessage(VirtualKeyCode key, VirtualKeyCode? modifier = null)
        {
            SetForegroundWindow(hWndLast);

            if (modifier != null)
            {
                InputSimulator.SimulateModifiedKeyStroke((VirtualKeyCode)modifier, key);
                return;
            }

            InputSimulator.SimulateKeyPress(key);
        }
    }
}
