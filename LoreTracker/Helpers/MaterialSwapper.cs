using System;
using System.Collections.Generic;
using UnityEngine;

namespace LoreTracker.Helpers;

public class MaterialSwapper
{
    private record MaterialData(Renderer Renderer, int MaterialIndex, Material Material)
    {
        public Renderer Renderer { get; } = Renderer;
        public int MaterialIndex { get; } = MaterialIndex;
        public Material Material { get; } = Material;
    }

    private static readonly int PropIDEmissionColor = Shader.PropertyToID("_EmissionColor");

    private static readonly Dictionary<int, Material> NewMaterials = new();
    
    private readonly List<MaterialData> _materials = new();

    public void Add(Renderer renderer, Material original, int materialIndex, Action<Material> initNewMaterial)
    {
        var id = original.GetInstanceID();
        if (!NewMaterials.TryGetValue(id, out var newMaterial))
        {
            newMaterial = UnityEngine.Object.Instantiate(original);
            newMaterial.name = $"{original.name}_LoreTracker";
            initNewMaterial(newMaterial);
            NewMaterials[id] = newMaterial;
        }

        _materials.Add(new MaterialData(renderer, materialIndex, newMaterial));
    }

    public void AddCommonMaterials(Renderer renderer, Material original, int materialIndex)
    {
        if (original.name.StartsWith("Structure_NOM_BlueGlow_mat"))
        {
            Add(renderer, original, materialIndex, newMat =>
            {
                newMat.color = LoreTracker.TranslatedColor;
                newMat.SetColor(PropIDEmissionColor, LoreTracker.TranslatedColor);
            });
        }

        if (original.name.StartsWith("Decal_NOM_Symbols"))
        {
            Add(renderer, original, materialIndex, newMat =>
            {
                newMat.color = LoreTracker.TranslatedColor;
                newMat.SetColor(PropIDEmissionColor, LoreTracker.TranslatedColor);
            });
        }
    }

    public void SwapMaterials()
    {
        foreach (var materialData in _materials)
        {
            var materials = materialData.Renderer.sharedMaterials;
            materials[materialData.MaterialIndex] = materialData.Material;
            materialData.Renderer.sharedMaterials = materials;
        }
    }
}