using System;
using System.Linq;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using BepInEx;
using RogueLibsCore;
using UnityEngine;
using System.Text.RegularExpressions;
using BepInEx.Logging;
using System.Globalization;
using System.Net.Sockets;
using Light2D;

namespace aTonOfMutators
{
    [BepInPlugin(GUID, Name, Version)]
    [BepInDependency(RogueLibs.GUID, "3.3.0")]
    public class AToMPlugin : BaseUnityPlugin
    {
        public const string GUID = "abbysssal.streetsofrogue.atonofmutators";
        public const string Name = "a Ton of Mutators";
        public const string Version = "2.0.0";

        private static readonly Regex regex = new Regex(@"\[(.*?)\|(.*?)\|(.*?)\|(.*?)\]");
        private static string FormatName(string text, float multiplier)
        {
            if (float.IsNaN(multiplier)) return text;
            string normMultiplier = multiplier is float.Epsilon ? " =1" : " x" + multiplier.ToString(CultureInfo.InvariantCulture);
            return "[aToM] " + text + normMultiplier;
        }
        private static CustomNameInfo FormatName(CustomNameInfo text, float multiplier)
        {
            if (float.IsNaN(multiplier)) return text;
            CustomNameInfo format = new CustomNameInfo();
            foreach (KeyValuePair<LanguageCode, string> translation in text)
                format[translation.Key] = FormatName(translation.Value, multiplier);
            return format;
        }
        private static string FormatDescription(string text, float multiplier)
        {
            if (float.IsNaN(multiplier)) return text;
            Match match = regex.Match(text);
            if (!match.Success) throw new ArgumentException($"Invalid format: {text}.");

            int group = multiplier is 0f ? 1
                : multiplier is float.Epsilon ? 2
                : multiplier >= 1f ? 3 : 4;
            string normMultiplier = (multiplier >= 1f ? multiplier : 1f / multiplier).ToString(CultureInfo.InvariantCulture) + "x";

            string replacer = match.Groups[group].Value.Replace(".x", normMultiplier);
            return text.Replace(match.Value, replacer);
        }
        private static CustomNameInfo FormatDescription(CustomNameInfo text, float multiplier)
        {
            if (float.IsNaN(multiplier)) return text;
            CustomNameInfo format = new CustomNameInfo();
            foreach (KeyValuePair<LanguageCode, string> translation in text)
                format[translation.Key] = FormatDescription(translation.Value, multiplier);
            return format;
        }

        private static int FindUnlockCost(float multiplier)
        {
            switch (multiplier)
            {
                case 0f: return 8;
                case float.Epsilon: return 8;
                case 0.125f: return 6;
                case 0.25f: return 4;
                case 0.5f: return 2;
                case 2f: return 2;
                case 4f: return 4;
                case 8f: return 6;
                case 999f: return 8;
                default: throw new ArgumentException("Invalid multiplier");
            }
        }
        private static float GetPrerequisiteMultiplier(float multiplier)
        {
            switch (multiplier)
            {
                case 0f: return 0.125f;
                case float.Epsilon: return 0.125f;
                case 0.125f: return 0.25f;
                case 0.25f: return 0.5f;
                case 0.5f: return -1f;
                case 2f: return -1f;
                case 4f: return 2f;
                case 8f: return 4f;
                case 999f: return 8f;
                default: throw new ArgumentException("Invalid multiplier");
            }
        }
        private static int FindPrerequisiteIndex(float[] multipliers, int index)
        {
            float prerequisite = multipliers[index];
            int prerequisiteIndex;
            do
            {
                prerequisite = GetPrerequisiteMultiplier(prerequisite);
                if (prerequisite is -1f) return -1;
                prerequisiteIndex = Array.IndexOf(multipliers, prerequisite);
            }
            while (prerequisiteIndex is -1);

            return prerequisiteIndex;
        }

