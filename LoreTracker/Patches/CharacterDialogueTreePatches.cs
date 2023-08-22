using HarmonyLib;
using LoreTracker.MonoBehaviours;

namespace LoreTracker.Patches;

[HarmonyPatch]
public static class CharacterDialogueTreePatches
{
    [HarmonyPostfix]
    [HarmonyPatch(typeof(CharacterDialogueTree), nameof(CharacterDialogueTree.Awake))]
    public static void PostAwake(CharacterDialogueTree __instance)
    {
        __instance.gameObject.AddComponent<DialogueTracker>();
        __instance.gameObject.AddComponent<LoreIndicator>();
    }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(CharacterDialogueTree), nameof(CharacterDialogueTree.InputDialogueOption))]
    public static void PreInputDialogueOption(CharacterDialogueTree __instance, int optionIndex)
    {
        __instance.gameObject.GetComponent<DialogueTracker>().ReadDialogue(optionIndex);
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CharacterDialogueTree), nameof(CharacterDialogueTree.InputDialogueOption))]
    public static void PostInputDialogueOption(CharacterDialogueTree __instance)
    {
        __instance.gameObject.GetComponent<DialogueTracker>().ColorDialogue();
    }

    [HarmonyPostfix]
    [HarmonyPatch(typeof(CharacterDialogueTree), nameof(CharacterDialogueTree.StartConversation))]
    public static void PostStartConversation(CharacterDialogueTree __instance)
    {
        __instance.gameObject.GetComponent<DialogueTracker>().StartConversation();
        __instance.gameObject.GetComponent<DialogueTracker>().ColorDialogue();
    }
}