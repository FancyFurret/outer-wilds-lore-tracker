using LoreTracker.Helpers;
using UnityEngine;

namespace LoreTracker.MonoBehaviours.ColorUpdaters;

[RequireComponent(typeof(NomaiRecorderEffects))]
public class NomaiRecorderColorUpdater : MonoBehaviour, ILateInitializer
{
    private static readonly int PropIDDetailEmissionColor = Shader.PropertyToID("_DetailEmissionColor");
    private static readonly int PropIDShadowColor = Shader.PropertyToID("_ShadowColor");

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

        _nomaiTextTracker = transform.parent.GetComponentInChildren<NomaiTextTracker>();
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
                _materialSwapper.AddCommonMaterials(renderer, material, i);

                if (material.name.StartsWith("Effects_NOM_VesselRecorderGlow_mat"))
                    _materialSwapper.Add(renderer, material, i,
                        newMat => { newMat.color = LoreTracker.TranslatedColor * 0.5f; });

                if (material.name.StartsWith("Effects_NOM_VesselRecorderRing_mat"))
                    _materialSwapper.Add(renderer, material, i, newMat =>
                    {
                        newMat.color = LoreTracker.TranslatedColor;
                        newMat.SetColor(PropIDShadowColor, LoreTracker.TranslatedColor * 0.5f);
                    });

                if (material.name.StartsWith("Effects_NOM_VesselComputerVolLight_mat"))
                    _materialSwapper.Add(renderer, material, i,
                        newMat => { newMat.color = LoreTracker.TranslatedColor; });

                if (material.name.StartsWith("Structure_NOM_TrimPattern_mat"))
                    _materialSwapper.Add(renderer, material, i,
                        newMat => { newMat.SetColor(PropIDDetailEmissionColor, LoreTracker.TranslatedColor); });
            }
        }
    }

    private void UpdateColor()
    {
        if (!_nomaiTextTracker.AllTextRead) return;

        foreach (var light in transform.parent.GetComponentsInChildren<Light>())
            light.color = LoreTracker.TranslatedColor;

        _materialSwapper.SwapMaterials();
    }
}