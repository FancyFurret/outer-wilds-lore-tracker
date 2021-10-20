using OWML.Common;
using TextRememberer.MonoBehaviours;

namespace TextRememberer.Patches
{
    public static class CharacterDialogueTreePatches
    {
        public static void Patch(IHarmonyHelper harmonyHelper)
        {
            harmonyHelper.AddPostfix<CharacterDialogueTree>("Awake", typeof(CharacterDialogueTreePatches),
                nameof(PostAwake));
            harmonyHelper.AddPrefix<CharacterDialogueTree>("InputDialogueOption", typeof(CharacterDialogueTreePatches),
                nameof(PreInputDialogueOption));
            harmonyHelper.AddPostfix<CharacterDialogueTree>("InputDialogueOption", typeof(CharacterDialogueTreePatches),
                nameof(PostInputDialogueOption));
            harmonyHelper.AddPostfix<CharacterDialogueTree>("StartConversation", typeof(CharacterDialogueTreePatches),
                nameof(PostStartConversation));
        }

        // ReSharper disable once InconsistentNaming
        static void PostAwake(CharacterDialogueTree __instance)
        {
            __instance.gameObject.AddComponent<DialogueRememberer>();
        }

        // ReSharper disable once InconsistentNaming
        static void PreInputDialogueOption(CharacterDialogueTree __instance, int optionIndex)
        {
            __instance.gameObject.GetComponent<DialogueRememberer>().ReadDialogue(optionIndex);
        }

        // ReSharper disable once InconsistentNaming
        static void PostInputDialogueOption(CharacterDialogueTree __instance)
        {
            __instance.gameObject.GetComponent<DialogueRememberer>().ColorDialogue();
        }

        // ReSharper disable once InconsistentNaming
        static void PostStartConversation(CharacterDialogueTree __instance)
        {
            __instance.gameObject.GetComponent<DialogueRememberer>().ColorDialogue();
        }
    }
}