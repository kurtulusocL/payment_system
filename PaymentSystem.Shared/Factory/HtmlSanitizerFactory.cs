using Ganss.Xss;

namespace PaymentSystem.Shared.Factory
{
    public static class HtmlSanitizerFactory
    {
        public static HtmlSanitizer Create()
        {
            var sanitizer = new HtmlSanitizer();

            var allowedTags = new[]
            {
            "p", "div", "span", "strong", "em", "i", "b", "u", "strike", "br",
            "ul", "ol", "li", "a", "img", "table", "tr", "td", "th", "thead", "tbody",
            "blockquote", "h1", "h2", "h3", "h4", "h5", "h6", "hr", "code", "pre", "sup", "sub"
        };

            foreach (var tag in allowedTags)
                sanitizer.AllowedTags.Add(tag);

            sanitizer.AllowedTags.Remove("script");
            sanitizer.AllowedTags.Remove("style");

            sanitizer.AllowedSchemes.Add("http");
            sanitizer.AllowedSchemes.Add("https");
            sanitizer.AllowedSchemes.Add("mailto");

            sanitizer.AllowedAttributes.Remove("onclick");
            sanitizer.AllowedAttributes.Remove("onload");
            sanitizer.AllowedAttributes.Remove("onerror");
            sanitizer.AllowedAttributes.Remove("onmouseover");
            sanitizer.AllowedAttributes.Remove("onfocus");
            sanitizer.AllowDataAttributes = false;

            sanitizer.AllowedCssProperties.Remove("behavior");
            sanitizer.AllowedCssProperties.Remove("expression");
            sanitizer.AllowedCssProperties.Remove("binding");

            sanitizer.FilterUrl += (sender, args) =>
            {
                if (args.OriginalUrl.StartsWith("javascript:", StringComparison.OrdinalIgnoreCase))
                    args.SanitizedUrl = null;
            };

            return sanitizer;
        }
    }
}
