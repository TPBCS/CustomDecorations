using ColossalFramework;
using ColossalFramework.Plugins;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using System.Collections;

namespace CustomDecorations
{
    public class CustomDecorationsManager : Singleton<CustomDecorationsManager>
    {
        internal bool InGame;

        internal UnityMainThreadDispatcher Dispatcher;

        internal DecorationRenderer DecorationRenderer => ReflectionUtil.GetField<DecorationRenderer>(TerrainManager.instance, "m_decoRenderer");

        internal TerrainProperties Terrain => Singleton<TerrainManager>.instance.m_properties;

        internal DecorationInfo[] GrassDecorations => Terrain.m_grassDecorations;

        internal DecorationInfo[] CliffDecorations => Terrain.m_cliffDecorations;

        internal DecorationInfo[] FertileDecorations => Terrain.m_fertileDecorations;

        private CustomDectorationsSettings settings;

        internal List<CustomDecorationsData> AvailablePacks => GetAvailablePacks();

        internal CustomDectorationsSettings Settings
        {
            get
            {
                if (settings == null)
                {
                    settings = CustomDectorationsSettings.Load();
                    if (settings == null)
                    {
                        settings = new CustomDectorationsSettings();
                        settings.Save();
                    }
                }
                return settings;
            }
            set
            {
                settings = value;
            }
        }

        internal List<CustomDecorationsData> GetAvailablePacks()
        {
            var identifier = "CustomDecorationsData.xml";

            var customDecorationsPacks = new List<CustomDecorationsData>();

            var plugins = PluginManager.instance.GetPluginsInfo().ToArray();

            foreach (var plugin in plugins)
            {
                string identifierFile = Path.Combine(plugin.modPath, identifier);

                if (!File.Exists(identifierFile)) continue;

                CustomDecorationsData pack = CustomDecorationsData.Deserialize(identifierFile);

                string resourcePath = Path.Combine(plugin.modPath, "Resources");

                if (pack == null || !Directory.Exists(resourcePath)) continue;

                if (pack.Name == null) pack.Name = plugin.name;

                pack.ResourcesPath = resourcePath;

                customDecorationsPacks.Add(pack);
            }
            return customDecorationsPacks;
        }

        internal void Load1(DecorationType type)
        {
            CustomDecorationsData pack;            
            
            switch (type)
            {
                case DecorationType.Grass:
                    pack = AvailablePacks.Find(p => p.Name == Settings.SelectedGrassPack);
                    break;
                case DecorationType.Fertile:
                    pack = AvailablePacks.Find(p => p.Name == Settings.SelectedFertilePack);
                    break;
                default:
                    pack = AvailablePacks.Find(p => p.Name == Settings.SelectedCliffPack);
                    break;
            }

            if (pack != null) LoadResources(pack, type);
        }

        internal void Load(DecorationType type)
        {
            CustomDecorationsData pack;

            switch (type)
            {
                case DecorationType.Grass:
                    pack = AvailablePacks.Find(p => p.Name == Settings.SelectedGrassPack);
                    break;
                case DecorationType.Fertile:
                    pack = AvailablePacks.Find(p => p.Name == Settings.SelectedFertilePack);
                    break;
                default:
                    pack = AvailablePacks.Find(p => p.Name == Settings.SelectedCliffPack);
                    break;
            }

            if (pack != null) UnityMainThreadDispatcher.Instance().Enqueue(LoadResources(pack, type));
        }

        internal IEnumerator LoadResources(CustomDecorationsData pack, DecorationType type)
        {
            var cliffNames = new string[] { "cliff0", "cliff1", "cliff2", "cliff3" };
            var fertileNames = new string[] { "fertile0", "fertile1", "fertile2", "fertile3" };
            var grassNames = new string[] { "grass0", "grass1", "grass2", "grass3" };

            var names = type == DecorationType.Cliff ? cliffNames : type == DecorationType.Fertile ? fertileNames : type == DecorationType.Grass ? grassNames : null;
            var decorationTarget = type == DecorationType.Cliff ? CliffDecorations : type == DecorationType.Fertile ? FertileDecorations : type == DecorationType.Grass ? GrassDecorations : null;

            for (int i = 0; i < decorationTarget.Length; i++)
            {
                
                var texturePath = Path.Combine(pack.ResourcesPath, names[i] + ".png");

                var textureXYCAPath = Path.Combine(pack.ResourcesPath, names[i] + "XYCA.png");

                var meshPath = Path.Combine(pack.ResourcesPath, names[i] + ".obj");

                var texture = Util.LoadTexture(texturePath);

                yield return null;

                var textureXYCA = Util.LoadTexture(textureXYCAPath);

                yield return null;

                var mesh = Util.LoadMesh(meshPath);

                yield return null;

                decorationTarget[i].m_mesh = mesh;

                decorationTarget[i].m_renderMaterial.SetTexture("_MainTex", texture);

                if(textureXYCA)decorationTarget[i].m_renderMaterial.SetTexture("_XYCAMap", textureXYCA);               

            }

            DecorationRenderer.SetResolution((int)Settings.SelectedResolution);

            yield break;
        } 

        internal void OnLevelLoaded()
        {
            InGame = true;
            Dispatcher = gameObject.AddComponent<UnityMainThreadDispatcher>();
            Load(DecorationType.Cliff);
            Load(DecorationType.Fertile);
            Load(DecorationType.Grass);
            Terrain.m_useGrassDecorations = Settings.UseGrassDecorations;
            Terrain.m_useFertileDecorations = Settings.UseFertileDecorations;
            Terrain.m_useCliffDecorations = Settings.UseCliffDecorations;
            DecorationRenderer.SetResolution((int)Settings.SelectedResolution);
            UpdateDensity(Settings.Density);
        }

        internal void UpdateDensity(int i)
        {
            DecorationMesh.m_mesh = ReflectionUtil.InvokeMethod<Mesh>(typeof(DecorationMesh), "GenerateMesh", new object[] { i });
        }

        internal void OnLevelUnloading()
        {
            InGame = false;
        }
    }
}