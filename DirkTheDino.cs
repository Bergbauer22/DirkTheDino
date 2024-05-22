using MelonLoader;
using BTD_Mod_Helper;
using DirkTheDino;
using UnityEngine;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Unity.Display;
using Il2CppSystem.IO;
using BTD_Mod_Helper.Api.Display;
using Il2CppAssets.Scripts.Unity.UI_New.InGame;
using System.Collections.Generic;
using Il2CppAssets.Scripts.Models.Towers;
using System.Linq;
using HarmonyLib;
using BTD_Mod_Helper.Api;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models;
using Il2CppAssets.Scripts.Simulation.Objects;
using Il2CppAssets.Scripts;
using Il2CppAssets.Scripts.Simulation.Towers;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors.Abilities;
using Il2CppAssets.Scripts.Simulation.Towers.Weapons;
using UnityEngine.UI;
using Color = UnityEngine.Color;
using Il2CppAssets.Scripts.Simulation.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Unity.Audio;
using System;
using Il2CppAssets.Scripts.Simulation.Bloons.Behaviors;
using Il2Cpp;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;
using Il2CppInterop.Runtime.InteropTypes.Arrays;


[assembly: MelonInfo(typeof(DirkTheDino.DirkTheDino), ModHelperData.Name, ModHelperData.Version, ModHelperData.RepoOwner)]
[assembly: MelonGame("Ninja Kiwi", "BloonsTD6")]

namespace DirkTheDino;
[HarmonyPatch]
public class DirkTheDino : BloonsTD6Mod
{
    static int rnd(int min,int max)
    {
        System.Random random = new System.Random();
        return random.Next(min, max);
    }
    public override void OnAbilityCast(Ability ability)
    {
        //ModHelper.Msg<DirkTheDino>(ability.abilityModel.name);
        if (ability.abilityModel.name == "AbilityModel_EvilPalutenFly")
        {
            cooldownSelectSounds = 10;
            if (ability.tower.towerModel.name.Contains("DirkTheDino-DinoDirk"))
            {
                if(cooldownSelectSounds <= 4)
                {
                    RandomAbility1Sound();
                    cooldownSelectSounds = 15;
                }

                TimeSinceTransformUsed = 30;
                DirksXKord = ability.tower.Position.X;
                DirksYKord = ability.tower.Position.Y;
                DirksZKord = ability.tower.Position.Z;
                this.currentHeroEP = ability.tower.entity.GetBehavior<Hero>().xp;
                this.currentDamage = (int)ability.tower.damageDealt;
                this.Timer = 300;
                if (!ability.tower.isDestroyed)
                {
                    ability.tower.worth = 0;
                    InGame.Bridge.SellTower(ability.tower.Id);
                }
            }
        }
        if (ability.abilityModel.name == "AbilityModel_Drone")
        {
            if (cooldownSelectSounds <= 4)
            {
                RandomAbility2Sound();
                cooldownSelectSounds = 15;
            }

            if (ability.tower.towerModel.name.Contains("DirkTheDino-DinoDirk"))
            {
                this.currentDamage = (int)ability.tower.damageDealt;
                this.ActivatedTransform = true;
            }
            if(ability.tower.towerModel.name.Contains("DirkTheDino-DinoDirk") && ability.tower.towerModel.tiers.Max() >= 14)
            {
                maxForwardSpeed = maxForwardSpeed * 1.2f;
            }
        }
        if(ability.abilityModel.name == "AbilityModel_PortalGun")
        {
            if (cooldownSelectSounds <= 4)
            {
                RandomAbility3Sound();
                cooldownSelectSounds = 15;
            }
            RandomPortalSoundEffect();
            AfterPortalGunSound();
            cooldownSelectSounds = 15;
        }
    }

    public void SaveClipboard()
    {
        List<Tower> towers = InGameExt.GetTowers(InGame.instance, null);
        int TC = towers.Count;

        for (int i = 0; i < TC; i++)
        {
            if (towers[i] != null && !towers[i].isDestroyed)
            {
                if (towers[i].towerModel.name.Contains("DirkTheDino-DinoDirk"))
                {
                    clipboard = towers[i].towerModel;
                    if(towers[i].towerModel.tiers.Max() <= 2)
                    {
                        currentHeroEP = 0;
                    }
                }
            }
        }
    }
    public override void OnRoundStart()
    {
        SaveClipboard();
    }
    //public override void 
    public override void OnTowerUpgraded(Tower tower, string upgradeName, TowerModel newBaseTowerModel)
    {
        SaveClipboard();
        
        if (tower.towerModel.name.Contains("DirkTheDino-DinoDirk"))
        {
            UpgradeEffect(tower);
            cooldownSelectSounds = 10;
            UpgradedManuel = true;
        }

    }


