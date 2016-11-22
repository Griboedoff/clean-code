using System.Collections.Generic;

namespace Markdown.Tokens
{
	public class AHtmlToken : HtmlToken
	{
		private readonly string url;
		private readonly string baseUrl;
		private bool IsReferece => url.StartsWith("/");
		public override int Length => Data.Length + url.Length + 4 + escapedCharacters;

		public AHtmlToken(string data, string url, int escapedCharacters, string baseUrl) : base("a", data, escapedCharacters)
		{
			this.url = url;
			this.baseUrl = baseUrl;
		}

		public AHtmlToken(List<HtmlToken> parsedTokens, int escapedCharacters)
			: base("a", parsedTokens, escapedCharacters)
		{
		}

		public override string Render(CssClassInfo cssClassInfo)
		{
			var buildedUrl = !IsReferece ? url : string.Join("", baseUrl, url);
			return $"<a href=\"{buildedUrl}\"{GetCssClassDef(cssClassInfo)}>{Data}</a>";
		}
	}
}