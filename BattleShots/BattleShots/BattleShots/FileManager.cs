using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Newtonsoft.Json;

namespace BattleShots
{
    public class FileManager
    {
        public string devicePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "device.txt");
        public string StoragePath = Environment.GetFolderPath(Environment.SpecialFolder.Personal);

        public string GetDevice()
        {
            if (File.Exists(devicePath))
            {
                return File.ReadAllText(devicePath);
            }
            else return null;
        }
        public void SaveDevice(string device)
        {
            File.WriteAllText(devicePath, device);
        }

        public bool CheckForExistingGame(string name)
        {
            if (File.Exists(Path.Combine(StoragePath, name + ".json")))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public GameSettings GetGameSettings (string name)
        {
            string jsonString = File.ReadAllText(Path.Combine(StoragePath, name + ".json"));
            return (GameSettings)JsonConvert.DeserializeObject(jsonString);
        }

        public void SaveGameSettings(GameSettings settings)
        {
            File.WriteAllText(Path.Combine(StoragePath, settings.ConnectedDeviceName + ".json") ,JsonConvert.SerializeObject(settings));
        }

        public void DeleteGameSetting(string name)
        {
            File.Delete(Path.Combine(StoragePath, name + ".json"));
        }
    }
}
