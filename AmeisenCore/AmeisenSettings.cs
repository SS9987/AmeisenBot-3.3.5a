﻿using System;
using System.IO;

namespace AmeisenCore
{
    public class AmeisenSettings
    {
        private static string configPath = AppDomain.CurrentDomain.BaseDirectory + "/config/";
        public Settings settings;
        private static AmeisenSettings i;
        public string loadedconfName; 

        private AmeisenSettings()
        {
            
        }

        public static AmeisenSettings GetInstance()
        {
            if (i == null)
                i = new AmeisenSettings();
            return i;
        }

        public void SaveToFile(string filename)
        {
            if (!Directory.Exists(configPath))
                Directory.CreateDirectory(configPath);
            File.WriteAllText(configPath + filename.ToLower() + ".json", Newtonsoft.Json.JsonConvert.SerializeObject(settings));
        }

        public void LoadFromFile(string filename)
        {
            if (!Directory.Exists(configPath))
                Directory.CreateDirectory(configPath);
            if (File.Exists(configPath + filename.ToLower() + ".json"))
                settings = Newtonsoft.Json.JsonConvert.DeserializeObject<Settings>(File.ReadAllText(configPath + filename.ToLower() + ".json"));
            else
            {
                settings = new Settings();
                settings.botMaxThreads = 2;
                settings.dataRefreshRate = 250;
                SaveToFile(filename);
            }

            loadedconfName = filename;
        }
    }

    public class Settings{
        public int dataRefreshRate;
        public int botMaxThreads;
    }
}