        private const int uniqueNumber = -948699;
        private static readonly List<AToMCategory> Categories = new List<AToMCategory>();
        private static AToMCategory CreateCategory(string id, CustomNameInfo name)
        {
            AToMCategory category = new AToMCategory("aToM:" + id + "Category", uniqueNumber + Categories.Count);
            CustomNameInfo format = new CustomNameInfo();
            foreach (KeyValuePair<LanguageCode, string> translation in name)
                format[translation.Key] = "[aToM] " + translation.Value;
            RogueLibs.CreateCustomUnlock(category)
                     .WithName(name);
            Categories.Add(category);
            return category;
        }
        public static void HideAllCategories() => Categories.ForEach(c => c.IsExpanded = false);
        public static void HideAllCategoriesExceptFor(AToMCategory exception) => Categories.ForEach(c => c.IsExpanded = c == exception);

        private class AToMMutatorGroupBuilder
        {
            public AToMMutatorGroupBuilder(UnlockBuilder[] builders)
            {
                Builders = builders;
                Multipliers = Array.ConvertAll(builders, b => ((AToMMutator)b.Unlock).MultiplierValue);
            }
            private readonly UnlockBuilder[] Builders;
            private readonly float[] Multipliers;
            private AToMMutator[] GetMutators() => Array.ConvertAll(Builders, b => (AToMMutator)b.Unlock);

            private Regex ownRegex;
            private CustomNameInfo FormatValues(CustomNameInfo text, int index)
            {
                CustomNameInfo format = new CustomNameInfo();
                foreach (KeyValuePair<LanguageCode, string> translation in format)
                    format[translation.Key] = FormatValues(translation.Value, index);
                return format;
            }
            private string FormatValues(string text, int index)
            {
                if (ownRegex is null)
                    ownRegex = new Regex(@"\[" + string.Join(@"\|", Enumerable.Repeat(@"(.*?)", Multipliers.Length)) + @"\]");
                return ownRegex.Replace(text, "$" + (index + 1));
            }

