using System;
using System.Linq;
using System.Collections.Generic;
using BepInEx;
using RogueLibsCore;
using UnityEngine;
using System.Text.RegularExpressions;
using BepInEx.Logging;
using System.Globalization;

namespace aTonOfMutators
{
	[BepInPlugin(pluginGuid, pluginName, pluginVersion)]
	[BepInDependency(RogueLibs.pluginGuid, "2.0")]
	public class ATOM : BaseUnityPlugin
	{
		public const string pluginGuid = "abbysssal.streetsofrogue.atonofmutators";
		public const string pluginName = "a Ton of Mutators";
		public const string pluginVersion = "1.3";
		
		public static void CrossConflictAndHide(params CustomMutator[] mutators)
		{
			for (int i = 0; i < mutators.Length; i++)
			{
				for (int j = 0; j < mutators.Length; j++)
					if (i != j)
						mutators[i].Conflicting.Add(mutators[j].Id);
				mutators[i].Available = false;
			}
		}
		public static void EachConflict(string[] conflicts, params CustomMutator[] mutators)
		{
			for (int i = 0; i < mutators.Length; i++)
				mutators[i].Conflicting.AddRange(conflicts);
		}
		
		public CustomMutator[] ConstructMutators(string generalName, string pattern, float[] multipliers)
		{
			CustomMutator[] mutators = new CustomMutator[multipliers.Length];
			for (int i = 0; i < multipliers.Length; i++)
			{
				string multStr = multipliers[i].ToString(CultureInfo.InvariantCulture).Replace(".", string.Empty);
				string id = "aToM:" + generalName.Replace(" ", string.Empty) + multStr;
				string name = "[aToM] " + generalName + (multipliers[i] == 1 ? " =1" : " x" + multipliers[i].ToString(CultureInfo.InvariantCulture));

				Regex regex = new Regex(@"\[(.*)\|(.*)\|(.*)\|(.*)\]");
				string description = regex.Replace(pattern,
					multipliers[i] == 0 ? "$1"
					: multipliers[i] == 1 ? "$2"
					: multipliers[i] > 1 ? "$3" : "$4").Replace(".x", (multipliers[i] > 1 ? multipliers[i] : 1f / multipliers[i]).ToString(CultureInfo.InvariantCulture) + "x");

				mutators[i] = RogueLibs.CreateCustomMutator(id, true, new CustomNameInfo(name), new CustomNameInfo(description));
			}
			return mutators;
		}
		public CustomMutator ConstructMutator(string id, string generalName, string description)
			=> RogueLibs.CreateCustomMutator("aToM:" + id, true, new CustomNameInfo("[aToM] " + generalName), new CustomNameInfo(description));

		public static int uniqueOrder = -3978;

