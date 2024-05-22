using Il2CppAssets.Scripts.Models.Towers;
using Il2CppAssets.Scripts.Models.Towers.Behaviors;
using Il2CppAssets.Scripts.Unity;
using BTD_Mod_Helper.Api.Towers;
using BTD_Mod_Helper.Extensions;
using BTD_Mod_Helper.Api.Display;
using Il2CppAssets.Scripts.Unity.Display;
using UnityEngine;
using Il2CppAssets.Scripts.Simulation.Towers.Weapons;
using static DirkTheDino.DirkTheDino;
using Il2CppAssets.Scripts.Models.Towers.Behaviors.Attack;
using Il2CppAssets.Scripts.Models.Towers.Projectiles.Behaviors;
using Il2CppAssets.Scripts.Models.Towers.Projectiles;
using Il2CppAssets.Scripts.Models.Towers.Weapons;
using Il2CppAssets.Scripts.Models.TowerSets;
using Il2CppAssets.Scripts.Models.Towers.Filters;
using Il2Cpp;
using Il2CppInterop.Runtime.InteropTypes.Arrays;

namespace DirkTheDino;

public class DinoDirk : ModHero
{
    public override string BaseTower => TowerType.Quincy;

    public override int Cost => 1150;

    public override string DisplayName => "Dirk";
    public override string Title => "Dirk the Dino";
    public override string Level1Description => "Dirk's family died 65 million years ago during a long battle....he's taking revenge now";
    public override string Portrait => "Dirki-Portrait";
    public override string Icon => "Dirki-Icon";
    public override string SelectSound => "Place5_1";
    public override string Description =>
        "Dirk the bouncer doesn't like bloons. And as a bouncer he's never going to leave his work.(You can't sell him)";
    public override string Button => "Icon";
    
    public override string Square => "ButtonIcon";
    public override string NameStyle => TowerType.Gwendolin; // Yellow colored
    public override string BackgroundStyle => TowerType.Etienne; // Yellow colored
    public override string GlowStyle => TowerType.StrikerJones; // Yellow colored
    public override int MaxLevel => 20;
    public override float XpRatio => 1.9f;
    /// <param name="towerModel"></param>
    public override void ModifyBaseTowerModel(TowerModel towerModel)
    {
        towerModel.ApplyDisplay<DirkDisplay>();
        var quincy = Game.instance.model.GetTowerWithName(TowerType.Quincy);
        towerModel.RemoveBehaviors<CreateSoundOnUpgradeModel>();
        towerModel.RemoveBehaviors<CreateSoundOnTowerPlaceModel>();
        towerModel.RemoveBehavior<CreateSoundOnBloonEnterTrackModel>();
        towerModel.RemoveBehavior<CreateSoundOnSelectedModel>();
        towerModel.RemoveBehavior<CreateSoundOnBloonLeakModel>();
        towerModel.RemoveBehavior<CreateSoundOnEndOfRoundModel>();
        towerModel.RemoveBehavior<CreateSoundOnDelayedCollisionModel>();
        towerModel.RemoveBehavior<CreateSoundOnBloonDestroyedModel>();
        towerModel.RemoveBehavior<CreateSoundOnAttachedModel>();
        towerModel.RemoveBehavior<CreateSoundOnBloonLeakModel>();
        towerModel.RemoveBehaviors<CreateSoundOnSellModel>();
        towerModel.GetAttackModel().weapons[0].projectile.ApplyDisplay<Leer>();
        towerModel.range = 35;
        towerModel.radius = quincy.radius*1.4f;
        towerModel.footprint = quincy.footprint.Duplicate();
        var w = towerModel.GetWeapon();
        w.rate = 0.6f;
        w.projectile.GetDamageModel().damage = 1;
        w.projectile.pierce = 2;
        w.projectile.maxPierce = 500;
        towerModel.blockSelling = true;
    }   
}



