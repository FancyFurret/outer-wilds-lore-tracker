using UnityEngine;

namespace LoreTracker.MonoBehaviours.ColorUpdaters;

[RequireComponent(typeof(NomaiRecorderEffects))]
public class NomaiRecorderColorUpdater : MonoBehaviour, ILateInitializer
{
    private static readonly int PropIDEmissionColor = Shader.PropertyToID("_EmissionColor");
    private static readonly int PropIDDetailEmissionColor = Shader.PropertyToID("_DetailEmissionColor");
    private static readonly int PropIDShadowColor = Shader.PropertyToID("_ShadowColor");
    private NomaiTextTracker _nomaiTextTracker;

    private void Awake()
    {
        LateInitializerManager.RegisterLateInitializer(this);
    }

    public void OnDestroy()
    {
        _nomaiTextTracker.OnAllTextRead -= UpdateColor;
    }

    public void LateInitialize()
    {
        LateInitializerManager.UnregisterLateInitializer(this);

        _nomaiTextTracker = transform.parent.GetComponentInChildren<NomaiTextTracker>();
        _nomaiTextTracker.OnAllTextRead += UpdateColor;

        UpdateColor();
    }

    private void UpdateColor()
    {
        if (!_nomaiTextTracker.AllTextRead)
            return;

        foreach (var light in transform.parent.GetComponentsInChildren<Light>())
            light.color = LoreTracker.TranslatedColor;

        foreach (var renderer in GetComponentsInChildren<MeshRenderer>())
        foreach (var material in renderer.materials)
        {
            if (material.name.StartsWith("Effects_NOM_VesselRecorderGlow_mat"))
                material.color = LoreTracker.TranslatedColor * 0.5f;

            if (material.name.StartsWith("Effects_NOM_VesselRecorderRing_mat"))
            {
                material.color = LoreTracker.TranslatedColor;
                material.SetColor(PropIDShadowColor, LoreTracker.TranslatedColor * 0.5f);
            }

            if (material.name.StartsWith("Effects_NOM_VesselComputerVolLight_mat"))
                material.color = LoreTracker.TranslatedColor;

            if (material.name.StartsWith("Structure_NOM_TrimPattern_mat"))
                material.SetColor(PropIDDetailEmissionColor, LoreTracker.TranslatedColor);

            if (material.name.StartsWith("Structure_NOM_BlueGlow_mat"))
            {
                material.color = LoreTracker.TranslatedColor;
                material.SetColor(PropIDEmissionColor, LoreTracker.TranslatedColor);
            }
        }
    }
}