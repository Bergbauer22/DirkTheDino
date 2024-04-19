using BTD_Mod_Helper.Api.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.TowerFilters;
using Il2CppAssets.Scripts.Models.Towers;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using System.Linq;
using BTD_Mod_Helper.Extensions;
using Il2CppAssets.Scripts.Unity;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Abilities.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2Cpp;
using BTD_Mod_Helper.Api;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using Il2CppAssets.Scripts.Models.Towers.Weapons.Behaviors;
using System.Threading;
using static Il2CppNinjaKiwi.LiNK.Endpoints.Payment_SteamGetIAPs;
using BTD_Mod_Helper.Api.Helpers;
using Il2CppAssets.Scripts.Models;
using static MelonLoader.MelonLogger;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Emissions;
using UnityEngine;
using Il2CppAssets.Scripts.Unity.Powers;
using static DirkTheDino.DirkTheDino;

namespace DirkTheDino;

public class Levels
{
    public class Level2 : ModHeroLevel<DinoDirk>
    {
        public override string Description => "Supporting his sons: Every Velociraptor/Tyrannosaurus Rex/Giganotosaurus recieves extra damage and attacks 5% faster";
        public override int Level => 2;
        public override void ApplyUpgrade(TowerModel towerModel)
        {
            var DamageBuff = new DamageSupportModel("Damage", true, 2f, "Damage", new Il2CppReferenceArray<TowerFilterModel>(new TowerFilterModel[]
            { new FilterInBaseTowerIdModel("FilterInBaseTowerIdModel_", new Il2CppStringArray(new[] { (TowerType.BeastHandler) })),new FilterInTowerUpgradeModel("FilterInTowerUpgradeModel_",0,3,0,true)}),
            true, true, 2000);
            var AttackSpeedBuff = new RateSupportModel("DirkBuff", 0.95f, true, "Rate:Support", true, 1, new Il2CppReferenceArray<TowerFilterModel>(new TowerFilterModel[]
            { new FilterInBaseTowerIdModel("FilterInBaseTowerIdModel_", new Il2CppStringArray(new[] { (TowerType.BeastHandler) })),new FilterInTowerUpgradeModel("FilterInTowerUpgradeModel_",0,3,0,true)}),
            "DirkBuff", "DinoBuff", false);
            if(towerModel.tiers.Max() >= 6)
            {
                AttackSpeedBuff = new RateSupportModel("DirkBuff", 0.9f, true, "Rate:Support", true, 1, new Il2CppReferenceArray<TowerFilterModel>(new TowerFilterModel[]
                            { new FilterInBaseTowerIdModel("FilterInBaseTowerIdModel_", new Il2CppStringArray(new[] { (TowerType.BeastHandler) })),new FilterInTowerUpgradeModel("FilterInTowerUpgradeModel_",0,3,0,true)}),
                            "DirkBuff", "DinoBuff", false);
            }
            if (towerModel.tiers.Max() >= 20)
            {
                AttackSpeedBuff = new RateSupportModel("DirkBuff", 0.75f, true, "Rate:Support", true, 1, new Il2CppReferenceArray<TowerFilterModel>(new TowerFilterModel[]
                            { new FilterInBaseTowerIdModel("FilterInBaseTowerIdModel_", new Il2CppStringArray(new[] { (TowerType.BeastHandler) })),new FilterInTowerUpgradeModel("FilterInTowerUpgradeModel_",0,3,0,true)}),
                            "DirkBuff", "DinoBuff", false);
            }
            AttackSpeedBuff.ApplyBuffIcon<BeastHandlerBuff>();
            DamageBuff.ApplyBuffIcon<BeastHandlerBuff>();
            AttackSpeedBuff.isGlobal = true;
            DamageBuff.isGlobal = true;
            towerModel.AddBehavior(DamageBuff);
            towerModel.AddBehavior(AttackSpeedBuff);
        }
    }
    public class Level3 : ModHeroLevel<DinoDirk>
    {
        public override string AbilityName => "AbilityModel_EvilPalutenFly";
        public override string AbilityDescription => "For a short time 5 towers can see how strong there dark site is...";
        public override string Description => "Support from his evil friend: For a short time 5 towers can see how strong the dark site is...";//
        public override int Level => 3;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.ApplyDisplay<DirkDisplayL3>();
            var oldTowerModel = towerModel;
            towerModel.AddBehavior(Game.instance.model.GetTowerFromId("Alchemist-050").GetBehaviors<AbilityModel>().First().Duplicate());
            var abilityModel = towerModel.GetAbility();
            abilityModel.maxActivationsPerRound = 1;
            abilityModel.GetBehavior<MorphTowerModel>().mutateSelf = false;
            abilityModel.GetBehavior<MorphTowerModel>().maxCost = 100000000f;
            abilityModel.GetBehavior<MorphTowerModel>().maxTowers = 5;
            abilityModel.GetBehavior<MorphTowerModel>().maxTier = 3;
            abilityModel.GetBehavior<MorphTowerModel>().lifespan = 20;
            abilityModel.icon = GetSpriteReference("DirkAbilityOne");
            abilityModel.GetBehavior<MorphTowerModel>().affectList = "DartMonkey, BoomerangMonkey, IceMonkey, GlueGunner, SniperMonkey, DartlingGunner, WizardMonkey, NinjaMonkey, Alchemist, Druid, EngineerMonkey,MortarMonkey";
            abilityModel.GetDescendants<AttackModel>().ForEach(range => range.range = 50f);
            abilityModel.GetDescendants<DamageModel>().ForEach(damage => damage.damage = 2f);
            abilityModel.cooldown = 120;
            abilityModel.sharedCooldown = true;
            abilityModel.GetDescendants<ProjectileModel>().ForEach(pierce => pierce.pierce = 3f);
            abilityModel.GetDescendants<ProjectileModel>().ForEach(aimbot => aimbot.AddBehavior(new TrackTargetWithinTimeModel("aimbot", 999999f, true, false, 144f, false, 99999999f, false, 3.47999978f, true)));
            abilityModel.GetDescendants<ProjectileModel>().ForEach(proj => proj.ApplyDisplay<TransformedProjectile>());
            abilityModel.GetBehavior<MorphTowerModel>().MorthTowerNotSelf.newTowerModel.GetDescendants<AttackModel>().ForEach(range => range.range = 50f);
            abilityModel.GetBehavior<MorphTowerModel>().morthTowerNotSelf.newTowerModel.GetDescendants<DamageModel>().ForEach(damage => damage.damage = 2f);
            abilityModel.GetBehavior<MorphTowerModel>().morthTowerNotSelf.newTowerModel.GetDescendants<ProjectileModel>().ForEach(pierce => pierce.pierce = 3f);
            abilityModel.GetBehavior<MorphTowerModel>().morthTowerNotSelf.newTowerModel.GetDescendants<ProjectileModel>().ForEach(proj => proj.ApplyDisplay<TransformedProjectile>());
            abilityModel.GetBehavior<MorphTowerModel>().secondaryTowerModel.range = 185f;
            abilityModel.GetBehavior<MorphTowerModel>().morthTowerNotSelf.newTowerModel.range = 50f;
            abilityModel.GetBehavior<MorphTowerModel>().morthTowerNotSelf.newTowerModel.GetDescendants<FilterInvisibleModel>().ForEach(filter => filter.isActive = false);
            abilityModel.GetBehavior<MorphTowerModel>().morthTowerNotSelf.newTowerModel.GetDescendants<AttackModel>().ForEach(filter => filter.attackThroughWalls = false);
            abilityModel.GetDescendants<TravelStraitModel>().ForEach(travels => travels.lifespan = 2);
            abilityModel.GetBehavior<MorphTowerModel>().morthTowerNotSelf.newTowerModel.GetDescendants<TravelStraitModel>().ForEach(travels => travels.lifespan = 4);
            abilityModel.GetBehavior<MorphTowerModel>().morthTowerNotSelf.newTowerModel.ApplyDisplay<TransformDisplay>();
            abilityModel.GetBehavior<MorphTowerModel>().resetOnDefeatScreen = true;
            abilityModel.GetBehavior<MorphTowerModel>().affectListArray = new Il2CppStringArray(new[] { TowerType.DartMonkey, TowerType.DartlingGunner });
            abilityModel.GetBehavior<MorphTowerModel>().mutatorId = "DirkTransform";
            abilityModel.name = "AbilityModel_EvilPalutenFly";
            abilityModel.displayName = "EvilPalutenFly";
            abilityModel.description = "For a short time 5 towers can see how strong there dark site is...";
            abilityModel.addedViaUpgrade = Id;
        }
    }
    public class Level4 : ModHeroLevel<DinoDirk>
    {
        public override string Description => "Dirk boosting his attack skills with energie trinks";//

        public override int Level => 4;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.range = 38;
            towerModel.GetAttackModel().range = towerModel.range;
            var w = towerModel.GetWeapon();
            w.rate = 0.45f;
            w.projectile.pierce = 2;
            var Fire = Game.instance.model.GetTower(TowerType.MortarMonkey, 0, 0, 2).GetAttackModel().weapons[0].projectile.GetBehavior<CreateProjectileOnExhaustFractionModel>().projectile.GetBehavior<AddBehaviorToBloonModel>();
            Fire.overlayType = ElectricShockDisplay2.CustomOverlayType2;
            towerModel.GetAttackModel().weapons[0].projectile.AddBehavior(Fire);
            towerModel.GetAttackModel().weapons[0].projectile.collisionPasses = new[] { -1, 0 };
        }
    }
    public class Level5 : ModHeroLevel<DinoDirk>
    {
        public override string Description => "Godzilla style: He's now able to so use a long range ligntning attack. He's now also targeting invisible bloons";

        public override int Level => 5;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            var Dart = Game.instance.model.GetTower(TowerType.DartMonkey);
            var Cookie = Dart.GetAttackModel().Duplicate();
            Cookie.weapons[0].projectile.AddBehavior(Game.instance.model.GetTowerFromId("Adora 20").GetAttackModel().weapons[0].projectile.GetBehavior<AdoraTrackTargetModel>().Duplicate());
            Cookie.weapons[0].rate = 3f;
            Cookie.weapons[0].projectile.GetDamageModel().damage = 1;
            Cookie.weapons[0].projectile.pierce = 5000;
            Cookie.weapons[0].projectile.ApplyDisplay<FlyingLigntning>();
            Cookie.weapons[0].projectile.GetBehavior<TravelStraitModel>().Lifespan = 8000.0f;
            Cookie.weapons[0].projectile.GetBehavior<TravelStraitModel>().Speed = 0.8f;
            Cookie.range = towerModel.range * 2.25f;
            Cookie.weapons[0].projectile.maxPierce = 10005;
            Cookie.weapons[0].ejectY += 25;
            towerModel.AddBehavior(Cookie);
            towerModel.GetDescendants<FilterInvisibleModel>().ForEach(filter => filter.isActive = false);
            if (towerModel.tiers.Max() >= 12)
            {
                Cookie.weapons[0].rate = 1f;
                Cookie.range = towerModel.range * 4.25f;
            }
            if (towerModel.tiers.Max() >= 17)
            {
                Cookie.weapons[0].projectile.GetDamageModel().damage = 3;
            }
        }
    }
    public class Level6 : ModHeroLevel<DinoDirk>
    {
        public override string Description => "Every Tyrannosaurus Rex and Giganotosaurus gains a 20% ability reducten. They also attacks 10% faster";

        public override int Level => 6;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            var ab = new AbilityCooldownScaleSupportModel("DirkAbiliBuff", true, 0.1f, false, true, new Il2CppReferenceArray<TowerFilterModel>(new TowerFilterModel[]
            { new FilterInBaseTowerIdModel("FilterInBaseTowerIdModel_", new Il2CppStringArray(new[] { (TowerType.BeastHandler) })),new FilterInTowerUpgradeModel("FilterInTowerUpgradeModel_",0,3,0,true)}), "DirkAbiliBuff", "none");
            ab.isGlobal = true;
            ab.customRadius = 999;
            ab.isCustomRadius = true;
            towerModel.AddBehavior(ab);
        }
    }
    public class Level7 : ModHeroLevel<DinoDirk>
    {
        public override string Description => "Dirk summons a flying Energy Ball";

        public override int Level => 7;

        public override void ApplyUpgrade(TowerModel towerModel)
        {

            towerModel.ApplyDisplay<DirkDisplayL7>();
            var attackModel = towerModel.GetAttackModel();
            var weaponModel = attackModel.weapons[0];
            var projectileModel = weaponModel.projectile;
            var damageModel = projectileModel.GetDamageModel();
            var DroneAttack3 = Game.instance.model.GetTower(TowerType.Etienne).GetBehavior<DroneSupportModel>().Duplicate();
            var airModel = DroneAttack3;
            airModel.droneModel.GetBehavior<AirUnitModel>().display = ModContent.CreatePrefabReference<FlyingLigntningSphere>();
            airModel.droneModel.GetAttackModel().weapons[0].Rate = 0.5f;
            airModel.droneModel.dontAddMutatorsFromParent = false;
            airModel.droneModel.footprint.doesntBlockTowerPlacement = true;
            airModel.droneModel.GetAttackModel().weapons[0].projectile.ApplyDisplay<ShortLighnnings>();
            airModel.droneModel.range = towerModel.range * 2;
            airModel.droneModel.GetAttackModel().range = towerModel.range * 2;
            airModel.droneModel.GetAttackModel().GetBehavior<PursuitSettingCustomModel>().mustBeInRangeOfParent = false;
            FilterModel[] filterModel = { new FilterInvisibleModel("FilterInvisibleModel_", false, false) };
            airModel.droneModel.GetAttackModel().GetBehavior<AttackFilterModel>().filters = new Il2CppReferenceArray<FilterModel>(filterModel);
            var LaserShock = Game.instance.model.GetTower(TowerType.DartlingGunner, 2, 0, 0).GetDescendant<AddBehaviorToBloonModel>().Duplicate();
            LaserShock.overlayType = ElectricShockDisplay.CustomOverlayType;
            airModel.droneModel.GetAttackModel().weapons[0].projectile.AddBehavior(LaserShock);
            airModel.droneModel.GetAttackModel().weapons[0].projectile.GetBehavior<AddBehaviorToBloonModel>().filters = null;
            airModel.droneModel.GetAttackModel().weapons[0].projectile.pierce = 2;
            int[] collPass = { -1, 0, 1 };
            airModel.droneModel.GetAttackModel().weapons[0].projectile.collisionPasses = new Il2CppStructArray<int>(collPass);
            airModel.droneModel.GetAttackModel().weapons[0].GetDescendants<FilterInvisibleModel>().ForEach(model => model.isActive = false);
            airModel.droneModel.GetAttackModel().weapons[0].projectile.GetDamageModel().immuneBloonProperties = BloonProperties.Purple;
            airModel.droneModel.GetAttackModel().weapons[0].ejectY += 5;
            airModel.droneModel.GetAttackModel().weapons[0].projectile.maxPierce = 777;
            airModel.count = 1;
            if (towerModel.tiers.Max() >= 12 && towerModel.tiers.Max() < 17)
            {
                airModel.count = 3;
                airModel.droneModel.GetAttackModel().weapons[0].Rate = 0.7f;
                airModel.droneModel.GetAttackModel().weapons[0].projectile.AddBehavior(Game.instance.model.GetTowerFromId("Adora 20").GetAttackModel().weapons[0].projectile.GetBehavior<AdoraTrackTargetModel>().Duplicate());
                airModel.droneModel.GetAttackModel().weapons[0].projectile.ApplyDisplay<FlyingLigntningV2>();
                airModel.droneModel.GetAttackModel().weapons[0].projectile.GetBehavior<TravelStraitModel>().Speed *= 3f;
            }

            towerModel.AddBehavior(airModel);

            if (towerModel.tiers.Max() >= 16)
            {
                towerModel.RemoveBehavior<DroneSupportModel>();
            }
        }
    }
    public class Level8 : ModHeroLevel<DinoDirk>
    {
        public override string Description => "Stronger fists push bloons aside";

        public override int Level => 8;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            WeaponModel w = TowerModelExt.GetWeapon(towerModel);
            ProjectileModelBehaviorExt.AddBehavior<WindModel>(w.projectile, new WindModel("WindModel_", 2f, 8f, 0.9f, true, null, 0.1f, "Moabs", 0.2f));
            var slowModel = ModelExt.Duplicate<SlowModel>(Game.instance.model.GetTower("BombShooter", 5, 0, 2).GetDescendant<SlowModel>());
            slowModel.lifespan = 3;
            slowModel.multiplier = 0.75f;
            
        }
    }
    public class Level9 : ModHeroLevel<DinoDirk>
    {
        public override string Description => "Dirk's total transformation clone is now able to transform up to 7 towers. He also doubled their piere";

        public override int Level => 9;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            AbilityModel abilityModel = TowerModelExt.GetAbility(towerModel);
            AbilityModelBehaviorExt.GetBehavior<MorphTowerModel>(abilityModel).maxTowers = 7;
            if(towerModel.tiers.Max() >= 12)
            {
                AbilityModelBehaviorExt.GetBehavior<MorphTowerModel>(abilityModel).maxTowers = 10;
            }
            Il2CppGenericIEnumerable.ForEach<ProjectileModel>(abilityModel.GetDescendants<ProjectileModel>(), delegate (ProjectileModel pierce)
            {
                pierce.pierce = 6f;
            });
            if (towerModel.tiers.Max() >= 20)
            {
                AbilityModelBehaviorExt.GetBehavior<MorphTowerModel>(abilityModel).maxTowers = 20;
                Il2CppGenericIEnumerable.ForEach<ProjectileModel>(abilityModel.GetDescendants<ProjectileModel>(), delegate (ProjectileModel pierce)
                {
                    pierce.pierce = 12f;
                });
            }
            Il2CppGenericIEnumerable.ForEach<ProjectileModel>(AbilityModelBehaviorExt.GetBehavior<MorphTowerModel>(abilityModel).morthTowerNotSelf.newTowerModel.GetDescendants<ProjectileModel>(), delegate (ProjectileModel pierce)
            {
                pierce.pierce = 3f;
            });
        }
    }
    public class Level10 : ModHeroLevel<DinoDirk>
    {
        public override string Description => "Etienne's drone V.2: Launch your drone to terminate bloons in the sky";

        public override int Level => 10;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            TowerModelBehaviorExt.AddBehavior<AbilityModel>(towerModel, new AbilityModel("AbilityModel_Drone", "Spawn Drone", "s", 1, 0f, base.GetSpriteReference("Middle_4-Icon"), 15f, null, false, false, null, 0f, 0, 9999999, false, false, true, false, false, false, false, false)
            {
                name = "AbilityModel_Drone",
                displayName = "DroneSpawn",
                icon = base.GetSpriteReference("DirkAbilityTwo"),
                description = "For a short time 5 towers can see how strong there dark site is...",
                addedViaUpgrade = base.Id,
                sharedCooldown = true
            });
        }
    }
    public class Level11 : ModHeroLevel<DinoDirk>
    {
        public override string Description => "Master developer: Dirk controlls 2 jets with his mind";

        public override int Level => 11;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.ApplyDisplay<DirkDisplayL11>();
            //BuccaneerLesserPlane was old name
            var jet = Game.instance.model.GetTowerFromId("BuccaneerPlane").Duplicate();
            jet.isSubTower = true;
            jet.baseId = "BadorJet";
            jet.name = "BadorJet";
            jet.GetDescendant<FighterMovementModel>().maxSpeed *= 1.3f;
            jet.GetDescendant<FighterMovementModel>().loopTimeBeforeNext *= 1.25f;
            jet.GetBehavior<TowerExpireOnParentUpgradedModel>().parentTowerUpgradeTier = towerModel.tiers.Max();
            jet.GetAttackModel(1).RemoveWeapon(jet.GetAttackModel(1).weapons[0]);
            jet.GetAttackModel(2).weapons[0].projectile.ApplyDisplay<JetProjectile>();
            jet.GetAttackModel(2).weapons[0].projectile.GetBehavior<TravelStraitModel>().Speed = 0.1f;
            jet.GetAttackModel(2).weapons[0].rate *= 1.5f;
            jet.GetAttackModel(0).weapons[0].projectile.GetDamageModel().damage = towerModel.tiers.Max() * 0.25f - 1.75f;
            jet.GetAttackModel(0).weapons[0].projectile.pierce = 2;
            jet.GetDescendants<FilterInvisibleModel>().ForEach(filter => filter.isActive = false);
            jet.GetAttackModel(0).weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("DamageModifierForTagModel_Ceramic", "Ceramic",
                1, 2, false, false));
            jet.GetAttackModel(0).weapons[0].projectile.ApplyDisplay<PinkLaserDisplay>();
            var LaserShock = Game.instance.model.GetTower(TowerType.DartlingGunner, 2, 0, 0).GetDescendant<AddBehaviorToBloonModel>().Duplicate();
            LaserShock.lifespan = towerModel.tiers.Max() * 0.5f - 4;
            LaserShock.filters = null;
            LaserShock.overlayType = ElectricShockDisplay.CustomOverlayType;
            int[] collPass = { -1, 0, 1 };
            jet.GetAttackModel(0).weapons[0].projectile.collisionPasses = new Il2CppStructArray<int>(collPass);
            jet.GetAttackModel(0).weapons[0].projectile.AddBehavior(LaserShock);
            jet.GetBehavior<AirUnitModel>().display = CreatePrefabReference<Jet>();
            var spawnerModel = Game.instance.model.GetTower(TowerType.MonkeyBuccaneer, pathOneTier: 4).Duplicate().GetAttackModels()[1];
            var subtowerFilter = spawnerModel.GetDescendant<SubTowerFilterModel>();
            subtowerFilter.baseSubTowerId = "BadorJet";
            subtowerFilter.maxNumberOfSubTowers = 2;
            var createTowerModel = spawnerModel.GetDescendant<CreateTowerModel>();
            if (towerModel.tiers.Max() >= 14)
            {
                var Bomber = Game.instance.model.GetTower(TowerType.MonkeyAce, 0, 3, 0).GetAttackModels()[1];
                jet.AddBehavior(Bomber);
            }
            createTowerModel.tower = jet;
            towerModel.AddBehavior(spawnerModel);
            if (towerModel.tiers.Max() >= 17)
            {
                jet.GetAttackModel(0).weapons[0].rate *= 0.8f;
                var Energie = Game.instance.model.GetTower(TowerType.TackShooter).GetAttackModel().weapons[0].projectile.Duplicate();
                Energie.pierce = 1;
                Energie.GetBehavior<TravelStraitModel>().Lifespan *= 0.5f;
                Energie.GetDescendants<FilterInvisibleModel>().ForEach(model => model.isActive = false);
                Energie.GetDamageModel().damage = 1;
                Energie.ApplyDisplay<DroneKette>();
                var fragmentModel = new CreateProjectileOnContactModel("aaa", Energie, new ArcEmissionModel("ArcEmissionModel_", 6, 0, 360, null, true, false), true, false, false)
                { name = "RifleShrapnel_" };
                jet.GetAttackModel().weapons[0].projectile.AddBehavior(fragmentModel);
            }
            if (towerModel.tiers.Max() >= 20)
            {
                jet.GetAttackModel(2).weapons[0].rate = 1;
                jet.GetAttackModel(0).weapons[0].rate *= 0.7f;
                subtowerFilter.maxNumberOfSubTowers = 3;
                jet.GetDescendant<FighterMovementModel>().maxSpeed *= 1.2f;
            }
        }
    }
    public class Level12 : ModHeroLevel<DinoDirk>
    {
        public override string Description => "Energie Shock: Every energy based attack is way better! Also he's transforming up to 10 Towers with his transformation ability";

        public override int Level => 12;

        public override void ApplyUpgrade(TowerModel towerModel)
        {

        }
    }
    public class Level13 : ModHeroLevel<DinoDirk>
    {
        public override string Description => "Portalgun: Switch your position with another Tower of your choice";

        public override int Level => 13;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            var entangle = Game.instance.model.GetTower(TowerType.EngineerMonkey, 0, 4).GetDescendant<OverclockModel>();
            towerModel.AddBehavior(new AbilityHelper("PortalGun")
            {
                IconReference = GetSpriteReference("DirkAbilityTree"),
                CanActivateBetweenRounds = true,
                Cooldown = 50,
                AddedViaUpgrade = Id,
                SharedCooldown = true,
                Behaviors = new Model[]
                    {
                    entangle.Duplicate("PortalGun")
                    }
            });
        }
    }
    public class Level14 : ModHeroLevel<DinoDirk>
    {
        public override string Description => "Terminating enginiering skills: Dirk upgrades his drone and adds new weapons to his jets";

        public override int Level => 14;

        public override void ApplyUpgrade(TowerModel towerModel)
        {

        }
    }
    public class Level15 : ModHeroLevel<DinoDirk>
    {
        public override string Description => "EnergieCores: Press tab to switch your current drone and control the position with your arrow keys";

        public override int Level => 15;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.ApplyDisplay<DirkDisplayL15>();
        }
    }
    public class Level16 : ModHeroLevel<DinoDirk>
    {
        public override string Description => "Rise of his power: Every non paragon tower recieves an energie buff(+15% Attack Speed)";

        public override int Level => 16;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            var BuffEnergie = new RateSupportModel("EnergieBuff", 0.85f, true, "Rate:Support", true, 1, null, "EnergieBuff", "Energie_Buff", false);
            BuffEnergie.ApplyBuffIcon<EnergieBuff>();
            BuffEnergie.isGlobal = true;
            towerModel.AddBehavior(BuffEnergie);
        }
    }
    public class Level17 : ModHeroLevel<DinoDirk>
    {
        public override string Description => "Overpowering himself: Energie can now spring to other bloons and his 3 energie balls combines into 1 Mega-Energie-Ball";

        public override int Level => 17;

        public override void ApplyUpgrade(TowerModel towerModel)
        {

            var LaserShock = Game.instance.model.GetTower(TowerType.DartlingGunner, 2, 0, 0).GetDescendant<AddBehaviorToBloonModel>().Duplicate();
            LaserShock.lifespan = towerModel.tiers.Max() * 0.5f - 4;
            LaserShock.filters = null;
            LaserShock.overlayType = ElectricShockDisplay.CustomOverlayType;
            int[] collPass = { -1, 0, 1 };


            towerModel.GetAttackModel(0).weapons[0].projectile.pierce += 2;
            var Energie3 = Game.instance.model.GetTower(TowerType.BoomerangMonkey,0,0,3).GetAttackModel().weapons[0].projectile.Duplicate();
            Energie3.pierce = 3;
            Energie3.GetDescendants<FilterInvisibleModel>().ForEach(model => model.isActive = false);
            Energie3.GetDamageModel().damage = 2;
            Energie3.ApplyDisplay<DroneKette>();
            Energie3.AddBehavior(new DamageModifierForTagModel("DamageModifierForTagModel_Ddt", "Ddt", 1, 5, false, false));
            var fragmentModel3 = new CreateProjectileOnContactModel("aaa", Energie3, new ArcEmissionModel("ArcEmissionModel_", 5, 0, 360, null, true, false), true, false, false)
            { name = "RifleShrapnel_" };
            towerModel.GetAttackModel().weapons[0].projectile.AddBehavior(fragmentModel3);
            var DroneAttack = Game.instance.model.GetTower(TowerType.Etienne).GetBehavior<DroneSupportModel>().Duplicate();
            var airModel2 = DroneAttack;
            FilterModel[] filterModel = { new FilterInvisibleModel("FilterInvisibleModel_", false, false) };
            airModel2.droneModel.GetAttackModel().weapons[0].projectile.AddBehavior(Game.instance.model.GetTowerFromId("Adora 20").GetAttackModel().weapons[0].projectile.GetBehavior<AdoraTrackTargetModel>().Duplicate());
            airModel2.droneModel.GetAttackModel().weapons[0].projectile.GetBehavior<TravelStraitModel>().Speed *= 4.5f;
            airModel2.droneModel.GetBehavior<AirUnitModel>().display = ModContent.CreatePrefabReference<EnergieSphereL16>();
            airModel2.droneModel.GetAttackModel().weapons[0].Rate = 0.7f;
            airModel2.droneModel.dontAddMutatorsFromParent = false;
            airModel2.droneModel.footprint.doesntBlockTowerPlacement = true;
            airModel2.droneModel.GetAttackModel().weapons[0].projectile.ApplyDisplay<FlyingLigntningV2>();
            airModel2.droneModel.range = towerModel.range * 2;
            airModel2.droneModel.GetAttackModel().range = towerModel.range * 2;
            airModel2.droneModel.GetAttackModel().GetBehavior<PursuitSettingCustomModel>().mustBeInRangeOfParent = false;
            airModel2.droneModel.GetAttackModel().GetBehavior<AttackFilterModel>().filters = new Il2CppReferenceArray<FilterModel>(filterModel);
            airModel2.droneModel.GetAttackModel().weapons[0].projectile.AddBehavior(LaserShock);
            airModel2.droneModel.GetAttackModel().weapons[0].projectile.GetDamageModel().damage = 1;
            airModel2.droneModel.GetAttackModel().weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("DamageModifierForTagModel_Ceramic", "Ceramic", 1, 2, false, false));
            airModel2.droneModel.GetAttackModel().weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("DamageModifierForTagModel_Fortified", "Fortified", 1, 5, false, false));
            airModel2.droneModel.GetAttackModel().weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("DamageModifierForTagModel_Moabs", "Moabs", 1, 5, false, false));
            airModel2.droneModel.GetAttackModel().weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("DamageModifierForTagModel_Ddt", "Ddt", 1, 25, false, false));
            airModel2.droneModel.GetAttackModel().weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("DamageModifierForTagModel_Bad", "Bad", 1, 10, false, false));
            airModel2.droneModel.GetAttackModel().weapons[0].projectile.GetBehavior<AddBehaviorToBloonModel>().filters = null;
            airModel2.droneModel.GetAttackModel().weapons[0].projectile.pierce = 10;
            airModel2.droneModel.GetAttackModel().weapons[0].projectile.collisionPasses = new Il2CppStructArray<int>(collPass);
            airModel2.droneModel.GetAttackModel().weapons[0].GetDescendants<FilterInvisibleModel>().ForEach(model => model.isActive = false);
            airModel2.droneModel.GetAttackModel().weapons[0].projectile.GetDamageModel().immuneBloonProperties = BloonProperties.Purple;
            airModel2.droneModel.GetAttackModel().weapons[0].ejectY += 5;
            airModel2.droneModel.GetAttackModel().weapons[0].projectile.maxPierce = 20;
            airModel2.count = 1;
            towerModel.AddBehavior(airModel2);
        }
    }
    public class Level18 : ModHeroLevel<DinoDirk>
    {
        public override string Description => "Dirkerine: Dirk optained a extream healing factor which makes him regenerate lives if you are under 1000 hearths. DOESN'T WORK WHILE YOUR DRONE IS ACTIVE";

        public override int Level => 18;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            //InUpdateFunction
        }
    }
    public class Level19 : ModHeroLevel<DinoDirk>
    {
        public override string Description => "Dirk turns into god mode and improves his energie with the power of lightnings!";

        public override int Level => 19;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            towerModel.GetAttackModel().weapons[0].rate *= 0.8f;
            //Level20
            /*var BuffEnergie = new RateSupportModel("EnergieBuff", 0.85f, true, "Rate:Support", true, 1, null, "EnergieBuff", "Energie_Buff", true);
            BuffEnergie.ApplyBuffIcon<EnergieBuff>();
            BuffEnergie.isGlobal = true;
            towerModel.AddBehavior(BuffEnergie);*/
        }
    }
    public class Level20s : ModHeroLevel<DinoDirk>
    {
        public override string Description => "Over the top: Dirk shares his power even with paragons!";

        public override int Level => 20;

        public override void ApplyUpgrade(TowerModel towerModel)
        {
            var BuffEnergie = new RateSupportModel("EnergieBuff", 0.85f, true, "Rate:Support", true, 1, null, "EnergieBuff", "Energie_Buff", true);
            BuffEnergie.ApplyBuffIcon<EnergieBuff>();
            BuffEnergie.isGlobal = true;
            towerModel.AddBehavior(BuffEnergie);
            towerModel.ApplyDisplay<DirkDisplayL20>();
        }
    }
}


