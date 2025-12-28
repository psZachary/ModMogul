using BepInEx;
using HarmonyLib;

namespace ModMogul
{
	[BepInPlugin("modmogul.core", "Mod Mogul", "0.0.1")]
	public class ModMogulPlugin : BaseUnityPlugin
	{
		private void Awake()
		{
			new Harmony("modmogul.core").PatchAll(typeof(ModMogul.Itemizer).Assembly);
		}

		[HarmonyPatch(typeof(EconomyManager), "Start")]
		private static class Patch_EconomyManager_Start_Postfix
		{
			private static void Postfix(EconomyManager __instance)
			{
				Itemizer.TryInjectAllIntoEconomy(__instance);
			}
		}
	}
}
