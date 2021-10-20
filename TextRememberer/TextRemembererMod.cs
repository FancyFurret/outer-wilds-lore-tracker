using OWML.ModHelper;
using TextRememberer.Patches;
using UnityEngine;

namespace TextRememberer
{
    public class TextRemembererMod : ModBehaviour
    {
        public static TextRemembererMod I { get; private set; }

        public static readonly Color UnreadColor = new Color(0.5294114f, 0.5762672f, 1.5f, 1f);
        public static readonly Color TranslatedColor = new Color(0.6f, 0.6f, 0.6f, 1f);

        private void Start()
        {
            I = this;

            NomaiTextPatches.Patch(ModHelper.HarmonyHelper);
            ScrollItemPatches.Patch(ModHelper.HarmonyHelper);
            CharacterDialogueTreePatches.Patch(ModHelper.HarmonyHelper);

            StandaloneProfileManager.SharedInstance.OnProfileReadDone += TextRemembererStorage.Load;
            StandaloneProfileManager.SharedInstance.OnProfileDataSaved += TextRemembererStorage.Save;
        }
    }
}