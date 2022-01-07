using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

namespace DrunkenBoxing {
    public class Settings {
        public static Settings instance = new Settings();

        public List<string> bosses;
        public List<string> dots;
        public Dictionary<string, Priority> priorities;
        public double fightDistance;
        public double ringDistance;
        public int ringMinimumCount;
        public int pveCaster;
        public int bossCaster;
        public Dictionary<Race, int> slayerCasters;

        private Settings() {
            fightDistance = 6.0;
            ringDistance = 4.0;
            ringMinimumCount = 3;
            bosses = new List<string>();
            dots = new List<string>();
            priorities = new Dictionary<string, Priority>();
            pveCaster = bossCaster = int.MinValue;
            slayerCasters = new Dictionary<Race, int>(Enum.GetNames(typeof(Race)).Length);

            foreach (Race r in Enum.GetValues(typeof(Race))) {
                slayerCasters[r] = int.MinValue;
            }
        }

        public void Dump(string characterName) {
            try {
                JsonSerializer serializer = new JsonSerializer();
                serializer.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                serializer.Formatting = Formatting.Indented;
                string formattedCharacterName = characterName.Replace(' ', '_');
                
                using (StreamWriter sw = new StreamWriter(@"drunkenboxing_settings_" + formattedCharacterName + ".json", false))
                using (JsonWriter writer = new JsonTextWriter(sw))
                {
                    serializer.Serialize(writer, this);
                }
            } catch (Exception ex) { Logger.LogError("Settings.Dump", ex); }
        }

        public bool Load(string characterName) {
            try {
                string formattedCharacterName = characterName.Replace(' ', '_');
                
                if (File.Exists(@"drunkenboxing_settings_" + formattedCharacterName + ".json")) {
                    Character.instance.state = State.Disabled;
                    JsonSerializer serializer = new JsonSerializer();
                    serializer.Converters.Add(new Newtonsoft.Json.Converters.StringEnumConverter());
                    
                    using (StreamReader sr = new StreamReader(@"drunkenboxing_settings_" + formattedCharacterName + ".json"))
                    using (JsonTextReader reader = new JsonTextReader(sr))
                    {
                        instance = serializer.Deserialize<Settings>(reader);
                    }

                    Character.instance.state = State.Ready;
                    // Maybe need to notify everything to respond to new settings here?
                    return true;
                }
                else {
                    Logger.LogMessage("No settings file found for " + formattedCharacterName + ".");
                    return false;
                }
            }
            catch (Exception ex) {
                Logger.LogError("Settings.Dump", ex);
                Character.instance.state = State.Ready;
                return false;
            }
        }
    }
}