using HarmonyLib;
using LoreTracker.MonoBehaviours.ColorUpdaters;

namespace LoreTracker.Patches;

[HarmonyPatch]
public static class NomaiSharedWhiteboardPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(NomaiSharedWhiteboard), nameof(NomaiSharedWhiteboard.Awake))]
    public static void PostAwake(NomaiSharedWhiteboard __instance)
    {
        __instance.gameObject.AddComponent<NomaiSharedWhiteboardColorUpdater>();
    }
}