public class Drone1_Stufe16 : ModTower
{
    public override string DisplayName => "Drone1";
    public override string Name => "Drone1";
    public override string BaseTower => "DartMonkey-200";
    public override TowerSet TowerSet => TowerSet.None;
    public override int Cost => 0;
    public override int TopPathUpgrades => 0;
    public override int MiddlePathUpgrades => 0;
    public override int BottomPathUpgrades => 0;
    public override bool DontAddToShop => true;
    public override string Portrait => "Dirki-Portrait";
    public override string Icon => "Dirki-Icon";
    public override string Description => "I am Lemmy and i am really bad in maths";
    public override void ModifyBaseTowerModel(TowerModel drone1)
    {
        drone1.ApplyDisplay<RedDrone>();
        drone1.range = 40f;
        drone1.radius = 0f;
        var LaserShock = Game.instance.model.GetTower(TowerType.DartlingGunner, 2, 0, 0).GetDescendant<AddBehaviorToBloonModel>().Duplicate();
        LaserShock.lifespan = 4;
        LaserShock.filters = null;
        LaserShock.overlayType = ElectricShockDisplay.CustomOverlayType;
        int[] collPass = { -1, 0, 1 };
        drone1.GetAttackModel(0).weapons[0].projectile.collisionPasses = new Il2CppStructArray<int>(collPass);
        drone1.GetAttackModel(0).weapons[0].projectile.AddBehavior(LaserShock);
        drone1.ignoreTowerForSelection = true;
        drone1.GetAttackModel().weapons[0].projectile.GetDamageModel().immuneBloonProperties = BloonProperties.None;
        drone1.isSubTower = true;
        drone1.GetDescendants<FilterInvisibleModel>().ForEach(filter => filter.isActive = false);
        drone1.footprint.ignoresTowerOverlap = true;
        drone1.footprint.doesntBlockTowerPlacement = true;
        drone1.footprint.ignoresPlacementCheck = true;
        drone1.GetAttackModel().weapons[0].projectile.GetDamageModel().damage = 0.05f;
        drone1.GetAttackModel().weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("DamageModifierForTagModel_Ddt", "Ddt", 1, 1, false, false));
        drone1.GetAttackModel().weapons[0].ejectY = 0;
        drone1.GetAttackModel().weapons[0].ejectX = 0;
        drone1.GetAttackModel().weapons[0].ejectZ = 0;
        drone1.GetAttackModel().weapons[0].projectile.ApplyDisplay<DroneKette>();
        drone1.GetAttackModel().weapons[0].projectile.GetBehavior<TravelStraitModel>().speed = 1000;
        drone1.GetAttackModel().weapons[0].projectile.GetBehavior<TravelStraitModel>().lifespan = 0;
        drone1.GetAttackModel().weapons[0].rate = 0.02f;
        drone1.AddBehavior(new CreditPopsToParentTowerModel("DamageForMainTower"));
        TowerModelBehaviorExt.GetBehavior<AttackModel>(drone1).fireWithoutTarget = true;
    }
}

public class Drone2_Stufe16 : ModTower
{
    public override string DisplayName => "Drone2";
    public override string Name => "Drone2";
    public override string BaseTower => "DartMonkey-200";
    public override TowerSet TowerSet => TowerSet.None;
    public override int Cost => 0;
    public override int TopPathUpgrades => 0;
    public override bool DontAddToShop => true;
    public override int MiddlePathUpgrades => 0;
    public override int BottomPathUpgrades => 0;
    public override string Portrait => "Dirki-Portrait";
    public override string Icon => "Dirki-Icon";
    public override string Description => "I am Punji and i am really bad in maths";
    public override void ModifyBaseTowerModel(TowerModel drone2)
    {
        var LaserShock = Game.instance.model.GetTower(TowerType.DartlingGunner, 2, 0, 0).GetDescendant<AddBehaviorToBloonModel>().Duplicate();
        LaserShock.lifespan = 4;
        LaserShock.filters = null;
        LaserShock.overlayType = ElectricShockDisplay.CustomOverlayType;
        int[] collPass = { -1, 0, 1 };
        drone2.ApplyDisplay<BlueDrone>();
        drone2.range = 40f;
        drone2.radius = 0f;
        drone2.GetAttackModel(0).weapons[0].projectile.collisionPasses = new Il2CppStructArray<int>(collPass);
        drone2.GetAttackModel(0).weapons[0].projectile.AddBehavior(LaserShock);
        drone2.ignoreTowerForSelection = true;
        drone2.isSubTower = true;
        drone2.GetDescendants<FilterInvisibleModel>().ForEach(filter => filter.isActive = false);
        drone2.GetAttackModel().weapons[0].projectile.GetDamageModel().immuneBloonProperties = BloonProperties.None;
        drone2.GetAttackModel().weapons[0].projectile.GetDamageModel().damage = 0.05f;
        drone2.GetAttackModel().weapons[0].projectile.AddBehavior(new DamageModifierForTagModel("DamageModifierForTagModel_Ddt", "Ddt", 1, 1, false, false));
        drone2.GetAttackModel().weapons[0].ejectY = 0;//-1
        drone2.GetAttackModel().weapons[0].ejectX = 0;//0.9
        drone2.GetAttackModel().weapons[0].ejectZ = 0;
        drone2.GetAttackModel().weapons[0].projectile.ApplyDisplay<DroneKette>();
        drone2.GetAttackModel().weapons[0].projectile.GetBehavior<TravelStraitModel>().speed = 1000;
        drone2.GetAttackModel().weapons[0].projectile.GetBehavior<TravelStraitModel>().lifespan = 0;
        drone2.GetAttackModel().weapons[0].rate = 0.02f;
        drone2.AddBehavior(new CreditPopsToParentTowerModel("DamageForMainTower"));
        drone2.footprint.ignoresTowerOverlap = true;
        drone2.footprint.doesntBlockTowerPlacement = true;
        drone2.footprint.ignoresPlacementCheck = true;
        TowerModelBehaviorExt.GetBehavior<AttackModel>(drone2).fireWithoutTarget = true;
    }
}