    [HarmonyPatch(typeof(Overclock), nameof(Overclock.Activate))]
    internal static class Overclock_Activate
    {
        [HarmonyPrefix]
        private static bool Prefix(Overclock __instance)
        {
            if (!__instance.model.name.EndsWith("PortalGun") ||
                !__instance.Sim.towerManager.GetTowerById(__instance.selectedTowerId).Is(out var selectedTower))
                return true;
            var Dirk = __instance.ability.tower;
            var otherTower = selectedTower;
            float xDirkoldPos = Dirk.Position.X;
            float yDirkoldPos = Dirk.Position.Y;
            float zyDirkoldPos = Dirk.Position.Z;
            Dirk.PositionTower(new Il2CppAssets.Scripts.Simulation.SMath.Vector2(otherTower.Position.X, otherTower.Position.Y));
            Dirk.transform.position.Z = otherTower.Position.Z;
            otherTower.PositionTower(new Il2CppAssets.Scripts.Simulation.SMath.Vector2(xDirkoldPos, yDirkoldPos));
            otherTower.transform.position.Z = zyDirkoldPos;
            
            InGame.instance.bridge.simulation.SpawnEffect(ModContent.CreatePrefabReference<OrangePortal>(), new Il2CppAssets.Scripts.Simulation.SMath.Vector3(otherTower.Position.X, otherTower.Position.Y, otherTower.Position.Z + 7), 0, 12, 7);
            InGame.instance.bridge.simulation.SpawnEffect(ModContent.CreatePrefabReference<BluePortal>(), new Il2CppAssets.Scripts.Simulation.SMath.Vector3(Dirk.Position.X, Dirk.Position.Y, Dirk.Position.Z + 7), 0, 21, 7);
            return false;
        }
    }
    public static void PlaySound(string name)
    {
        Game.instance.audioFactory.PlaySoundFromUnity(null, name, "Voice", -1, 1.2f,0,false,"",false,false,true,Il2CppAssets.Scripts.Unity.Bridge.AudioType.VOICE);
    }
    public static void PlaySoundRoar(string name)
    {
        Game.instance.audioFactory.PlaySoundFromUnity(null, name, "Voice", -1, 0.6f, 1.2f, false, "", false, false, true, Il2CppAssets.Scripts.Unity.Bridge.AudioType.VOICE);
    }
    public static void PlaySoundPlace(string name)
    {
        Game.instance.audioFactory.PlaySoundFromUnity(null, name, "Voice", -1, 1.2f, 0, false, "", false, false, true, Il2CppAssets.Scripts.Unity.Bridge.AudioType.VOICE);
    }
    public static void PlaySoundDelayed(string name)
    {
        Game.instance.audioFactory.PlaySoundFromUnity(null, name, "Voice", -1, 1.2f, 3.7f, false, "", false, false, true, Il2CppAssets.Scripts.Unity.Bridge.AudioType.VOICE);
    }
    public static void PlayPortalGunSound(string name)
    {
        Game.instance.audioFactory.PlaySoundFromUnity(null, name, "Voice", -1, 0.6f, 0.7f, false, "", false, false, true, Il2CppAssets.Scripts.Unity.Bridge.AudioType.VOICE);
    }
    [HarmonyPatch(typeof(AudioFactory), "Start")]
    public class AudioFactoryStart_Patch
    {
        [HarmonyPostfix]
        public static void Postfix(AudioFactory __instance)
        {
            foreach (UnityEngine.Object asset in ModContent.GetBundle<DirkTheDino>("dirk_sounds").LoadAllAssetsAsync<AudioClip>().allAssets)
            {
                __instance.RegisterAudioClip(asset.name, asset.Cast<AudioClip>());
            }
        }
    }
    public void UpgradeEffect(Tower Dirk)
    {
        InGame.instance.bridge.simulation.SpawnEffect(ModContent.CreatePrefabReference<UpgradeEffect1>(), new Il2CppAssets.Scripts.Simulation.SMath.Vector3(Dirk.Position.X, Dirk.Position.Y, Dirk.Position.Z), 0,9,10);
        if (cooldownSelectSounds <= 6)
        {
            cooldownSelectSounds = 16;
            switch (Dirk.towerModel.tiers.Max())
            {
                case 2:
                    RandomBeastHandlerBuffSound();
                    break;
                case 6:
                    RandomBeastHandlerBuffSound();
                    break;
                case 3:
                    RandomAbility1Sound();
                    break;
                case 9:
                    RandomAbility1Sound();
                    break;
                case 10:
                    RandomAbility2Sound();
                    break;
                case 13:
                    RandomAbility3Sound();
                    break;
                case 5:
                    RandomElectroSound();
                    break;
                case 7:
                    RandomElectroSound();
                    break;
                case 4:
                    RandomElectroSound();
                    break;
                case 12:
                    RandomElectroSound();
                    break;
                case 15:
                    RandomAbility2Sound();
                    break;
                case 11:
                    RandomJetSound();
                    break;
                case 14:
                    RandomJetSound();
                    break;
                case 16:
                    RandomJetSound();
                    break;
                case 8:
                    PlaySound("DirkRoarSound");
                    break;
                case 17:
                    RandomIsabLoveSoundEffect();
                    break;
                case 18:
                    PlaySound("DirkRoarSound");
                    break;
                case 19:
                    DeadpoolSoundEffect();
                    break;
                    }
            }
    }
    public void RandomPortalSoundEffect()
    {
        int random1 = rnd(1, 7);
        while (LastPortalSound == random1)
        {
            random1 = rnd(1, 7);
        }
        LastPortalSound = random1;
        switch (random1)
        {
            case 1:
                PlayPortalGunSound("PortalGun1");
                break;
            case 2:
                PlayPortalGunSound("PortalGun2");
                break;
            case 3:
                PlayPortalGunSound("PortalGun3");
                break;
            case 4:
                PlayPortalGunSound("PortalGun4");
                break;
            case 5:
                PlayPortalGunSound("PortalGun5");
                break;
            case 6:
                PlayPortalGunSound("PortalGun6");
                break;
        }
    }
    public void RandomIsabLoveSoundEffect()
    {
        int random1 = rnd(1, 4);
        switch (random1)
        {
            case 1:
                PlayPortalGunSound("Common1_1");
                break;
            case 2:
                PlayPortalGunSound("Common1_2");
                break;
            case 3:
                PlayPortalGunSound("Common1_3");
                break;

        }
    }
    public void DeadpoolSoundEffect()
    {
        int random1 = rnd(1, 4);
        switch (random1)
        {
            case 1:
                PlayPortalGunSound("MaxLevel1_1");
                break;
            case 2:
                PlayPortalGunSound("MaxLevel1_2");
                break;
            case 3:
                PlayPortalGunSound("MaxLevel1_3");
                break;

        }
    }
    public void RandomElectroSound()
    {
        int random1 = rnd(1, 4);
        while (LastElectroSound == random1)
        {
            random1 = rnd(1, 4);
        }
        LastElectroSound = random1;
        int random2 = rnd(1, 3);
        int random3 = rnd(1, 4);
        int random4 = rnd(1, 5);
        switch (random1)
        {
            case 1:
                if (random2 == 1)
                {
                    PlaySound("EnergieOrb1_1");
                }
                else
                {
                    PlaySound("EnergieOrb1_2");
                }
                break;
            case 2:
                if (random3 == 1)
                {
                    PlaySound("EnergieOrb2_1");
                }
                else if (random3 == 1)
                {
                    PlaySound("EnergieOrb2_2");
                }
                else
                {
                    PlaySound("EnergieOrb2_3");
                }
                break;
            case 3:
                if (random2 == 1)
                {
                    PlaySound("EnergieOrb3_1");
                }
                else
                {
                    PlaySound("EnergieOrb3_2");
                }
                break;
        }
    }
    public void RandomJetSound()
    {
        int random1 = rnd(1, 4);
        while (LastJetSound == random1)
        {
            random1 = rnd(1, 4);
        }
        LastJetSound = random1;
        int random2 = rnd(1, 3);
        int random3 = rnd(1, 4);
        int random4 = rnd(1, 5);
        switch (random1)
        {
            case 1:
                if (random3 == 1)
                {
                    PlaySound("Jets1_1");
                }
                else if (random3 == 2)
                {
                    PlaySound("Jets1_2");
                }
                else
                {
                    PlaySound("Jets1_3");
                }
                break;
            case 2:
                if (random3 == 1)
                {
                    PlaySound("Jets2_1");
                }
                else if (random3 == 2)
                {
                    PlaySound("Jets2_2");
                }
                else
                {
                    PlaySound("Jets2_3");
                }
                break;
            case 3:
                if (random2 == 1)
                {
                    PlaySound("Jets3_1");
                }
                else
                {
                    PlaySound("Jets3_2");
                }
                break;
        }
    }
    public void RandomAbility1Sound()
    {
        int SoundsUnlocked = 4;
        int random1 = rnd(1, SoundsUnlocked + 1);
        while (LastVoicelineAB1 == random1)
        {
            random1 = rnd(1, SoundsUnlocked + 1);
        }
        LastVoicelineAB1 = random1;
        int random2 = rnd(1, 3);
        int random3 = rnd(1, 4);
        int random4 = rnd(1, 5);
        switch (random1)
        {
            case 1:
                if (random2 == 1)
                {
                    PlaySound("Transform1_1");
                }
                else
                {
                    PlaySound("Transform1_2");
                }
                break;
            case 2:
                if (random2 == 1)
                {
                    PlaySound("Transform2_1");
                }
                else
                {
                    PlaySound("Transform2_2");
                }
                break;

            case 3:
                if (random4 == 1)
                {
                    PlaySound("Transform3_1");
                }
                else if (random4 == 2)
                {
                    PlaySound("Transform3_2");
                }
                else if (random4 == 3)
                {
                    PlaySound("Transform3_3");
                }
                else
                {
                    PlaySound("Transform3_4");
                }
                break;

            case 4:
                if (random2 == 1)
                {
                    PlaySound("Transform4_1");
                }
                else
                {
                    PlaySound("Transform4_2");
                }
                break;
        }
    }

    public void RandomBeastHandlerBuffSound()
    {
        int random1 = rnd(1, 4);
        int random2 = rnd(1, 3);
        int random4 = rnd(1, 5);
        switch (random1)
        {
            case 1:
                if (random2 == 1)
                {
                    PlaySound("BeastHandlerBuff1_1");
                }
                else
                {
                    PlaySound("BeastHandlerBuff2_2");
                }
                break;
            case 2:
                if (random4 == 1)
                {
                    PlaySound("BeastHandlerBuff2_1");
                }
                else if (random4 == 2)
                {
                    PlaySound("BeastHandlerBuff2_2");
                }
                else if (random4 == 3)
                {
                    PlaySound("BeastHandlerBuff2_3");
                }
                else
                {
                    PlaySound("BeastHandlerBuff2_4");
                }
                break;

            case 3:
                if (random2 == 1)
                {
                    PlaySound("BeastHandlerBuff3_1");
                }
                else
                {
                    PlaySound("BeastHandlerBuff3_2");
                }
                break;
        }
    }


