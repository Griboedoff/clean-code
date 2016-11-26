using System.Collections.Generic;

namespace Markdown.Tokens
{
	public class HHtmlToken : HtmlToken

	{
		private readonly int headerImportance;

		public HHtmlToken(string data, int headerImportance) : base($"h{headerImportance}", data, 0)
		{
			this.headerImportance = headerImportance;
		}

		public HHtmlToken(List<HtmlToken> parsedTokens, int headerImportance) : base($"h{headerImportance}", parsedTokens, 0)
		{
			this.headerImportance = headerImportance;
		}
	}
}