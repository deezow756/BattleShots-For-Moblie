using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace BattleShots
{
    public class FileManager
    {
        public static string devicePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Device.txt");

        public string GetDevice()
        {
            if (!File.Exists(devicePath))
            {
                return File.ReadAllText(devicePath);
            }
            else return null;
        }

        public void SaveDevice(string device)
        {
            File.WriteAllText(devicePath, device);
        }
    }
}
