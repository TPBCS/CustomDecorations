using ColossalFramework.UI;
using ICities;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System;

namespace CustomDecorations
{
    public class UserMod : IUserMod
    {
        internal static readonly string name = "Custom Terrain Decorations";

        private readonly string description = "Allows the use of custom 'decoration' sprites.";

        public string Name => name;

        public string Description => description;

        private CustomDecorationsManager Manager => CustomDecorationsManager.instance;

        private CustomDectorationsSettings Settings => Manager.Settings;
                
        private string[] PackNames => GetPackNames();

        private Vector2 dropdownSize = new Vector2(400f, 30f);

        private int uiSpacing = 10;

        private string[] resolutionList = new string[] { DecorationResolution.Low.ToString(), DecorationResolution.Medium.ToString(), DecorationResolution.High.ToString(), DecorationResolution.Ultra.ToString() };
                
        private UIDropDown resolutionDropdown;

        private UIDropDown cliffDropdown;

        private UIDropDown fertileDropdown;

        private UIDropDown grassDropdown;

        private UISlider densitySlider;

        private int SelectedResolutionIndex => resolutionList.ToList().FindIndex(resolution => resolution == Settings.SelectedResolution.ToString());

        private int SelectedCliffDropdownIndex => PackNames.ToList().FindIndex(pack => pack == Settings.SelectedCliffPack) == -1 ? 0 : PackNames.ToList().FindIndex(pack => pack == Settings.SelectedCliffPack);

        private int SelectedFertileDropdownIndex => PackNames.ToList().FindIndex(pack => pack == Settings.SelectedFertilePack) == -1 ? 0 : PackNames.ToList().FindIndex(pack => pack == Settings.SelectedFertilePack);

        private int SelectedGrassDropdownIndex => PackNames.ToList().FindIndex(pack => pack == Settings.SelectedGrassPack) == -1 ? 0 : PackNames.ToList().FindIndex(pack => pack == Settings.SelectedGrassPack);

        public void OnSettingsUI(UIHelperBase helper)
        {
            helper.AddCheckbox("Use Grass Decorations", Settings.UseGrassDecorations, (b) =>
            {
                Settings.UseGrassDecorations = b;
                Settings.Save();
                if (Manager.InGame) Manager.Terrain.m_useGrassDecorations = b;
            });
            helper.AddSpace(uiSpacing);

            helper.AddCheckbox("Use Fertile Decorations", Settings.UseFertileDecorations, (b) =>
            {
                Settings.UseFertileDecorations = b;
                Settings.Save();
                if (Manager.InGame) Manager.Terrain.m_useFertileDecorations = b;
            });
            helper.AddSpace(uiSpacing);

            helper.AddCheckbox("Use Cliff Decorations", Settings.UseCliffDecorations, (b) =>
            {
                Settings.UseCliffDecorations = b;
                Settings.Save();
                if (Manager.InGame) Manager.Terrain.m_useCliffDecorations = b;
            });
            helper.AddSpace(uiSpacing);
            resolutionDropdown = (UIDropDown)helper.AddDropdown("Resolution", resolutionList, SelectedResolutionIndex, (index) =>
            {
                DecorationResolution resolution;
                switch (index)
                {
                    case 0:
                        resolution = DecorationResolution.Low;
                        break;
                    case 1:
                        resolution = DecorationResolution.Medium;
                        break;
                    case 2:
                        resolution = DecorationResolution.High;
                        break;
                    default:
                        resolution = DecorationResolution.Ultra;
                        break;
                }

                Settings.SelectedResolution = resolution;
                Settings.Save();
                if (Manager.InGame) Manager.DecorationRenderer.SetResolution((int)resolution);
            });
            helper.AddSpace(uiSpacing);

            densitySlider = (UISlider)helper.AddSlider($"Decorations Density: {Settings.Density}", 5f, 127f, 1f, Settings.Density, (f) =>
            {
                var density = Convert.ToInt32(f);
                densitySlider.parent.Find<UILabel>("Label").text = $"Decorations Density: {Convert.ToInt32(f).ToString()}";
                Settings.Density = density;
                Settings.Save();
                if (Manager.InGame) Manager.UpdateDensity(density);
            });
            densitySlider.scrollWheelAmount = 1f;
            var sprite = densitySlider.thumbObject as UISprite;
            sprite.spriteName = "InfoIconBaseHovered";
            sprite.size = new Vector2(10f, 10f); 
            densitySlider.height = 5f;
            helper.AddSpace(uiSpacing);

            cliffDropdown = (UIDropDown)helper.AddDropdown("Cliff", PackNames, SelectedCliffDropdownIndex, (index) =>
            {
                Settings.SelectedCliffPack = PackNames[index];
                Settings.Save();
                if (Manager.InGame)
                {
                    var cliffObject = new GameObject("CliffObject");
                    cliffObject.AddComponent<CliffLoader>();
                }
            });
            cliffDropdown.size = dropdownSize;
            helper.AddSpace(uiSpacing);

            fertileDropdown = (UIDropDown)helper.AddDropdown("Fertile", PackNames, SelectedFertileDropdownIndex, (index) =>
            {
                Settings.SelectedFertilePack = PackNames[index];
                Settings.Save();
                if (Manager.InGame)
                {
                    var fertileObject = new GameObject("FertileObject");
                    fertileObject.AddComponent<FertileLoader>();
                }
            });
            fertileDropdown.size = dropdownSize;
            helper.AddSpace(uiSpacing);

            grassDropdown = (UIDropDown)helper.AddDropdown("Grass", PackNames, SelectedGrassDropdownIndex, (index) =>
            {
                Settings.SelectedGrassPack = PackNames[index];
                Settings.Save();
                if (Manager.InGame)
                {
                    var grassObject = new GameObject("GrassObject");
                    grassObject.AddComponent<GrassLoader>();
                }
            });
            grassDropdown.size = dropdownSize;
            helper.AddSpace(uiSpacing);

            helper.AddButton("Load All", () =>
            {
                var cliffObject = new GameObject("CliffObject");
                var fertileObject = new GameObject("FertileObject");
                var grassObject = new GameObject("GrassObject");
                cliffObject.AddComponent<CliffLoader>();
                fertileObject.AddComponent<FertileLoader>();
                grassObject.AddComponent<GrassLoader>();
            });
        }        

        private string[] GetPackNames()
        {
            List<string> names = new List<string>();
            foreach (var pack in Manager.AvailablePacks)
            {
                names.Add(pack.Name);
            }
            names.Sort();
            return names.ToArray();
        }
    }
}
