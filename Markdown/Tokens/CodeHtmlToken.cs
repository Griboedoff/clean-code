using System.Collections.Generic;

namespace Markdown.Tokens
{
	public class CodeHtmlToken : HtmlToken
	{
		public CodeHtmlToken(string data) : base("code", data, 0)
		{
		}

		public CodeHtmlToken(List<HtmlToken> parsedTokens) : base("code", parsedTokens, 0)
		{
		}

		public override string Render(CssClassInfo cssClassInfo)
		{
			return InsertInToTags("pre", InsertInToTags(Data, cssClassInfo), cssClassInfo);
		}
	}
}