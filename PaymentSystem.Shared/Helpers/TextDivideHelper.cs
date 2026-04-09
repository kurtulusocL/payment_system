using System.Text;
using Microsoft.AspNetCore.Html;

namespace PaymentSystem.Shared.Helpers
{
    public static class TextDivideHelper
    {
        public static IHtmlContent WrapText(this string text, int chunkSize = 100)
        {
            if (string.IsNullOrWhiteSpace(text) || text.Length <= chunkSize)
                return new HtmlString(text ?? "");

            var sb = new StringBuilder();
            for (int i = 0; i < text.Length; i += chunkSize)
            {
                int length = Math.Min(chunkSize, text.Length - i);
                sb.Append(text.Substring(i, length));
                if (i + chunkSize < text.Length)
                {
                    sb.Append("<br />");
                }
            }
            return new HtmlString(sb.ToString());
        }
    }
}
