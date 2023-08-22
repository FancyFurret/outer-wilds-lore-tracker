using System.Text;

namespace LoreTracker.Extensions;

public static class NomaiTextExtensions
{
    public static int GetTrackerId(this NomaiText text, NomaiText.NomaiTextData data)
    {
        if (data.TextNode == null)
            return -1;

        var str = new StringBuilder(data.TextNode.InnerText);

        var currentData = data;
        while (currentData.ParentID != -1)
        {
            currentData = text._dictNomaiTextData[currentData.ParentID];
            str.Append(currentData.TextNode.InnerText);
        }

        return str.ToString().GetStableHashCode();
    }
}