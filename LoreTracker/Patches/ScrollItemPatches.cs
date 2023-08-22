using HarmonyLib;
using LoreTracker.MonoBehaviours.ColorUpdaters;

namespace LoreTracker.Patches;

[HarmonyPatch]
public static class ScrollItemPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(ScrollItem), nameof(ScrollItem.Awake))]
    public static void PostAwake(ScrollItem __instance)
    {
        __instance.gameObject.AddComponent<ScrollItemColorUpdater>();
    }
}