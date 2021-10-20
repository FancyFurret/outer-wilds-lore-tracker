using OWML.Common;
using TextRememberer.Extensions;
using TextRememberer.MonoBehaviours;

namespace TextRememberer.Patches
{
    public static class NomaiTextPatches
    {
        
        public static void Patch(IHarmonyHelper harmonyHelper)
        {
            harmonyHelper.AddPostfix<NomaiText>("Awake", typeof(NomaiTextPatches), nameof(PostAwake));
            harmonyHelper.AddPrefix<NomaiText>("SetAsTranslated", typeof(NomaiTextPatches), nameof(PreSetAsTranslated));
        }

        // ReSharper disable once InconsistentNaming
        static void PostAwake(NomaiText __instance)
        {
            __instance.gameObject.AddComponent<NomaiTextRememberer>();
        }

        // ReSharper disable once InconsistentNaming
        static void PreSetAsTranslated(NomaiText __instance, int id)
        {
            __instance.GetComponent<NomaiTextRememberer>().ReadText(__instance.GetUniqueId(id));
        }
    }
}