using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace LoreTracker.MonoBehaviours;

[RequireComponent(typeof(DialogueTracker))]
public class LoreIndicator : MonoBehaviour, ILateInitializer
{
    private float _canvasOffset = .3f;

    private DialogueTracker _dialogueTracker;
    private Transform _indicatorTransform;
    private bool _isInitialized;
    private Transform _playerTransform;
    private Transform _targetTransform;

    private void Start()
    {
        LateInitializerManager.RegisterLateInitializer(this);
    }

    private void Update()
    {
        if (!_isInitialized)
            return;

        _indicatorTransform.position = _targetTransform.position + transform.up * _canvasOffset;
        _indicatorTransform.LookAt(_playerTransform, _playerTransform.up);
    }

    private void OnDestroy()
    {
        if (_isInitialized)
            _dialogueTracker.OnAllDialogueRead -= UpdateIndicator;
    }

    public void LateInitialize()
    {
        LateInitializerManager.UnregisterLateInitializer(this);

        // Seems like sometimes these get destroyed before LateInitialize is called
        if (!this || !gameObject)
            return;

        _dialogueTracker = GetComponent<DialogueTracker>();
        _playerTransform = Locator.GetPlayerTransform();
        _targetTransform = transform;

        var isRecorder = GetComponent<HearthianRecorderEffects>() != null;
        if (!isRecorder)
        {
            var targetNamePriorities = new[] { "Head_Top", "Neck_Top" };
            var allTransforms = transform.parent.GetComponentsInChildren<Transform>();
            foreach (var targetName in targetNamePriorities)
            {
                var targetTransform = allTransforms.FirstOrDefault(t => t.name.Contains(targetName));
                if (!targetTransform) continue;
                _targetTransform = targetTransform;
                break;
            }

            if (transform.name.Contains("Riebeck"))
                _canvasOffset = .6f;
        }

        var indicatorCanvas = new GameObject().AddComponent<Canvas>();
        _indicatorTransform = indicatorCanvas.transform;
        _indicatorTransform.SetParent(transform, false);
        _indicatorTransform.localPosition = new Vector3(0, _canvasOffset, 0);

        var canvasRectTransform = indicatorCanvas.GetComponent<RectTransform>();
        canvasRectTransform.sizeDelta = Vector2.one;

        const float size = 1000f;
        var text = new GameObject().AddComponent<Text>();
        text.transform.SetParent(_indicatorTransform, false);
        text.alignment = TextAnchor.MiddleCenter;
        // ReSharper disable once Unity.UnknownResource
        text.font = Resources.Load<Font>("fonts/english - latin/HVD Fonts - BrandonGrotesque-Bold_Dynamic");
        text.color = LoreTracker.UnreadColor;
        text.fontSize = 300;
        text.text = "!";

        text.rectTransform.sizeDelta = new Vector2(size, size);
        text.rectTransform.localScale = new Vector3(1 / size, 1 / size, 1);

        _dialogueTracker.OnAllDialogueRead += UpdateIndicator;

        _isInitialized = true;

        UpdateIndicator();
    }

    private void UpdateIndicator()
    {
        _indicatorTransform.gameObject.SetActive(!_dialogueTracker.AllDialogueRead);
    }
}