    public void RandomAbility2Sound()
    {
        int SoundsUnlocked = 3;
        int random1 = rnd(1, SoundsUnlocked + 1);
        while (LastVoicelineAB2 == random1)
        {
            random1 = rnd(1, SoundsUnlocked + 1);
        }
        LastVoicelineAB2 = random1;
        int random2 = rnd(1, 3);
        int random3 = rnd(1, 4);
        int random4 = rnd(1, 5);
        switch (random1)
        {
            case 1:
                if (random2 == 1)
                {
                    PlaySound("DroneAbility1_1");
                }
                else
                {
                    PlaySound("DroneAbility1_2");
                }
                break;

            case 2:
                if (random3 == 1)
                {
                    PlaySound("DroneAbility2_1");
                }
                else if (random3 == 2)
                {
                    PlaySound("DroneAbility2_2");
                }
                else
                {
                    PlaySound("DroneAbility2_3");
                }
                break;


            case 3:
                if (random3 == 1)
                {
                    PlaySound("DroneAbility3_1");
                }
                else if (random3 == 2)
                {
                    PlaySound("DroneAbility3_2");
                }
                else
                {
                    PlaySound("DroneAbility3_3");
                }
                break;
        }
    }
    public void RandomAbility3Sound()
    {
        int random2 = rnd(1, 3);
        if (random2 == 1)
        {
            PlaySound("PortalGun2_1");
        }
        else
        {
            PlaySound("PortalGun2_2");
        }
    }
    public void AfterPortalGunSound()
    {
        int SoundsUnlocked = 3;
        int random1 = rnd(1, SoundsUnlocked + 1);
        while (LastVoicelineAB3 == random1)
        {
            random1 = rnd(1, SoundsUnlocked + 1);
        }
        LastVoicelineAB3 = random1;
        int random2 = rnd(1, 3);
        int random3 = rnd(1, 4);
        int random4 = rnd(1, 5);
        switch (random1)
        {
            case 1:
                if (random3 == 1)
                {
                    PlaySoundDelayed("PortalGun1_1");
                }
                else if (random3 == 2)
                {
                    PlaySoundDelayed("PortalGun1_2");
                }
                else
                {
                    PlaySoundDelayed("PortalGun1_3");
                }
                break;
            case 2:
                if (random3 == 1)
                {
                    PlaySoundDelayed("PortalGun3_1");
                }
                else if (random3 == 2)
                {
                    PlaySoundDelayed("PortalGun3_2");
                }
                else
                {
                    PlaySoundDelayed("PortalGun3_3");
                }
                break;

            case 3:
                if (random2 == 1)
                {
                    PlaySoundDelayed("PortalGun4_1");
                }
                else
                {
                    PlaySoundDelayed("PortalGun4_2");
                }
                break;
        }
    }


    public void RandomPlaceSound()
        {
        int random1= rnd(1,6);
        int random2 = rnd(1, 3);
        int random3 = rnd(1, 4);
        int random4 = rnd(1, 5);
        ModHelper.Msg<DirkTheDino>(random1);
        switch (random1)
        {
            case 1:
                if(random3 == 1)
                {
                    PlaySoundPlace("Place1_1");
                }
                else if (random3 == 2)
                {
                    PlaySoundPlace("Place1_2");
                }
                else
                {
                    PlaySoundPlace("Place1_2");
                }
                
                break;
            case 2:
                if (random3 == 1)
                {
                    PlaySoundPlace("Place2_1");
                }
                else if (random3 == 2)
                {
                    PlaySoundPlace("Place2_2");
                }
                else
                {
                    PlaySoundPlace("Place2_3");
                }
                break;
            case 3:
                if (random2 == 1)
                {
                    PlaySoundPlace("Place3_1");
                }
                else
                {
                    PlaySoundPlace("Place3_2");
                }
                break;
            case 4:
                if (random4 == 1)
                {
                    PlaySoundPlace("Place4_1");
                }
                else if (random4 == 2)
                {
                    PlaySoundPlace("Place4_2");
                }
                else if (random4 == 3)
                {
                    PlaySoundPlace("Place4_3");
                }
                else
                {
                    PlaySoundPlace("Place4_4");
                }
                break;
            case 5:
                if (random2 == 1)
                {
                    PlaySoundPlace("Place5_1");
                }
                else
                {
                    PlaySoundPlace("Place5_2");
                }
                break;
        }
    }
    public override void OnTowerCreated(Tower tower, Entity target, Model modelToUse)
    {
        if (tower.towerModel.name.Contains("DirkTheDino-DinoDirk"))
        {
            clipboard = tower.towerModel;
            cooldownSelectSounds = 10;
            this.Timer = 250;
            tower.damageDealt += (long)this.currentDamage;
            this.currentDamage = 0;
            tower.entity.GetBehavior<Hero>().xp = this.currentHeroEP;
            this.currentHeroEP = 0f;
            if(tower.towerModel.tiers.Max() <= 3 && TimeSinceTransformUsed <= 0)
            {
                RandomPlaceSound();
            }  
        }
        if (tower.towerModel.name.Contains("DirkTheDino-DinoDirk"))
        {

        }
        if (tower.towerModel.name.Contains("DroneAttackDirk") && !this.isDrone && !tower.isDestroyed)
        {
            tower.Destroy();
        }
        else if (tower.towerModel.name.Contains("DroneAttackDirk") && this.isDrone)
        {
            TowerModelBehaviorExt.GetBehavior<AttackModel>(tower.towerModel).fireWithoutTarget = true;
        }
    }


    public override void OnMatchStart()
    {
        this.current_Y_Rotation = 180f;
        this.current_X_Rotation = 0f;
        this.current_Z_Rotation = 0f;
        this.current_Y_Pos = 75f;
        this.current_X_Pos = 130f;
        this.current_Z_Pos = 75f;
        SaveClipboard();
    }
    public override void OnRestart()
    {
        SaveClipboard();
    }

    public override void OnDefeat()
    {
        if (GameObjectExt.Exists<GameObject>(GameObject.Find("Scene")))
        {
            GameObject.Find("Scene").GetComponent<Camera>().enabled = true;
        }
        if (GameObjectExt.Exists<GameObject>(GameObject.Find("DirksCamTop(Clone)")))
        {
            GameObjectExt.Destroy(GameObject.Find("DirksCamTop(Clone)"));
        }
        this.current_Y_Rotation = 180f;
        this.current_X_Rotation = 0f;
        this.current_Z_Rotation = 0f;
        this.current_Y_Pos = 75f;
        this.current_X_Pos = 130f;
        this.current_Z_Pos = 75f;
    }

    public override void OnVictory()
    {
        if (GameObjectExt.Exists<GameObject>(GameObject.Find("Scene")))
        {
            GameObject.Find("Scene").GetComponent<Camera>().enabled = true;
        }
        if (GameObjectExt.Exists<GameObject>(GameObject.Find("DirksCamTop(Clone)")))
        {
            GameObjectExt.Destroy(GameObject.Find("DirksCamTop(Clone)"));
        }
        this.current_Y_Rotation = 180f;
        this.current_X_Rotation = 0f;
        this.current_Z_Rotation = 0f;
        this.current_Y_Pos = 75f;
        this.current_X_Pos = 130f;
        this.current_Z_Pos = 75f;
        
    }
    void ActivateAllChildren(Transform parentObject)
    {
        for(int i = 0; i < parentObject.childCount; i++)
        {
            parentObject.GetChild(i).gameObject.SetActive(true);
        }
    }
    
    void DeactivateAllChildren(Transform parentObject)
    {

        for (int i = 0; i < parentObject.childCount; i++)
        {
            parentObject.GetChild(i).gameObject.SetActive(false);
        }
    }
    public override void OnTitleScreen()
    {
        if (GameObjectExt.Exists<GameObject>(GameObject.Find("Scene")))
        {
            GameObject.Find("Scene").GetComponent<Camera>().enabled = true;
        }
        if (GameObjectExt.Exists<GameObject>(GameObject.Find("DirksCamTop(Clone)")))
        {
            GameObjectExt.Destroy(GameObject.Find("DirksCamTop(Clone)"));
        }
        this.current_Y_Rotation = 180f;
        this.current_X_Rotation = 0f;
        this.current_Z_Rotation = 0f;
        this.current_Y_Pos = 75f;
        this.current_X_Pos = 130f;
        this.current_Z_Pos = 75f;
    }
    public override void OnMatchEnd()
    {
        DestroyDrone();
    }
    public void DestroyDrone()
    {
        this.current_Y_Rotation = 180f;
        this.current_X_Rotation = 0f;
        this.current_Z_Rotation = 0f;
        this.current_Y_Pos = 75f;
        this.current_X_Pos = 130f;
        this.current_Z_Pos = 75f;
        this.Drone_Y_Rotation = 0f;
        if (GameObjectExt.Exists<GameObject>(GameObject.Find("Scene")))
        {
            GameObject.Find("Scene").GetComponent<Camera>().enabled = true;
        }
        if (GameObject.Find("DirksCamTop(Clone)").Exists())
        {
            GameObjectExt.Destroy(GameObject.Find("DirksCamTop(Clone)"));
        }
        DestroyAllDroneAttacks();
    }

