using System.Collections.Generic;
using TextRemembererStorageData =
    System.Collections.Generic.Dictionary<string, System.Collections.Generic.HashSet<int>>;

namespace TextRememberer
{
    public static class TextRemembererStorage
    {
        private static TextRemembererStorageData _readText;
        private static string _currentProfile;

        // TODO: Save in save folder so steam cloud syncs it
        public static void Load()
        {
            TextRemembererMod.I.ModHelper.Console.WriteLine("Loading data");
            _readText = TextRemembererMod.I.ModHelper.Storage.Load<TextRemembererStorageData>("RememberedText.json")
                        ?? new Dictionary<string, HashSet<int>>();
            _currentProfile = StandaloneProfileManager.SharedInstance.GetActiveProfile().profileName;
            if (!_readText.ContainsKey(_currentProfile))
                _readText[_currentProfile] = new HashSet<int>();
        }

        public static void Save(bool success)
        {
            TextRemembererMod.I.ModHelper.Console.WriteLine("Saving data");
            // TextRemembererMod.I.ModHelper.Storage.Save(_readText, "RememberedText.json");
        }

        public static bool IsTextRead(int id)
        {
            return _readText[_currentProfile].Contains(id);
        }

        public static void SetTextRead(int id)
        {
            if (IsTextRead(id))
                return;

            _readText[_currentProfile].Add(id);
            TextRemembererMod.I.ModHelper.Console.WriteLine($"Marking [{id}] as read");
        }
    }
}