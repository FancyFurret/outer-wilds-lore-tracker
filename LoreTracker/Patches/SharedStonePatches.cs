using HarmonyLib;
using LoreTracker.MonoBehaviours.ColorUpdaters;

namespace LoreTracker.Patches;

[HarmonyPatch]
public static class SharedStonePatches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(SharedStone), nameof(SharedStone.Awake))]
    public static void PostAwake(SharedStone __instance)
    {
        __instance.gameObject.AddComponent<SharedStoneColorUpdater>();
    }
}