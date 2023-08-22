using System.Reflection;
using HarmonyLib;
using OWML.Common;
using OWML.ModHelper;
using UnityEngine;

namespace LoreTracker;

public class LoreTracker : ModBehaviour
{
    public static readonly Color UnreadColor = new(0.5294114f, 0.5762672f, 1.5f, 1f);
    public static readonly Color TranslatedColor = new(0.6f, 0.6f, 0.6f, 1f);
    public static LoreTracker I { get; private set; }

    private void Start()
    {
        ModHelper.Console.WriteLine("LoreTracker loaded!", MessageType.Success);
        I = this;

        Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly());

        StandaloneProfileManager.SharedInstance.OnProfileReadDone += LoreTrackerStorage.Load;
    }
}