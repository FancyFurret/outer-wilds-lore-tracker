using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LoreTracker.MonoBehaviours.ColorUpdaters;

[RequireComponent(typeof(SharedStone))]
public class SharedStoneColorUpdater : MonoBehaviour, ILateInitializer
{
    private static readonly int PropIDEmissionColor = Shader.PropertyToID("_EmissionColor");

    private readonly List<NomaiTextTracker> _associatedNomaiTexts = new();

    private void Awake()
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

        var sharedStone = GetComponent<SharedStone>();
        var remoteId = sharedStone.GetRemoteCameraID();
        var associatedNomaiTextHashes = new HashSet<int>();

        foreach (var whiteboard in FindObjectsOfType<NomaiSharedWhiteboard>())
            for (var i = 0; i < whiteboard._remoteIDs.Length; i++)
            {
                if (whiteboard._remoteIDs[i] != remoteId) continue;

                var tracker = whiteboard._nomaiTexts[i].GetComponent<NomaiTextTracker>();
                var hash = tracker.GetNomaiTextAssetHash();
                if (associatedNomaiTextHashes.Contains(hash))
                    continue;

                _associatedNomaiTexts.Add(tracker);
                associatedNomaiTextHashes.Add(hash);
                break;
            }

        foreach (var tracker in _associatedNomaiTexts)
            tracker.OnAllTextRead += UpdateColor;

        UpdateColor();
    }

    private void UpdateColor()
    {
        if (_associatedNomaiTexts.Any(tracker => !tracker.AllTextRead))
            return;

        foreach (var renderer in GetComponentsInChildren<MeshRenderer>())
        foreach (var material in renderer.materials)
            if (material.name.StartsWith("Structure_NOM_BlueGlow_mat"))
            {
                material.color = LoreTracker.TranslatedColor;
                material.SetColor(PropIDEmissionColor, LoreTracker.TranslatedColor);
            }
    }
}