            public AToMMutatorGroupBuilder WithName(CustomNameInfo name)
            {
                if (float.IsNaN(Multipliers[0]))
                {
                    for (int i = 0; i < Builders.Length; i++)
                        Builders[i].WithName(FormatValues(name, i));
                    return this;
                }
                for (int i = 0; i < Builders.Length; i++)
                    Builders[i].WithName(FormatName(name, Multipliers[i]));
                return this;
            }
            public AToMMutatorGroupBuilder WithName(LanguageCode code, string translation)
            {
                if (float.IsNaN(Multipliers[0]))
                {
                    for (int i = 0; i < Builders.Length; i++)
                    {
                        if (Builders[i].Name is null) Builders[i].WithName(new CustomNameInfo());
                        Builders[i].Name[code] = FormatValues(translation, i);
                    }
                    return this;
                }
                for (int i = 0; i < Builders.Length; i++)
                {
                    if (Builders[i].Name is null) Builders[i].WithName(new CustomNameInfo());
                    Builders[i].Name[code] = FormatName(translation, Multipliers[i]);
                }
                return this;
            }
            public AToMMutatorGroupBuilder WithDescription(CustomNameInfo description)
            {
                if (float.IsNaN(Multipliers[0]))
                {
                    for (int i = 0; i < Builders.Length; i++)
                        Builders[i].WithDescription(FormatValues(description, i));
                    return this;
                }
                for (int i = 0; i < Builders.Length; i++)
                    Builders[i].WithDescription(FormatDescription(description, Multipliers[i]));
                return this;
            }
            public AToMMutatorGroupBuilder WithDescription(LanguageCode code, string translation)
            {
                if (float.IsNaN(Multipliers[0]))
                {
                    for (int i = 0; i < Builders.Length; i++)
                    {
                        if (Builders[i].Description is null) Builders[i].WithDescription(new CustomNameInfo());
                        Builders[i].Description[code] = FormatValues(translation, i);
                    }
                    return this;
                }
                for (int i = 0; i < Builders.Length; i++)
                {
                    if (Builders[i].Description is null) Builders[i].WithDescription(new CustomNameInfo());
                    Builders[i].Description[code] = FormatDescription(translation, Multipliers[i]);
                }
                return this;
            }
            public AToMMutatorGroupBuilder InCategory(AToMCategory category)
            {
                category.AddRange(GetMutators());
                return this;
            }
        }
        private static AToMMutatorGroupBuilder CreateMutatorGroup(MultiplierType type, float[] multipliers)
        {
            UnlockBuilder[] builders = new UnlockBuilder[multipliers.Length];
            for (int i = 0; i < builders.Length; i++)
            {
                float multiplier = multipliers[i];
                UnlockBuilder builder = RogueLibs.CreateCustomUnlock(new AToMMutator(type, multiplier));
                builders[i] = builder;
                builder.Unlock.UnlockCost = FindUnlockCost(multiplier);
            }
            for (int i = 0; i < builders.Length; i++)
            {
                int prerequisite = FindPrerequisiteIndex(multipliers, i);
                if (prerequisite != -1) builders[i].Unlock.Prerequisites.Add(builders[prerequisite].Unlock.Name);
            }
            return new AToMMutatorGroupBuilder(builders);
        }
        private static AToMMutatorGroupBuilder CreateMutatorGroup<T>(MultiplierType type, T[] values, int[] unlockCosts)
            where T : struct, Enum
        {
            UnlockBuilder[] builders = new UnlockBuilder[values.Length];
            for (int i = 0; i < builders.Length; i++)
            {
                T value = values[i];
                UnlockBuilder builder = RogueLibs.CreateCustomUnlock(new AToMMutator(values[i].ToString(), type));
                builders[i] = builder;
                builder.Unlock.UnlockCost = unlockCosts[i];
            }
            return new AToMMutatorGroupBuilder(builders);
        }
        private static AToMMutatorGroupBuilder CreateSingleMutator(MultiplierType type, int unlockCost)
        {
            UnlockBuilder builder = new UnlockBuilder(new AToMMutator(string.Empty, type));
            builder.Unlock.UnlockCost = unlockCost;
            return new AToMMutatorGroupBuilder(new UnlockBuilder[1] { builder });
        }

