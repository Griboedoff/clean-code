using System.Collections.Generic;
using System.Linq;

namespace Markdown.Tokens
{
	public class ListItemHtmlToken : HtmlToken
	{
		public ListItemHtmlToken(string data) : base("li", data, 0)
		{
		}

		public ListItemHtmlToken(List<HtmlToken> parsedTokens) : base("li", parsedTokens, 0)
		{
		}
	}
}