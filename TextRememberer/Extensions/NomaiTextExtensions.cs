using System.Text;

namespace TextRememberer.Extensions
{
    public static class NomaiTextExtensions
    {
        public static int GetUniqueId(this NomaiText text, int id)
        {
            var str = new StringBuilder();
            for (var i = 1; i <= id; i++)
                str.Append(text.GetTextNode(i) ?? "");
            return str.ToString().GetStableHashCode();
        }
    }
}