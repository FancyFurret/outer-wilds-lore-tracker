using System.Collections.Generic;
using LoreTracker.Extensions;
using UnityEngine;

namespace LoreTracker.MonoBehaviours;

[RequireComponent(typeof(NomaiText))]
public class NomaiTextTracker : MonoBehaviour, ILateInitializer
{
    public delegate void AllTextReadHandler();

    private NomaiText _nomaiText;
    private Dictionary<int, int> _nomaiToTrackerIds;

    private HashSet<int> _readTexts;
    private HashSet<int> _trackerIds;

    public bool AllTextRead => _readTexts.Count > 0 && _readTexts.Count == _trackerIds.Count;


    private void Start()
    {
        _readTexts = new HashSet<int>();
        _trackerIds = new HashSet<int>();
        _nomaiToTrackerIds = new Dictionary<int, int>();
        _nomaiText = GetComponent<NomaiText>();

        LateInitializerManager.RegisterLateInitializer(this);
    }

    public void LateInitialize()
    {
        LateInitializerManager.UnregisterLateInitializer(this);
        _nomaiText.VerifyInitialized();

        foreach (var (id, data) in _nomaiText._dictNomaiTextData)
        {
            var trackerId = _nomaiText.GetTrackerId(data);
            _nomaiToTrackerIds[id] = trackerId;
            _trackerIds.Add(trackerId);
        }

        RecallText();
    }

    public event AllTextReadHandler OnAllTextRead;

    private void RecallText()
    {
        foreach (var (id, trackerId) in _nomaiToTrackerIds)
            if (LoreTrackerStorage.IsTextRead(trackerId))
                ReadText(id);
    }

    public void ReadText(int id)
    {
        var trackerId = _nomaiToTrackerIds[id];
        if (_readTexts.Contains(id) || _nomaiText.IsTranslated(id) || _nomaiText.GetTextNode(id) == null)
            return;

        _readTexts.Add(id);
        _nomaiText.SetAsTranslated(id);

        // If this is the first time reading this text, let all other NomaiTexts know since they might reference
        // the same text
        if (!LoreTrackerStorage.IsTextRead(trackerId))
        {
            LoreTrackerStorage.SetTextRead(trackerId);
            foreach (var tr in FindObjectsOfType<NomaiTextTracker>())
                if (tr._trackerIds.Contains(trackerId) && tr != this)
                    tr.ReadText(id);
        }

        CheckIfAllRead();
    }

    public int GetNomaiTextAssetHash()
    {
        return _nomaiText._nomaiTextAsset.text.GetStableHashCode();
    }

    private void CheckIfAllRead()
    {
        if (!AllTextRead) return;
        OnAllTextRead?.Invoke();
    }
}