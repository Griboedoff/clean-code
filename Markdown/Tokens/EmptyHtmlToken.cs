using System.Collections.Generic;
using System.Linq;

namespace Markdown.Tokens
{
	public class EmptyHtmlToken : HtmlToken
	{
		public EmptyHtmlToken(string data, int escapedCharacters) : base("", data, escapedCharacters)
		{
		}

		public EmptyHtmlToken(List<HtmlToken> parsedTokens, int escapedCharacters)
			: base("", parsedTokens, escapedCharacters)
		{
		}
	}
}