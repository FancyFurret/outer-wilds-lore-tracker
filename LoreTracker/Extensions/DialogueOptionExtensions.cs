using System.Text;

namespace LoreTracker.Extensions;

public static class DialogueOptionExtensions
{
    public static int GetTrackerId(this DialogueOption dialogueOption, CharacterDialogueTree tree)
    {
        var str = new StringBuilder();
        str.Append(dialogueOption.Text);

        if (!string.IsNullOrEmpty(dialogueOption.TargetName))
        {
            var answerNode = tree._mapDialogueNodes[dialogueOption.TargetName];
            foreach (var answerText in answerNode.DisplayTextData.GetDisplayStringList())
                str.Append(answerText);
        }

        return str.ToString().GetStableHashCode();
    }
}