		public void Awake()
		{
			#region Melee Mutators
			CustomMutator showMelee = RogueLibs.CreateCustomMutator("aToM:ShowMelee", true,
				new CustomNameInfo("[aToM] MELEE MUTATORS (show)"),
				new CustomNameInfo(""));
			CustomMutator hideMelee = RogueLibs.CreateCustomMutator("aToM:HideMelee", true,
				new CustomNameInfo("[aToM] MELEE MUTATORS (hide)"),
				new CustomNameInfo(""));
			hideMelee.Available = false;

			CustomMutator[] meleeDamage = ConstructMutators("Melee Damage", "All melee weapons deal [|1|.x more|.x less] damage",
				new float[] { 1f, 0.125f, 0.25f, 0.5f, 2f, 4f, 8f, 999f });
			CrossConflictAndHide(meleeDamage);
			EachConflict(new string[] { "NoMelee" }, meleeDamage);

			CustomMutator[] meleeDurability = ConstructMutators("Melee Durability", "All melee weapons have [|1|.x more|.x less] durability",
				new float[] { 1f, 0.125f, 0.25f, 0.5f, 2f, 4f, 8f });
			CrossConflictAndHide(meleeDurability);
			EachConflict(new string[] { "NoMelee", "InfiniteMeleeDurability" }, meleeDurability);

			CustomMutator[] meleeLunge = ConstructMutators("Melee Lunge", "All melee weapons [don't lunge||have .x longer lunges|have .x shorter lunges]",
				new float[] { 0f, 0.125f, 0.25f, 0.5f, 2f, 4f, 8f });
			CrossConflictAndHide(meleeLunge);
			EachConflict(new string[] { "NoMelee" }, meleeLunge);

			CustomMutator[] meleeSpeed = ConstructMutators("Melee Speed", "All melee weapons attack [||.x faster|.x slower]",
				new float[] { 0.25f, 0.5f, 2f, 4f });
			CrossConflictAndHide(meleeSpeed);
			EachConflict(new string[] { "NoMelee" }, meleeSpeed);

			showMelee.ScrollingMenu_PushedButton = (menu, button) =>
			{
				showMelee.Available = false;
				hideMelee.Available = true;
				foreach (CustomMutator m in meleeDamage.Concat(meleeDurability).Concat(meleeLunge).Concat(meleeSpeed))
					m.Available = true;
				GameController.gameController.mainGUI.scrollingMenuScript.OpenScrollingMenu();
				return false;
			};
			hideMelee.ScrollingMenu_PushedButton = (menu, button) =>
			{
				showMelee.Available = true;
				hideMelee.Available = false;
				foreach (CustomMutator m in meleeDamage.Concat(meleeDurability).Concat(meleeLunge).Concat(meleeSpeed))
					m.Available = false;
				GameController.gameController.mainGUI.scrollingMenuScript.OpenScrollingMenu();
				return false;
			};
			#endregion

			#region Thrown Mutators
			CustomMutator showThrown = RogueLibs.CreateCustomMutator("aToM:ShowThrown", true,
				new CustomNameInfo("[aToM] THROWN MUTATORS (show)"),
				new CustomNameInfo(""));
			CustomMutator hideThrown = RogueLibs.CreateCustomMutator("aToM:HideThrown", true,
				new CustomNameInfo("[aToM] THROWN MUTATORS (hide)"),
				new CustomNameInfo(""));
			hideThrown.Available = false;

			CustomMutator[] thrownDamage = ConstructMutators("Thrown Damage", "All thrown weapons deal [|1|.x more|.x less] damage",
				new float[] { 1f, 0.125f, 0.25f, 0.5f, 2f, 4f, 8f, 999f });
			CrossConflictAndHide(thrownDamage);

			CustomMutator[] thrownCount = ConstructMutators("Thrown Count", "All thrown weapons appear in [|stacks of 1|.x larger stacks|.x smaller stacks]",
				new float[] { 1f, 0.125f, 0.25f, 0.5f, 2f, 4f, 8f });
			CrossConflictAndHide(thrownCount);

			CustomMutator[] thrownDistance = ConstructMutators("Thrown Distance", "All thrown weapons can be thrown at [||.x smaller distance|.x larger distance]",
				new float[] { 0.125f, 0.25f, 0.5f, 2f, 4f, 8f });
			CrossConflictAndHide(thrownDistance);

			showThrown.ScrollingMenu_PushedButton = (menu, button) =>
			{
				showThrown.Available = false;
				hideThrown.Available = true;
				foreach (CustomMutator m in thrownDamage.Concat(thrownCount).Concat(thrownDistance))
					m.Available = true;
				GameController.gameController.mainGUI.scrollingMenuScript.OpenScrollingMenu();
				return false;
			};
			hideThrown.ScrollingMenu_PushedButton = (menu, button) =>
			{
				showThrown.Available = true;
				hideThrown.Available = false;
				foreach (CustomMutator m in thrownDamage.Concat(thrownCount).Concat(thrownDistance))
					m.Available = false;
				GameController.gameController.mainGUI.scrollingMenuScript.OpenScrollingMenu();
				return false;
			};
			#endregion

			#region Ranged Mutators
			CustomMutator showRanged = RogueLibs.CreateCustomMutator("aToM:ShowRanged", true,
				new CustomNameInfo("[aToM] RANGED MUTATORS (show)"),
				new CustomNameInfo(""));
			CustomMutator hideRanged = RogueLibs.CreateCustomMutator("aToM:HideRanged", true,
				new CustomNameInfo("[aToM] RANGED MUTATORS (hide)"),
				new CustomNameInfo(""));
			hideRanged.Available = false;

			CustomMutator[] rangedDamage = ConstructMutators("Ranged Damage", "All ranged weapons deal [|1|.x more|.x less] damage",
				new float[] { 1f, 0.125f, 0.25f, 0.5f, 2f, 4f, 8f, 999f });
			CrossConflictAndHide(rangedDamage);
			EachConflict(new string[] { "NoGuns" }, rangedDamage);

			CustomMutator[] rangedAmmo = ConstructMutators("Ranged Ammo", "All ranged weapons appear with [|1|.x more|.x less] ammo",
				new float[] { 1f, 0.125f, 0.25f, 0.5f, 2f, 4f, 8f });
			CrossConflictAndHide(rangedAmmo);
			EachConflict(new string[] { "NoGuns", "InfiniteAmmo" }, rangedAmmo);

			CustomMutator[] rangedFirerate = ConstructMutators("Ranged Firerate", "All ranged weapons have [||.x faster|.x slower] rate of fire",
				new float[] { 0.125f, 0.25f, 0.5f, 2f, 4f, 8f });
			CrossConflictAndHide(rangedFirerate);
			EachConflict(new string[] { "NoGuns" }, rangedFirerate);

			CustomMutator rangedFullAuto = ConstructMutator("RangedFullAuto", "Fully Automatic Ranged Weapons", "All ranged weapons are fully automatic");
			rangedFullAuto.Available = false;
			EachConflict(new string[] { "NoGuns" }, rangedFullAuto);

			showRanged.ScrollingMenu_PushedButton = (menu, button) =>
			{
				showRanged.Available = false;
				hideRanged.Available = true;
				foreach (CustomMutator m in rangedDamage.Concat(rangedAmmo).Concat(rangedFirerate).Concat(new CustomMutator[] { rangedFullAuto }))
					m.Available = true;
				GameController.gameController.mainGUI.scrollingMenuScript.OpenScrollingMenu();
				return false;
			};
			hideRanged.ScrollingMenu_PushedButton = (menu, button) =>
			{
				showRanged.Available = true;
				hideRanged.Available = false;
				foreach (CustomMutator m in rangedDamage.Concat(rangedAmmo).Concat(rangedFirerate).Concat(new CustomMutator[] { rangedFullAuto }))
					m.Available = false;
				GameController.gameController.mainGUI.scrollingMenuScript.OpenScrollingMenu();
				return false;
			};
			#endregion

			#region Projectile Mutators
			CustomMutator showProjectile = RogueLibs.CreateCustomMutator("aToM:ShowProjectile", true,
				new CustomNameInfo("[aToM] PROJECTILE MUTATORS (show)"),
				new CustomNameInfo(""));
			CustomMutator hideProjectile = RogueLibs.CreateCustomMutator("aToM:HideProjectile", true,
				new CustomNameInfo("[aToM] PROJECTILE MUTATORS (hide)"),
				new CustomNameInfo(""));
			hideProjectile.Available = false;

			CustomMutator[] projectileSpeed = ConstructMutators("Projectile Speed", "All projectiles travel at [zero||.x greater|.x lower] speed",
				new float[] { 0f, 0.125f, 0.25f, 0.5f, 2f, 4f, 8f });
			CrossConflictAndHide(projectileSpeed);

			CustomMutator[] projectileType = new CustomMutator[3];
			projectileType[0] = ConstructMutator("ProjectileTypeRocket", "Rocket Projectiles", "All projectiles are rockets");
			projectileType[1] = ConstructMutator("ProjectileTypeRandom", "Random Projectiles", "All projectiles are random");
			projectileType[2] = ConstructMutator("ProjectileTypeRandomEffect", "Random Effect Projectiles", "All projectiles are Water Pistol bullets with random effects");
			CrossConflictAndHide(projectileType);

			showProjectile.ScrollingMenu_PushedButton = (menu, button) =>
			{
				showProjectile.Available = false;
				hideProjectile.Available = true;
				foreach (CustomMutator m in projectileSpeed.Concat(projectileType))
					m.Available = true;
				GameController.gameController.mainGUI.scrollingMenuScript.OpenScrollingMenu();
				return false;
			};
			hideProjectile.ScrollingMenu_PushedButton = (menu, button) =>
			{
				showProjectile.Available = true;
				hideProjectile.Available = false;
				foreach (CustomMutator m in projectileSpeed.Concat(projectileType))
					m.Available = false;
				GameController.gameController.mainGUI.scrollingMenuScript.OpenScrollingMenu();
				return false;
			};
			#endregion

			#region Explosion Mutators
			CustomMutator showExplosion = RogueLibs.CreateCustomMutator("aToM:ShowExplosion", true,
				new CustomNameInfo("[aToM] EXPLOSION MUTATORS (show)"),
				new CustomNameInfo(""));
			CustomMutator hideExplosion = RogueLibs.CreateCustomMutator("aToM:HideExplosion", true,
				new CustomNameInfo("[aToM] EXPLOSION MUTATORS (hide)"),
				new CustomNameInfo(""));
			hideExplosion.Available = false;
			
			CustomMutator[] explosionDamage = ConstructMutators("Explosion Damage", "All explosions deal [|1|.x more|.x less] damage",
				new float[] { 1f, 0.125f, 0.25f, 0.5f, 2f, 4f, 8f, 999f });
			CrossConflictAndHide(explosionDamage);

			CustomMutator[] explosionPower = ConstructMutators("Explosion Power", "All explosions are [||.x more|.x less] powerful",
				new float[] { 0.125f, 0.25f, 0.5f, 2f, 4f, 8f });
			CrossConflictAndHide(explosionPower);

			showExplosion.ScrollingMenu_PushedButton = (menu, button) =>
			{
				showExplosion.Available = false;
				hideExplosion.Available = true;
				foreach (CustomMutator m in explosionDamage.Concat(explosionPower))
					m.Available = true;
				GameController.gameController.mainGUI.scrollingMenuScript.OpenScrollingMenu();
				return false;
			};
			hideExplosion.ScrollingMenu_PushedButton = (menu, button) =>
			{
				showExplosion.Available = true;
				hideExplosion.Available = false;
				foreach (CustomMutator m in explosionDamage.Concat(explosionPower))
					m.Available = false;
				GameController.gameController.mainGUI.scrollingMenuScript.OpenScrollingMenu();
				return false;
			};
			#endregion

			

			#region Sorting orders and indexes
			int index = 0;
			hideMelee.SortingOrder = showMelee.SortingOrder = ++uniqueOrder;
			foreach (CustomMutator m in meleeDamage.Concat(meleeDurability).Concat(meleeLunge).Concat(meleeDamage))
			{ m.SortingOrder = uniqueOrder; m.SortingIndex = index++; }

			hideThrown.SortingOrder = showThrown.SortingOrder = ++uniqueOrder;
			foreach (CustomMutator m in thrownDamage.Concat(thrownCount).Concat(thrownDistance))
			{ m.SortingOrder = uniqueOrder; m.SortingIndex = index++; }

			hideRanged.SortingOrder = showRanged.SortingOrder = ++uniqueOrder;
			foreach (CustomMutator m in rangedDamage.Concat(rangedAmmo).Concat(rangedFirerate).Concat(new CustomMutator[] { rangedFullAuto }))
			{ m.SortingOrder = uniqueOrder; m.SortingIndex = index++; }

			hideProjectile.SortingOrder = showProjectile.SortingOrder = ++uniqueOrder;
			foreach (CustomMutator m in projectileSpeed.Concat(projectileType))
			{ m.SortingOrder = uniqueOrder; m.SortingIndex = index++; }

			hideExplosion.SortingOrder = showExplosion.SortingOrder = ++uniqueOrder;
			foreach (CustomMutator m in explosionDamage.Concat(explosionPower))
			{ m.SortingOrder = uniqueOrder; m.SortingIndex = index++; }
			#endregion

			RoguePatcher patcher = new RoguePatcher(this, GetType());

			patcher.Postfix(typeof(InvItem), "SetupDetails", new Type[] { typeof(bool) });
			patcher.Prefix(typeof(Movement), "KnockForward", new Type[] { typeof(Quaternion), typeof(float), typeof(bool) });
			patcher.Postfix(typeof(Melee), "Attack", new Type[] { typeof(bool) });
			patcher.Postfix(typeof(Bullet), "SetupBullet", new Type[] { });
			patcher.Prefix(typeof(SpawnerMain), "SpawnBullet", new Type[] { typeof(Vector3), typeof(bulletStatus), typeof(PlayfieldObject), typeof(int) });
			patcher.Postfix(typeof(Gun), "SetWeaponCooldown", new Type[] { typeof(float), typeof(InvItem) });
			patcher.Postfix(typeof(Explosion), "SetupExplosion", new Type[] { });
			patcher.Prefix(typeof(Gun), "spawnBullet", new Type[] { typeof(bulletStatus), typeof(InvItem), typeof(int), typeof(bool), typeof(string) });
		}

