using LoreTracker.Helpers;
using UnityEngine;

namespace LoreTracker.MonoBehaviours.ColorUpdaters;

[RequireComponent(typeof(ScrollItem))]
public class ScrollItemColorUpdater : MonoBehaviour, ILateInitializer
{
    private static readonly int PropIDDetail1EmissionColor = Shader.PropertyToID("_Detail1EmissionColor");
    private static readonly int PropIDDetail2EmissionColor = Shader.PropertyToID("_Detail2EmissionColor");
    private static readonly int PropIDDetail3EmissionColor = Shader.PropertyToID("_Detail3EmissionColor");

    private readonly MaterialSwapper _materialSwapper = new();
    private NomaiTextTracker _nomaiTextTracker;

    private void Start()
    {
        LateInitializerManager.RegisterLateInitializer(this);
    }

    public void OnDestroy()
    {
        if (_nomaiTextTracker != null)
            _nomaiTextTracker.OnAllTextRead -= UpdateColor;
    }

    public void LateInitialize()
    {
        LateInitializerManager.UnregisterLateInitializer(this);

        _nomaiTextTracker = GetComponentInChildren<NomaiTextTracker>();
        GenerateMaterials();

        _nomaiTextTracker.OnAllTextRead += UpdateColor;

        UpdateColor();
    }

    private void GenerateMaterials()
    {
        foreach (var renderer in GetComponentsInChildren<MeshRenderer>())
        {
            for (var i = 0; i < renderer.sharedMaterials.Length; i++)
            {
                var material = renderer.sharedMaterials[i];

                if (material.name.StartsWith("Props_NOM_Scroll_mat"))
                    _materialSwapper.Add(renderer, material, i, newMat =>
                    {
                        newMat.SetColor(PropIDDetail1EmissionColor, LoreTracker.TranslatedColor);
                        newMat.SetColor(PropIDDetail2EmissionColor, LoreTracker.TranslatedColor);
                        newMat.SetColor(PropIDDetail3EmissionColor, LoreTracker.TranslatedColor);
                    });
            }
        }
    }

    private void UpdateColor()
    {
        if (!_nomaiTextTracker.AllTextRead) return;
        _materialSwapper.SwapMaterials();
    }
}