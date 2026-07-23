using AngleSharp.Dom;
using AngleSharp.Html.Parser;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Encodings.Web;

namespace Application.Utilities
{
    public static class SafeHtmlRenderer
    {
        private static readonly HashSet<string> AllowedElements =
            new(StringComparer.OrdinalIgnoreCase)
            {
                "p", "br", "strong", "b", "em", "i", "u", "s",
                "h1", "h2", "h3", "h4", "blockquote", "ul", "ol", "li",
                "table", "thead", "tbody", "tr", "th", "td", "a", "img"
            };

        private static readonly HashSet<string> SuppressedElements =
            new(StringComparer.OrdinalIgnoreCase)
            {
                "script", "style", "iframe", "object", "embed", "svg", "math", "template"
            };

        private static readonly HashSet<string> VoidElements =
            new(StringComparer.OrdinalIgnoreCase) { "br", "img" };

        public static string Sanitize(string html)
        {
            if (string.IsNullOrWhiteSpace(html))
            {
                return string.Empty;
            }

            var document = new HtmlParser().ParseDocument(html);
            var output = new StringBuilder();
            foreach (var child in document.Body.ChildNodes)
            {
                WriteNode(child, output);
            }

            return output.ToString();
        }

        private static void WriteNode(INode node, StringBuilder output)
        {
            if (node is IText text)
            {
                output.Append(HtmlEncoder.Default.Encode(text.Data));
                return;
            }

            if (node is not IElement element)
            {
                return;
            }

            var tagName = element.TagName.ToLowerInvariant();
            if (SuppressedElements.Contains(tagName))
            {
                return;
            }

            if (!AllowedElements.Contains(tagName))
            {
                WriteChildren(element, output);
                return;
            }

            output.Append('<').Append(tagName);
            WriteAllowedAttributes(element, tagName, output);
            output.Append('>');

            if (!VoidElements.Contains(tagName))
            {
                WriteChildren(element, output);
                output.Append("</").Append(tagName).Append('>');
            }
        }

        private static void WriteChildren(IElement element, StringBuilder output)
        {
            foreach (var child in element.ChildNodes)
            {
                WriteNode(child, output);
            }
        }

        private static void WriteAllowedAttributes(IElement element, string tagName, StringBuilder output)
        {
            if (tagName == "a")
            {
                var href = NormalizeUrl(element.GetAttribute("href"), allowDataImage: false);
                if (href != null)
                {
                    WriteAttribute("href", href, output);
                    WriteAttribute("rel", "noopener noreferrer", output);
                }
            }
            else if (tagName == "img")
            {
                var source = NormalizeUrl(element.GetAttribute("src"), allowDataImage: false);
                if (source != null)
                {
                    WriteAttribute("src", source, output);
                }

                var alternativeText = element.GetAttribute("alt");
                if (!string.IsNullOrWhiteSpace(alternativeText))
                {
                    WriteAttribute("alt", alternativeText, output);
                }
            }

            if (tagName is "th" or "td")
            {
                if (int.TryParse(element.GetAttribute("colspan"), out var colspan) &&
                    colspan is >= 1 and <= 20)
                {
                    WriteAttribute("colspan", colspan.ToString(), output);
                }

                if (int.TryParse(element.GetAttribute("rowspan"), out var rowspan) &&
                    rowspan is >= 1 and <= 100)
                {
                    WriteAttribute("rowspan", rowspan.ToString(), output);
                }
            }
        }

        private static string NormalizeUrl(string value, bool allowDataImage)
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                return null;
            }

            value = value.Trim();
            if (value.StartsWith("/", StringComparison.Ordinal) &&
                !value.StartsWith("//", StringComparison.Ordinal))
            {
                return value;
            }

            if (!Uri.TryCreate(value, UriKind.Absolute, out var uri))
            {
                return null;
            }

            if (uri.Scheme is "https" or "http" or "mailto" or "tel")
            {
                return uri.ToString();
            }

            return allowDataImage && uri.Scheme == "data" ? value : null;
        }

        private static void WriteAttribute(string name, string value, StringBuilder output)
        {
            output.Append(' ')
                .Append(name)
                .Append("=\"")
                .Append(HtmlEncoder.Default.Encode(value))
                .Append('"');
        }
    }
}
