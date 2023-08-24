using System.Collections.Generic;
using System.Linq;
using LoreTracker.Helpers;
using UnityEngine;

namespace LoreTracker.MonoBehaviours.ColorUpdaters;

[RequireComponent(typeof(SharedStone))]
public class SharedStoneColorUpdater : MonoBehaviour, ILateInitializer
{
    private readonly MaterialSwapper _materialSwapper = new();
    private List<NomaiTextTracker> _associatedNomaiTexts = new();


    private void Start()
    {
        LateInitializerManager.RegisterLateInitializer(this);
    }

    public void OnDestroy()
    {
        foreach (var tracker in _associatedNomaiTexts)
            tracker.OnAllTextRead -= UpdateColor;
    }

    public void LateInitialize()
    {
        LateInitializerManager.UnregisterLateInitializer(this);

        _associatedNomaiTexts = FindAssociatedNomaiTexts();
        GenerateMaterials();

        foreach (var tracker in _associatedNomaiTexts)
            tracker.OnAllTextRead += UpdateColor;

        UpdateColor();
    }

    private List<NomaiTextTracker> FindAssociatedNomaiTexts()
    {
        var sharedStone = GetComponent<SharedStone>();
        var remoteId = sharedStone.GetRemoteCameraID();
        var associatedNomaiTexts = new List<NomaiTextTracker>();
        var associatedNomaiTextHashes = new HashSet<int>();

        foreach (var whiteboard in FindObjectsOfType<NomaiSharedWhiteboard>())
        {
            for (var i = 0; i < whiteboard._remoteIDs.Length; i++)
            {
                if (whiteboard._remoteIDs[i] != remoteId) continue;

                var tracker = whiteboard._nomaiTexts[i].GetComponent<NomaiTextTracker>();
                var hash = tracker.GetNomaiTextAssetHash();
                if (associatedNomaiTextHashes.Contains(hash))
                    continue;

                associatedNomaiTexts.Add(tracker);
                associatedNomaiTextHashes.Add(hash);
                break;
            }
        }

        return associatedNomaiTexts;
    }

    private void GenerateMaterials()
    {
        foreach (var renderer in GetComponentsInChildren<MeshRenderer>())
        {
            for (var i = 0; i < renderer.sharedMaterials.Length; i++)
            {
                var material = renderer.sharedMaterials[i];
                _materialSwapper.AddCommonMaterials(renderer, material, i);
            }
        }
    }

    private void UpdateColor()
    {
        if (_associatedNomaiTexts.Any(tracker => !tracker.AllTextRead)) return;
        _materialSwapper.SwapMaterials();
    }
}