    public void PlaceFireDroneAttack()
    {
        TowerModel attackDroneFire = ModelExt.Duplicate<TowerModel>(ModContent.GetTowerModel<DroneAttackModell1>(0, 0, 0));
        if (!InGame.Bridge.Simulation.towerInventories[InGame.Bridge.MyPlayerNumber].towerCounts.ContainsKey(attackDroneFire.baseId))
        {
            InGame.Bridge.Simulation.towerInventories[InGame.Bridge.MyPlayerNumber].towerCounts[attackDroneFire.baseId] = 0;
        }
        InGame.instance.GetTowerManager().CreateTower(attackDroneFire,new Il2CppAssets.Scripts.Simulation.SMath.Vector3(1,1,200), InGame.Bridge.MyPlayerNumber, ObjectId.FromData(1), ObjectId.FromData(4294967295));

    }
    public void PlaceIceDroneAttack()
    {
        TowerModel attackDroneIce = ModelExt.Duplicate<TowerModel>(ModContent.GetTowerModel<DroneAttackModell2>(0, 0, 0));
        if (!InGame.Bridge.Simulation.towerInventories[InGame.Bridge.MyPlayerNumber].towerCounts.ContainsKey(attackDroneIce.baseId))
        {
            InGame.Bridge.Simulation.towerInventories[InGame.Bridge.MyPlayerNumber].towerCounts[attackDroneIce.baseId] = 0;
        }
        InGame.instance.GetTowerManager().CreateTower(attackDroneIce, new Il2CppAssets.Scripts.Simulation.SMath.Vector3(1, 1, 200), InGame.Bridge.MyPlayerNumber, ObjectId.FromData(1), ObjectId.FromData(4294967295));

    }
    public override void OnRoundEnd()
    {
        List<Tower> towers = InGameExt.GetTowers(InGame.instance, null);
        int TC2 = towers.Count;
        for (int i = 0; i < TC2; i++)
        {
            if (towers[i] != null)
            {
                if (towers[i].towerModel.name.Contains("DroneAttackDirk"))
                {
                    if (!towers[i].isDestroyed)
                    {
                        int currentDirkLevel = towers[i].towerModel.tiers.Max();
                        if (LastLevel != currentDirkLevel && UpgradedManuel == false)
                        {
                            UpgradeEffect(towers[i]);
                        }
                        LastLevel = currentDirkLevel;
                        UpgradedManuel = false;
                    }
                }
            }
        }
    }
    public void PlaceAirDroneAttack()
    {
        TowerModel attackDroneAir = ModelExt.Duplicate<TowerModel>(ModContent.GetTowerModel<DroneAttackModell3>(0, 0, 0));
        if (!InGame.Bridge.Simulation.towerInventories[InGame.Bridge.MyPlayerNumber].towerCounts.ContainsKey(attackDroneAir.baseId))
        {
            InGame.Bridge.Simulation.towerInventories[InGame.Bridge.MyPlayerNumber].towerCounts[attackDroneAir.baseId] = 0;
        }
        InGame.instance.GetTowerManager().CreateTower(attackDroneAir, new Il2CppAssets.Scripts.Simulation.SMath.Vector3(1, 1, 200), InGame.Bridge.MyPlayerNumber, ObjectId.FromData(1), ObjectId.FromData(4294967295));
    }
    public void PlaceDrone1()
    {
        TowerModel attackDroneAir = ModelExt.Duplicate<TowerModel>(ModContent.GetTowerModel<Drone1_Stufe16>(0, 0, 0));
        if (!InGame.Bridge.Simulation.towerInventories[InGame.Bridge.MyPlayerNumber].towerCounts.ContainsKey(attackDroneAir.baseId))
        {
            InGame.Bridge.Simulation.towerInventories[InGame.Bridge.MyPlayerNumber].towerCounts[attackDroneAir.baseId] = 0;
        }
        InGame.instance.GetTowerManager().CreateTower(attackDroneAir, new Il2CppAssets.Scripts.Simulation.SMath.Vector3(-50, 0, 200), InGame.Bridge.MyPlayerNumber, ObjectId.FromData(1), ObjectId.FromData(4294967295));
    }
    public void PlaceDrone2()
    {
        TowerModel attackDroneAir = ModelExt.Duplicate<TowerModel>(ModContent.GetTowerModel<Drone2_Stufe16>(0, 0, 0));
        if (!InGame.Bridge.Simulation.towerInventories[InGame.Bridge.MyPlayerNumber].towerCounts.ContainsKey(attackDroneAir.baseId))
        {
            InGame.Bridge.Simulation.towerInventories[InGame.Bridge.MyPlayerNumber].towerCounts[attackDroneAir.baseId] = 0;
        }
        InGame.instance.GetTowerManager().CreateTower(attackDroneAir, new Il2CppAssets.Scripts.Simulation.SMath.Vector3(50, 0, 200), InGame.Bridge.MyPlayerNumber, ObjectId.FromData(1), ObjectId.FromData(4294967295));
    }
    public void DestroyAllDroneAttacks()
    {
        List<Tower> towers = InGameExt.GetTowers(InGame.instance, null);
        int TC2 = towers.Count;
        for (int i = 0; i < TC2; i++)
        {
            if (towers[i] != null)
            {
                if (towers[i].towerModel.name.Contains("DroneAttackDirk"))
                {
                    if (!towers[i].isDestroyed)
                    {
                        TowerExt.SellTower(towers[i]);
                    }
                }
            }
        }
    }
    static double CalculateAngle(float x1, float y1, float x2, float y2)
    {
        // Differenz in x- und y-Koordinaten berechnen
        double dx = x2 - x1;
        double dy = y2 - y1;
        // Winkel berechnen (in Grad)
        double angle = System.Math.Atan2(dy, dx) * (180 / System.Math.PI);
        // Umwandlung in den Bereich von 0 bis 360 Grad
        if (angle < 0)
        {
            angle += 360;
        }
        return angle;
    }

