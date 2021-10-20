using OWML.Common;
using TextRememberer.MonoBehaviours;

namespace TextRememberer.Patches
{
    public static class ScrollItemPatches
    {
        public static void Patch(IHarmonyHelper harmonyHelper)
        {
            harmonyHelper.AddPostfix<ScrollItem>("Awake", typeof(ScrollItemPatches), nameof(PostAwake));
        }

        // ReSharper disable once InconsistentNaming
        static void PostAwake(ScrollItem __instance)
        {
            __instance.gameObject.AddComponent<DetailMaskColorUpdater>();
        }
    }
}