        public void Awake()
        {
            RogueLibs.CreateCustomName("CategoryShow", "Interface", new CustomNameInfo(" (show)"));
            RogueLibs.CreateCustomName("CategoryHide", "Interface", new CustomNameInfo(" (hide)"));
            RogueLibs.CreateCustomName("CategoryShow", "Description", new CustomNameInfo("Reveals {0} other mutators."));
            RogueLibs.CreateCustomName("CategoryHide", "Description", new CustomNameInfo("Conceals {0} other mutators."));

            AToMCategory meleeCategory = CreateCategory("Melee", new CustomNameInfo
            {
                English = "MELEE MUTATORS",
            });
            CreateMutatorGroup(MultiplierType.MeleeDamage, new float[]
                                   { 0f, float.Epsilon, 0.125f, 0.25f, 0.5f, 2f, 4f, 8f, 999f })
                .WithName(LanguageCode.English, "Melee Damage")
                .WithDescription(LanguageCode.English, "All melee weapons deal [no|one|.x more|.x less] damage.")
                .InCategory(meleeCategory);
            CreateMutatorGroup(MultiplierType.MeleeDurability, new float[]
                                   { float.Epsilon, 0.125f, 0.25f, 0.5f, 2f, 4f, 8f, 999f })
                .WithName(LanguageCode.English, "Melee Durability")
                .WithDescription(LanguageCode.English, "All melee weapons have [|one|.x more|.x less] durability.")
                .InCategory(meleeCategory);
            CreateMutatorGroup(MultiplierType.MeleeLunge, new float[]
                                   { 0f, 0.125f, 0.25f, 0.5f, 2f, 4f, 8f })
                .WithName(LanguageCode.English, "Melee Lunge")
                .WithDescription(LanguageCode.English, "All melee weapons [don't lunge||have .x longer lunges|have .x shorter lunges].")
                .InCategory(meleeCategory);
            CreateMutatorGroup(MultiplierType.MeleeSpeed, new float[]
                                   { 0.125f, 0.25f, 0.5f, 2f, 4f, 8f, 999f })
                .WithName(LanguageCode.English, "Melee Speed")
                .WithDescription(LanguageCode.English, "All melee weapons attack [||.x faster|.x slower].")
                .InCategory(meleeCategory);

            AToMCategory thrownCategory = CreateCategory("Thrown", new CustomNameInfo
            {
                English = "THROWN MUTATORS",
            });
            CreateMutatorGroup(MultiplierType.ThrownDamage, new float[]
                                   { 0f, float.Epsilon, 0.125f, 0.25f, 0.5f, 2f, 4f, 8f, 999f })
                .WithName(LanguageCode.English, "Thrown Damage")
                .WithDescription(LanguageCode.English, "All thrown weapons deal [no|one|.x more|.x less] damage.")
                .InCategory(thrownCategory);
            CreateMutatorGroup(MultiplierType.ThrownCount, new float[]
                                   { float.Epsilon, 0.125f, 0.25f, 0.5f, 2f, 4f, 8f, 999f })
                .WithName(LanguageCode.English, "Thrown Count")
                .WithDescription(LanguageCode.English, "All thrown weapons appear in [|stacks of one|.x larger stacks|.x smaller stacks].")
                .InCategory(thrownCategory);
            CreateMutatorGroup(MultiplierType.ThrownDistance, new float[]
                                   { 0.125f, 0.25f, 0.5f, 2f, 4f, 8f })
                .WithName(LanguageCode.English, "Thrown Distance")
                .WithDescription(LanguageCode.English, "All thrown weapons can be thrown at [||.x smaller|.x larger] distance.")
                .InCategory(thrownCategory);

            AToMCategory rangedCategory = CreateCategory("Ranged", new CustomNameInfo
            {
                English = "RANGED MUTATORS",
            });
            CreateMutatorGroup(MultiplierType.RangedDamage, new float[]
                                   { 0f, float.Epsilon, 0.125f, 0.25f, 0.5f, 2f, 4f, 8f, 999f })
                .WithName(LanguageCode.English, "Ranged Damage")
                .WithDescription(LanguageCode.English, "All ranged weapons deal [no|one|.x more|.x less] damage.")
                .InCategory(rangedCategory);
            CreateMutatorGroup(MultiplierType.RangedAmmo, new float[]
                                   { 0f, float.Epsilon, 0.125f, 0.25f, 0.5f, 2f, 4f, 8f, 999f })
                .WithName(LanguageCode.English, "Ranged Ammo")
                .WithDescription(LanguageCode.English, "All ranged weapons have [|one|.x more|.x less] ammo.")
                .InCategory(rangedCategory);
            CreateMutatorGroup(MultiplierType.RangedFireRate, new float[]
                                   { 0.125f, 0.25f, 0.5f, 2f, 4f, 8f, 999f })
                .WithName(LanguageCode.English, "Ranged Ammo")
                .WithDescription(LanguageCode.English, "All ranged weapons fire [||.x faster|.x slower].")
                .InCategory(rangedCategory);
            CreateSingleMutator(MultiplierType.RangedFullAuto, 4)
                .WithName(LanguageCode.English, "Fully Automatic Ranged Weapons")
                .WithDescription(LanguageCode.English, "All ranged weapons are fully automatic.")
                .InCategory(rangedCategory);

            AToMCategory projectileCategory = CreateCategory("Projectile", new CustomNameInfo
            {
                English = "PROJECTILE MUTATORS",
            });
            CreateMutatorGroup(MultiplierType.ProjectileSpeed, new float[]
                                   { 0f, 0.125f, 0.25f, 0.5f, 2f, 4f, 8f })
                .WithName(LanguageCode.English, "Projectile Speed")
                .WithDescription(LanguageCode.English, "All projectiles travel at [zero||.x greater|.x lower] speed.")
                .InCategory(projectileCategory);
            CreateMutatorGroup(MultiplierType.ProjectileType, new ProjectileType[]
                                   { ProjectileType.Rocket, ProjectileType.Random, ProjectileType.RandomEffect },
                               new int[] { 8, 8, 8 })
                .WithName(LanguageCode.English, "[Rocket|Random|Random Effect] Projectiles")
                .WithDescription(LanguageCode.English, "ALL projectiles are [rockets|random|Water Pistol bullets with random effects].")
                .InCategory(projectileCategory);

            AToMCategory explosionCategory = CreateCategory("Explosion", new CustomNameInfo
            {
                English = "EXPLOSION MUTATORS",
            });
            CreateMutatorGroup(MultiplierType.ExplosionDamage, new float[]
                                   { 0f, float.Epsilon, 0.125f, 0.25f, 0.5f, 2f, 4f, 8f })
                .WithName(LanguageCode.English, "Explosion Damage")
                .WithDescription(LanguageCode.English, "All explosions deal [zero|one|.x more|.x less] damage.")
                .InCategory(explosionCategory);
            CreateMutatorGroup(MultiplierType.ExplosionPower, new float[]
                                   { 0.125f, 0.25f, 0.5f, 2f, 4f, 8f })
                .WithName(LanguageCode.English, "Explosion Power")
                .WithDescription(LanguageCode.English, "All explosions are [||.x larger|.x smaller].")
                .InCategory(explosionCategory);

            Logger.LogInfo($"There are currently {Categories.Sum(c => c.Mutators.Count)} mutators in aToM.");

            new AToMPatches().Patch(this);

        }
    }
    public class AToMPatches
    {
        public void Patch(AToMPlugin caller)
        {
            RoguePatcher patcher = new RoguePatcher(caller, GetType());

            patcher.Postfix(typeof(InvItem), "SetupDetails", new Type[] { typeof(bool) });
            patcher.Prefix(typeof(Movement), "KnockForward", new Type[] { typeof(Quaternion), typeof(float), typeof(bool) });
            patcher.Postfix(typeof(Melee), "Attack", new Type[] { typeof(bool) });
            patcher.Postfix(typeof(Bullet), "SetupBullet", Type.EmptyTypes);
            patcher.Prefix(typeof(SpawnerMain), "SpawnBullet", new Type[] { typeof(Vector3), typeof(bulletStatus), typeof(PlayfieldObject), typeof(int) });
            patcher.Postfix(typeof(Gun), "SetWeaponCooldown", new Type[] { typeof(float), typeof(InvItem) });
            patcher.Postfix(typeof(Explosion), "SetupExplosion", Type.EmptyTypes);
            patcher.Prefix(typeof(Gun), "spawnBullet", new Type[] { typeof(bulletStatus), typeof(InvItem), typeof(int), typeof(bool), typeof(string) });

        }

