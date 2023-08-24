using System.Collections.Generic;
using System.Linq;
using LoreTracker.Helpers;
using UnityEngine;

namespace LoreTracker.MonoBehaviours.ColorUpdaters;

[RequireComponent(typeof(NomaiSharedWhiteboard))]
public class NomaiSharedWhiteboardColorUpdater : MonoBehaviour, ILateInitializer
{
    private readonly MaterialSwapper _materialSwapper = new();
    private readonly List<NomaiTextTracker> _nomaiTexts = new();

    private void Start()
    {
        LateInitializerManager.RegisterLateInitializer(this);
    }

    public void OnDestroy()
    {
        foreach (var tracker in _nomaiTexts)
            tracker.OnAllTextRead -= UpdateColor;
    }

    public void LateInitialize()
    {
        LateInitializerManager.UnregisterLateInitializer(this);

        var nomaiSharedWhiteboard = GetComponent<NomaiSharedWhiteboard>();
        for (var i = 0; i < nomaiSharedWhiteboard._remoteIDs.Length; i++)
        {
            var tracker = nomaiSharedWhiteboard._nomaiTexts[i].GetComponent<NomaiTextTracker>();
            _nomaiTexts.Add(tracker);
        }
        
        GenerateMaterials();

        foreach (var tracker in _nomaiTexts)
            tracker.OnAllTextRead += UpdateColor;

        UpdateColor();
    }
    
    private void GenerateMaterials()
    {
        var socket = GetComponentInChildren<SharedStoneSocket>();
        foreach (var renderer in socket.GetComponentsInChildren<Renderer>())
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
        if (_nomaiTexts.Any(tracker => !tracker.AllTextRead)) return;
        _materialSwapper.SwapMaterials();
    }
}