using HarmonyLib;
using LoreTracker.MonoBehaviours.ColorUpdaters;

namespace LoreTracker.Patches;

[HarmonyPatch]
public static class NomaiRecorderEffectsPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(NomaiRecorderEffects), nameof(NomaiRecorderEffects.Awake))]
    public static void PostAwake(NomaiRecorderEffects __instance)
    {
        __instance.gameObject.AddComponent<NomaiRecorderColorUpdater>();
    }
}