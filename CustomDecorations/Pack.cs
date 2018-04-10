using System;
using System.IO;
using System.Xml.Serialization;
using UnityEngine;

namespace CustomDecorations
{
    [XmlRoot("CustomDecorationsData")]
    public class CustomDecorationsData
    {
        public string Name;
        [XmlIgnore]
        public string ResourcesPath;

        public CustomDecorationsData() { }

        public void OnPreSerialize() { }

        public void OnPostDeserialize() { } 
        
        public static CustomDecorationsData Deserialize(string filePath)
        { 
            var serializer = new XmlSerializer(typeof(CustomDecorationsData));

            try
            {
                using (var reader = new StreamReader(filePath))
                {
                    var pack = serializer.Deserialize(reader) as CustomDecorationsData;
                    return pack;
                }
            }
            catch (Exception ex)
            {
                Debug.Log($"Error Parsing {filePath}: {ex}");
                return null;
            }
        }  
    }    
}
