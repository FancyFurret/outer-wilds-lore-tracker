﻿using System.Collections.Generic;
using System.Text;

namespace TextRememberer.Extensions
{
    public static class DialogueNodeExtensions
    {
        public static int GetUniqueId(this DialogueNode dialogueNode)
        {
            var str = new StringBuilder();
            foreach (var text in dialogueNode.DisplayTextData?.GetDisplayStringList() ?? new List<string>())
                str.Append(text);

            return str.ToString().GetStableHashCode();
        }
    }
}