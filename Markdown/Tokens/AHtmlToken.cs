using System.Collections.Generic;

namespace Markdown.Tokens
{
	public class AHtmlToken : HtmlToken
	{
		private readonly string url;
		private readonly bool isReferece;
		public override int Length => Data.Length + url.Length + 4 + escapedCharacters;
		public static readonly Dictionary<string, string> ReferenceUrlsBase = new Dictionary<string, string>();

		public AHtmlToken(string data, string url, int escapedCharacters) : base("a", data, escapedCharacters)
		{
			this.url = url;
			this.isReferece = isReferece;
		}

		public AHtmlToken(List<HtmlToken> parsedTokens, int escapedCharacters)
			: base("a", parsedTokens, escapedCharacters)
		{
		}

		public static void AddReferenceUrlBase(string url, string name)
		{
			ReferenceUrlsBase[name] = url;
		}

		public override string ToString()
		{
			if (!isReferece)
				return $"<a href=\"{url}\">{Data}</a>";
			return "";
		}
	}
}