		public static void Multiplier(ref int value, float multiplier) => value = (int)Mathf.Ceil(value * multiplier);

		public static void InvItem_SetupDetails(InvItem __instance)
		{
			if (__instance.itemType == "WeaponMelee")
			{
				Multiplier(ref __instance.meleeDamage, GetMultiplier(MultiplierType.MeleeDamage));

				Multiplier(ref __instance.initCount, GetMultiplier(MultiplierType.MeleeDurability));
				Multiplier(ref __instance.initCountAI, GetMultiplier(MultiplierType.MeleeDurability));
				Multiplier(ref __instance.rewardCount, GetMultiplier(MultiplierType.MeleeDurability));
			}
			else if (__instance.itemType == "WeaponThrown")
			{
				Multiplier(ref __instance.throwDamage, GetMultiplier(MultiplierType.ThrownDamage));

				Multiplier(ref __instance.initCount, GetMultiplier(MultiplierType.ThrownCount));
				Multiplier(ref __instance.initCountAI, GetMultiplier(MultiplierType.ThrownCount));
				Multiplier(ref __instance.rewardCount, GetMultiplier(MultiplierType.ThrownCount));

				Multiplier(ref __instance.throwDistance, GetMultiplier(MultiplierType.ThrownDistance));
			}
			else if (__instance.itemType == "WeaponProjectile")
			{
				if (__instance.invItemName != "Taser")
				{
					Multiplier(ref __instance.initCount, GetMultiplier(MultiplierType.RangedAmmo));
					Multiplier(ref __instance.initCountAI, GetMultiplier(MultiplierType.RangedAmmo));
					Multiplier(ref __instance.rewardCount, GetMultiplier(MultiplierType.RangedAmmo));
					Multiplier(ref __instance.itemValue, 1f / GetMultiplier(MultiplierType.RangedAmmo));
				}
				if (GetMultiplier(MultiplierType.RangedFullAuto) == 0f) __instance.rapidFire = true;
			}
		}
		public static void Movement_KnockForward(ref float strength) => strength *= GetMultiplier(MultiplierType.MeleeLunge);
		public static void Melee_Attack(Melee __instance)
		{
			__instance.agent.weaponCooldown /= GetMultiplier(MultiplierType.MeleeSpeed);
			__instance.meleeContainerAnim.speed *= GetMultiplier(MultiplierType.MeleeSpeed);
		}
		public static void Bullet_SetupBullet(Bullet __instance)
		{
			Multiplier(ref __instance.damage, GetMultiplier(MultiplierType.RangedDamage));
			Multiplier(ref __instance.speed, GetMultiplier(MultiplierType.ProjectileSpeed));
		}
		public static void SpawnerMain_SpawnBullet(ref bulletStatus bulletType)
		{
			if ((ProjectileType)(int)GetMultiplier(MultiplierType.ProjectileType) == ProjectileType.Rocket)
				bulletType = bulletStatus.Rocket;
			else if ((ProjectileType)(int)GetMultiplier(MultiplierType.ProjectileType) == ProjectileType.Random)
				do
					bulletType = (bulletStatus)new System.Random().Next(1, 22); // [1;21]
				while (bulletType == bulletStatus.ZombieSpit || bulletType == bulletStatus.Laser || bulletType == bulletStatus.MindControl);
		}
		public static void Gun_SetWeaponCooldown(Gun __instance)
		{
			__instance.agent.weaponCooldown /= GetMultiplier(MultiplierType.RangedFirerate);
			__instance.agent.weaponCooldown = Mathf.Max(__instance.agent.weaponCooldown, 0.05f);
		}
		public static void Explosion_SetupExplosion(Explosion __instance)
		{
			Multiplier(ref __instance.damage, GetMultiplier(MultiplierType.ExplosionDamage));
			__instance.circleCollider2D.radius *= Mathf.Sqrt(GetMultiplier(MultiplierType.ExplosionPower));
		}
		public static void Gun_spawnBullet(ref bulletStatus bulletType, ref string myStatusEffect)
		{
			if ((ProjectileType)(int)GetMultiplier(MultiplierType.ProjectileType) == ProjectileType.RandomEffect || (bulletType == bulletStatus.WaterPistol && string.IsNullOrEmpty(myStatusEffect)))
			{
				bulletType = bulletStatus.WaterPistol;
				List<string> list = new List<string>()
				{
					"Poisoned", "Drunk", "Slow", "Fast", "Strength", "Weak", "Paralyzed", "Accurate",
					"RegenerateHealth", "Acid", "Invincible", "Invisible", "Confused", "FeelingUnlucky",
					"Resurrection", "AlwaysCrit", "Shrunk", "ElectroTouch", "BadVision", "BlockDebuffs",
					"DecreaseAllStats", "Dizzy", "DizzyB", "Enraged", "FeelingLucky", "WerewolfEffect",
					"Frozen", "HearingBlocked", "Nicotine", "Tranquilized", "Electrocuted", "Giant",
					"IncreaseAllStats", "KillerThrower", "MindControlled", "NiceSmelling", "Cyanide"
				};
				int rand = new System.Random().Next(list.Count);
				myStatusEffect = list[rand];
			}
		}

