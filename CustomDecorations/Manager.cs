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

        private CustomDectorationsSettings settings;        
        
        public List<CustomDecorationsData> AvailablePacks;

        public CustomDectorationsSettings Settings
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

        public DecorationRenderer DecorationRenderer => ReflectionUtil.GetField<DecorationRenderer>(TerrainManager.instance, "m_decoRenderer");

        public TerrainProperties Terrain => Singleton<TerrainManager>.instance.m_properties;

        public List<CustomDecorationsData> GetAvailablePacks()
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
        
        public void LoadPack(DecorationType type)
        {
            CustomDecorationsData pack;
            AvailablePacks = GetAvailablePacks();
            try
            {
                switch (type)
                {
                    case DecorationType.Grass:
                        pack = AvailablePacks.Find(p => p.Name == Settings.SelectedGrassPack);
                        pack.Load(type);
                        break;
                    case DecorationType.Fertile:
                        pack = AvailablePacks.Find(p => p.Name == Settings.SelectedFertilePack);
                        pack.Load(type);
                        break;
                    default:
                        pack = AvailablePacks.Find(p => p.Name == Settings.SelectedCliffPack);
                        pack.Load(type);
                        break;
                }
            }
            catch (Exception)
            {
                
            }
           
        }
        
        public void OnLevelLoaded()
        {
            InGame = true;
            Terrain.m_useGrassDecorations = Settings.UseGrassDecorations;
            Terrain.m_useFertileDecorations = Settings.UseFertileDecorations;
            Terrain.m_useCliffDecorations = Settings.UseCliffDecorations;
            DecorationRenderer.SetResolution((int)Settings.SelectedResolution);
            UpdateDensity(Settings.Density);
            LoadPack(DecorationType.Cliff);
            LoadPack(DecorationType.Fertile);
            LoadPack(DecorationType.Grass);
        }

        internal void UpdateDensity(int i)
        {
            DecorationMesh.m_mesh = ReflectionUtil.InvokeMethod<Mesh>(typeof(DecorationMesh), "GenerateMesh", new object[] { i });
        }

        public void OnLevelUnloading()
        {
            InGame = false;
        }
    }
}