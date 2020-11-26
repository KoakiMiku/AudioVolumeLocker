using System;
using System.Windows;

namespace AudioVolumeLocker
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            var status = AppSettings.Init();
            if (!status)
            {
                return;
            }

            var application = new Application();
            var window = new MainWindow();
            application.Run(window);
        }
    }
}
