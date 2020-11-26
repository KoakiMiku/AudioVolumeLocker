using System;
using System.Diagnostics;
using System.IO;
using Newtonsoft.Json.Linq;

namespace AudioVolumeLocker
{
    class AppSettings
    {
        public static int Type { get; set; }
        public static string Device { get; set; }
        public static int Volume { get; set; }

        public static bool Init()
        {
            try
            {
                var file = File.ReadAllText("AppSettings.json");
                var json = JObject.Parse(file);
                Type = json.Value<int>("Type");
                Device = json.Value<string>("Device");
                Volume = json.Value<int>("Volume");

                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);

                return false;
            }
        }
    }
}
