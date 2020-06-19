## [Contents](https://github.com/Abbysssal/aToM) ##

1. [Main page](https://github.com/Abbysssal/aToM/blob/master/README.md)
2. [Mutators](https://github.com/Abbysssal/aToM/blob/master/Mutators.md)
3. **How does it all work?**
4. [Changelog](https://github.com/Abbysssal/aToM/blob/master/Changelog.md)

## Mutators list ##

* [Melee Damage x0/x0.25/x0.5/x2/x4/x8/x999](https://github.com/Abbysssal/aToM/blob/master/HowItAllWorks.md#melee-damage);
* [Melee Durability 1/x0.25/x0.5/x2/x4/x8](https://github.com/Abbysssal/aToM/blob/master/HowItAllWorks.md#melee-durability);
* [Melee Lunge x0/x0.25/x0.5/x2/x4/x8](https://github.com/Abbysssal/aToM/blob/master/HowItAllWorks.md#melee-lunge);
* [Melee Speed x0.25/x0.5/x2/x4](https://github.com/Abbysssal/aToM/blob/master/HowItAllWorks.md#melee-speed);
* [Thrown Damage x0/x0.25/x0.5/x2/x4/x8/x999](https://github.com/Abbysssal/aToM/blob/master/HowItAllWorks.md#thrown-damage);
* [Thrown Count x0.25/x0.5/x2/x4/x8](https://github.com/Abbysssal/aToM/blob/master/HowItAllWorks.md#thrown-count);
* [Thrown Distance x0.25/x0.5/x2/x4](https://github.com/Abbysssal/aToM/blob/master/HowItAllWorks.md#thrown-distance);
* [Ranged Damage x0/x0.25/x0.5/x2/x4/x8/x999](https://github.com/Abbysssal/aToM/blob/master/HowItAllWorks.md#ranged-damage);
* [Ranged Ammo 1/x0.25/x0.5/x2/x4/x8](https://github.com/Abbysssal/aToM/blob/master/HowItAllWorks.md#ranged-ammo);
* [Ranged Firerate x0.25/x0.5/x2/x4/x8](https://github.com/Abbysssal/aToM/blob/master/HowItAllWorks.md#ranged-firerate);
* [Fully Automatic Ranged Weapons](https://github.com/Abbysssal/aToM/blob/master/HowItAllWorks.md#fully-automatic-ranged-weapons);
* [Projectile Speed x0/x0.25/x0.5/x2/x4](https://github.com/Abbysssal/aToM/blob/master/HowItAllWorks.md#projectile-speed);
* [Rocket/Random/Random Effect Projectiles](https://github.com/Abbysssal/aToM/blob/master/HowItAllWorks.md#projectile-types);
* [Explosion Damage x0.25/x0.5/x2/x4/x8](https://github.com/Abbysssal/aToM/blob/master/HowItAllWorks.md#explosion-damage);
* [Explosion Power x0.25/x0.5/x2/x4/x8](https://github.com/Abbysssal/aToM/blob/master/HowItAllWorks.md#explosion-power);

### [Melee Damage](https://github.com/Abbysssal/aToM/blob/master/Mutators.md#melee-damage-) ###
```cs
RoguePatcher patcher = new RoguePatcher(this, GetType());
patcher.Postfix(typeof(InvItem), "SetupDetails", new Type[] { typeof(bool) });
```
```cs
public static void InvItem_SetupDetails(InvItem __instance)
{
    if (__instance.itemType == "WeaponMelee")
        __instance.meleeDamage = (int)Math.Ceiling((float)__instance.meleeDamage * MeleeDamageMultiplier);
}
```
### [Melee Durability](https://github.com/Abbysssal/aToM/blob/master/Mutators.md#melee-durability-) ###
```cs
RoguePatcher patcher = new RoguePatcher(this, GetType());
patcher.Postfix(typeof(InvItem), "SetupDetails", new Type[] { typeof(bool) });
```
```cs
public static void InvItem_SetupDetails(InvItem __instance)
{
    if (__instance.itemType == "WeaponMelee")
    {
        __instance.initCount = (int)Math.Ceiling((float)__instance.initCount * MeleeDurabilityMultiplier);
        __instance.initCountAI = (int)Math.Ceiling((float)__instance.initCountAI * MeleeDurabilityMultiplier);
        __instance.rewardCount = (int)Math.Ceiling((float)__instance.rewardCount * MeleeDurabilityMultiplier);
    }
}
```
### [Melee Lunge](https://github.com/Abbysssal/aToM/blob/master/Mutators.md#melee-lunge-) ###
```cs
RoguePatcher patcher = new RoguePatcher(this, GetType());
patcher.Prefix(typeof(Movement), "KnockForward", new Type[] { typeof(Quaternion), typeof(float), typeof(bool) });
```
```cs
public static void Movement_KnockForward(ref float strength)
{
    strength *= MeleeLungeMultiplier;
}
```
### [Melee Speed](https://github.com/Abbysssal/aToM/blob/master/Mutators.md#melee-speed-) ###
```cs
RoguePatcher patcher = new RoguePatcher(this, GetType());
patcher.Postfix(typeof(Melee), "Attack", new Type[] { typeof(bool) });
```
```cs
public static void Melee_Attack(Melee __instance)
{
    __instance.agent.weaponCooldown /= MeleeSpeedMultiplier;
    __instance.meleeContainerAnim.speed *= MeleeSpeedMultiplier;
}
```
### [Thrown Damage](https://github.com/Abbysssal/aToM/blob/master/Mutators.md#thrown-damage-) ###
```cs
RoguePatcher patcher = new RoguePatcher(this, GetType());
patcher.Postfix(typeof(InvItem), "SetupDetails", new Type[] { typeof(bool) });
```
```cs
public static void InvItem_SetupDetails(InvItem __instance)
{
    if (__instance.itemType == "WeaponThrown")
        __instance.throwDamage = (int)Math.Ceiling((float)__instance.throwDamage * ThrownDamageMultiplier);
}
```
### [Thrown Count](https://github.com/Abbysssal/aToM/blob/master/Mutators.md#thrown-count-) ###
```cs
RoguePatcher patcher = new RoguePatcher(this, GetType());
patcher.Postfix(typeof(InvItem), "SetupDetails", new Type[] { typeof(bool) });
```
```cs
public static void InvItem_SetupDetails(InvItem __instance)
{
    if (__instance.itemType == "WeaponThrown")
    {
        __instance.initCount = (int)Math.Ceiling((float)__instance.initCount * ThrownCountMultiplier);
        __instance.initCountAI = (int)Math.Ceiling((float)__instance.initCountAI * ThrownCountMultiplier);
        __instance.rewardCount = (int)Math.Ceiling((float)__instance.rewardCount * ThrownCountMultiplier);
        __instance.itemValue = (int)Math.Ceiling((float)__instance.itemValue / ThrownCountMultiplier);
    }
}
```
### [Thrown Distance](https://github.com/Abbysssal/aToM/blob/master/Mutators.md#thrown-distance-) ###
```cs
RoguePatcher patcher = new RoguePatcher(this, GetType());
patcher.Postfix(typeof(InvItem), "SetupDetails", new Type[] { typeof(bool) });
```
```cs
public static void InvItem_SetupDetails(InvItem __instance)
{
    if (__instance.itemType == "WeaponThrown")
        __instance.throwDistance = (int)Math.Ceiling((float)__instance.throwDistance * ThrownDistanceMultiplier);
}
```
### [Ranged Damage](https://github.com/Abbysssal/aToM/blob/master/Mutators.md#ranged-damage-) ###
```cs
RoguePatcher patcher = new RoguePatcher(this, GetType());
patcher.Postfix(typeof(Bullet), "SetupBullet", new Type[] { });
```
```cs
public static void Bullet_SetupBullet(Bullet __instance)
{
    __instance.damage = (int)Math.Ceiling((float)__instance.damage * RangedDamageMultiplier);
}
```
### [Ranged Ammo](https://github.com/Abbysssal/aToM/blob/master/Mutators.md#ranged-ammo-) ###
```cs
RoguePatcher patcher = new RoguePatcher(this, GetType());
patcher.Postfix(typeof(InvItem), "SetupDetails", new Type[] { typeof(bool) });
```
```cs
public static void InvItem_SetupDetails(InvItem __instance)
{
    if (__instance.itemType == "WeaponProjectile" && __instance.invItemName != "Taser")
    {
        __instance.initCount = (int)Math.Ceiling((float)__instance.initCount * RangedAmmoMultiplier);
        __instance.initCountAI = (int)Math.Ceiling((float)__instance.initCountAI * RangedAmmoMultiplier);
        __instance.rewardCount = (int)Math.Ceiling((float)__instance.rewardCount * RangedAmmoMultiplier);
        __instance.initValue = (int)Math.Ceiling((float)__instance.initValue / RangedAmmoMultiplier);
    }
}
```
### [Ranged Firerate](https://github.com/Abbysssal/aToM/blob/master/Mutators.md#ranged-firerate-) ###
```cs
RoguePatcher patcher = new RoguePatcher(this, GetType());
patcher.Postfix(typeof(Gun), "SetWeaponCooldown", new Type[] { typeof(float), typeof(InvItem) });
```
```cs
public static void Gun_SetWeaponCooldown(Gun __instance)
{
    __instance.agent.weaponCooldown /= RangedFirerateMultiplier;
    __instance.agent.weaponCooldown = Mathf.Max(__instance.agent.weaponCooldown, 0.05f);
}
```
### [Fully Automatic Ranged Weapons](https://github.com/Abbysssal/aToM/blob/master/Mutators.md#fully-automatic-ranged-weapons-) ###
```cs
RoguePatcher patcher = new RoguePatcher(this, GetType());
patcher.Postfix(typeof(InvItem), "SetupDetails", new Type[] { typeof(bool) });
```
```cs
public static void InvItem_SetupDetails(InvItem __instance)
{
    if (__instance.itemType == "WeaponProjectile" && RangedFullAutoEnabled)
        __instance.rapidFire = true;
}
```
### [Projectile Speed](https://github.com/Abbysssal/aToM/blob/master/Mutators.md#projectile-speed-) ###
```cs
RoguePatcher patcher = new RoguePatcher(this, GetType());
patcher.Postfix(typeof(Bullet), "SetupBullet", new Type[] { });
```
```cs
public static void Bullet_SetupBullet(Bullet __instance)
{
    __instance.speed = (int)Math.Ceiling((float)__instance.speed * ProjectileSpeedMultiplier);
}
```
### [Projectile Types](https://github.com/Abbysssal/aToM/blob/master/Mutators.md#projectile-types-) ###
```cs
RoguePatcher patcher = new RoguePatcher(this, GetType());
patcher.Prefix(typeof(SpawnerMain), "SpawnBullet", new Type[] { typeof(Vector3), typeof(bulletStatus), typeof(PlayfieldObject), typeof(int) });
patcher.Prefix(typeof(Gun), "spawnBullet", new Type[] { typeof(bulletStatus), typeof(InvItem), typeof(int), typeof(bool), typeof(string) });
```
```cs
public static void SpawnerMain_SpawnBullet(ref bulletStatus bulletType)
{
    if (ProjectileTypeRocketEnabled)
        bulletType = bulletStatus.Rocket;
    else if (ProjectileTypeRandomEnabled)
        do
            bulletType = (bulletStatus)(new System.Random().Next(1, 22)); // [1;21]
        while (bulletType == bulletStatus.ZombieSpit || bulletType == bulletStatus.Laser || bulletType == bulletStatus.MindControl);
}
public static void Gun_spawnBullet(ref bulletStatus bulletType, ref string myStatusEffect)
{
    if (ProjectileTypeRandomEffectEnabled || (bulletType == bulletStatus.WaterPistol && (myStatusEffect == null || myStatusEffect == string.Empty)))
    {
        bulletType = bulletStatus.WaterPistol;
        List<string> list = new List<string>()
        {
            "Poisoned",
            "Drunk",
            "Slow",
            "Fast",
            "Strength",
            "Weak",
            "Paralyzed",
            "Accurate",
            "RegenerateHealth",
            "Acid",
            "Invincible",
            "Invisible",
            "Confused",
            "Giant",
            "Resurrection",
            "AlwaysCrit",
            "Shrunk",
            "ElectroTouch",
            "BadVision",
            "Cyanide",
            "DecreaseAllStats",
            "Dizzy",
            "DizzyB",
            "Enraged",
            "FeelingLucky",
            "FeelingUnlucky",
            "Frozen",
            "HearingBlocked",
            "Nicotine",
            "Tranquilized",
            "Electrocuted",
            "BlockDebuffs",
            "IncreaseAllStats",
            "Invisible",
            "KillerThrower",
            "MindControlled",
            "NiceSmelling",
            "WerewolfEffect"
        };
        int rand = new System.Random().Next(list.Count);
        myStatusEffect = list[rand];
    }
}
```
### [Explosion Damage](https://github.com/Abbysssal/aToM/blob/master/Mutators.md#explosion-damage-) ###
```cs
RoguePatcher patcher = new RoguePatcher(this, GetType());
patcher.Postfix(typeof(Explosion), "SetupExplosion", new Type[] { });
```
```cs
public static void Explosion_SetupExplosion(Explosion __instance)
{
    __instance.damage = (int)Math.Ceiling((float)__instance.damage * ExplosionDamageMultiplier);
}
```
### [Explosion Power](https://github.com/Abbysssal/aToM/blob/master/Mutators.md#explosion-power-) ###
```cs
RoguePatcher patcher = new RoguePatcher(this, GetType());
patcher.Postfix(typeof(Explosion), "SetupExplosion", new Type[] { });
```
```cs
public static void Explosion_SetupExplosion(Explosion __instance)
{
    __instance.circleCollider2D.radius *= Mathf.Sqrt(ExplosionPowerMultiplier);
}
```







