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

        public void Load(DecorationType type)
        {
            var size = type == DecorationType.Cliff ? TerrainManager.instance.m_properties.m_cliffDecorations.Length : type == DecorationType.Fertile ? TerrainManager.instance.m_properties.m_fertileDecorations.Length : TerrainManager.instance.m_properties.m_grassDecorations.Length;

            var cliffNames = new string[] { "cliff0", "cliff1", "cliff2", "cliff3" };

            var fertileNames = new string[] { "fertile0", "fertile1", "fertile2", "fertile3" };

            var grassNames = new string[] { "grass0", "grass1", "grass2", "grass3" };

            var names = type == DecorationType.Cliff ? cliffNames : type == DecorationType.Fertile ? fertileNames : grassNames;

            for (int i = 0; i < size; i++)
            {
                var texturePath = Path.Combine(ResourcesPath, names[i] + ".png");

                var meshPath = Path.Combine(ResourcesPath, names[i] + ".obj");                

                var texture = Util.LoadTexture(texturePath);

                var mesh = Util.LoadMesh(meshPath);                

                try
                {
                    switch (type)
                    {
                        case DecorationType.Grass:
                            TerrainManager.instance.m_properties.m_grassDecorations[i].m_mesh = mesh;
                            TerrainManager.instance.m_properties.m_grassDecorations[i].m_renderMaterial.SetTexture("_MainTex", texture);
                            break;
                        case DecorationType.Fertile:
                            TerrainManager.instance.m_properties.m_fertileDecorations[i].m_mesh = mesh;
                            TerrainManager.instance.m_properties.m_fertileDecorations[i].m_renderMaterial.SetTexture("_MainTex", texture);
                            break;
                        case DecorationType.Cliff:
                            TerrainManager.instance.m_properties.m_cliffDecorations[i].m_mesh = mesh;
                            TerrainManager.instance.m_properties.m_cliffDecorations[i].m_renderMaterial.SetTexture("_MainTex", texture);
                            break;
                        default:
                            break;
                    }
                }
                catch (Exception x)
                {
                    Debug.Log($"Message: {x.Message} StackTrace: {x.StackTrace}");
                }
                
            }
        }       
    }    
}