		public static float ExtractNumber(string startsWith)
		{
			string ch = ExtractString(startsWith);
			if (ch == null) return 1f;
			if (ch == "1") return Mathf.Epsilon;
			if (ch[0] == '0') ch = "0." + ch.Substring(1);
			return float.Parse(ch, CultureInfo.InvariantCulture);
		}
		public static string ExtractString(string startsWith)
		{
			string ch = GameController.gameController.challenges.Find(c => c.StartsWith("aToM:" + startsWith));
			if (ch == null) return null;
			return ch.Substring("aToM:".Length + startsWith.Length);
		}
		public static float GetMultiplier(MultiplierType type)
		{
			switch (type)
			{
				case MultiplierType.MeleeDamage: return ExtractNumber("MeleeDamage");
				case MultiplierType.MeleeDurability: return ExtractNumber("MeleeDurability");
				case MultiplierType.MeleeLunge: return ExtractNumber("MeleeLunge");
				case MultiplierType.MeleeSpeed: return ExtractNumber("MeleeSpeed");

				case MultiplierType.ThrownDamage: return ExtractNumber("ThrownDamage");
				case MultiplierType.ThrownCount: return ExtractNumber("ThrownCount");
				case MultiplierType.ThrownDistance: return ExtractNumber("ThrownDistance");

				case MultiplierType.RangedDamage: return ExtractNumber("RangedDamage");
				case MultiplierType.RangedAmmo: return ExtractNumber("RangedAmmo");
				case MultiplierType.RangedFirerate: return ExtractNumber("RangedFirerate");
				case MultiplierType.RangedFullAuto: return ExtractString("RangedFullAuto") != null ? 0f : 1f;

				case MultiplierType.ProjectileSpeed: return ExtractNumber("ProjectileSpeed");
				case MultiplierType.ProjectileType:
					string type2 = ExtractString("ProjectileType");
					if (type2 == "Rocket") return (int)ProjectileType.Rocket;
					else if (type2 == "Random") return (int)ProjectileType.Random;
					else if (type2 == "RandomEffect") return (int)ProjectileType.RandomEffect;
					else return (int)ProjectileType.Normal;

				case MultiplierType.ExplosionDamage: return ExtractNumber("ExplosionDamage");
				case MultiplierType.ExplosionPower: return ExtractNumber("ExplosionPower");
			}
			return 1f;
		}
	}
	public enum MultiplierType
	{
		MeleeDamage, MeleeDurability, MeleeLunge, MeleeSpeed,
		ThrownDamage, ThrownCount, ThrownDistance,
		RangedDamage, RangedAmmo, RangedFirerate, RangedFullAuto,
		ProjectileSpeed, ProjectileType,
		ExplosionDamage, ExplosionPower
	}
	public enum ProjectileType
	{
		Normal, Rocket, Random, RandomEffect
	}
}