        public static void InvItem_SetupDetails(InvItem __instance)
        {
            if (__instance.itemType == "WeaponMelee")
            {
                Multiplier(ref __instance.meleeDamage, GetMultiplier(MultiplierType.MeleeDamage));
                float durability = GetMultiplier(MultiplierType.MeleeDurability);
                Multiplier(ref __instance.initCount, durability);
                Multiplier(ref __instance.initCountAI, durability);
                Multiplier(ref __instance.rewardCount, durability);
            }
            else if (__instance.itemType == "WeaponThrown")
            {
                Multiplier(ref __instance.throwDamage, GetMultiplier(MultiplierType.ThrownDamage));
                float count = GetMultiplier(MultiplierType.ThrownCount);
                Multiplier(ref __instance.initCount, count);
                Multiplier(ref __instance.initCountAI, count);
                Multiplier(ref __instance.rewardCount, count);
                Multiplier(ref __instance.throwDistance, GetMultiplier(MultiplierType.ThrownDistance));
            }
            else if (__instance.itemType == "WeaponProjectile")
            {
                if (__instance.invItemName != "Taser")
                {
                    float ammo = GetMultiplier(MultiplierType.RangedAmmo);
                    Multiplier(ref __instance.initCount, ammo);
                    Multiplier(ref __instance.initCountAI, ammo);
                    Multiplier(ref __instance.rewardCount, ammo);
                    Multiplier(ref __instance.itemValue, 1f / ammo);
                }
                if (IsMutatorActive(MultiplierType.RangedFullAuto)) __instance.rapidFire = true;
            }
        }
        public static void Movement_KnockForward(ref float strength) => Multiplier(ref strength, MultiplierType.MeleeLunge);
        public static void Melee_Attack(Melee __instance)
        {
            float speed = GetMultiplier(MultiplierType.MeleeSpeed);
            __instance.agent.weaponCooldown /= speed;
            __instance.meleeContainerAnim.speed *= speed;
        }
        public static void Bullet_SetupBullet(Bullet __instance)
        {
            Multiplier(ref __instance.damage, MultiplierType.RangedDamage);
            Multiplier(ref __instance.speed, MultiplierType.ProjectileSpeed);
        }
        public static void SpawnerMain_SpawnBullet(ref bulletStatus bulletType)
        {
            ProjectileType type = GetProjectileType();
            if (type == ProjectileType.Rocket)
                bulletType = bulletStatus.Rocket;
            else if (type == ProjectileType.Random)
            {
                do bulletType = (bulletStatus)new System.Random().Next(1, 22); // [1;21]
                while (bulletType == bulletStatus.ZombieSpit || bulletType == bulletStatus.Laser || bulletType == bulletStatus.MindControl);
            }
        }
        public static void Gun_SetWeaponCooldown(Gun __instance)
        {
            InverseMultiplier(ref __instance.agent.weaponCooldown, MultiplierType.RangedFireRate);
            __instance.agent.weaponCooldown = Mathf.Max(__instance.agent.weaponCooldown, 0.05f);
        }
        public static void Explosion_SetupExplosion(Explosion __instance)
        {
            Multiplier(ref __instance.damage, MultiplierType.ExplosionDamage);
            __instance.circleCollider2D.radius *= Mathf.Sqrt(GetMultiplier(MultiplierType.ExplosionPower));
        }

