using System.Collections.Generic;
using TextRememberer.Extensions;
using UnityEngine;

namespace TextRememberer.MonoBehaviours
{
    [RequireComponent(typeof(NomaiText))]
    public class NomaiTextRememberer : MonoBehaviour, ILateInitializer
    {
        public delegate void AllTextReadHandler();

        public event AllTextReadHandler OnAllTextRead;

        public bool AllTextRead => _readTexts.Count == _textIds.Count;

        private Dictionary<int, int> _textIds;
        private HashSet<int> _readTexts;
        private NomaiText _nomaiText;

        private void Start()
        {
            _textIds = new Dictionary<int, int>();
            _readTexts = new HashSet<int>();
            _nomaiText = GetComponent<NomaiText>();

            LateInitializerManager.RegisterLateInitializer(this);
        }

        public void LateInitialize()
        {
            LateInitializerManager.UnregisterLateInitializer(this);
            _nomaiText.VerifyInitialized();
            
            for (var i = 1; i <= _nomaiText.GetNumTextBlocks(); i++)
                _textIds[_nomaiText.GetUniqueId(i)] = i;

            RecallText();
        }

        private void RecallText()
        {
            for (var i = 1; i <= _nomaiText.GetNumTextBlocks(); i++)
                if (TextRemembererStorage.IsTextRead(_nomaiText.GetUniqueId(i)))
                    ReadText(_nomaiText.GetUniqueId(i));
        }

        public void ReadText(int id)
        {
            var nomaiId = _textIds[id];
            if (_readTexts.Contains(id) || _nomaiText.IsTranslated(nomaiId) || _nomaiText.GetTextNode(nomaiId) == null)
                return;

            _readTexts.Add(id);
            _nomaiText.SetAsTranslated(nomaiId);

            // If this is the first time reading this text, let all other NomaiTexts know since they might reference
            // the same text
            if (!TextRemembererStorage.IsTextRead(id))
            {
                TextRemembererStorage.SetTextRead(id);
                foreach (var tr in FindObjectsOfType<NomaiTextRememberer>())
                    if (tr._textIds.ContainsKey(id) && tr != this)
                        tr.ReadText(id);
            }

            CheckIfAllRead();
        }

        private void CheckIfAllRead()
        {
            if (AllTextRead)
            {
                TextRemembererMod.I.ModHelper.Console.WriteLine($"{_nomaiText} has been completely read!");
                OnAllTextRead?.Invoke();
            }
        }
    }
}