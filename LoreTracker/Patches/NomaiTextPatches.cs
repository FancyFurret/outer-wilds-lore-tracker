using HarmonyLib;
using LoreTracker.MonoBehaviours;

namespace LoreTracker.Patches;

[HarmonyPatch]
public static class NomaiTextPatches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(NomaiText), nameof(NomaiText.Awake))]
    public static void PostAwake(NomaiText __instance)
    {
        __instance.gameObject.AddComponent<NomaiTextTracker>();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(NomaiText), nameof(NomaiText.SetAsTranslated))]
    public static void PreSetAsTranslated(NomaiText __instance, int id)
    {
        __instance.GetComponent<NomaiTextTracker>().ReadText(id);
    }
}