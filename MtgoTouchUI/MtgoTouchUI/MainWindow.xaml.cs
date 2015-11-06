using System;
using System.Windows;
using System.Windows.Threading;
using WindowsInput;

namespace MtgoTouchUI
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ProcessProvider processProvider;
        public MainWindow()
        {
            InitializeComponent();
        
            action = delegate
            {
                Label1.Content = processProvider.GetLastWindowText();
            };

            processProvider = new ProcessProvider(ActiveWindowChanged, "MTGO");
        }

        private readonly Action action;

        private void MainWindow_OnLoaded(object sender, RoutedEventArgs e)
        {
            var desktopWorkingArea = SystemParameters.WorkArea;
            this.Left = desktopWorkingArea.Right - this.Width;
            this.Top = desktopWorkingArea.Top;
        }

        private Exception ActiveWindowChanged(IntPtr hWnd)
        {
            Dispatcher.Invoke(DispatcherPriority.Normal, action);

            return null;
        }

        private void ButtonBase1_OnClick(object sender, RoutedEventArgs e)
        {
            processProvider.SendMessage(VirtualKeyCode.F2);
        }

        private void ButtonBase2_OnClick(object sender, RoutedEventArgs e)
        {
            processProvider.SendMessage(VirtualKeyCode.F4);
        }

        private void ButtonBase3_OnClick(object sender, RoutedEventArgs e)
        {
            processProvider.SendMessage(VirtualKeyCode.MENU, VirtualKeyCode.VK_Y);
        }

        private void ButtonBase4_OnClick(object sender, RoutedEventArgs e)
        {
            processProvider.SendMessage(VirtualKeyCode.MENU, VirtualKeyCode.VK_N);
        }

        private void ButtonBase5_OnClick(object sender, RoutedEventArgs e)
        {
            processProvider.SendMessage(VirtualKeyCode.ESCAPE);
        }

        private void ButtonBase6_OnClick(object sender, RoutedEventArgs e)
        {
            processProvider.SendMessage(VirtualKeyCode.CONTROL, VirtualKeyCode.VK_Z);
        }
    }
}