        private static readonly string[] RandomEffects =
        {
            "Poisoned", "Drunk", "Slow", "Fast", "Strength", "Weak", "Paralyzed", "Accurate",
            "RegenerateHealth", "Acid", "Invincible", "Invisible", "Confused", "FeelingUnlucky",
            "Resurrection", "AlwaysCrit", "Shrunk", "ElectroTouch", "BadVision", "BlockDebuffs",
            "DecreaseAllStats", "Dizzy", "DizzyB", "Enraged", "FeelingLucky", "WerewolfEffect",
            "Frozen", "HearingBlocked", "Nicotine", "Tranquilized", "Electrocuted", "Giant",
            "IncreaseAllStats", "KillerThrower", "MindControlled", "NiceSmelling", "Cyanide",
        };
        public static void Gun_spawnBullet(ref bulletStatus bulletType, ref string myStatusEffect)
        {
            ProjectileType type = GetProjectileType();
            if (type == ProjectileType.RandomEffect || (bulletType == bulletStatus.WaterPistol && string.IsNullOrEmpty(myStatusEffect)))
            {
                bulletType = bulletStatus.WaterPistol;
                int rand = new System.Random().Next(RandomEffects.Length);
                myStatusEffect = RandomEffects[rand];
            }
        }

        public static void Multiplier(ref int value, MultiplierType type) => value = (int)Mathf.Ceil(value * GetMultiplier(type));
        public static void Multiplier(ref int value, float multiplier) => value = (int)Mathf.Ceil(value * multiplier);
        public static void Multiplier(ref float value, MultiplierType type) => value *= GetMultiplier(type);
        public static void Multiplier(ref float value, float multiplier) => value *= multiplier;
        public static void InverseMultiplier(ref int value, MultiplierType type) => value = (int)Mathf.Ceil(value / GetMultiplier(type));
        public static void InverseMultiplier(ref int value, float multiplier) => value = (int)Mathf.Ceil(value / multiplier);
        public static void InverseMultiplier(ref float value, MultiplierType type) => value /= GetMultiplier(type);
        public static void InverseMultiplier(ref float value, float multiplier) => value /= multiplier;

