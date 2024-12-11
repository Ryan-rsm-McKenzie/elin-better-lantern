#pragma warning disable CA1812 // Avoid uninstantiated internal classes


using BepInEx;
using System.Collections.Generic;
using HarmonyLib;
using System.Reflection.Emit;
using System;

namespace ElinConvenientCharges;

[BepInPlugin("0B5DECC2-16E9-4CBB-ACC5-EC3F3143A697", "elinbetterlantern", "1.0")]
internal sealed class Mod : BaseUnityPlugin
{
	private static Harmony? s_harmony;
	private static Mod? s_instance;

	internal static void Error(object data)
	{
		s_instance!.Logger.LogError(data);
	}

#pragma warning disable IDE0051 // Remove unused private members
	private void Start()
	{
		s_instance = this;
		s_harmony = new Harmony("ryan.elinbetterlantern");
		s_harmony.PatchAll();
	}
#pragma warning restore IDE0051
}

[HarmonyPatch(typeof(TraitLightSource))]
[HarmonyPatch("get_LightRadius")]
internal sealed class TraitLightSource_LightRadius
{
	public static IEnumerable<CodeInstruction> Transpiler()
	{
		return [
			new(OpCodes.Ldarg_0),
			new(OpCodes.Call, AccessTools.Method(
				type: typeof(TraitLightSource_LightRadius),
				name: nameof(Detour)
			)),
			new(OpCodes.Ret),
		];
	}

	private static int Detour(TraitLightSource self)
	{
		int radius = self.GetParam(1).ToInt();
		var owner = self.owner;
		return radius + Math.Max(owner.material.hardness / 10, 0) + owner.encLV + (int)owner.blessedState;
	}
}
