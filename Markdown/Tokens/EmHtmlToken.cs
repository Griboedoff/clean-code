using System.Collections.Generic;

namespace Markdown.Tokens
{
	public class EmHtmlToken : HtmlToken
	{
		//Todo Непонятно. Выглядит как какой-то хак
		public int Length => base.Length + 2;

		public EmHtmlToken(string data, int escapedCharsCount) : base("em", data, escapedCharsCount)
		{
		}

		public EmHtmlToken(List<HtmlToken> parsedTokens, int escapedCharsCount)
			: base("em", parsedTokens, escapedCharsCount)
		{
		}
	}
}