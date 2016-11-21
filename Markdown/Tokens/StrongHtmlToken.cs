using System.Collections.Generic;

namespace Markdown.Tokens
{
	public class StrongHtmlToken : HtmlToken
	{
		public override int Length => base.Length + 4;

		public StrongHtmlToken(string data, int escapedCharacters) : base("strong", data, escapedCharacters)
		{
		}

		public StrongHtmlToken(List<HtmlToken> parsedTokens, int escapedCharacters)
			: base("strong", parsedTokens, escapedCharacters)
		{
		}
	}
}