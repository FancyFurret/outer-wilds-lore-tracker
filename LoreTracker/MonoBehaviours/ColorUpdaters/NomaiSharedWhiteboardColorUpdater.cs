using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace LoreTracker.MonoBehaviours.ColorUpdaters;

[RequireComponent(typeof(NomaiSharedWhiteboard))]
public class NomaiSharedWhiteboardColorUpdater : MonoBehaviour, ILateInitializer
{
    private static readonly int PropIDEmissionColor = Shader.PropertyToID("_EmissionColor");

    private readonly List<NomaiTextTracker> _nomaiTexts = new();

    private void Awake()
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

        foreach (var tracker in _nomaiTexts)
            tracker.OnAllTextRead += UpdateColor;

        UpdateColor();
    }

    private void UpdateColor()
    {
        if (_nomaiTexts.Any(tracker => !tracker.AllTextRead))
            return;

        var socket = GetComponentInChildren<SharedStoneSocket>();
        foreach (var renderer in socket.GetComponentsInChildren<Renderer>())
        foreach (var material in renderer.materials)
        {
            if (material.name.StartsWith("Structure_NOM_BlueGlow_mat"))
            {
                material.color = LoreTracker.TranslatedColor;
                material.SetColor(PropIDEmissionColor, LoreTracker.TranslatedColor);
            }

            if (material.name.StartsWith("Decal_NOM_Symbols"))
            {
                material.color = LoreTracker.TranslatedColor;
                material.SetColor(PropIDEmissionColor, LoreTracker.TranslatedColor);
            }
        }
    }
}