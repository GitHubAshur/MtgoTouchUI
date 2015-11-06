using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WindowsInput;
using NUnit.Framework;

namespace MtgoTouchUI
{
    [TestFixture]
    internal class UnitTests
    {
        private IntPtr hWnd = IntPtr.Zero;

        private Exception ActiveWindowChanged(bool active, IntPtr hWnd)
        {
            this.hWnd = hWnd;

            return null;
        }

        [Test]
        public void TestGetWindow()
        {
            using (var pp = new ProcessProvider(ActiveWindowChanged, "notepad"))
            {
                Thread.Sleep(500);

                Assert.That(hWnd != IntPtr.Zero);

                Console.WriteLine(pp.GetLastWindowText());

                pp.SendMessage(VirtualKeyCode.VK_A);
            }
        }
    }
}
