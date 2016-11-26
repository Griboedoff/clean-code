using System.Collections.Generic;

namespace Markdown.Tokens
{
	public class OrderedListHtmlToken : HtmlToken
	{
		public OrderedListHtmlToken(string data) : base("ol", data, 0)
		{
		}

		public OrderedListHtmlToken(List<HtmlToken> parsedTokens) : base("ol", parsedTokens, 0)
		{
		}
	}
}