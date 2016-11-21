using System.Collections.Generic;

namespace Markdown.Tokens
{
	public class EmHtmlToken : HtmlToken
	{
		public override int Length => base.Length + 2;

		public EmHtmlToken(string data, int escapedCharacters) : base("em", data, escapedCharacters)
		{
		}

		public EmHtmlToken(List<HtmlToken> parsedTokens, int escapedCharacters)
			: base("em", parsedTokens, escapedCharacters)
		{
		}
	}
}