using System.Collections.Generic;
using System.Text;
using Harmony;

namespace TextRememberer.Extensions
{
    public static class DialogueOptionExtensions
    {
        public static int GetUniqueId(this DialogueOption dialogueOption, CharacterDialogueTree tree)
        {
            var nodes = new Traverse(tree).Field("_mapDialogueNodes").GetValue<IDictionary<string, DialogueNode>>();
            var answerNode = nodes[dialogueOption.TargetName];
            
            var str = new StringBuilder();
            str.Append(dialogueOption.Text);
            foreach (var answerText in answerNode.DisplayTextData.GetDisplayStringList())
                str.Append(answerText);

            return str.ToString().GetStableHashCode();
        }
    }
}