        private static string GetMultiplierString(MultiplierType type)
        {
            string mutatorName = type.ToString();
            string ch = GameController.gameController.challenges.Find(c => c.StartsWith("aToM:" + mutatorName, StringComparison.InvariantCulture));
            return ch?.Substring("aToM:".Length + mutatorName.Length);
        }
        public static float GetMultiplier(MultiplierType type)
        {
            string ch = GetMultiplierString(type);
            if (ch is null) return 1f;
            if (ch == "1") return Mathf.Epsilon;
            if (ch[0] == '0') ch = "0." + ch.Substring(1);
            return float.Parse(ch, CultureInfo.InvariantCulture);
        }
        public static bool IsMutatorActive(MultiplierType type) => GetMultiplierString(type) != null;
        public static ProjectileType GetProjectileType()
        {
            string str = GetMultiplierString(MultiplierType.ProjectileType);
            return str is null ? ProjectileType.Normal : (ProjectileType)Enum.Parse(typeof(ProjectileType), str);
        }

    }
    public sealed class AToMCategory : MutatorUnlock
    {
        public AToMCategory(string name, int sortingOrder) : base(name, true)
        {
            IgnoreStateSorting = true;
            SortingOrder = sortingOrder;
            SortingIndex = -1;
            IsAvailable = true;
            Mutators = new ReadOnlyCollection<AToMMutator>(_mutators);
        }
        private readonly List<AToMMutator> _mutators = new List<AToMMutator>();
        public ReadOnlyCollection<AToMMutator> Mutators { get; }

        public void AddRange(IEnumerable<AToMMutator> mutators)
        {
            foreach (AToMMutator mutator in mutators)
            {
                _mutators.Add(mutator);
                mutator.Category = this;
                mutator.IgnoreStateSorting = true;
                mutator.SortingOrder = SortingOrder;
                mutator.SortingIndex = orderIncrement++;
                mutator.IsAvailable = isExpanded;
            }
        }

        private int orderIncrement;
        private bool isExpanded;
        public bool IsExpanded
        {
            get => isExpanded;
            set
            {
                if (isExpanded == value) return;
                isExpanded = value;
                if (value) AToMPlugin.HideAllCategoriesExceptFor(this);
                _mutators.ForEach(m => m.IsAvailable = value);
            }
        }

