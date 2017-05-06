using System.Globalization;
using System.Text;

namespace Handlebars.Core
{
    public class HtmlEncoder : ITextEncoder
    {
        public string Encode(string text)
        {
            if (text == null)
                return string.Empty;

            var sb = new StringBuilder(text.Length);

            foreach (var c in text)
            {
                switch (c)
                {
                    case '"':
                        sb.Append("&quot;");
                        break;
                    case '&':
                        sb.Append("&amp;");
                        break;
                    case '<':
                        sb.Append("&lt;");
                        break;
                    case '>':
                        sb.Append("&gt;");
                        break;

                    default:
                        if (c > 159)
                        {
                            sb.Append("&#");
                            sb.Append(((int) c).ToString(CultureInfo.InvariantCulture));
                            sb.Append(";");
                        }
                        else
                            sb.Append(c);
                        break;
                }
            }
            return sb.ToString();
        }
    }
}