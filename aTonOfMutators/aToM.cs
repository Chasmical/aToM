using System;
using System.Collections.Generic;
using BepInEx;
using RogueLibsCore;
using UnityEngine;

namespace aTonOfMutators
{
	[BepInPlugin(pluginGuid, pluginName, pluginVersion)]
	[BepInDependency(RogueLibs.pluginGuid, "2.0")]
	public class ATOM : BaseUnityPlugin
	{
		public class MyRogueUtilities
		{
			public static void CrossConflict(params CustomMutator[] mutators)
			{
				for (int i = 0; i < mutators.Length; i++)
					for (int j = 0; j < mutators.Length; j++)
						if (i != j)
							mutators[i].Conflicting.Add(mutators[j].Id);
			}
			public static void EachConflict(string[] conflicts, params CustomMutator[] mutators)
			{
				for (int i = 0; i < mutators.Length; i++)
					mutators[i].Conflicting.AddRange(conflicts);
			}
		}

		public const string pluginGuid = "abbysssal.streetsofrogue.atom";
		public const string pluginName = "a Ton of Mutators";
		public const string pluginVersion = "1.2";

		public static MutatorCollection MyMutators { get; set; }

		public static CustomMutator MeleeShow { get; set; }
		public static CustomMutator MeleeHide { get; set; }

		public static CustomMutator ThrownShow { get; set; }
		public static CustomMutator ThrownHide { get; set; }

		public static CustomMutator RangedShow { get; set; }
		public static CustomMutator RangedHide { get; set; }

		public static CustomMutator ProjectileShow { get; set; }
		public static CustomMutator ProjectileHide { get; set; }

		public static CustomMutator ExplosionShow { get; set; }
		public static CustomMutator ExplosionHide { get; set; }

		public int UniqueInt = -6245;

		public static int Divide(int dividend, int divisor) => (int)Math.Ceiling((float)dividend / divisor);

