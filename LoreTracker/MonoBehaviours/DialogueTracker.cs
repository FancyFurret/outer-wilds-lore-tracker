using System.Collections.Generic;
using LoreTracker.Extensions;
using UnityEngine;
using UnityEngine.UI;

namespace LoreTracker.MonoBehaviours;

[RequireComponent(typeof(CharacterDialogueTree))]
public class DialogueTracker : MonoBehaviour, ILateInitializer
{
    public delegate void AllDialogueReadHandler();

    private readonly Stack<DialogueNode> _nodePath = new();

    private CharacterDialogueTree _characterDialogueTree;

    public bool AllDialogueRead
    {
        get
        {
            var entryNode = GetEntryNode();
            return entryNode != null && IsNodeCompletelyRead(entryNode);
        }
    }

    private DialogueNode CurrentNode => _characterDialogueTree._currentNode;

    private IDictionary<string, DialogueNode> DialogueNodes => _characterDialogueTree._mapDialogueNodes;

    private DialogueBoxVer2 DialogueBox { get; } = FindObjectOfType<DialogueBoxVer2>();

    private List<DialogueOptionUI> OptionsUIElements =>
        DialogueBox ? DialogueBox._optionsUIElements : new List<DialogueOptionUI>();

    private Text MainTextField => DialogueBox ? DialogueBox._mainTextField : null;

    private void Start()
    {
        _characterDialogueTree = GetComponent<CharacterDialogueTree>();
        LateInitializerManager.RegisterLateInitializer(this);
    }

    public void LateInitialize()
    {
        LateInitializerManager.UnregisterLateInitializer(this);
        _characterDialogueTree.VerifyInitialized();

        CheckIfAllRead();
    }

    public event AllDialogueReadHandler OnAllDialogueRead;

    public void ReadDialogue(int optionIndex)
    {
        DialogueNode targetNode = null;

        if (optionIndex >= 0)
        {
            var option = DialogueBox.OptionFromUIIndex(optionIndex);
            LoreTrackerStorage.SetTextRead(option.GetTrackerId(_characterDialogueTree));
            if (!string.IsNullOrEmpty(option.TargetName))
                targetNode = DialogueNodes[option.TargetName];
        }

        if (CurrentNode != null && !CurrentNode.HasNext())
        {
            LoreTrackerStorage.SetTextRead(CurrentNode.GetTrackerId());
            if (!string.IsNullOrEmpty(CurrentNode.TargetName))
                targetNode = DialogueNodes[CurrentNode.TargetName];
        }

        if (targetNode != null)
        {
            if (_nodePath.Contains(targetNode))
                while (_nodePath.Peek() != targetNode)
                    _nodePath.Pop();
            else
                _nodePath.Push(targetNode);
        }

        CheckIfAllRead();
    }

    public void ColorDialogue()
    {
        if (MainTextField != null && CurrentNode != null)
        {
            var read = LoreTrackerStorage.IsTextRead(CurrentNode.GetTrackerId());
            MainTextField.color = read ? Color.white : LoreTracker.UnreadColor;
        }

        for (var i = 0; i < OptionsUIElements.Count; i++)
        {
            var read = IsOptionCompletelyRead(DialogueBox.OptionFromUIIndex(i));
            OptionsUIElements[i].textElement.color = read ? Color.white : LoreTracker.UnreadColor;
        }
    }

    public void StartConversation()
    {
        _nodePath.Clear();
        _nodePath.Push(GetEntryNode());
    }

    private void CheckIfAllRead()
    {
        if (!AllDialogueRead) return;
        OnAllDialogueRead?.Invoke();
    }


    private DialogueNode GetEntryNode()
    {
        DialogueNode rootNode = null;
        var nodes = new List<DialogueNode>(DialogueNodes.Values);
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

    private bool IsOptionCompletelyRead(DialogueOption option)
    {
        if (!LoreTrackerStorage.IsTextRead(option.GetTrackerId(_characterDialogueTree)))
            return false;

        if (string.IsNullOrEmpty(option.TargetName))
            return true;

        return IsNodeCompletelyRead(DialogueNodes[option.TargetName], new HashSet<DialogueNode>(_nodePath));
    }

    private bool IsNodeCompletelyRead(DialogueNode node, ISet<DialogueNode> seen = null)
    {
        // BFS to find all nodes possible from this nodes
        var queue = new Queue<DialogueNode>();
        queue.Enqueue(node);

        var visitedNodes = new HashSet<DialogueNode>();
        if (seen != null)
            foreach (var seenNode in seen)
                visitedNodes.Add(seenNode);

        var allNodes = new HashSet<DialogueNode>();
        var allOptions = new HashSet<DialogueOption>();

        while (queue.Count > 0)
        {
            var currentNode = queue.Dequeue();
            visitedNodes.Add(currentNode);
            allNodes.Add(currentNode);

            // Look for option's nodes
            foreach (var option in currentNode.ListDialogueOptions)
            {
                // Make sure this option is currently available
                if (!IsOptionConditionsSatisfied(option))
                    continue;

                allOptions.Add(option);

                if (option.TargetName == string.Empty)
                    continue;

                var targetNode = DialogueNodes[option.TargetName];
                if (visitedNodes.Contains(targetNode))
                    continue;

                if (!targetNode.TargetNodeConditionsSatisfied())
                    continue;

                queue.Enqueue(targetNode);
            }

            // Look for a direct target node
            if (currentNode.Target != null)
            {
                var targetNode = currentNode.Target;
                if (visitedNodes.Contains(targetNode))
                    continue;

                // Make sure this node is currently available
                if (!targetNode.TargetNodeConditionsSatisfied())
                    continue;

                queue.Enqueue(targetNode);
            }
        }

        foreach (var currentNode in allNodes)
            if (!LoreTrackerStorage.IsTextRead(currentNode.GetTrackerId()))
                return false;

        foreach (var currentOption in allOptions)
            if (!LoreTrackerStorage.IsTextRead(currentOption.GetTrackerId(_characterDialogueTree)))
                return false;

        return true;
    }

    private bool IsOptionConditionsSatisfied(DialogueOption option)
    {
        if (option.ConditionRequirement != string.Empty &&
            !DialogueConditionManager.SharedInstance.GetConditionState(option.ConditionRequirement))
            return false;

        foreach (var condition in option.PersistentCondition)
            if (!PlayerData.GetPersistentCondition(condition))
                return false;

        foreach (var requirement in option.LogConditionRequirement)
            if (!Locator.GetShipLogManager().GetFact(requirement)?.IsRevealed() ?? true)
                return false;

        if (option.CancelledRequirement != string.Empty &&
            DialogueConditionManager.SharedInstance.GetConditionState(option.CancelledRequirement))
            return false;

        foreach (var cancelled in option.CancelledPersistentRequirement)
            if (PlayerData.GetPersistentCondition(cancelled))
                return false;

        return true;
    }
}