using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using NAudio.CoreAudioApi;

namespace AudioVolumeLocker
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private bool isRunning;
        private bool isOk;

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            try
            {
                Dispatcher.Invoke(new Action(() =>
                {
                    Title = AppSettings.Device;
                }));
                grid.Dispatcher.Invoke(new Action(() =>
                {
                    grid.Background = SystemParameters.WindowGlassBrush;
                }));
                button.Dispatcher.Invoke(new Action(() =>
                {
                    button.Content = AppSettings.Volume;
                }));

                Task.Run(() => StatusUpdater());
                Task.Run(() => VolumeLocker());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void Window_Closing(object sender, CancelEventArgs e)
        {
            try
            {
                isRunning = false;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                isRunning = !isRunning;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }


        private void Window_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            try
            {
                if (e.Delta > 0 && AppSettings.Volume < 100)
                {
                    AppSettings.Volume++;
                }
                else if (e.Delta < 0 && AppSettings.Volume > 0)
                {
                    AppSettings.Volume--;
                }
                button.Dispatcher.Invoke(new Action(() =>
                {
                    button.Content = AppSettings.Volume;
                }));
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private async Task StatusUpdater()
        {
            while (true)
            {
                try
                {
                    if (isRunning && isOk)
                    {
                        label.Dispatcher.Invoke(new Action(() =>
                        {
                            label.Content = "Running";
                            label.Background = Brushes.DeepSkyBlue;
                        }));
                        button.Dispatcher.Invoke(new Action(() =>
                        {
                            button.Content = AppSettings.Volume;
                            button.Background = Brushes.DeepSkyBlue;
                        }));
                    }
                    else if (!isRunning && isOk)
                    {
                        label.Dispatcher.Invoke(new Action(() =>
                        {
                            label.Content = "Stopped";
                            label.Background = Brushes.Crimson;
                        }));
                        button.Dispatcher.Invoke(new Action(() =>
                        {
                            button.Content = AppSettings.Volume;
                            button.Background = Brushes.Crimson;
                        }));
                    }
                    else
                    {
                        label.Dispatcher.Invoke(new Action(() =>
                        {
                            label.Content = "NotFound";
                            label.Background = Brushes.DimGray;
                        }));
                        button.Dispatcher.Invoke(new Action(() =>
                        {
                            button.Content = AppSettings.Volume;
                            button.Background = Brushes.DimGray;
                        }));
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                finally
                {
                    await Task.Delay(10);
                }
            }
        }

        private async Task VolumeLocker()
        {
            var type = (DataFlow)AppSettings.Type;
            var device = AppSettings.Device;
            var mmd = new MMDeviceEnumerator()
                .EnumerateAudioEndPoints(type, DeviceState.Active)
                .Cast<MMDevice>()
                .Where(x => x.FriendlyName.Contains(device))
                .FirstOrDefault();
            if (mmd == null)
            {
                isOk = false;
                return;
            }
            else
            {
                isOk = true;
            }

            while (true)
            {
                try
                {
                    if (isRunning)
                    {
                        var volume = AppSettings.Volume / 100f;
                        var aev = mmd.AudioEndpointVolume;
                        aev.MasterVolumeLevelScalar = volume;
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                }
                finally
                {
                    await Task.Delay(10);
                }
            }
        }
    }
}