		public void Awake()
		{
			MyMutators = new MutatorCollection();

			#region Melee Damage/Durability/Lunge/Speed
			MeleeShow = RogueLibs.CreateCustomMutator("aToM:MeleeShow", true,
				new CustomNameInfo("[aToM] MELEE MUTATORS (show)", null, null, null, null, "[aToM] МУТАТОРЫ БЛИЖНЕГО БОЯ (показать)", null, null),
				new CustomNameInfo("...", null, null, null, null, "...", null, null));
			MeleeHide = RogueLibs.CreateCustomMutator("aToM:MeleeHide", true,
				new CustomNameInfo("[aToM] MELEE MUTATORS (hide)", null, null, null, null, "[aToM] МУТАТОРЫ БЛИЖНЕГО БОЯ (скрыть)", null, null),
				new CustomNameInfo("...", null, null, null, null, "...", null, null));
			MeleeShow.OnToggledInMutatorMenu += (m, b, state) => ToggleMelee(true);
			MeleeHide.OnToggledInMutatorMenu += (m, b, state) => ToggleMelee(false);

			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:MeleeDamage0", true,
				new CustomNameInfo("[aToM] Melee Damage x0", null, null, null, null, "[aToM] Урон оружия ближнего боя x0", null, null),
				new CustomNameInfo("All melee weapons deal zero damage", null, null, null, null, "Всё оружие ближнего боя наносит ноль урона", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:MeleeDamage025", true,
				new CustomNameInfo("[aToM] Melee Damage x0.25", null, null, null, null, "[aToM] Урон оружия ближнего боя x0.25", null, null),
				new CustomNameInfo("All melee weapons deal 4x less damage", null, null, null, null, "Всё оружие ближнего боя наносит в 4x меньше урона", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:MeleeDamage05", true,
				new CustomNameInfo("[aToM] Melee Damage x0.5", null, null, null, null, "[aToM] Урон оружия ближнего боя x0.5", null, null),
				new CustomNameInfo("All melee weapons deal 2x less damage", null, null, null, null, "Всё оружие ближнего боя наносит в 2x меньше урона", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:MeleeDamage2", true,
				new CustomNameInfo("[aToM] Melee Damage x2", null, null, null, null, "[aToM] Урон оружия ближнего боя x2", null, null),
				new CustomNameInfo("All melee weapons deal 2x more damage", null, null, null, null, "Всё оружие ближнего боя наносит в 2x больше урона", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:MeleeDamage4", true,
				new CustomNameInfo("[aToM] Melee Damage x4", null, null, null, null, "[aToM] Урон оружия ближнего боя x4", null, null),
				new CustomNameInfo("All melee weapons deal 4x more damage", null, null, null, null, "Всё оружие ближнего боя наносит в 4x больше урона", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:MeleeDamage8", true,
				new CustomNameInfo("[aToM] Melee Damage x8", null, null, null, null, "[aToM] Урон оружия ближнего боя x8", null, null),
				new CustomNameInfo("All melee weapons deal 8x more damage", null, null, null, null, "Всё оружие ближнего боя наносит в 8x больше урона", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:MeleeDamage999", true,
				new CustomNameInfo("[aToM] Melee Damage x999", null, null, null, null, "[aToM] Урон оружия ближнего боя x999", null, null),
				new CustomNameInfo("All melee weapons deal 999x more damage", null, null, null, null, "Всё оружие ближнего боя наносит в 999x больше урона", null, null)));

			MyMutators.CrossConflict(m => m.Id.StartsWith("aToM:MeleeDamage"));
			MyMutators.EachConflict(m => m.Id.StartsWith("aToM:MeleeDamage"), "NoMelee");

			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:MeleeDurability1", true,
				new CustomNameInfo("[aToM] Melee Durability 1", null, null, null, null, "[aToM] Прочность оружия ближнего боя 1", null, null),
				new CustomNameInfo("All melee weapons appear with 1 durability", null, null, null, null, "Всё оружие ближнего боя появляется с 1 запасом прочности", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:MeleeDurability025", true,
				new CustomNameInfo("[aToM] Melee Durability x0.25", null, null, null, null, "[aToM] Прочность оружия ближнего боя x0.25", null, null),
				new CustomNameInfo("All melee weapons appear with 4x less durability", null, null, null, null, "Всё оружие ближнего боя появляется с в 4x меньшим запасом прочности", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:MeleeDurability05", true,
				new CustomNameInfo("[aToM] Melee Durability x0.5", null, null, null, null, "[aToM] Прочность оружия ближнего боя x0.5", null, null),
				new CustomNameInfo("All melee weapons appear with 2x less durability", null, null, null, null, "Всё оружие ближнего боя появляется с в 2x меньшим запасом прочности", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:MeleeDurability2", true,
				new CustomNameInfo("[aToM] Melee Durability x2", null, null, null, null, "[aToM] Прочность оружия ближнего боя x2", null, null),
				new CustomNameInfo("All melee weapons appear with 2x more durability", null, null, null, null, "Всё оружие ближнего боя появляется с в 2x большим запасом прочности", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:MeleeDurability4", true,
				new CustomNameInfo("[aToM] Melee Durability x4", null, null, null, null, "[aToM] Прочность оружия ближнего боя x4", null, null),
				new CustomNameInfo("All melee weapons appear with 4x more durability", null, null, null, null, "Всё оружие ближнего боя появляется с в 4x большим запасом прочности", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:MeleeDurability8", true,
				new CustomNameInfo("[aToM] Melee Durability x8", null, null, null, null, "[aToM] Прочность оружия ближнего боя x8", null, null),
				new CustomNameInfo("All melee weapons appear with 8x more durability", null, null, null, null, "Всё оружие ближнего боя появляется с в 8x большим запасом прочности", null, null)));

			MyMutators.CrossConflict(m => m.Id.StartsWith("aToM:MeleeDurability"));
			MyMutators.EachConflict(m => m.Id.StartsWith("aToM:MeleeDurability"), "NoMelee", "InfiniteMeleeDurability");

			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:MeleeLunge0", true,
				new CustomNameInfo("[aToM] Melee Lunge x0", null, null, null, null, "[aToM] Выпад оружия ближнего боя x0", null, null),
				new CustomNameInfo("All melee weapons don't lunge", null, null, null, null, "У оружия ближнего боя нет выпадов", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:MeleeLunge025", true,
				new CustomNameInfo("[aToM] Melee Lunge x0.25", null, null, null, null, "[aToM] Выпад оружия ближнего боя x0.25", null, null),
				new CustomNameInfo("All melee weapons have 4x shorter lunge", null, null, null, null, "Выпады у оружия ближнего боя в 4x слабее", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:MeleeLunge05", true,
				new CustomNameInfo("[aToM] Melee Lunge x0.5", null, null, null, null, "[aToM] Выпад оружия ближнего боя x0.5", null, null),
				new CustomNameInfo("All melee weapons have 2x shorter lunge", null, null, null, null, "Выпады у оружия ближнего боя в 2x слабее", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:MeleeLunge2", true,
				new CustomNameInfo("[aToM] Melee Lunge x2", null, null, null, null, "[aToM] Выпад оружия ближнего боя x2", null, null),
				new CustomNameInfo("All melee weapons have 2x longer lunge", null, null, null, null, "Выпады у оружия ближнего боя в 2x дальше", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:MeleeLunge4", true,
				new CustomNameInfo("[aToM] Melee Lunge x4", null, null, null, null, "[aToM] Выпад оружия ближнего боя x4", null, null),
				new CustomNameInfo("All melee weapons have 4x longer lunge", null, null, null, null, "Выпады у оружия ближнего боя в 4x дальше", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:MeleeLunge8", true,
				new CustomNameInfo("[aToM] Melee Lunge x8", null, null, null, null, "[aToM] Выпад оружия ближнего боя x8", null, null),
				new CustomNameInfo("All melee weapons have 8x longer lunge", null, null, null, null, "Выпады у оружия ближнего боя в 8x дальше", null, null)));

			MyMutators.CrossConflict(m => m.Id.StartsWith("aToM:MeleeLunge"));
			MyMutators.EachConflict(m => m.Id.StartsWith("aToM:MeleeLunge"), "NoMelee");
			
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:MeleeSpeed025", true,
				new CustomNameInfo("[aToM] Melee Speed x0.25", null, null, null, null, "[aToM] Скорость оружия ближнего боя x0.25", null, null),
				new CustomNameInfo("All melee weapons attack 4x slower", null, null, null, null, "Всё оружие ближнего боя атакует в 4x медленнее", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:MeleeSpeed05", true,
				new CustomNameInfo("[aToM] Melee Speed x0.5", null, null, null, null, "[aToM] Скорость оружия ближнего боя x0.5", null, null),
				new CustomNameInfo("All melee weapons attack 2x slower", null, null, null, null, "Всё оружие ближнего боя атакует в 2x медленнее", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:MeleeSpeed2", true,
				new CustomNameInfo("[aToM] Melee Speed x2", null, null, null, null, "[aToM] Скорость оружия ближнего боя x2", null, null),
				new CustomNameInfo("All melee weapons attack 2x faster", null, null, null, null, "Всё оружие ближнего боя атакует в 2x быстрее", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:MeleeSpeed4", true,
				new CustomNameInfo("[aToM] Melee Speed x4", null, null, null, null, "[aToM] Скорость оружия ближнего боя x4", null, null),
				new CustomNameInfo("All melee weapons attack 4x faster", null, null, null, null, "Всё оружие ближнего боя атакует в 4x быстрее", null, null)));

			MyMutators.CrossConflict(m => m.Id.StartsWith("aToM:MeleeSpeed"));
			MyMutators.EachConflict(m => m.Id.StartsWith("aToM:MeleeSpeed"), "NoMelee");

			int order = UniqueInt + 1;
			MeleeShow.SortingOrder = MeleeHide.SortingOrder = order;
			MeleeShow.SortingIndex = MeleeHide.SortingIndex = 0;
			MeleeHide.Available = false;
			MyMutators.NumAndHide(m => m.Id.StartsWith("aToM:Melee"), order);
			#endregion

			#region Thrown Damage/Count/Distance
			ThrownShow = RogueLibs.CreateCustomMutator("aToM:ThrownShow", true,
				new CustomNameInfo("[aToM] THROWN MUTATORS (show)", null, null, null, null, "[aToM] МУТАТОРЫ КИДАТЕЛЬНОГО ОРУЖИЯ (показать)", null, null),
				new CustomNameInfo("...", null, null, null, null, "...", null, null));
			ThrownHide = RogueLibs.CreateCustomMutator("aToM:ThrownHide", true,
				new CustomNameInfo("[aToM] THROWN MUTATORS (hide)", null, null, null, null, "[aToM] МУТАТОРЫ КИДАТЕЛЬНОГО ОРУЖИЯ (скрыть)", null, null),
				new CustomNameInfo("...", null, null, null, null, "...", null, null));
			ThrownShow.OnToggledInMutatorMenu += (m, b, state) => ToggleThrown(true);
			ThrownHide.OnToggledInMutatorMenu += (m, b, state) => ToggleThrown(false);

			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:ThrownDamage0", true,
				new CustomNameInfo("[aToM] Thrown Damage x0", null, null, null, null, "[aToM] Урон кидательного оружия x0", null, null),
				new CustomNameInfo("All thrown weapons deal zero damage", null, null, null, null, "Всё кидательное оружие наносит ноль урона", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:ThrownDamage025", true,
				new CustomNameInfo("[aToM] Thrown Damage x0.25", null, null, null, null, "[aToM] Урон кидательного оружия x0.25", null, null),
				new CustomNameInfo("All thrown weapons deal 4x less damage", null, null, null, null, "Всё кидательное оружие наносит в 4x меньше урона", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:ThrownDamage05", true,
				new CustomNameInfo("[aToM] Thrown Damage x0.5", null, null, null, null, "[aToM] Урон кидательного оружия x0.5", null, null),
				new CustomNameInfo("All thrown weapons deal 2x less damage", null, null, null, null, "Всё кидательное оружие наносит в 2x меньше урона", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:ThrownDamage2", true,
				new CustomNameInfo("[aToM] Thrown Damage x2", null, null, null, null, "[aToM] Урон кидательного оружия x2", null, null),
				new CustomNameInfo("All thrown weapons deal 2x more damage", null, null, null, null, "Всё кидательное оружие наносит в 2x больше урона", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:ThrownDamage4", true,
				new CustomNameInfo("[aToM] Thrown Damage x4", null, null, null, null, "[aToM] Урон кидательного оружия x4", null, null),
				new CustomNameInfo("All thrown weapons deal 4x more damage", null, null, null, null, "Всё кидательное оружие наносит в 4x больше урона", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:ThrownDamage8", true,
				new CustomNameInfo("[aToM] Thrown Damage x8", null, null, null, null, "[aToM] Урон кидательного оружия x8", null, null),
				new CustomNameInfo("All thrown weapons deal 8x more damage", null, null, null, null, "Всё кидательное оружие наносит в 8x больше урона", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:ThrownDamage999", true,
				new CustomNameInfo("[aToM] Thrown Damage x999", null, null, null, null, "[aToM] Урон кидательного оружия x999", null, null),
				new CustomNameInfo("All thrown weapons deal 999x more damage", null, null, null, null, "Всё кидательное оружие наносит в 999x больше урона", null, null)));

			MyMutators.CrossConflict(m => m.Id.StartsWith("aToM:ThrownDamage"));

			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:ThrownCount025", true,
				new CustomNameInfo("[aToM] Thrown Count x0.25", null, null, null, null, "[aToM] Количество кидательного оружия x0.25", null, null),
				new CustomNameInfo("All thrown weapons appear in 4x smaller stacks", null, null, null, null, "Количество кидательного оружия в стаках в 4x меньше", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:ThrownCount05", true,
				new CustomNameInfo("[aToM] Thrown Count x0.5", null, null, null, null, "[aToM] Количество кидательного оружия x0.5", null, null),
				new CustomNameInfo("All thrown weapons appear in 2x smaller stacks", null, null, null, null, "Количество кидательного оружия в стаках в 2x меньше", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:ThrownCount2", true,
				new CustomNameInfo("[aToM] Thrown Count x2", null, null, null, null, "[aToM] Количество кидательного оружия x2", null, null),
				new CustomNameInfo("All thrown weapons appear in 2x larger stacks", null, null, null, null, "Количество кидательного оружия в стаках в 2x больше", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:ThrownCount4", true,
				new CustomNameInfo("[aToM] Thrown Count x4", null, null, null, null, "[aToM] Количество кидательного оружия x4", null, null),
				new CustomNameInfo("All thrown weapons appear in 4x larger stacks", null, null, null, null, "Количество кидательного оружия в стаках в 4x больше", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:ThrownCount8", true,
				new CustomNameInfo("[aToM] Thrown Count x8", null, null, null, null, "[aToM] Количество кидательного оружия x8", null, null),
				new CustomNameInfo("All thrown weapons appear in 8x larger stacks", null, null, null, null, "Количество кидательного оружия в стаках в 8x больше", null, null)));

			MyMutators.CrossConflict(m => m.Id.StartsWith("aToM:ThrownCount"));

			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:ThrownDistance025", true,
				new CustomNameInfo("[aToM] Thrown Distance x0.25", null, null, null, null, "[aToM] Дальность кидательного оружия x0.25", null, null),
				new CustomNameInfo("All thrown weapons can be thrown at 4x smaller distance", null, null, null, null, "Всё кидательное оружие можно кидать на в 4x меньшее расстояние", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:ThrownDistance05", true,
				new CustomNameInfo("[aToM] Thrown Distance x0.5", null, null, null, null, "[aToM] Дальность кидательного оружия x0.5", null, null),
				new CustomNameInfo("All thrown weapons can be thrown at 2x smaller distance", null, null, null, null, "Всё кидательное оружие можно кидать на в 2x меньшее расстояние", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:ThrownDistance2", true,
				new CustomNameInfo("[aToM] Thrown Distance x2", null, null, null, null, "[aToM] Дальность кидательного оружия x2", null, null),
				new CustomNameInfo("All thrown weapons can be thrown at 2x greater distance", null, null, null, null, "Всё кидательное оружие можно кидать на в 2x большее расстояние", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:ThrownDistance4", true,
				new CustomNameInfo("[aToM] Thrown Distance x4", null, null, null, null, "[aToM] Дальность кидательного оружия x4", null, null),
				new CustomNameInfo("All thrown weapons can be thrown at 4x greater distance", null, null, null, null, "Всё кидательное оружие можно кидать на в 4x большее расстояние", null, null)));

			MyMutators.CrossConflict(m => m.Id.StartsWith("aToM:ThrownDistance"));

			order = UniqueInt + 2;
			ThrownShow.SortingOrder = ThrownHide.SortingOrder = order;
			ThrownShow.SortingIndex = ThrownHide.SortingIndex = 0;
			ThrownHide.Available = false;
			MyMutators.NumAndHide(m => m.Id.StartsWith("aToM:Thrown"), order);
			#endregion

			#region Ranged Damage/Ammo/Firerate/Fully Automatic
			RangedShow = RogueLibs.CreateCustomMutator("aToM:RangedShow", true,
				new CustomNameInfo("[aToM] RANGED MUTATORS (show)", null, null, null, null, "[aToM] МУТАТОРЫ ДАЛЬНЕГО БОЯ (показать)", null, null),
				new CustomNameInfo("...", null, null, null, null, "...", null, null));
			RangedHide = RogueLibs.CreateCustomMutator("aToM:RangedHide", true,
				new CustomNameInfo("[aToM] RANGED MUTATORS (hide)", null, null, null, null, "[aToM] МУТАТОРЫ ДАЛЬНЕГО БОЯ (скрыть)", null, null),
				new CustomNameInfo("...", null, null, null, null, "...", null, null));
			RangedShow.OnToggledInMutatorMenu += (m, b, state) => ToggleRanged(true);
			RangedHide.OnToggledInMutatorMenu += (m, b, state) => ToggleRanged(false);

			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:RangedDamage0", true,
				new CustomNameInfo("[aToM] Ranged Damage x0", null, null, null, null, "[aToM] Урон оружия дальнего боя x0", null, null),
				new CustomNameInfo("All ranged weapons deal zero damage", null, null, null, null, "Всё оружие дальнего боя наносит ноль урона", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:RangedDamage025", true,
				new CustomNameInfo("[aToM] Ranged Damage x0.25", null, null, null, null, "[aToM] Урон оружия дальнего боя x0.25", null, null),
				new CustomNameInfo("All ranged weapons deal 4x less damage", null, null, null, null, "Всё оружие дальнего боя наносит в 4x меньше урона", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:RangedDamage05", true,
				new CustomNameInfo("[aToM] Ranged Damage x0.5", null, null, null, null, "[aToM] Урон оружия дальнего боя x0.5", null, null),
				new CustomNameInfo("All ranged weapons deal 2x less damage", null, null, null, null, "Всё оружие дальнего боя наносит в 2x меньше урона", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:RangedDamage2", true,
				new CustomNameInfo("[aToM] Ranged Damage x2", null, null, null, null, "[aToM] Урон оружия дальнего боя x2", null, null),
				new CustomNameInfo("All ranged weapons deal 2x more damage", null, null, null, null, "Всё оружие дальнего боя наносит в 2x больше урона", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:RangedDamage4", true,
				new CustomNameInfo("[aToM] Ranged Damage x4", null, null, null, null, "[aToM] Урон оружия дальнего боя x4", null, null),
				new CustomNameInfo("All ranged weapons deal 4x more damage", null, null, null, null, "Всё оружие дальнего боя наносит в 4x больше урона", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:RangedDamage8", true,
				new CustomNameInfo("[aToM] Ranged Damage x8", null, null, null, null, "[aToM] Урон оружия дальнего боя x8", null, null),
				new CustomNameInfo("All ranged weapons deal 8x more damage", null, null, null, null, "Всё оружие дальнего боя наносит в 8x больше урона", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:RangedDamage999", true,
				new CustomNameInfo("[aToM] Ranged Damage x999", null, null, null, null, "[aToM] Урон оружия дальнего боя x999", null, null),
				new CustomNameInfo("All ranged weapons deal 999x more damage", null, null, null, null, "Всё оружие дальнего боя наносит в 999x больше урона", null, null)));

			MyMutators.CrossConflict(m => m.Id.StartsWith("aToM:RangedDamage"));
			MyMutators.EachConflict(m => m.Id.StartsWith("aToM:RangedDamage"), "NoGuns");

			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:RangedAmmo1", true,
				new CustomNameInfo("[aToM] Ranged Ammo 1", null, null, null, null, "[aToM] Запас аммуниции оружия дальнего боя 1", null, null),
				new CustomNameInfo("All ranged weapons appear with 1 ammo", null, null, null, null, "Всё оружие дальнего боя появляется с 1 запасом аммуниции", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:RangedAmmo025", true,
				new CustomNameInfo("[aToM] Ranged Ammo x0.25", null, null, null, null, "[aToM] Запас аммуниции оружия дальнего боя x0.25", null, null),
				new CustomNameInfo("All ranged weapons appear with 4x less ammo", null, null, null, null, "Всё оружие дальнего боя появляется с в 4x меньшим запасом аммуниции", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:RangedAmmo05", true,
				new CustomNameInfo("[aToM] Ranged Ammo x0.5", null, null, null, null, "[aToM] Запас аммуниции оружия дальнего боя x0.5", null, null),
				new CustomNameInfo("All ranged weapons appear with 2x less ammo", null, null, null, null, "Всё оружие дальнего боя появляется с в 2x меньшим запасом аммуниции", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:RangedAmmo2", true,
				new CustomNameInfo("[aToM] Ranged Ammo x2", null, null, null, null, "[aToM] Запас аммуниции оружия дальнего боя x2", null, null),
				new CustomNameInfo("All ranged weapons appear with 2x more ammo", null, null, null, null, "Всё оружие дальнего боя появляется с в 2x большим запасом аммуниции", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:RangedAmmo4", true,
				new CustomNameInfo("[aToM] Ranged Ammo x4", null, null, null, null, "[aToM] Запас аммуниции оружия дальнего боя x4", null, null),
				new CustomNameInfo("All ranged weapons appear with 4x more ammo", null, null, null, null, "Всё оружие дальнего боя появляется с в 4x большим запасом аммуниции", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:RangedAmmo8", true,
				new CustomNameInfo("[aToM] Ranged Ammo x8", null, null, null, null, "[aToM] Запас аммуниции оружия дальнего боя x8", null, null),
				new CustomNameInfo("All ranged weapons appear with 8x more ammo", null, null, null, null, "Всё оружие дальнего боя появляется с в 8x большим запасом аммуниции", null, null)));

			MyMutators.CrossConflict(m => m.Id.StartsWith("aToM:RangedAmmo"));
			MyMutators.EachConflict(m => m.Id.StartsWith("aToM:RangedAmmo"), "NoGuns", "InfiniteAmmo");
			
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:RangedFirerate025", true,
				new CustomNameInfo("[aToM] Ranged Firerate x0.25", null, null, null, null, "[aToM] Скорострельность оружия дальнего боя x0.25", null, null),
				new CustomNameInfo("All ranged weapons have 4x slower rate of fire", null, null, null, null, "Всё оружие дальнего боя стреляет в 4x медленнее", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:RangedFirerate05", true,
				new CustomNameInfo("[aToM] Ranged Firerate x0.5", null, null, null, null, "[aToM] Скорострельность оружия дальнего боя x0.5", null, null),
				new CustomNameInfo("All ranged weapons have 2x slower rate of fire", null, null, null, null, "Всё оружие дальнего боя стреляет в 2x медленнее", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:RangedFirerate2", true,
				new CustomNameInfo("[aToM] Ranged Firerate x2", null, null, null, null, "[aToM] Скорострельность оружия дальнего боя x2", null, null),
				new CustomNameInfo("All ranged weapons have 2x faster rate of fire", null, null, null, null, "Всё оружие дальнего боя стреляет в 2x быстрее", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:RangedFirerate4", true,
				new CustomNameInfo("[aToM] Ranged Firerate x4", null, null, null, null, "[aToM] Скорострельность оружия дальнего боя x4", null, null),
				new CustomNameInfo("All ranged weapons have 4x faster rate of fire", null, null, null, null, "Всё оружие дальнего боя стреляет в 4x быстрее", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:RangedFirerate8", true,
				new CustomNameInfo("[aToM] Ranged Firerate x8", null, null, null, null, "[aToM] Скорострельность оружия дальнего боя x8", null, null),
				new CustomNameInfo("All ranged weapons have 8x faster rate of fire", null, null, null, null, "Всё оружие дальнего боя стреляет в 8x быстрее", null, null)));

			MyMutators.CrossConflict(m => m.Id.StartsWith("aToM:RangedFirerate"));
			MyMutators.EachConflict(m => m.Id.StartsWith("aToM:RangeFirerate"), "NoGuns");

			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:RangedFullAuto", true,
				new CustomNameInfo("[aToM] Fully Automatic Ranged Weapons", null, null, null, null, "[aToM] Самострельное оружие дальнего боя", null, null),
				new CustomNameInfo("All ranged weapons are fully automatic", null, null, null, null, "Всё оружие дальнего боя может вести непрерывный огонь", null, null)));

			order = UniqueInt + 3;
			RangedShow.SortingOrder = RangedHide.SortingOrder = order;
			RangedShow.SortingIndex = RangedHide.SortingIndex = 0;
			RangedHide.Available = false;
			MyMutators.NumAndHide(m => m.Id.StartsWith("aToM:Ranged"), order);
			#endregion

			#region Projectile Speed/Random Projectiles
			ProjectileShow = RogueLibs.CreateCustomMutator("aToM:ProjectileShow", true,
				new CustomNameInfo("[aToM] PROJECTILE MUTATORS (show)", null, null, null, null, "[aToM] МУТАТОРЫ СНАРЯДОВ (показать)", null, null),
				new CustomNameInfo("...", null, null, null, null, "...", null, null));
			ProjectileHide = RogueLibs.CreateCustomMutator("aToM:ProjectileHide", true,
				new CustomNameInfo("[aToM] PROJECTILE MUTATORS (hide)", null, null, null, null, "[aToM] МУТАТОРЫ СНАРЯДОВ (скрыть)", null, null),
				new CustomNameInfo("...", null, null, null, null, "...", null, null));
			ProjectileShow.OnToggledInMutatorMenu += (m, b, state) => ToggleProjectile(true);
			ProjectileHide.OnToggledInMutatorMenu += (m, b, state) => ToggleProjectile(false);

			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:ProjectileSpeed0", true,
				new CustomNameInfo("[aToM] Projectile Speed x0", null, null, null, null, "[aToM] Скорость снарядов x0", null, null),
				new CustomNameInfo("All projectiles travel at zero speed", null, null, null, null, "Все снаряды летят с нулевой скоростью", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:ProjectileSpeed025", true,
				new CustomNameInfo("[aToM] Projectile Speed x0.25", null, null, null, null, "[aToM] Скорость снарядов x0.25", null, null),
				new CustomNameInfo("All projectiles travel at 4x lower speed", null, null, null, null, "Все снаряды летят с в 4x меньшей скоростью", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:ProjectileSpeed05", true,
				new CustomNameInfo("[aToM] Projectile Speed x0.5", null, null, null, null, "[aToM] Скорость снарядов x0.5", null, null),
				new CustomNameInfo("All projectiles travel at 2x lower speed", null, null, null, null, "Все снаряды летят с в 2x меньшей скоростью", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:ProjectileSpeed2", true,
				new CustomNameInfo("[aToM] Projectile Speed x2", null, null, null, null, "[aToM] Скорость снарядов x2", null, null),
				new CustomNameInfo("All projectiles travel at 2x greater speed", null, null, null, null, "Все снаряды летят с в 2x большей скоростью", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:ProjectileSpeed4", true,
				new CustomNameInfo("[aToM] Projectile Speed x4", null, null, null, null, "[aToM] Скорость снарядов x4", null, null),
				new CustomNameInfo("All projectiles travel at 4x greater speed", null, null, null, null, "Все снаряды летят с в 4x большей скоростью", null, null)));

			MyMutators.CrossConflict(m => m.Id.StartsWith("aToM:ProjectileSpeed"));
			
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:ProjectileTypeRocket", true,
				new CustomNameInfo("[aToM] Rocket Projectiles", null, null, null, null, "[aToM] Снаряды-ракеты", null, null),
				new CustomNameInfo("All projectiles are rockets", null, null, null, null, "Все снаряды - ракеты", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:ProjectileTypeRandom", true,
				new CustomNameInfo("[aToM] Random Projectiles", null, null, null, null, "[aToM] Рандомные снаряды", null, null),
				new CustomNameInfo("All projectiles are random", null, null, null, null, "Все снаряды рандомны", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:ProjectileTypeRandomEffect", true,
				new CustomNameInfo("[aToM] Random Effect Projectiles", null, null, null, null, "[aToM] Снаряды с рандомными эффектами", null, null),
				new CustomNameInfo("All projectiles are water pistol bullets with random effects", null, null, null, null, "Все снаряды - пули водяного пистолета с рандомными эффектами", null, null)));

			MyMutators.CrossConflict(m => m.Id.StartsWith("aToM:ProjectileType"));

			order = UniqueInt + 4;
			ProjectileShow.SortingOrder = ProjectileHide.SortingOrder = order;
			ProjectileShow.SortingIndex = ProjectileHide.SortingIndex = 0;
			ProjectileHide.Available = false;
			MyMutators.NumAndHide(m => m.Id.StartsWith("aToM:Projectile"), order);
			#endregion
			
			#region Explosion Damage/Power
			ExplosionShow = RogueLibs.CreateCustomMutator("aToM:ExplosionShow", true,
				new CustomNameInfo("[aToM] EXPLOSION MUTATORS (show)", null, null, null, null, "[aToM] МУТАТОРЫ ВЗРЫВОВ (показать)", null, null),
				new CustomNameInfo("...", null, null, null, null, "...", null, null));
			ExplosionHide = RogueLibs.CreateCustomMutator("aToM:ExplosionHide", true,
				new CustomNameInfo("[aToM] EXPLOSION MUTATORS (hide)", null, null, null, null, "[aToM] МУТАТОРЫ ВЗРЫВОВ (скрыть)", null, null),
				new CustomNameInfo("...", null, null, null, null, "...", null, null));
			ExplosionShow.OnToggledInMutatorMenu += (m, b, state) => ToggleExplosion(true);
			ExplosionHide.OnToggledInMutatorMenu += (m, b, state) => ToggleExplosion(false);

			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:ExplosionDamage025", true,
				new CustomNameInfo("[aToM] Explosion Damage x0.25", null, null, null, null, "[aToM] Урон от взрывов x0.25", null, null),
				new CustomNameInfo("All explosions deal 4x less damage", null, null, null, null, "Все взрывы наносят в 4x меньше урона", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:ExplosionDamage05", true,
				new CustomNameInfo("[aToM] Explosion Damage x0.5", null, null, null, null, "[aToM] Урон от взрывов x0.5", null, null),
				new CustomNameInfo("All explosions deal 2x less damage", null, null, null, null, "Все взрывы наносят в 2x меньше урона", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:ExplosionDamage2", true,
				new CustomNameInfo("[aToM] Explosion Damage x2", null, null, null, null, "[aToM] Урон от взрывов x2", null, null),
				new CustomNameInfo("All explosions deal 2x more damage", null, null, null, null, "Все взрывы наносят в 2x больше урона", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:ExplosionDamage4", true,
				new CustomNameInfo("[aToM] Explosion Damage x4", null, null, null, null, "[aToM] Урон от взрывов x4", null, null),
				new CustomNameInfo("All explosions deal 4x more damage", null, null, null, null, "Все взрывы наносят в 4x больше урона", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:ExplosionDamage8", true,
				new CustomNameInfo("[aToM] Explosion Damage x8", null, null, null, null, "[aToM] Урон от взрывов x8", null, null),
				new CustomNameInfo("All explosions deal 8x more damage", null, null, null, null, "Все взрывы наносят в 8x больше урона", null, null)));

			MyMutators.CrossConflict(m => m.Id.StartsWith("aToM:ExplosionDamage"));

			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:ExplosionPower025", true,
				new CustomNameInfo("[aToM] Explosion Power x0.25", null, null, null, null, "[aToM] Мощность взрывов x0.25", null, null),
				new CustomNameInfo("All explosions are 4x smaller", null, null, null, null, "Все взрывы в 4x меньше", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:ExplosionPower05", true,
				new CustomNameInfo("[aToM] Explosion Power x0.5", null, null, null, null, "[aToM] Мощность взрывов x0.5", null, null),
				new CustomNameInfo("All explosions are 2x smaller", null, null, null, null, "Все взрывы в 2x меньше", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:ExplosionPower2", true,
				new CustomNameInfo("[aToM] Explosion Power x2", null, null, null, null, "[aToM] Мощность взрывов x2", null, null),
				new CustomNameInfo("All explosions are 2x bigger", null, null, null, null, "Все взрывы в 2x больше", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:ExplosionPower4", true,
				new CustomNameInfo("[aToM] Explosion Power x4", null, null, null, null, "[aToM] Мощность взрывов x4", null, null),
				new CustomNameInfo("All explosions are 4x bigger", null, null, null, null, "Все взрывы в 4x больше", null, null)));
			MyMutators.Add(RogueLibs.CreateCustomMutator("aToM:ExplosionPower8", true,
				new CustomNameInfo("[aToM] Explosion Power x8", null, null, null, null, "[aToM] Мощность взрывов x8", null, null),
				new CustomNameInfo("All explosions are 8x bigger", null, null, null, null, "Все взрывы в 8x больше", null, null)));

			MyMutators.CrossConflict(m => m.Id.StartsWith("aToM:ExplosionPower"));

			order = UniqueInt + 5;
			ExplosionShow.SortingOrder = ExplosionHide.SortingOrder = order;
			ExplosionShow.SortingIndex = ExplosionHide.SortingIndex = 0;
			ExplosionHide.Available = false;
			MyMutators.NumAndHide(m => m.Id.StartsWith("aToM:Explosion"), order);
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

		public static void InvItem_SetupDetails(InvItem __instance)
		{
			if (__instance.itemType == "WeaponMelee")
			{
				if (MyMutators["aToM:MeleeDamage0"]) __instance.meleeDamage *= 0;
				else if (MyMutators["aToM:MeleeDamage025"]) __instance.meleeDamage = Divide(__instance.meleeDamage, 4);
				else if (MyMutators["aToM:MeleeDamage05"]) __instance.meleeDamage = Divide(__instance.meleeDamage, 2);
				else if (MyMutators["aToM:MeleeDamage2"]) __instance.meleeDamage *= 2;
				else if (MyMutators["aToM:MeleeDamage4"]) __instance.meleeDamage *= 4;
				else if (MyMutators["aToM:MeleeDamage8"]) __instance.meleeDamage *= 8;
				else if (MyMutators["aToM:MeleeDamage999"]) __instance.meleeDamage *= 999;

				if (MyMutators["aToM:MeleeDurability1"]) { __instance.initCount = __instance.initCountAI = __instance.rewardCount = 1; }
				else if (MyMutators["aToM:MeleeDurability025"]) { __instance.initCount = Divide(__instance.initCount, 4); __instance.initCountAI = Divide(__instance.initCountAI, 4); __instance.rewardCount = Divide(__instance.rewardCount, 4); }
				else if (MyMutators["aToM:MeleeDurability05"]) { __instance.initCount = Divide(__instance.initCount, 2); __instance.initCountAI = Divide(__instance.initCountAI, 2); __instance.rewardCount = Divide(__instance.rewardCount, 2); }
				else if (MyMutators["aToM:MeleeDurability2"]) { __instance.initCount *= 2; __instance.initCountAI *= 2; __instance.rewardCount *= 2; }
				else if (MyMutators["aToM:MeleeDurability4"]) { __instance.initCount *= 4; __instance.initCountAI *= 4; __instance.rewardCount *= 4; }
				else if (MyMutators["aToM:MeleeDurability8"]) { __instance.initCount *= 8; __instance.initCountAI *= 8; __instance.rewardCount *= 8; }
			}
			else if (__instance.itemType == "WeaponThrown")
			{
				if (MyMutators["aToM:ThrownDamage0"]) __instance.throwDamage *= 0;
				else if (MyMutators["aToM:ThrownDamage025"]) __instance.throwDamage = Divide(__instance.throwDamage, 4);
				else if (MyMutators["aToM:ThrownDamage05"]) __instance.throwDamage = Divide(__instance.throwDamage, 2);
				else if (MyMutators["aToM:ThrownDamage2"]) __instance.throwDamage *= 2;
				else if (MyMutators["aToM:ThrownDamage4"]) __instance.throwDamage *= 4;
				else if (MyMutators["aToM:ThrownDamage8"]) __instance.throwDamage *= 8;
				else if (MyMutators["aToM:ThrownDamage999"]) __instance.throwDamage *= 999;

				if (MyMutators["aToM:ThrownCount025"]) { __instance.initCount = Divide(__instance.initCount, 4); __instance.initCountAI = Divide(__instance.initCountAI, 4); __instance.rewardCount = Divide(__instance.rewardCount, 4); __instance.itemValue *= 4; }
				else if (MyMutators["aToM:ThrownCount05"]) { __instance.initCount = Divide(__instance.initCount, 2); __instance.initCountAI = Divide(__instance.initCountAI, 2); __instance.rewardCount = Divide(__instance.rewardCount, 2); __instance.itemValue *= 2; }
				else if (MyMutators["aToM:ThrownCount2"]) { __instance.initCount *= 2; __instance.initCountAI *= 2; __instance.rewardCount *= 2; __instance.itemValue = Divide(__instance.itemValue, 2); }
				else if (MyMutators["aToM:ThrownCount4"]) { __instance.initCount *= 4; __instance.initCountAI *= 4; __instance.rewardCount *= 4; __instance.itemValue = Divide(__instance.itemValue, 4); }
				else if (MyMutators["aToM:ThrownCount8"]) { __instance.initCount *= 8; __instance.initCountAI *= 8; __instance.rewardCount *= 8; __instance.itemValue = Divide(__instance.itemValue, 8); }

				if (MyMutators["aToM:ThrownDistance025"]) __instance.throwDistance = Divide(__instance.throwDistance, 4);
				else if (MyMutators["aToM:ThrownDistance05"]) __instance.throwDistance = Divide(__instance.throwDistance, 2);
				else if (MyMutators["aToM:ThrownDistance2"]) __instance.throwDistance *= 2;
				else if (MyMutators["aToM:ThrownDistance4"]) __instance.throwDistance *= 4;
			}
			else if (__instance.itemType == "WeaponProjectile")
			{
				if (__instance.invItemName != "Taser")
				{
					if (MyMutators["aToM:RangedAmmo1"]) { __instance.itemValue *= __instance.initCount; __instance.initCount = __instance.initCountAI = __instance.rewardCount = 1; }
					else if (MyMutators["aToM:RangedAmmo025"]) { __instance.initCount = Divide(__instance.initCount, 4); __instance.initCountAI = Divide(__instance.initCountAI, 4); __instance.rewardCount = Divide(__instance.rewardCount, 4); __instance.itemValue *= 4; }
					else if (MyMutators["aToM:RangedAmmo05"]) { __instance.initCount = Divide(__instance.initCount, 2); __instance.initCountAI = Divide(__instance.initCountAI, 2); __instance.rewardCount = Divide(__instance.rewardCount, 2); __instance.itemValue *= 2; }
					else if (MyMutators["aToM:RangedAmmo2"]) { __instance.initCount *= 2; __instance.initCountAI *= 2; __instance.rewardCount *= 2; __instance.itemValue = Divide(__instance.itemValue, 2); }
					else if (MyMutators["aToM:RangedAmmo4"]) { __instance.initCount *= 4; __instance.initCountAI *= 4; __instance.rewardCount *= 4; __instance.itemValue = Divide(__instance.itemValue, 4); }
					else if (MyMutators["aToM:RangedAmmo8"]) { __instance.initCount *= 8; __instance.initCountAI *= 8; __instance.rewardCount *= 8; __instance.itemValue = Divide(__instance.itemValue, 8); }
				}

				if (MyMutators["aToM:RangedFullAuto"])
					__instance.rapidFire = true;
			}
		}
		public static void Movement_KnockForward(ref float strength)
		{
			if (MyMutators["aToM:MeleeLunge0"]) strength *= 0;
			else if (MyMutators["aToM:MeleeLunge025"]) strength *= 0.25f;
			else if (MyMutators["aToM:MeleeLunge05"]) strength *= 0.5f;
			else if (MyMutators["aToM:MeleeLunge2"]) strength *= 2;
			else if (MyMutators["aToM:MeleeLunge4"]) strength *= 4;
			else if (MyMutators["aToM:MeleeLunge8"]) strength *= 8;
		}
		public static void Melee_Attack(Melee __instance)
		{
			if (MyMutators["aToM:MeleeSpeed025"]) { __instance.agent.weaponCooldown *= 4f; __instance.meleeContainerAnim.speed *= 0.25f; }
			else if (MyMutators["aToM:MeleeSpeed05"]) { __instance.agent.weaponCooldown *= 2f; __instance.meleeContainerAnim.speed *= 0.5f; }
			else if (MyMutators["aToM:MeleeSpeed2"]) { __instance.agent.weaponCooldown *= 0.5f; __instance.meleeContainerAnim.speed *= 2f; }
			else if (MyMutators["aToM:MeleeSpeed4"]) { __instance.agent.weaponCooldown *= 0.25f; __instance.meleeContainerAnim.speed *= 4f; }
		}
		public static void Bullet_SetupBullet(Bullet __instance)
		{
			if (MyMutators["aToM:RangedDamage0"]) __instance.damage *= 0;
			else if (MyMutators["aToM:RangedDamage025"]) __instance.damage = Divide(__instance.damage, 4);
			else if (MyMutators["aToM:RangedDamage05"]) __instance.damage = Divide(__instance.damage, 2);
			else if (MyMutators["aToM:RangedDamage2"]) __instance.damage *= 2;
			else if (MyMutators["aToM:RangedDamage4"]) __instance.damage *= 4;
			else if (MyMutators["aToM:RangedDamage8"]) __instance.damage *= 8;
			else if (MyMutators["aToM:RangedDamage999"]) __instance.damage *= 999;

			if (MyMutators["aToM:ProjectileSpeed0"]) __instance.speed *= 0;
			else if (MyMutators["aToM:ProjectileSpeed025"]) __instance.speed = Divide(__instance.speed, 4);
			else if (MyMutators["aToM:ProjectileSpeed05"]) __instance.speed = Divide(__instance.speed, 2);
			else if (MyMutators["aToM:ProjectileSpeed2"]) __instance.speed *= 2;
			else if (MyMutators["aToM:ProjectileSpeed4"]) __instance.speed *= 4;
		}
		public static void SpawnerMain_SpawnBullet(ref bulletStatus bulletType)
		{
			if (MyMutators["aToM:ProjectileTypeRocket"])
				bulletType = bulletStatus.Rocket;
			else if (MyMutators["aToM:ProjectileTypeRandom"])
				do
					bulletType = (bulletStatus)new System.Random().Next(1, 22); // [1;21]
				while (bulletType == bulletStatus.ZombieSpit || bulletType == bulletStatus.Laser || bulletType == bulletStatus.MindControl);
		}
		public static void Gun_SetWeaponCooldown(Gun __instance)
		{
			if (MyMutators["aToM:RangedFirerate025"])
				__instance.agent.weaponCooldown *= 4f;
			else if (MyMutators["aToM:RangedFirerate05"])
				__instance.agent.weaponCooldown *= 2f;
			else if (MyMutators["aToM:RangedFirerate2"])
				__instance.agent.weaponCooldown *= 0.5f;
			else if (MyMutators["aToM:RangedFirerate4"])
				__instance.agent.weaponCooldown *= 0.25f;
			else if (MyMutators["aToM:RangedFirerate8"])
				__instance.agent.weaponCooldown *= 0.125f;

			__instance.agent.weaponCooldown = Mathf.Max(__instance.agent.weaponCooldown, 0.05f);
		}
		public static void Explosion_SetupExplosion(Explosion __instance)
		{
			if (MyMutators["aToM:ExplosionDamage025"]) __instance.damage = Divide(__instance.damage, 4);
			else if (MyMutators["aToM:ExplosionDamage05"]) __instance.damage = Divide(__instance.damage, 2);
			else if (MyMutators["aToM:ExplosionDamage2"]) __instance.damage *= 2;
			else if (MyMutators["aToM:ExplosionDamage4"]) __instance.damage *= 4;
			else if (MyMutators["aToM:ExplosionDamage8"]) __instance.damage *= 8;

			if (MyMutators["aToM:ExplosionPower025"]) __instance.circleCollider2D.radius *= 0.5f;
			else if (MyMutators["aToM:ExplosionPower05"]) __instance.circleCollider2D.radius *= Mathf.Sqrt(0.5f);
			else if (MyMutators["aToM:ExplosionPower2"]) __instance.circleCollider2D.radius *= Mathf.Sqrt(2f);
			else if (MyMutators["aToM:ExplosionPower4"]) __instance.circleCollider2D.radius *= 2f;
			else if (MyMutators["aToM:ExplosionPower8"]) __instance.circleCollider2D.radius *= Mathf.Sqrt(8f);
		}
		public static void Gun_spawnBullet(ref bulletStatus bulletType, ref string myStatusEffect)
		{
			if (MyMutators["aToM:ProjectileTypeRandomEffect"] || (bulletType == bulletStatus.WaterPistol && (myStatusEffect == null || myStatusEffect == string.Empty)))
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

		public void ToggleMelee(bool state)
		{
			MeleeShow.Available = !state;
			MeleeShow.IsActive = false;
			MeleeHide.Available = state;
			MeleeHide.IsActive = false;

			MyMutators.ForEach(m => m.Id.StartsWith("aToM:Melee"), m => m.Available = state);

			GameController.gameController.mainGUI.scrollingMenuScript.OpenScrollingMenu();
		}
		public void ToggleThrown(bool state)
		{
			ThrownShow.Available = !state;
			ThrownShow.IsActive = false;
			ThrownHide.Available = state;
			ThrownHide.IsActive = false;

			MyMutators.ForEach(m => m.Id.StartsWith("aToM:Thrown"), m => m.Available = state);

			GameController.gameController.mainGUI.scrollingMenuScript.OpenScrollingMenu();
		}
		public void ToggleRanged(bool state)
		{
			RangedShow.Available = !state;
			RangedShow.IsActive = false;
			RangedHide.Available = state;
			RangedHide.IsActive = false;

			MyMutators.ForEach(m => m.Id.StartsWith("aToM:Ranged"), m => m.Available = state);

			GameController.gameController.mainGUI.scrollingMenuScript.OpenScrollingMenu();
		}
		public void ToggleProjectile(bool state)
		{
			ProjectileShow.Available = !state;
			ProjectileShow.IsActive = false;
			ProjectileHide.Available = state;
			ProjectileHide.IsActive = false;

			MyMutators.ForEach(m => m.Id.StartsWith("aToM:Projectile"), m => m.Available = state);

			GameController.gameController.mainGUI.scrollingMenuScript.OpenScrollingMenu();
		}
		public void ToggleExplosion(bool state)
		{
			ExplosionShow.Available = !state;
			ExplosionShow.IsActive = false;
			ExplosionHide.Available = state;
			ExplosionHide.IsActive = false;

			MyMutators.ForEach(m => m.Id.StartsWith("aToM:Explosion"), m => m.Available = state);

			GameController.gameController.mainGUI.scrollingMenuScript.OpenScrollingMenu();
		}

	}
	public class MutatorCollection : List<CustomMutator>
	{
		public MutatorCollection() : base() { }

		public bool this[string id]
		{
			get
			{
				CustomMutator mut = Find(m => m.Id == id);
				return mut != null && mut.IsActive;
			}
		}
		public void ForEach(Predicate<CustomMutator> predicate, Action<CustomMutator> action)
		{
			foreach (CustomMutator mutator in this)
				if (predicate(mutator))
					action(mutator);
		}
		public void CrossConflict(Predicate<CustomMutator> predicate)
		{
			List<CustomMutator> selected = new List<CustomMutator>();
			foreach (CustomMutator mutator in this)
				if (predicate(mutator))
					selected.Add(mutator);
			ATOM.MyRogueUtilities.CrossConflict(selected.ToArray());
		}
		public void EachConflict(Predicate<CustomMutator> predicate, params string[] conflicts)
		{
			List<CustomMutator> selected = new List<CustomMutator>();
			foreach (CustomMutator mutator in this)
				if (predicate(mutator))
					selected.Add(mutator);
			ATOM.MyRogueUtilities.EachConflict(conflicts, selected.ToArray());
		}
		public void NumAndHide(Predicate<CustomMutator> predicate, int order)
		{
			int index = 1;
			foreach (CustomMutator mutator in this)
				if (predicate(mutator))
				{
					mutator.SortingOrder = order;
					mutator.SortingIndex = index++;
					mutator.Available = false;
				}
		}
	}
}