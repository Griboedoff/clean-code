using System.Collections.Generic;

namespace Markdown.Tokens
{
	public class AHtmlToken : HtmlToken
	{
		private readonly string url;
		private readonly string baseUrl;
		private bool IsReferece => url.StartsWith("/");
		public int Length => Data.Length + url.Length + 4 + EscapedCharsCount;

		public AHtmlToken(string data, string url, int escapedCharsCount, string baseUrl) : base("a", data, escapedCharsCount)
		{
			this.url = url;
			this.baseUrl = baseUrl;
		}

		public AHtmlToken(List<HtmlToken> parsedTokens, int escapedCharsCount)
			: base("a", parsedTokens, escapedCharsCount)
		{
		}

		public override string Render(CssClassInfo cssClassInfo)
		{
			var buildedUrl = !IsReferece ? url : string.Join("", baseUrl, url);
			return $"<a href=\"{buildedUrl}\"{GetCssClassDef(cssClassInfo)}>{Data}</a>";
		}
	}
}