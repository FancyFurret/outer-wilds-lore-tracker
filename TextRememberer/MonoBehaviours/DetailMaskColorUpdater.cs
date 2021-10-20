using UnityEngine;

namespace TextRememberer.MonoBehaviours
{
    public class DetailMaskColorUpdater : MonoBehaviour, ILateInitializer
    {
        // ReSharper disable once InconsistentNaming
        private static int _propID_Detail1EmissionColor;
        
        // ReSharper disable once InconsistentNaming
        private static int _propID_Detail2EmissionColor;
        
        // ReSharper disable once InconsistentNaming
        private static int _propID_Detail3EmissionColor;

        private void Awake()
        {
            _propID_Detail1EmissionColor = Shader.PropertyToID("_Detail1EmissionColor");
            _propID_Detail2EmissionColor = Shader.PropertyToID("_Detail2EmissionColor");
            _propID_Detail3EmissionColor = Shader.PropertyToID("_Detail3EmissionColor");
            LateInitializerManager.RegisterLateInitializer(this);
        }

        public void LateInitialize()
        {
            LateInitializerManager.UnregisterLateInitializer(this);
            GetComponentInChildren<NomaiTextRememberer>().OnAllTextRead += UpdateColor;
        }

        private void UpdateColor()
        {
            var nomaiText = GetComponentInChildren<NomaiTextRememberer>();
            if (!nomaiText.AllTextRead)
                return;

            foreach (var renderer in GetComponentsInChildren<OWRenderer>())
            {
                renderer.SetMaterialProperty(_propID_Detail1EmissionColor, TextRemembererMod.TranslatedColor);
                renderer.SetMaterialProperty(_propID_Detail2EmissionColor, TextRemembererMod.TranslatedColor);
                renderer.SetMaterialProperty(_propID_Detail3EmissionColor, TextRemembererMod.TranslatedColor);
            }
        }
    }
}