        public override void UpdateButton()
        {
            Text = GetFancyName();
            if (IsUnlocked)
                State = IsExpanded ? UnlockButtonState.Selected : UnlockButtonState.Normal;
            else if (Unlock.nowAvailable && UnlockCost <= gc.sessionDataBig.nuggets)
                State = UnlockButtonState.Purchasable;
            else State = UnlockButtonState.Locked;
        }
        public override void OnPushedButton()
        {
            if (IsUnlocked)
            {
                if (gc.serverPlayer)
                {
                    PlaySound(VanillaAudio.ClickButton);
                    IsExpanded = !IsExpanded;
                    //UpdateButton();
                    //UpdateAllUnlocks();
                    //UpdateMenu();
                    GameController.gameController.mainGUI.scrollingMenuScript.OpenScrollingMenu();
                }
                else PlaySound(VanillaAudio.CantDo);
            }
            else if (Unlock.nowAvailable && UnlockCost <= gc.sessionDataBig.nuggets)
            {
                PlaySound(VanillaAudio.BuyUnlock);
                gc.unlocks.SubtractNuggets(UnlockCost);
                gc.unlocks.DoUnlockForced(Name, Type);
                UpdateAllUnlocks();
                UpdateMenu();
            }
            else PlaySound(VanillaAudio.CantDo);
        }
        public override string GetFancyName()
        {
            string name = base.GetFancyName();
            if (IsUnlocked || Menu.ShowLockedUnlocks)
                name += gc.nameDB.GetName(IsExpanded ? "CategoryHide" : "CategoryShow", "Interface");
            return name;
        }
        public override string GetDescription()
            => gc.nameDB.GetName(IsExpanded ? "CategoryHide" : "CategoryShow", "Description")
                 .Replace("{0}", _mutators.Count.ToString(CultureInfo.InvariantCulture));

    }
    public class AToMMutator : MutatorUnlock
    {
        public AToMMutator(MultiplierType type, float value) : base(CreateId(type, value))
        {
            MultiplierType = type;
            MultiplierValue = value;
        }
        public AToMMutator(string valueName, MultiplierType type) : base("aToM:" + type + valueName)
        {
            MultiplierType = type;
            MultiplierValue = float.NaN;
        }
        private static string CreateId(MultiplierType type, float value)
        {
            string multiplier = value.ToString(CultureInfo.InvariantCulture);
            int dotIndex = multiplier.IndexOf('.');
            if (dotIndex != -1) multiplier = multiplier.Remove(dotIndex, 1);
            return "aToM:" + type + multiplier;
        }

        public MultiplierType MultiplierType { get; }
        public float MultiplierValue { get; }
        public AToMCategory Category { get; internal set; }

        public override void OnPushedButton()
        {
            if (IsUnlocked)
            {
                if (gc.serverPlayer)
                {
                    PlaySound(VanillaAudio.ClickButton);
                    IsEnabled = !IsEnabled;
                    if (IsEnabled)
                    {
                        foreach (DisplayedUnlock du in EnumerateCancellations())
                        {
                            du.IsEnabled = false;
                            du.UpdateButton();
                        }
                        foreach (AToMMutator mutator in Category.Mutators.Where(m => m.MultiplierType == MultiplierType))
                        {
                            if (mutator == this) continue;
                            mutator.IsEnabled = false;
                            mutator.UpdateButton();
                        }
                    }
                    SendAnnouncementInChat(IsEnabled ? "AddedChallenge" : "RemovedChallenge", Name);
                    UpdateButton();
                    UpdateMenu();
                }
                else PlaySound(VanillaAudio.CantDo);
            }
            else if (Unlock.nowAvailable && UnlockCost <= gc.sessionDataBig.nuggets)
            {
                PlaySound(VanillaAudio.BuyUnlock);
                gc.unlocks.SubtractNuggets(UnlockCost);
                gc.unlocks.DoUnlockForced(Name, Type);
                UpdateAllUnlocks();
                UpdateMenu();
            }
            else PlaySound(VanillaAudio.CantDo);
        }
    }
    public enum MultiplierType
    {
        MeleeDamage, MeleeDurability, MeleeLunge, MeleeSpeed,
        ThrownDamage, ThrownCount, ThrownDistance,
        RangedDamage, RangedAmmo, RangedFireRate, RangedFullAuto,
        ProjectileSpeed, ProjectileType,
        ExplosionDamage, ExplosionPower,
    }
    public enum ProjectileType
    {
        Normal, Rocket, Random, RandomEffect,
    }
}