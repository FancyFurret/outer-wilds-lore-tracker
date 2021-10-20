using System.Collections.Generic;
using Harmony;
using TextRememberer.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace TextRememberer.MonoBehaviours
{
    [RequireComponent(typeof(CharacterDialogueTree))]
    public class DialogueRememberer : MonoBehaviour, ILateInitializer
    {
        private DialogueNode CurrentNode =>
            new Traverse(_characterDialogueTree).Field("_currentNode").GetValue<DialogueNode>();

        private IDictionary<string, DialogueNode> MapDialogueNodes =>
            new Traverse(_characterDialogueTree).Field("_mapDialogueNodes")
                .GetValue<IDictionary<string, DialogueNode>>();

        private DialogueBoxVer2 DialogueBox { get; } = FindObjectOfType<DialogueBoxVer2>();

        private List<DialogueOptionUI> OptionsUIElements => DialogueBox
            ? new Traverse(DialogueBox).Field("_optionsUIElements").GetValue<List<DialogueOptionUI>>()
            : new List<DialogueOptionUI>();

        private Text MainTextField => DialogueBox
            ? new Traverse(DialogueBox).Field("_mainTextField").GetValue<Text>()
            : null;

        private CharacterDialogueTree _characterDialogueTree;

        private const float CanvasOffset = .3f;
        private Transform _canvasTransform;
        private Transform _targetTransform;
        private Transform _playerTransform;

        private void Start()
        {
            _characterDialogueTree = GetComponent<CharacterDialogueTree>();
            LateInitializerManager.RegisterLateInitializer(this);
        }

        public void LateInitialize()
        {
            LateInitializerManager.UnregisterLateInitializer(this);
            _characterDialogueTree.VerifyInitialized();

            _playerTransform = Locator.GetPlayerTransform();

            foreach (var t in transform.parent.GetComponentsInChildren<Transform>())
                if (t.name.Contains("Head_Top"))
                    _targetTransform = t;
            if (_targetTransform == null)
                _targetTransform = transform;

            var canvas = new GameObject().AddComponent<Canvas>();
            var canvasRectTransform = canvas.GetComponent<RectTransform>();
            canvas.transform.SetParent(transform, false);
            canvasRectTransform.sizeDelta = Vector2.one;
            _canvasTransform = canvasRectTransform;

            const float size = 1000f;
            var text = new GameObject().AddComponent<Text>();
            var textRectTransform = text.GetComponent<RectTransform>();
            text.transform.SetParent(canvas.transform, false);
            textRectTransform.sizeDelta = new Vector2(size, size);
            textRectTransform.localScale = new Vector3(1 / size, 1 / size, 1);
            text.alignment = TextAnchor.MiddleCenter;
            text.font = Resources.Load<Font>("fonts/english - latin/HVD Fonts - BrandonGrotesque-Bold_Dynamic");
            text.color = TextRemembererMod.UnreadColor;
            text.fontSize = 300;
            text.text = "!";

            CheckIfAllRead();
        }

        private void Update()
        {
            if (_canvasTransform == null)
                return;
            
            _canvasTransform.position = _targetTransform.position + transform.up * CanvasOffset;
            _canvasTransform.LookAt(_playerTransform, _playerTransform.up);
        }

        public void ReadDialogue(int optionIndex)
        {
            if (optionIndex >= 0)
            {
                var option = DialogueBox.OptionFromUIIndex(optionIndex);
                TextRemembererStorage.SetTextRead(option.GetUniqueId(_characterDialogueTree));
            }

            if (CurrentNode != null && !CurrentNode.HasNext())
                TextRemembererStorage.SetTextRead(CurrentNode.GetUniqueId());

            CheckIfAllRead();
        }

        public void CheckIfAllRead()
        {
            _canvasTransform.gameObject.SetActive(true);

            var entryNode = GetEntryNode();
            if (entryNode == null)
                return;

            if (!IsNodeCompletelyRead(entryNode))
                return;

            TextRemembererMod.I.ModHelper.Console.WriteLine($"{_characterDialogueTree} has been completely read!");
            _canvasTransform.gameObject.SetActive(false);
        }

        private DialogueNode GetEntryNode()
        {
            DialogueNode rootNode = null;
            var nodes = new List<DialogueNode>(MapDialogueNodes.Values);
            nodes.Reverse();
            foreach (var node in nodes)
            {
                if (!node.EntryConditionsSatisfied())
                    continue;

                rootNode = node;
                break;
            }

            return rootNode;
        }

        public void ColorDialogue()
        {
            if (MainTextField != null)
                MainTextField.color =
                    CurrentNode != null && !TextRemembererStorage.IsTextRead(CurrentNode.GetUniqueId())
                        ? TextRemembererMod.UnreadColor
                        : Color.white;

            var index = 0;
            foreach (var uiOption in OptionsUIElements)
            {
                if (!IsOptionCompletelyRead(DialogueBox.OptionFromUIIndex(index)))
                    uiOption.textElement.color = TextRemembererMod.UnreadColor;
                index++;
            }
        }

        private bool IsOptionCompletelyRead(DialogueOption option)
        {
            if (!TextRemembererStorage.IsTextRead(option.GetUniqueId(_characterDialogueTree)))
                return false;

            if (option.TargetName == string.Empty)
                return true;

            return IsNodeCompletelyRead(MapDialogueNodes[option.TargetName]);
        }

        private bool IsNodeCompletelyRead(DialogueNode node)
        {
            // BFS to find all nodes possible from this nodes
            var queue = new Queue<DialogueNode>();
            queue.Enqueue(node);
            
            var allNodes = new HashSet<DialogueNode>();
            allNodes.Add(node);

            var allOptions = new HashSet<DialogueOption>();

            while (queue.Count > 0)
            {
                var currentNode = queue.Dequeue();
                
                // Look for option nodes
                foreach (var option in currentNode.ListDialogueOptions)
                {
                    // Make sure this option is currently available
                    if (!IsOptionConditionsSatisfied(option))
                        continue;
                    
                    allOptions.Add(option);
                    
                    if (option.TargetName == string.Empty)
                        continue;

                    var targetNode = MapDialogueNodes[option.TargetName];
                    if (allNodes.Contains(targetNode))
                        continue;
                    
                    allNodes.Add(targetNode);
                    queue.Enqueue(targetNode);
                }

                // Look for a direct target node
                if (currentNode.Target != null)
                {
                    var targetNode = currentNode.Target;
                    if (allNodes.Contains(targetNode))
                        continue;

                    // Make sure this node is currently available
                    if (!currentNode.TargetNodeConditionsSatisfied())
                        continue;

                    allNodes.Add(targetNode);
                    queue.Enqueue(targetNode);
                }
            }

            foreach (var currentNode in allNodes)
                if (!TextRemembererStorage.IsTextRead(currentNode.GetUniqueId()))
                    return false;
            
            foreach (var currentOption in allOptions)
                if (!TextRemembererStorage.IsTextRead(currentOption.GetUniqueId(_characterDialogueTree)))
                    return false;

            return true;
        }

        private bool IsOptionConditionsSatisfied(DialogueOption option)
        {
            if (option.ConditionRequirement != string.Empty &&
                !DialogueConditionManager.SharedInstance.GetConditionState(option.ConditionRequirement))
                return false;

            if (option.PersistentCondition != string.Empty &&
                !PlayerData.GetPersistentCondition(option.PersistentCondition))
                return false;

            foreach (var requirement in option.LogConditionRequirement)
                if (!Locator.GetShipLogManager().GetFact(requirement)?.IsRevealed() ?? true)
                    return false;

            if (option.CancelledRequirement != string.Empty &&
                DialogueConditionManager.SharedInstance.GetConditionState(option.CancelledRequirement))
                return false;

            return true;
        }
    }
}