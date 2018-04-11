using ColossalFramework;
using ColossalFramework.Plugins;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System;
using UnityEngine;

namespace CustomDecorations
{
    class CustomDecorationsManager : Singleton<CustomDecorationsManager>
    {
        internal bool InGame;

        internal Mesh[] CliffMeshes;

        internal Texture2D[] CliffTextures;

        internal Mesh[] FertileMeshes;

        internal Texture2D[] FertileTextures;

        internal Mesh[] GrassMeshes;

        internal Texture2D[] GrassTextures;

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

        internal void Prepare(DecorationType type)
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

        internal void LoadResources(CustomDecorationsData pack, DecorationType type)
        {
            var cliffNames = new string[] { "cliff0", "cliff1", "cliff2", "cliff3" };
            var fertileNames = new string[] { "fertile0", "fertile1", "fertile2", "fertile3" };
            var grassNames = new string[] { "grass0", "grass1", "grass2", "grass3" };

            CliffMeshes = new Mesh[4];
            FertileMeshes = new Mesh[4];
            GrassMeshes = new Mesh[4];
            CliffTextures = new Texture2D[4];
            FertileTextures = new Texture2D[4];
            GrassTextures = new Texture2D[4];

            var names = type == DecorationType.Cliff ? cliffNames : type == DecorationType.Fertile ? fertileNames : type == DecorationType.Grass ? grassNames : null;
            var meshList = type == DecorationType.Cliff ? CliffMeshes : type == DecorationType.Fertile ? FertileMeshes : type == DecorationType.Grass ? GrassMeshes : null;
            var textureList = type == DecorationType.Cliff ? CliffTextures : type == DecorationType.Fertile ? FertileTextures : type == DecorationType.Grass ? GrassTextures : null;

            for (int i = 0; i < 4; i++)
            {
                var texturePath = Path.Combine(pack.ResourcesPath, names[i] + ".png");

                var meshPath = Path.Combine(pack.ResourcesPath, names[i] + ".obj");

                var texture = Util.LoadTexture(texturePath);

                var mesh = Util.LoadMesh(meshPath);

                try
                {
                    meshList[i] = mesh;

                    textureList[i] = texture;
                }
                catch (Exception x)
                {
                    Debug.LogError($"{x.Message} - {x.StackTrace}");
                }
            }
        }        

        internal void OnLevelLoaded()
        {
            InGame = true;
            Terrain.m_useGrassDecorations = Settings.UseGrassDecorations;
            Terrain.m_useFertileDecorations = Settings.UseFertileDecorations;
            Terrain.m_useCliffDecorations = Settings.UseCliffDecorations;
            DecorationRenderer.SetResolution((int)Settings.SelectedResolution);
            UpdateDensity(Settings.Density);
            gameObject.AddComponent<CliffLoader>();
            gameObject.AddComponent<FertileLoader>();
            gameObject.AddComponent<GrassLoader>();
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