public class DroneAttackModell1 : ModTower
{
    protected override int Order
    {
        get
        {
            return 0;
        }
    }
    public override TowerSet TowerSet
    {
        get
        {
            return 0;
        }
    }
    public override string BaseTower
    {
        get
        {
            return "DartMonkey-200";
        }
    }
    public override int Cost => 0;
    public override int TopPathUpgrades => 0;
    public override int MiddlePathUpgrades => 0;
    public override int BottomPathUpgrades => 0;
    public override string Name => "DroneAttackDirk1";
    public override bool Use2DModel => true;
    public override bool DontAddToShop => true;
    public override string Description => "I am Bador and i am really bad in maths";
    public override void ModifyBaseTowerModel(TowerModel towerModel)
    {
        towerModel.icon = (towerModel.portrait = Game.instance.model.GetTowerFromId("DartMonkey-010").portrait);
        AttackModel attackModel = TowerModelExt.GetAttackModel(towerModel);
        ProjectileModelBehaviorExt.AddBehavior<DamageModifierForTagModel>(attackModel.weapons[0].projectile, new DamageModifierForTagModel("dmgMod", "Moabs", 1f, 1f, false, false));
        attackModel.weapons[0].projectile.hasDamageModifiers = true;
        ProjectileModelExt.GetDamageModel(attackModel.weapons[0].projectile).damage += 2f;
        ProjectileModelBehaviorExt.GetBehavior<TravelStraitModel>(attackModel.weapons[0].projectile).Speed = 250f;
        ProjectileModelBehaviorExt.GetBehavior<TravelStraitModel>(attackModel.weapons[0].projectile).lifespan = 4f;
        attackModel.weapons[0].ejectY = -40f;
        ModelExt.SetName<ProjectileModel>(attackModel.weapons[0].projectile, "DroneP1");
        attackModel.weapons[0].projectile.canCollisionBeBlockedByMapLos = false;
        attackModel.weapons[0].rate *= 0.1f;
        towerModel.range *= 10.6f;
        attackModel.weapons[0].projectile.pierce += 8f;
        attackModel.weapons[0].projectile.ApplyDisplay<Leer>();
        towerModel.ignoreBlockers = true;
        TowerModelExt.GetWeapon(towerModel).projectile.ignoreBlockers = true;
        TowerModelExt.GetWeapon(towerModel).projectile.canCollisionBeBlockedByMapLos = false;
        TowerModelExt.GetAttackModel(towerModel).attackThroughWalls = true;
        var Fire = Game.instance.model.GetTower(TowerType.MortarMonkey, 0, 0, 2).GetAttackModel().weapons[0].projectile.GetBehavior<CreateProjectileOnExhaustFractionModel>().projectile.GetBehavior<AddBehaviorToBloonModel>();
        Fire.lifespan = 9;
        towerModel.GetAttackModel().weapons[0].projectile.AddBehavior(Fire);
        towerModel.GetAttackModel().weapons[0].projectile.collisionPasses = new[] { -1, 0 };
        attackModel.weapons[0].projectile.GetDamageModel().immuneBloonProperties = BloonProperties.None;
        towerModel.GetAttackModel().weapons[0].GetDescendants<FilterInvisibleModel>().ForEach(model => model.isActive = false);
        WeaponModel attackoffset = ModelExt.Duplicate<WeaponModel>(attackModel.weapons[0]);
        WeaponModel attackoffset2 = ModelExt.Duplicate<WeaponModel>(attackModel.weapons[0]);
        attackoffset.ejectX = -50f;
        attackoffset2.ejectX = 50f;
        AttackModelExt.AddWeapon(attackModel, attackoffset);
        AttackModelExt.AddWeapon(attackModel, attackoffset2);
        towerModel.range = 40f;
        towerModel.radius = 0f;
        towerModel.ignoreTowerForSelection = true;
        towerModel.isSubTower = true;
        towerModel.footprint.ignoresTowerOverlap = true;
        towerModel.footprint.doesntBlockTowerPlacement = true;
        towerModel.footprint.ignoresPlacementCheck = true;
    }
}
public class DroneAttackModell2 : ModTower
{
    protected override int Order
    {
        get
        {
            return 0;
        }
    }
    public override TowerSet TowerSet
    {
        get
        {
            return 0;
        }
    }
    public override string BaseTower
    {
        get
        {
            return "DartMonkey-200";
        }
    }
    public override int Cost => 0;
    public override int TopPathUpgrades => 0;
    public override int MiddlePathUpgrades => 0;
    public override int BottomPathUpgrades => 0;
    public override string Name => "DroneAttackDirk2";
    public override bool Use2DModel => true;
    public override bool DontAddToShop => true;
    public override string Description => "I am Bador and i am really bad in maths";
    public override void ModifyBaseTowerModel(TowerModel towerModel)
    {
        towerModel.icon = (towerModel.portrait = Game.instance.model.GetTowerFromId("DartMonkey-010").portrait);
        AttackModel attackModel = TowerModelExt.GetAttackModel(towerModel);
        ProjectileModelBehaviorExt.AddBehavior<DamageModifierForTagModel>(attackModel.weapons[0].projectile, new DamageModifierForTagModel("dmgMod", "Moabs", 1f, 1f, false, false));
        attackModel.weapons[0].projectile.hasDamageModifiers = true;
        ProjectileModelExt.GetDamageModel(attackModel.weapons[0].projectile).damage += 1f;
        ProjectileModelBehaviorExt.GetBehavior<TravelStraitModel>(attackModel.weapons[0].projectile).Speed = 250f;
        ProjectileModelBehaviorExt.GetBehavior<TravelStraitModel>(attackModel.weapons[0].projectile).lifespan = 4f;
        attackModel.weapons[0].ejectY = -40f;
        ModelExt.SetName<ProjectileModel>(attackModel.weapons[0].projectile, "DroneP1");
        attackModel.weapons[0].projectile.canCollisionBeBlockedByMapLos = false;
        attackModel.weapons[0].rate *= 0.2f;
        towerModel.range *= 10.6f;
        attackModel.weapons[0].projectile.pierce += 20f;
        attackModel.weapons[0].projectile.ApplyDisplay<Leer>();
        towerModel.ignoreBlockers = true;
        TowerModelExt.GetWeapon(towerModel).projectile.ignoreBlockers = true;
        TowerModelExt.GetWeapon(towerModel).projectile.canCollisionBeBlockedByMapLos = false;
        TowerModelExt.GetAttackModel(towerModel).attackThroughWalls = true;
        var Ice = Game.instance.model.GetTowerFromId("IceMonkey-320").GetAttackModel().weapons[0].projectile.GetBehavior<FreezeModel>().Duplicate();
        Ice.canFreezeMoabs = true;
        Ice.lifespan = 3;
        towerModel.GetAttackModel().weapons[0].projectile.AddBehavior(Ice);
        towerModel.GetAttackModel().weapons[0].projectile.collisionPasses = new[] { -1, 0,1};
        attackModel.weapons[0].projectile.GetDamageModel().immuneBloonProperties = BloonProperties.None;
        towerModel.GetAttackModel().weapons[0].GetDescendants<FilterInvisibleModel>().ForEach(model => model.isActive = false);
        WeaponModel attackoffset = ModelExt.Duplicate<WeaponModel>(attackModel.weapons[0]);
        WeaponModel attackoffset2 = ModelExt.Duplicate<WeaponModel>(attackModel.weapons[0]);
        attackoffset.ejectX = -50f;
        attackoffset2.ejectX = 50f;
        AttackModelExt.AddWeapon(attackModel, attackoffset);
        AttackModelExt.AddWeapon(attackModel, attackoffset2);
        towerModel.range = 40f;
        towerModel.radius = 0f;
        towerModel.ignoreTowerForSelection = true;
        towerModel.isSubTower = true;
        towerModel.footprint.ignoresTowerOverlap = true;
        towerModel.footprint.doesntBlockTowerPlacement = true;
        towerModel.footprint.ignoresPlacementCheck = true;
    }
}
public class DroneAttackModell3 : ModTower
{
    protected override int Order
    {
        get
        {
            return 0;
        }
    }
    public override TowerSet TowerSet
    {
        get
        {
            return 0;
        }
    }
    public override string BaseTower
    {
        get
        {
            return "DartMonkey-200";
        }
    }
    public override int Cost => 0;
    public override int TopPathUpgrades => 0;
    public override int MiddlePathUpgrades => 0;
    public override int BottomPathUpgrades => 0;
    public override string Name => "DroneAttackDirk3";
    public override bool Use2DModel => true;
    public override bool DontAddToShop => true;
    public override string Description => "I am Bador and i am really bad in maths";
    public override void ModifyBaseTowerModel(TowerModel towerModel)
    {
        towerModel.icon = (towerModel.portrait = Game.instance.model.GetTowerFromId("DartMonkey-010").portrait);
        AttackModel attackModel = TowerModelExt.GetAttackModel(towerModel);
        ProjectileModelBehaviorExt.AddBehavior<DamageModifierForTagModel>(attackModel.weapons[0].projectile, new DamageModifierForTagModel("dmgMod", "Moabs", 1f, 1f, false, false));
        attackModel.weapons[0].projectile.hasDamageModifiers = true;
        ProjectileModelExt.GetDamageModel(attackModel.weapons[0].projectile).damage += 1f;
        ProjectileModelBehaviorExt.GetBehavior<TravelStraitModel>(attackModel.weapons[0].projectile).Speed = 250f;
        ProjectileModelBehaviorExt.GetBehavior<TravelStraitModel>(attackModel.weapons[0].projectile).lifespan = 4f;
        attackModel.weapons[0].ejectY = -40f;
        ModelExt.SetName<ProjectileModel>(attackModel.weapons[0].projectile, "DroneP1");
        attackModel.weapons[0].projectile.canCollisionBeBlockedByMapLos = false;
        attackModel.weapons[0].rate *= 0.15f;
        towerModel.range *= 10.6f;
        attackModel.weapons[0].projectile.pierce += 20f;
        attackModel.weapons[0].projectile.ApplyDisplay<Leer>();
        towerModel.ignoreBlockers = true;
        TowerModelExt.GetWeapon(towerModel).projectile.ignoreBlockers = true;
        TowerModelExt.GetWeapon(towerModel).projectile.canCollisionBeBlockedByMapLos = false;
        TowerModelExt.GetAttackModel(towerModel).attackThroughWalls = true;
        attackModel.weapons[0].projectile.AddBehavior(new WindModel("WindModel_", 1, 2, 0.5f,true,null,5, "Ddt",5));
        attackModel.weapons[0].projectile.AddBehavior(new WindModel("WindModel_", 1.2f, 3, 0.4f, true, null, 2, "Moabs", 2));
        attackModel.weapons[0].projectile.GetDamageModel().immuneBloonProperties = BloonProperties.None;
        towerModel.GetAttackModel().weapons[0].GetDescendants<FilterInvisibleModel>().ForEach(model => model.isActive = false);
        WeaponModel attackoffset = ModelExt.Duplicate<WeaponModel>(attackModel.weapons[0]);
        WeaponModel attackoffset2 = ModelExt.Duplicate<WeaponModel>(attackModel.weapons[0]);
        attackoffset.ejectX = -50f;
        attackoffset2.ejectX = 50f;
        AttackModelExt.AddWeapon(attackModel, attackoffset);
        AttackModelExt.AddWeapon(attackModel, attackoffset2);
        towerModel.range = 40f;
        towerModel.radius = 0f;
        towerModel.ignoreTowerForSelection = true;
        towerModel.isSubTower = true;
        towerModel.footprint.ignoresTowerOverlap = true;
        towerModel.footprint.doesntBlockTowerPlacement = true;
        towerModel.footprint.ignoresPlacementCheck = true;
    }
}