    static double GetAngleDifference(double angle1, double angle2)
    {
        // Differenz zwischen zwei Winkeln berechnen
        double difference = System.Math.Abs(angle1 - angle2);
        // Unterschied auf den Bereich von 0 bis 360 Grad begrenzen
        difference = difference % 360;
        // Unterschied in den Bereich von 0 bis 180 Grad umwandeln
        return difference;
    }
    static double CalculateDistance(double x1, double y1, double x2, double y2)
    {
        // Berechnung der Distanz nach dem Satz des Pythagoras
        double distance = System.Math.Sqrt(System.Math.Pow(x2 - x1, 2) + System.Math.Pow(y2 - y1, 2));
        return distance;
    }
    public override void OnUpdate()
    {   if(InGameData.CurrentGame != null && InGame.Bridge != null)
        {
            TimeSinceTransformUsed = TimeSinceTransformUsed - 0.1f;
            LastRandomSound = LastRandomSound - 0.1f;
            List<Tower> towers = InGame.instance.GetTowers();
            int TC = towers.Count;

            for (int i = 0; i < TC; i++)
            {
                if (towers[i] != null)
                {
                    if (towers[i].towerModel.name.Contains("DirkTheDino-DinoDirk"))
                    {
                        if (LastRandomSound <= 0 && rnd(1, 20000) == 3 && cooldownSelectSounds <= 0)
                        {
                            RandomSelectSound(towers[i].towerModel.tiers.Max());
                            cooldownSelectSounds = 30;
                            LastRandomSound = 190;
                        }
                    }        
                } 
            }
        }
        
        if (this.ActivatedTransform && InGameData.CurrentGame != null && InGame.Bridge != null)
        {
            
            
            this.ActivatedTransform = false;
            this.isDrone = !GameObject.Find("DirksCamTop(Clone)").Exists();
            if (this.isDrone)
            {
                GameObject.Find("Scene").GetComponent<Camera>().enabled = false;
                this.current_Y_Rotation = 180f;
                this.current_X_Rotation = 0f;
                this.current_Z_Rotation = 0f;
                this.current_Y_Pos = 75f;
                this.current_X_Pos = 130f;
                this.Drone_Y_Rotation = 0f;
                this.current_Z_Pos = 75f;
                if (DirkTheDino.MainAssetBundle == null)
                {
                    DirkTheDino.MainAssetBundle = ModContent.GetBundle<DirkTheDino>("dinodirk");
                }
                DirkTheDino.Drone = DirkTheDino.MainAssetBundle.LoadAsset("DirksCamTop").Cast<GameObject>();
                DirkTheDino.FlyDrone = GameObject.Instantiate<GameObject>(DirkTheDino.Drone);
                DirkTheDino.FlyDrone.transform.Translate(0f, 55f, 0f);
                DestroyAllDroneAttacks();
                if (current_Drone_Weapon == 0)
                {
                    ActivateAllChildren(GameObject.Find("Firethrower").transform);
                    DeactivateAllChildren(GameObject.Find("Icethrower").transform);
                    DeactivateAllChildren(GameObject.Find("Airblower").transform);
                    ActivateAllChildren(GameObject.Find("AttackOne").transform);
                    DeactivateAllChildren(GameObject.Find("AttackTwo").transform);
                    DeactivateAllChildren(GameObject.Find("AttackTree").transform);
                    PlaceFireDroneAttack();
                }
                else if (current_Drone_Weapon == 1)
                {
                    DeactivateAllChildren(GameObject.Find("Firethrower").transform);
                    ActivateAllChildren(GameObject.Find("Icethrower").transform);
                    DeactivateAllChildren(GameObject.Find("Airblower").transform);
                    DeactivateAllChildren(GameObject.Find("AttackOne").transform);
                    ActivateAllChildren(GameObject.Find("AttackTwo").transform);
                    DeactivateAllChildren(GameObject.Find("AttackTree").transform);
                    PlaceIceDroneAttack();
                }
                else if (current_Drone_Weapon == 2)
                {
                    DeactivateAllChildren(GameObject.Find("Firethrower").transform);
                    DeactivateAllChildren(GameObject.Find("Icethrower").transform);
                    ActivateAllChildren(GameObject.Find("Airblower").transform);
                    DeactivateAllChildren(GameObject.Find("AttackOne").transform);
                    DeactivateAllChildren(GameObject.Find("AttackTwo").transform);
                    ActivateAllChildren(GameObject.Find("AttackTree").transform);
                    PlaceAirDroneAttack();
                }
                
            }
            else
            {
                if (!this.isDrone)
                {
                    DestroyDrone();
                }
            }
        }
        if(InGame.instance != null && InGameData.CurrentGame != null && !GameObject.Find("DirksCamTop(Clone)").Exists() && InGame.Bridge != null)
        {

            TimeSinceDirkAway = TimeSinceDirkAway - 0.2f;
            cooldownSelectSounds = cooldownSelectSounds - 0.05f;
            if (TimeSinceDirkAway <= -1)
            {
                List<Tower> towersY = InGameExt.GetTowers(InGame.instance, null);
                int TCY = towersY.Count;
                for (int iY = 0; iY < TCY; iY++)
                {
                    if (towersY[iY] != null && !towersY[iY].isDestroyed)
                    {
                        if (towersY[iY].towerModel.name.Contains("Drone1"))
                        {
                            towersY[iY].Destroy();
                        }
                        if (towersY[iY].towerModel.name.Contains("Drone2"))
                        {
                            towersY[iY].Destroy();
                        }
                    }
                }
            }
            List<Tower> towers = InGame.instance.GetTowers();
            int TC3 = towers.Count;

            for (int i = 0; i < TC3; i++)
            {
                if (towers[i] != null)
                {
                    //Level18
                    if (towers[i].towerModel.name.Contains("DirkTheDino-DinoDirk") && towers[i].towerModel.tiers.Max() >= 18)
                    {
                        if (InGame.instance.GetHealth() < LastLiveCount)
                        {
                            TimeLastLiveLost = 30;
                        }
                        LastLiveCount = InGame.instance.GetHealth();
                        TimeLastLiveLost = TimeLastLiveLost -0.1f;
                        int LiveRegenMultiplier = 0;
                        if(InGame.instance.GetHealth() < 100)
                        {
                            LiveRegenMultiplier = 1;
                        }
                        else if (InGame.instance.GetHealth() < 300)
                        {
                            LiveRegenMultiplier = 3;
                        }
                        else if (InGame.instance.GetHealth() < 600)
                        {
                            LiveRegenMultiplier = 6;
                        }
                        else if (InGame.instance.GetHealth() < 1000)
                        {
                            LiveRegenMultiplier = 12;
                        }
                        for (int l = 0; l < LiveRegenMultiplier; l++)
                        {
                            if (InGame.instance.GetHealth() < 1000 && TimeLastLiveLost <= 0)
                            {
                                InGame.instance.AddHealth(1);
                            }
                        }  
                    }
                    if (towers[i].towerModel.name.Contains("DirkTheDino-DinoDirk") && towers[i].towerModel.tiers.Max() >= 15)
                    {
                        TimeSinceDirkAway = 30f;
                        //Spawn Drones_L15
                        List<Tower> towersD = InGame.instance.GetTowers();
                        int TCD = towers.Count;
                        bool Drone1_alreadyExist = false;
                        bool Drone2_alreadyExist = false;
                        for (int iD = 0; iD < TC3; iD++)
                        {
                            if (towersD[iD] != null)
                            {
                                if (towersD[iD].towerModel.name.Contains("Drone1"))
                                {
                                    Drone1_alreadyExist = true;
                                }
                                if (towersD[iD].towerModel.name.Contains("Drone2"))
                                {
                                    Drone2_alreadyExist = true;
                                }
                            }
                        }
                        if (!Drone1_alreadyExist)
                        {
                            PlaceDrone1();
                        }
                        if (!Drone2_alreadyExist)
                        {
                            PlaceDrone2();
                        }
                    }

                    //Level15
                    if (towers[i].towerModel.name.Contains("Drone1"))
                    {
                        List<Tower> towers5 = InGame.instance.GetTowers();
                        int TC5 = towers5.Count;

                        for (int i5 = 0; i5 < TC5; i5++)
                        {
                            if (towers5[i5] != null)
                            {
                                if (towers5[i5].towerModel.name.Contains("Drone2"))
                                {
                                    double Winkel = CalculateAngle(towers5[i5].Position.X + 500, towers[i].Position.Y + 500, towers[i].Position.X + 500, towers5[i5].Position.Y + 500);
                                    Winkel = Winkel + 90;
                                    double angleDifference = GetAngleDifference(towers[i].Rotation % 360, Winkel);
                                    for (int ix = 0; ix < 160; ix++)
                                    {
                                        if (angleDifference < 179.99)
                                        {
                                            towers[i].RotateTower((float)(-0.005), false, false);
                                        }
                                        else if (angleDifference > 180.01)
                                        {
                                            towers[i].RotateTower((float)(0.005), false, false);
                                        }
                                        angleDifference = GetAngleDifference(towers[i].Rotation % 360, Winkel);
                                    }
                                    double distance = CalculateDistance(towers5[i5].Position.X + 500, towers[i].Position.Y + 500, towers[i].Position.X + 500, towers5[i5].Position.Y + 500);
                                    //ModHelper.Msg<DirkTheDino>("AngleDifference: " + angleDifference + " Distanz: " + distance + " Lifetime: " + towers5[i5].towerModel.GetAttackModel().weapons[0].projectile.GetBehavior<TravelStraitModel>().lifespan);
                                    var newTowerModel = towers[i].towerModel.Duplicate();
                                    newTowerModel.GetAttackModel().weapons[0].projectile.GetBehavior<TravelStraitModel>().lifespan = (float)distance * 0.00134f * 0.55f + 0.015f;
                                    towers[i].UpdateRootModel(newTowerModel);

                                }
                            }
                        }
                        if (currentDrone == true)
                        {
                            if (Input.GetKeyDown(KeyCode.Tab))
                            {
                                currentDrone = false;
                                List<Tower> towers2 = InGame.instance.GetTowers();
                                int TC2 = towers2.Count;
                                for (int i2 = 0; i2 < TC2; i2++)
                                {
                                    if (towers2[i2] != null)
                                    {
                                        if (towers2[i2].towerModel.name.Contains("Drone2"))
                                        {
                                            Drone1Pos = new Il2CppAssets.Scripts.Simulation.SMath.Vector2(towers[i].Position.X, towers[i].Position.Y);
                                            towers[i].PositionTower(new Il2CppAssets.Scripts.Simulation.SMath.Vector2(towers2[i2].Position.X, towers2[i2].Position.Y));
                                            towers2[i2].PositionTower(Drone1Pos);
                                        }
                                    }
                                }
                            }
                            if (Input.GetKey(KeyCode.UpArrow) && towers[i].Position.Y >= -109)
                            {
                                towers[i].MoveTower(new Il2CppAssets.Scripts.Simulation.SMath.Vector2(0, -1));
                            }
                            if (Input.GetKey(KeyCode.DownArrow) && towers[i].Position.Y <= 114)
                            {
                                towers[i].MoveTower(new Il2CppAssets.Scripts.Simulation.SMath.Vector2(0, 1));
                            }
                            if (Input.GetKey(KeyCode.LeftArrow) && towers[i].Position.X >= -145)
                            {
                                towers[i].MoveTower(new Il2CppAssets.Scripts.Simulation.SMath.Vector2(-1, 0));
                            }
                            if (Input.GetKey(KeyCode.RightArrow) && towers[i].Position.X <= 145)
                            {
                                towers[i].MoveTower(new Il2CppAssets.Scripts.Simulation.SMath.Vector2(1, 0));
                            }
                        }
                        else if (currentDrone == false)
                        {
                            //towers[i].towerModel.display = ModContent.CreatePrefabReference<BlueDrone>();
                            if (Input.GetKeyDown(KeyCode.Tab))
                            {
                                currentDrone = true;



                                List<Tower> towers2 = InGame.instance.GetTowers();
                                int TC2 = towers2.Count;

                                for (int i2 = 0; i2 < TC2; i2++)
                                {
                                    if (towers2[i2] != null)
                                    {
                                        if (towers2[i2].towerModel.name.Contains("Drone2"))
                                        {
                                            Drone1Pos = new Il2CppAssets.Scripts.Simulation.SMath.Vector2(towers[i].Position.X, towers[i].Position.Y);
                                            towers[i].PositionTower(new Il2CppAssets.Scripts.Simulation.SMath.Vector2(towers2[i2].Position.X, towers2[i2].Position.Y));
                                            towers2[i2].PositionTower(Drone1Pos);
                                        }
                                    }
                                }
                            }

                            if (Input.GetKey(KeyCode.UpArrow) && towers[i].Position.Y >= -109)
                            {
                                towers[i].MoveTower(new Il2CppAssets.Scripts.Simulation.SMath.Vector2(0, -1));
                            }
                            if (Input.GetKey(KeyCode.DownArrow) && towers[i].Position.Y <= 114)
                            {
                                towers[i].MoveTower(new Il2CppAssets.Scripts.Simulation.SMath.Vector2(0, 1));
                            }
                            if (Input.GetKey(KeyCode.LeftArrow) && towers[i].Position.X >= -145)
                            {
                                towers[i].MoveTower(new Il2CppAssets.Scripts.Simulation.SMath.Vector2(-1, 0));
                            }
                            if (Input.GetKey(KeyCode.RightArrow) && towers[i].Position.X <= 145)
                            {
                                towers[i].MoveTower(new Il2CppAssets.Scripts.Simulation.SMath.Vector2(1, 0));
                            }
                        }
                    }
                    if (towers[i].towerModel.name.Contains("Drone2"))
                    {
                        List<Tower> towers2 = InGame.instance.GetTowers();
                        int TC2 = towers2.Count;

                        for (int i2 = 0; i2 < TC2; i2++)
                        {
                            if (towers2[i2] != null)
                            {
                                if (towers2[i2].towerModel.name.Contains("Drone1"))
                                {

                                    double Winkel = CalculateAngle(towers2[i2].Position.X + 500, towers[i].Position.Y + 500, towers[i].Position.X + 500, towers2[i2].Position.Y + 500);
                                    Winkel = Winkel + 90;
                                    double angleDifference = GetAngleDifference(towers[i].Rotation % 360, Winkel);
                                    for (int ix = 0; ix < 160; ix++)
                                    {
                                        if (angleDifference < 179.99)
                                        {
                                            towers[i].RotateTower((float)(-0.005), false, false);
                                        }
                                        else if (angleDifference > 180.01)
                                        {
                                            towers[i].RotateTower((float)(0.005), false, false);
                                        }
                                        angleDifference = GetAngleDifference(towers[i].Rotation % 360, Winkel);
                                    }
                                    double distance = CalculateDistance(towers2[i2].Position.X + 500, towers[i].Position.Y + 500, towers[i].Position.X + 500, towers2[i2].Position.Y + 500);
                                    //ModHelper.Msg<DirkTheDino>("AngleDifference: " + angleDifference + " Distanz: " + distance + " Lifetime: " + towers[i].towerModel.GetAttackModel().weapons[0].projectile.GetBehavior<TravelStraitModel>().lifespan);
                                    var newTowerModel = towers[i].towerModel.Duplicate();
                                    newTowerModel.GetAttackModel().weapons[0].projectile.GetBehavior<TravelStraitModel>().lifespan = (float)distance * 0.00134f * 0.55f + 0.015f;
                                    towers[i].UpdateRootModel(newTowerModel);
                                }
                            }
                        }
                    }
                }
            }    
        }
        if (InGame.instance != null && this.isDrone && GameObjectExt.Exists<GameObject>(GameObject.Find("DirksCamTop(Clone)")))
        {
            WeaponSwitchCooldown = WeaponSwitchCooldown - 0.1f;
            ActiveDrone = GameObject.Find("DirksCamTop(Clone)");
            if (ActiveDrone != null)
            {
                if (Input.GetKey((KeyCode)273))
                {
                    if(ActiveDrone.transform.position.y < 400f)
                    {
                        this.current_Y_Pos += 0.5f;
                    }
                }
                if (Input.GetKey((KeyCode)274))
                {
                    if (ActiveDrone.transform.position.y > 4f)
                    {
                        this.current_Y_Pos -= 0.5f;
                    }
                }
                if (Input.GetKey((KeyCode)276))
                {
                    this.current_Y_Rotation -= 1.8f;
                }
                if (Input.GetKey((KeyCode)275))
                {
                    this.current_Y_Rotation += 1.8f;
                }
                if (Input.GetKeyDown(KeyCode.Tab) && WeaponSwitchCooldown <= 0)
                {
                    WeaponSwitchCooldown = 3;
                    DestroyAllDroneAttacks();
                    current_Drone_Weapon++;
                    if(current_Drone_Weapon >= 3)
                    {
                        current_Drone_Weapon = 0;
                    }
                    if (current_Drone_Weapon == 0)
                    {
                        ActivateAllChildren(GameObject.Find("Firethrower").transform);
                        DeactivateAllChildren(GameObject.Find("Icethrower").transform);
                        DeactivateAllChildren(GameObject.Find("Airblower").transform);
                        ActivateAllChildren(GameObject.Find("AttackOne").transform);
                        DeactivateAllChildren(GameObject.Find("AttackTwo").transform);
                        DeactivateAllChildren(GameObject.Find("AttackTree").transform);
                        PlaceFireDroneAttack();
                    }
                    else if (current_Drone_Weapon == 1)
                    {
                        DeactivateAllChildren(GameObject.Find("Firethrower").transform);
                        ActivateAllChildren(GameObject.Find("Icethrower").transform);
                        DeactivateAllChildren(GameObject.Find("Airblower").transform);
                        DeactivateAllChildren(GameObject.Find("AttackOne").transform);
                        ActivateAllChildren(GameObject.Find("AttackTwo").transform);
                        DeactivateAllChildren(GameObject.Find("AttackTree").transform);
                        PlaceIceDroneAttack();
                    }
                    else if (current_Drone_Weapon == 2)
                    {
                        DeactivateAllChildren(GameObject.Find("Firethrower").transform);
                        DeactivateAllChildren(GameObject.Find("Icethrower").transform);
                        ActivateAllChildren(GameObject.Find("Airblower").transform);
                        DeactivateAllChildren(GameObject.Find("AttackOne").transform);
                        DeactivateAllChildren(GameObject.Find("AttackTwo").transform);
                        ActivateAllChildren(GameObject.Find("AttackTree").transform);
                        PlaceAirDroneAttack();
                    }
                }
                List<Tower> towers3 = InGame.instance.GetTowers();
                int TC3 = towers3.Count;
                for (int k = 0; k < TC3; k++)
                {
                    if (towers3[k] != null)
                    {
                        if (towers3[k].towerModel.name.Contains("DroneAttackDirk") && !towers3[k].isDestroyed)
                        {
                            if (towers3[k].Rotation + 3.5f < this.Drone_Y_Rotation + 180f)
                            {
                                towers3[k].RotateTower(0.11f, true, true);
                            }
                            if (towers3[k].Rotation - 3.5f > this.Drone_Y_Rotation + 180f)
                            {
                                towers3[k].RotateTower(-0.11f, true, true);
                            }
                            towers3[k].PositionTower(new Il2CppAssets.Scripts.Simulation.SMath.Vector2(ActiveDrone.transform.position.x, -ActiveDrone.transform.position.z), true);
                        }
                    }
                }
                GameObject.Find("Time").GetComponent<Text>().text = "Pos:  X: " +Mathf.Round(ActiveDrone.transform.position.x).ToString()+" Y: "+Mathf.Round(ActiveDrone.transform.position.y).ToString()+" Z: "+Mathf.Round(ActiveDrone.transform.position.z).ToString();
                ActiveDrone.transform.Translate(this.current_X_Pos, this.current_Y_Pos, this.current_Z_Pos);
                ActiveDrone.transform.Rotate(this.current_X_Rotation, this.current_Y_Rotation, this.current_Z_Rotation);
                this.Drone_Y_Rotation -= this.current_Y_Rotation;
                this.current_Y_Pos = 0f;
                this.current_Y_Rotation = 0f;
                this.current_Z_Pos = 0f;
                this.current_Z_Rotation = 0f;
                this.current_X_Pos = 0f;
                this.current_X_Rotation = 0f;
                if (Input.GetKey((KeyCode)306))
                {
                    this.accelerationTimer += Time.deltaTime;
                    if (this.accelerationTimer < this.accelerationTime)
                    {
                        this.currentForwardSpeed = Mathf.Lerp(0f, this.maxForwardSpeed, this.accelerationTimer / this.accelerationTime);
                    }
                    else
                    {
                        this.currentForwardSpeed = this.maxForwardSpeed;
                    }
                }
                else
                {
                    this.decelerationTimer += Time.deltaTime;
                    if (this.decelerationTimer < this.decelerationTime)
                    {
                        this.currentForwardSpeed = Mathf.Lerp(this.maxForwardSpeed, 0f, this.decelerationTimer / this.decelerationTime);
                    }
                    else
                    {
                        this.currentForwardSpeed = 0f;
                    }
                }
                DirkTheDino.ActiveDrone.transform.Translate(UnityEngine.Vector3.forward * this.currentForwardSpeed * Time.deltaTime);
                if (Input.GetKeyDown(KeyCode.LeftShift) || Input.GetKeyDown(KeyCode.RightShift))
                {
                    if (GameObjectExt.Exists<GameObject>(GameObject.Find("Scene")))
                    {
                        GameObject.Find("Scene").GetComponent<Camera>().enabled = true;
                    }
                    if (GameObjectExt.Exists<GameObject>(GameObject.Find("DirksCamTop(Clone)")))
                    {
                        GameObjectExt.Destroy(GameObject.Find("DirksCamTop(Clone)"));
                    }
                    this.current_Y_Rotation = 180f;
                    this.current_X_Rotation = 0f;
                    this.current_Z_Rotation = 0f;
                    this.current_Y_Pos = 75f;
                    this.current_X_Pos = 130f;
                    this.current_Z_Pos = 75f;
                    DestroyAllDroneAttacks();
                }
            }
        }
        if (this.Timer >= 1)
        {
            this.Timer--;
        }
        if (GameObjectExt.Exists<GameObject>(GameObject.Find("Paluten(Clone)")))
        {
            this.FrameCount++;
            if (this.FrameCount >= 2000)
            {
                GameObjectExt.Destroy(GameObject.Find("Paluten(Clone)"));
            }
        }
        if (InGameData.CurrentGame != null && this.Timer >= 175 && InGame.Bridge != null)
        {
            if (InGame.instance != null && this.Timer > 1)
            {
                List<Tower> towers4 = InGameExt.GetTowers(InGame.instance, null);
                int TC4 = towers4.Count;
                for (int l = 0; l < TC4; l++)
                {
                    if (towers4[l] != null)
                    {
                        if (towers4[l].towerModel.name.Contains("DirkTheDino-DinoDirk") && towers4[l].towerModel.tiers.Max() >= 3)
                        {
                            if (this.Timer >= 295)
                            {
                                if (GameObjectExt.Exists<GameObject>(GameObject.Find("Paluten(Clone)")))
                                {
                                    GameObject.Find("Paluten(Clone)").GetComponent<Animator>().SetTrigger("Paluten");
                                }
                                else
                                {
                                    if (!GameObjectExt.Exists<GameObject>(GameObject.Find("Paluten(Clone)")))
                                    {
                                        this.FrameCount = 0;
                                        if (DirkTheDino.MainAssetBundle == null)
                                        {
                                            DirkTheDino.MainAssetBundle = ModContent.GetBundle<DirkTheDino>("dinodirk");
                                        }
                                        DirkTheDino.FlyPalle = DirkTheDino.MainAssetBundle.LoadAsset("Paluten").Cast<GameObject>();
                                        DirkTheDino.SummonFP = GameObject.Instantiate<GameObject>(DirkTheDino.FlyPalle);
                                    }
                                }
                                this.Timer -= 6;
                            }
                        }
                    }
                }
                if (this.Timer >= 270 && this.Timer <= 280)
                {
                    //InGame.instance.bridge.CreateTowerAt(this.OldDirkPos, clipboard, default(ObjectId), true, DirkTheDino.action2, true, true, false);
                    InGame.instance.GetTowerManager().CreateTower(clipboard, new Il2CppAssets.Scripts.Simulation.SMath.Vector3(DirksXKord, DirksYKord, DirksZKord), InGame.Bridge.MyPlayerNumber, ObjectId.FromData(1), ObjectId.FromData(4294967295),null,true,false);
                    this.Timer -= 20;
                }
            }
        }
    }
    public static Shader? GetOutlineShader()
    {
        var superMonkey = GetVanillaAsset("Assets/Monkeys/DartMonkey/Graphics/SuperMonkey.prefab")?.Cast<GameObject>();
        if (superMonkey == null)
        {
            return null;
        }
        superMonkey.AddComponent<UnityDisplayNode>();
        var litOutlineShader = superMonkey.GetComponent<UnityDisplayNode>().GetMeshRenderer().material.shader;
        return litOutlineShader;
    }
    public static UnityEngine.Object? GetVanillaAsset(string name)
    {
        foreach (var assetBundle in AssetBundle.GetAllLoadedAssetBundles().ToArray())
        {
            if (assetBundle.Contains(name))
            {
                return assetBundle.LoadAsset(name);
            }
        }
        return null;
    }
    public override void OnWeaponFire(Weapon weapon)
    {
        if (weapon.attack.tower.towerModel.name.Contains("DirkTheDino-DinoDirk") && !weapon.attack.tower.activeMutation && this.Timer <= 200)
        {
            if (weapon.weaponModel.projectile.maxPierce == 500f)
            {
                weapon.attack.tower.Node.graphic.GetComponent<Animator>().SetTrigger("Hit");
            }
        }
    }
    public void RandomSelectSound(int currentLevel)
    {
        int SoundsUnlocked = 4;
        if (currentLevel >= 2)
        {
            SoundsUnlocked += 2;
        }
        if (currentLevel >= 3)
        {
            SoundsUnlocked += 1;
        }
        if (currentLevel >= 5)
        {
            SoundsUnlocked += 2;
        }
        if (currentLevel >= 7)
        {
            SoundsUnlocked += 1;
        }
        if (currentLevel >= 10)
        {
            SoundsUnlocked += 1;
        }
        if (currentLevel >= 11)
        {
            SoundsUnlocked += 3;
        }
        if (currentLevel >= 13)
        {
            SoundsUnlocked += 1;
        }
        if (currentLevel >= 19)
        {
            SoundsUnlocked += 1;
        }
        int random1 = rnd(1, SoundsUnlocked + 1);
        while(LastVoiceline == random1 || PreLastVoiceline == random1)
        {
            random1 = rnd(1, SoundsUnlocked + 1);
        }
        PreLastVoiceline = LastVoiceline;
        LastVoiceline = random1;
        int random2 = rnd(1, 3);
        int random3 = rnd(1, 4);
        int random4 = rnd(1, 5);
        //ModHelper.Msg<DirkTheDino>("Random1: " + random1 + " SoundsUnlocked: " + SoundsUnlocked +" R2: " + random2 + " R3: " + random3 + " R4: " + random4);
        switch (random1)
        {
            case 1:
                if (random2 == 1)
                {
                    PlaySound("Place5_1");
                }
                else
                {
                    PlaySound("Place5_2");
                }
                break;
            case 2:
                if (random3 == 1)
                {
                    PlaySound("Common1_1");
                }
                else if (random3 == 2)
                {
                    PlaySound("Common1_2");
                }
                else
                {
                    PlaySound("Common1_3");
                }
                break;
            case 3:
                if (random2 == 1)
                {
                    PlaySound("Place3_1");
                }
                else
                {
                    PlaySound("Place3_2");
                }
                break;
            case 4:
                if (random4 == 1)
                {
                    PlaySound("Place4_1");
                }
                else if (random4 == 2)
                {
                    PlaySound("Place4_2");
                }
                else if (random4 == 3)
                {
                    PlaySound("Place4_3");
                }
                else
                {
                    PlaySound("Place4_4");
                }
                break;
            case 5:
                PlaySound("BeastHandlerBuff1_1");
                break;
            case 6:
                if (random4 == 1)
                {
                    PlaySound("BeastHandlerBuff2_1");
                }
                else if (random4 == 2)
                {
                    PlaySound("BeastHandlerBuff2_2");
                }
                else if (random4 == 3)
                {
                    PlaySound("BeastHandlerBuff2_3");
                }
                else
                {
                    PlaySound("BeastHandlerBuff2_4");
                }

                break;
            case 7:
                if (random2 == 1)
                {
                    PlaySound("Transform2_1");
                }
                else
                {
                    PlaySound("Transform2_2");
                }
                break;
            case 8:
                if (random2 == 1)
                {
                    PlaySound("EnergieOrb1_1");
                }
                else
                {
                    PlaySound("EnergieOrb1_2");
                }
                break;
            case 9:
                if (random3 == 1)
                {
                    PlaySound("EnergieOrb2_1");
                }
                else if (random3 == 1)
                {
                    PlaySound("EnergieOrb2_2");
                }
                else
                {
                    PlaySound("EnergieOrb2_3");
                }
                break;
            case 10:
                if (random2 == 1)
                {
                    PlaySound("EnergieOrb3_1");
                }
                else
                {
                    PlaySound("EnergieOrb3_2");
                }
                break;
            case 11:
                if (random2 == 1)
                {
                    PlaySound("DroneAbility1_1");
                }
                else
                {
                    PlaySound("DroneAbility1_2");
                }
                break;
            case 12:
                if (random3 == 1)
                {
                    PlaySound("Jets1_1");
                }
                else if (random3 == 2)
                {
                    PlaySound("Jets1_2");
                }
                else
                {
                    PlaySound("Jets1_3");
                }
                break;
            case 13:
                if (random3 == 1)
                {
                    PlaySound("Jets2_1");
                }
                else if (random3 == 2)
                {
                    PlaySound("Jets2_2");
                }
                else
                {
                    PlaySound("Jets2_3");
                }
                break;
            case 14:
                if (random2 == 1)
                {
                    PlaySound("Jets3_1");
                }
                else
                {
                    PlaySound("Jets3_2");
                }
                break;

            case 15:
                if (random3 == 1)
                {
                    PlaySound("PortalGun3_1");
                }
                else if (random3 == 2)
                {
                    PlaySound("PortalGun3_2");
                }
                else
                {
                    PlaySound("PortalGun3_3");
                }
                break;
            case 16:
                if (random3 == 1)
                {
                    PlaySound("MaxLevel1_1");
                }
                else if (random3 == 2)
                {
                    PlaySound("MaxLevel1_2");
                }
                else
                {
                    PlaySound("MaxLevel1_3");
                }
                break;
        }
    }
    public override void OnTowerSelected(Tower tower)
    {
        if (tower.towerModel.name.Contains("DirkTheDino-DinoDirk"))
        {
            foreach (SkinnedMeshRenderer s in tower.Node.graphic.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                s.material.shader = DirkTheDino.GetOutlineShader();
                RendererExt.SetOutlineColor(s, new Color(1f, 1f, 1f));
            }
            if(cooldownSelectSounds <= 0)
            {
                cooldownSelectSounds = 18;
                RandomSelectSound(tower.towerModel.tiers.Max());
            }
        }
    }
    public override void OnTowerDeselected(Tower tower)
    {
        if (tower.towerModel.name.Contains("DirkTheDino-DinoDirk-20"))
        {
            foreach (SkinnedMeshRenderer s in tower.Node.graphic.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                s.material.shader = DirkTheDino.GetOutlineShader();
                RendererExt.SetOutlineColor(s, new Color(0.9490196f, 0.02745098f, 0.02745098f));
            }
        }
        else
        {
            if (tower.towerModel.name.Contains("DirkTheDino-DinoDirk"))
            {
                foreach (SkinnedMeshRenderer s2 in tower.Node.graphic.GetComponentsInChildren<SkinnedMeshRenderer>())
                {
                    s2.material.shader = DirkTheDino.GetOutlineShader();
                    RendererExt.SetOutlineColor(s2, new Color(0.15686275f, 0.28235295f, 0.5647059f));
                }
            }
        }
    }
    public override void OnApplicationStart()
    {
        ModHelper.Msg<DirkTheDino>("DirkTheDino loaded!");
    }
    public int Timer;
    public int LastElectroSound;
    public int LastPortalSound;
    public int LastBeastBuffSound;
    public int LastJetSound;
    public float DirksXKord;
    public float DirksYKord;
    public float DirksZKord;
    public int LastVoiceline;
    public float TimeSinceTransformUsed;
    public int PreLastVoiceline;
    public int LastVoicelineAB1;
    public int LastVoicelineAB2;
    public int LastVoicelineAB3;
    public Il2CppAssets.Scripts.Simulation.SMath.Vector2 Drone1Pos;
    private static TowerModel? clipboard;
    private float currentHeroEP;
    public int FrameCount;
    private int currentDamage;
    public static GameObject? SummonFP = null;
    public static GameObject? FlyPalle = null;
    public static GameObject? Drone = null;
    public static GameObject? ActiveDrone = null;
    public static GameObject? FlyDrone = null;
    public float current_Y_Rotation = 180f;
    public float current_X_Rotation = 0f;
    public float current_Z_Rotation = 0f;
    public float current_Y_Pos = 75f;
    public float DirkOldXPos;
    public float DirkOldYPos;
    public float current_X_Pos = 130f;
    public float TimeSinceDirkAway;
    public float current_Z_Pos = 75f;
    public float currentDroneRotation;
    public float maxForwardSpeed = 30f;
    public float accelerationTime = 4f;
    public float cooldownSelectSounds;
    public float decelerationTime = 0.8f;
    private float currentForwardSpeed = 0f;
    private float accelerationTimer = 0f;
    private float decelerationTimer = 0f;
    public float Drone_Y_Rotation;
    public int current_Drone_Weapon = 0;
    private static float timeSinceLastSpawn;
    public bool ActivatedTransform = false;
    public bool isDrone = false;
    public float WeaponSwitchCooldown = 3;
    public bool currentDrone = true;
    public static AssetBundle? MainAssetBundle;
    public float TimeLastLiveLost;
    public double LastLiveCount;
    public int LastLevel;
    public bool UpgradedManuel;
    public float LastRandomSound = 60;
    public class DirkDisplay : ModCustomDisplay
    {
        public override string AssetBundleName => "dinodirk";
        public override string PrefabName => "DinoDirkLevel_zero";
        public override float Scale => 2;
        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            foreach (SkinnedMeshRenderer s in node.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                s.material.shader = DirkTheDino.GetOutlineShader();
                RendererExt.SetOutlineColor(s, new Color(0.15686275f, 0.28235295f, 0.5647059f));
            }
        }
    }
    public class DirkDisplayL3 : ModCustomDisplay
    {
        public override string AssetBundleName => "dinodirk";
        public override string PrefabName => "DinoDirkLevel_three";
        public override float Scale => 2;
        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            foreach (SkinnedMeshRenderer s in node.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                s.material.shader = DirkTheDino.GetOutlineShader();
                RendererExt.SetOutlineColor(s, new Color(0.15686275f, 0.28235295f, 0.5647059f));
            }
        }
    }
    public class DirkDisplayL7 : ModCustomDisplay
    {
        public override string AssetBundleName => "dinodirk";
        public override string PrefabName => "DinoDirkLevel_seven";
        public override float Scale => 2;
        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            foreach (SkinnedMeshRenderer s in node.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                s.material.shader = DirkTheDino.GetOutlineShader();
                RendererExt.SetOutlineColor(s, new Color(0.15686275f, 0.28235295f, 0.5647059f));
            }
        }
    }
    public class DirkDisplayL11 : ModCustomDisplay
    {
        public override string AssetBundleName => "dinodirk";
        public override string PrefabName => "DinoDirkLevel_eleven";
        public override float Scale => 2;
        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            foreach (SkinnedMeshRenderer s in node.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                s.material.shader = DirkTheDino.GetOutlineShader();
                RendererExt.SetOutlineColor(s, new Color(0.15686275f, 0.28235295f, 0.5647059f));
            }
        }
    }
    public class DirkDisplayL15 : ModCustomDisplay
    {
        public override string AssetBundleName => "dinodirk";
        public override string PrefabName => "DinoDirkLevel_fifteen";
        public override float Scale => 2;
        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            foreach (SkinnedMeshRenderer s in node.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                s.material.shader = DirkTheDino.GetOutlineShader();
                RendererExt.SetOutlineColor(s, new Color(0.15686275f, 0.28235295f, 0.5647059f));
            }
        }
    }
    public class DirkDisplayL20 : ModCustomDisplay
    {
        public override string AssetBundleName => "dinodirk";
        public override string PrefabName => "DinoDirkLevel_twenty";
        public override float Scale => 2;
        public override void ModifyDisplayNode(UnityDisplayNode node)
        {
            foreach (SkinnedMeshRenderer s in node.GetComponentsInChildren<SkinnedMeshRenderer>())
            {
                s.material.shader = DirkTheDino.GetOutlineShader();
                RendererExt.SetOutlineColor(s, new Color(0.15686275f, 0.28235295f, 0.5647059f));
            }
        }
    }
}

