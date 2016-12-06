using System.Collections.Generic;

namespace Markdown.Tokens
{
	public class StrongHtmlToken : HtmlToken
	{
		public int Length => base.Length + 4;

		public StrongHtmlToken(string data, int escapedCharsCount) : base("strong", data, escapedCharsCount)
		{
		}

		public StrongHtmlToken(List<HtmlToken> parsedTokens, int escapedCharsCount)
			: base("strong", parsedTokens, escapedCharsCount)
		{
		}
	}
}