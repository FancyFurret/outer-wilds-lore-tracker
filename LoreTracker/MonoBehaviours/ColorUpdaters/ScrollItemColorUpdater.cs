using UnityEngine;

namespace LoreTracker.MonoBehaviours.ColorUpdaters;

[RequireComponent(typeof(ScrollItem))]
public class ScrollItemColorUpdater : MonoBehaviour, ILateInitializer
{
    private static readonly int PropIDDetail1EmissionColor = Shader.PropertyToID("_Detail1EmissionColor");
    private static readonly int PropIDDetail2EmissionColor = Shader.PropertyToID("_Detail2EmissionColor");
    private static readonly int PropIDDetail3EmissionColor = Shader.PropertyToID("_Detail3EmissionColor");

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

        _nomaiTextTracker = GetComponentInChildren<NomaiTextTracker>();
        _nomaiTextTracker.OnAllTextRead += UpdateColor;

        UpdateColor();
    }

    private void UpdateColor()
    {
        if (!_nomaiTextTracker.AllTextRead)
            return;

        foreach (var renderer in GetComponentsInChildren<MeshRenderer>())
        foreach (var material in renderer.materials)
            if (material.name.StartsWith("Props_NOM_Scroll_mat"))
            {
                material.SetColor(PropIDDetail1EmissionColor, LoreTracker.TranslatedColor);
                material.SetColor(PropIDDetail2EmissionColor, LoreTracker.TranslatedColor);
                material.SetColor(PropIDDetail3EmissionColor, LoreTracker.TranslatedColor);
            }
    }
}