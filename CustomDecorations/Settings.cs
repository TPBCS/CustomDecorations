using ColossalFramework.IO;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace CustomDecorations
{
    [XmlRoot("CustomDectorationsSettings")]
    public class CustomDectorationsSettings
    {
        [XmlIgnore]
        private static readonly string configurationPath = Path.Combine(DataLocation.localApplicationData, "CustomDecorations.xml");
        [XmlIgnore]
        private static CustomDecorationsManager Manager => CustomDecorationsManager.instance;

        public bool UseGrassDecorations = true;

        public bool UseFertileDecorations = true;

        public bool UseCliffDecorations = true;

        public int Density = 100;

        public string SelectedCliffPack = "CDM Built-in";

        public string SelectedFertilePack = "CDM Built-in";

        public string SelectedGrassPack = "CDM Built-in";

        public DecorationResolution SelectedResolution = DecorationResolution.Medium;
        

        public static string ConfigurationPath
        {
            get
            {
                return configurationPath;
            }
        }

        public CustomDectorationsSettings() { }

        public void OnPreSerialize() { }

        public void OnPostDeserialize() { }

        public void Save()
        {
            var fileName = ConfigurationPath;
            var config = CustomDecorationsManager.instance.Settings;
            var serializer = new XmlSerializer(typeof(CustomDectorationsSettings));
            using (var writer = new StreamWriter(fileName))
            {
                config.OnPreSerialize();
                serializer.Serialize(writer, config);
            }
        }


        public static CustomDectorationsSettings Load()
        {
            var fileName = ConfigurationPath;
            var serializer = new XmlSerializer(typeof(CustomDectorationsSettings));
            try
            {
                using (var reader = new StreamReader(fileName))
                {
                    var config = serializer.Deserialize(reader) as CustomDectorationsSettings;
                    return config;
                }
            }
            catch (Exception ex)
            {
                UnityEngine.Debug.Log($"[{UserMod.name}]: Error Parsing {fileName}: {ex}");
                return null;
            }
        }
    }
}
