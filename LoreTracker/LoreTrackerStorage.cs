using System.Collections.Generic;
using System.IO;
using OWML.Common;
using OWML.Utils;

namespace LoreTracker;

public class LoreTrackerStorageData
{
    public HashSet<int> ReadTexts { get; } = new();
}

public static class LoreTrackerStorage
{
    private const string SaveFileName = "LoreTrackerData.owsave";
    private const string OldSaveFileName = "LoreTrackerData.json";

    private static LoreTrackerStorageData _data;
    private static string CurrentProfile => StandaloneProfileManager.SharedInstance.currentProfile.profileName;

    private static string SaveFilePath =>
        Path.Combine(StandaloneProfileManager.SharedInstance._profilesPath, CurrentProfile, SaveFileName);
    private static string OldSaveFilePath =>
        Path.Combine(StandaloneProfileManager.SharedInstance._profilesPath, CurrentProfile, OldSaveFileName);

    public static void Load()
    {
        _data = JsonHelper.LoadJsonObject<LoreTrackerStorageData>(SaveFilePath)
                ?? JsonHelper.LoadJsonObject<LoreTrackerStorageData>(OldSaveFilePath)
                ?? new LoreTrackerStorageData();
        LoreTracker.I.ModHelper.Console.WriteLine("Loaded LoreTracker data", MessageType.Success);
    }

    public static void Save()
    {
        JsonHelper.SaveJsonObject(SaveFilePath, _data);
        LoreTracker.I.ModHelper.Console.WriteLine("Saved LoreTracker data", MessageType.Success);
    }

    public static bool IsTextRead(int id)
    {
        return _data.ReadTexts.Contains(id);
    }

    public static void SetTextRead(int id)
    {
        if (IsTextRead(id))
            return;

        _data.ReadTexts.Add(id);
        LoreTracker.I.ModHelper.Console.WriteLine($"Marked [{id}] as read", MessageType.Success);

        Save();
    }
}