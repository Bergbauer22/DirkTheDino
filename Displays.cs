using BTD_Mod_Helper.Api;
using BTD_Mod_Helper.Api.Display;
using BTD_Mod_Helper.Extensions;
using Il2Cpp;
using Il2CppAssets.Scripts.Data.Bloons;
using Il2CppAssets.Scripts.Data;
using Il2CppAssets.Scripts.Unity.Display;
using Il2CppAssets.Scripts.Unity.Display.Animation;
using Il2CppNinjaKiwi.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Il2CppAssets.Scripts.Utils;
using Il2CppNinjaKiwi.Common.ResourceUtils;

namespace DirkTheDino
{
    public class BeastHandlerBuff : ModBuffIcon
    {
        public override string Icon => "DinoBuff";
        public override int MaxStackSize => 0;
    }
    
    public class EnergieBuff : ModBuffIcon
    {
        public override string Icon => "EnergieBuff";
        public override int MaxStackSize => 0;
    }
    public class Leer : ModDisplay
    {
        public override string BaseDisplay => Generic2dDisplay;
        public override void ModifyDisplayNode(UnityDisplayNode node)
        { Set2DTexture(node, "Leer"); }
    }
    public class TransformedProjectile : ModCustomDisplay
    {
        public override string AssetBundleName => "dinodirk";
        public override string PrefabName => "Projectile";
        public override float Scale => 0.2f;
    }
    public class OrangePortal : ModCustomDisplay {
        public override string AssetBundleName => "dinodirk";
        public override string PrefabName => "Orange_Portal";
    }
    public class UpgradeEffect1 : ModCustomDisplay
    {
        public override string AssetBundleName => "dinodirk";
        public override string PrefabName => "Lightning";
    }
    public class BluePortal : ModCustomDisplay
    {
        public override string AssetBundleName => "dinodirk";
        public override string PrefabName => "Blue_Portal";
        
    }
    public class VerySmallProjectile : ModCustomDisplay
    {
        public override string AssetBundleName => "dinodirk";
        public override string PrefabName => "ProjectileLigntning";
        public override float Scale => 0.4f;
    }
    public class TransformDisplay : ModCustomDisplay
    {
        public override string AssetBundleName => "dinodirk";
        public override string PrefabName => "PalutenTower";
        public override float Scale => 1f;
        
    }
    public class FlyingLigntning : ModCustomDisplay
    {
        public override string AssetBundleName => "dinodirk";
        public override string PrefabName => "ProjectileLigntning";
        public override float Scale => 0.5f;
    }
    public class FlyingLigntningV2 : ModCustomDisplay
    {
        public override string AssetBundleName => "dinodirk";
        public override string PrefabName => "ProjectileLigntning2";
        public override float Scale => 0.2f;
    }
    public class DroneKette : ModCustomDisplay
    {
        public override string AssetBundleName => "dinodirk";
        public override string PrefabName => "DroneKette";
        public override float Scale => 0.2f;
    }
    public class FlyingLigntningSphere : ModCustomDisplay
    {
        public override string AssetBundleName => "dinodirk";
        public override string PrefabName => "ProjectileLigntning";
        public override float Scale => 1f;
    }
    public class EnergieSphereL16 : ModCustomDisplay
    {
        public override string AssetBundleName => "dinodirk";
        public override string PrefabName => "EnergySphere_16";
        public override float Scale => 1f;
    }
    public class Jet : ModCustomDisplay
    {
        public override string AssetBundleName => "dinodirk";
        public override string PrefabName => "Jet";
        public override float Scale => 1f;
    }
    public class RedDrone : ModCustomDisplay
    {
        public override string AssetBundleName => "dinodirk";
        public override string PrefabName => "RedDrone";
        public override float Scale => 0.25f;
        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            foreach (SkinnedMeshRenderer s in node.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                s.material.shader = DirkTheDino.GetOutlineShader();
                RendererExt.SetOutlineColor(s, new Color(212, 11, 4));
            }
        }
    }
    public class BlueDrone : ModCustomDisplay
    {
        public override string AssetBundleName => "dinodirk";
        public override string PrefabName => "BlueDrone";
        public override float Scale => 0.25f;
        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            foreach (SkinnedMeshRenderer s in node.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                s.material.shader = DirkTheDino.GetOutlineShader();
                RendererExt.SetOutlineColor(s, new Color(4, 56, 212));
            }
        }
    }
    public class JetProjectile : ModCustomDisplay
    {
        public override string AssetBundleName => "dinodirk";
        public override string PrefabName => "JetProjectile";
        public override float Scale => 1f;
    }
    public class PinkLaserDisplay : ModDisplay
    {
        public override string BaseDisplay => Generic2dDisplay;
        public override float Scale => 0.5f;

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            Set2DTexture(node, "PinkLaserDisplay");
        }
    }
    public class ShortLighnnings : ModCustomDisplay
    {
        public override string AssetBundleName => "dinodirk";
        public override string PrefabName => "ShortLightningProjectile";
        public override float Scale => 0.2f;
    }


    public class ElectricShockDisplay2 : ModDisplay
    {
        private const string BaseOverlayType = "LaserShock";
        public static readonly string CustomOverlayType2 = "ElectricShock2";
        private static SerializableDictionary<string, BloonOverlayScriptable> OverlayTypes => GameData.Instance.bloonOverlays.overlayTypes;
        public override string Name => base.Name + "-" + overlayClass;
        public override PrefabReference BaseDisplayReference => OverlayTypes[BaseOverlayType].assets[overlayClass];
        protected readonly BloonOverlayClass overlayClass;

        public ElectricShockDisplay2() { }

        public ElectricShockDisplay2(BloonOverlayClass overlayClass)
        {
            this.overlayClass = overlayClass;
        }

        public override IEnumerable<ModContent> Load() => Enum.GetValues(typeof(BloonOverlayClass))
            .Cast<BloonOverlayClass>()
            .Select(bc => new ElectricShockDisplay2(bc));

        public override void Register()
        {
            base.Register();
            BloonOverlayScriptable electricShock;
            if (!OverlayTypes.ContainsKey(CustomOverlayType2))
            {
                electricShock = OverlayTypes[CustomOverlayType2] = ScriptableObject.CreateInstance<BloonOverlayScriptable>();
                electricShock.assets = new SerializableDictionary<BloonOverlayClass, PrefabReference>();
                electricShock.displayLayer = OverlayTypes[BaseOverlayType].displayLayer;
            }
            else
            {
                electricShock = OverlayTypes[CustomOverlayType2];
            }
            electricShock.assets[overlayClass] = CreatePrefabReference(Id);
        }

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            if (node.GetComponentInChildren<CustomSpriteFrameAnimator>())
            {
                Il2CppSystem.Collections.Generic.List<Sprite> frames2 = new Il2CppSystem.Collections.Generic.List<Sprite>();
                frames2.Add(GetSprite("PinkLaserEffect1"));
                frames2.Add(GetSprite("PinkLaserEffect2"));
                frames2.Add(GetSprite("PinkLaserEffect3"));
                frames2.Add(GetSprite("PinkLaserEffect4"));
                node.GetComponentInChildren<CustomSpriteFrameAnimator>().frames = frames2;
            }
            if (node.GetComponentInChildren<MeshRenderer>())
            {
                node.GetComponentInChildren<MeshRenderer>().SetMainTexture(GetTexture(CustomOverlayType2));
            }
        }
    }
    public class ElectricShockDisplay : ModDisplay
    {
        private const string BaseOverlayType = "LaserShock";
        public static readonly string CustomOverlayType = "ElectricShock";
        private static SerializableDictionary<string, BloonOverlayScriptable> OverlayTypes => GameData.Instance.bloonOverlays.overlayTypes;
        public override string Name => base.Name + "-" + overlayClass;
        public override PrefabReference BaseDisplayReference => OverlayTypes[BaseOverlayType].assets[overlayClass];
        protected readonly BloonOverlayClass overlayClass;

        public ElectricShockDisplay() { }

        public ElectricShockDisplay(BloonOverlayClass overlayClass)
        {
            this.overlayClass = overlayClass;
        }

        public override IEnumerable<ModContent> Load() => Enum.GetValues(typeof(BloonOverlayClass))
            .Cast<BloonOverlayClass>()
            .Select(bc => new ElectricShockDisplay(bc));

        public override void Register()
        {
            base.Register();
            BloonOverlayScriptable electricShock;
            if (!OverlayTypes.ContainsKey(CustomOverlayType))
            {
                electricShock = OverlayTypes[CustomOverlayType] = ScriptableObject.CreateInstance<BloonOverlayScriptable>();
                electricShock.assets = new SerializableDictionary<BloonOverlayClass, PrefabReference>();
                electricShock.displayLayer = OverlayTypes[BaseOverlayType].displayLayer;
            }
            else
            {
                electricShock = OverlayTypes[CustomOverlayType];
            }
            electricShock.assets[overlayClass] = CreatePrefabReference(Id);
        }

        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            if (node.GetComponentInChildren<CustomSpriteFrameAnimator>())
            {
                Il2CppSystem.Collections.Generic.List<Sprite> frames = new Il2CppSystem.Collections.Generic.List<Sprite>();
                frames.Add(GetSprite("BlueLaserEffect1"));
                frames.Add(GetSprite("BlueLaserEffect2"));
                frames.Add(GetSprite("BlueLaserEffect3"));
                frames.Add(GetSprite("BlueLaserEffect4"));
                node.GetComponentInChildren<CustomSpriteFrameAnimator>().frames = frames;
            }
            if (node.GetComponentInChildren<MeshRenderer>())
            {
                node.GetComponentInChildren<MeshRenderer>().SetMainTexture(GetTexture(CustomOverlayType));
            }
        }
    }
}
