using System.Collections.Generic;
using System.Linq;

namespace Markdown.Tokens
{
	public abstract class HtmlToken
	{
		protected readonly string Tag;
		protected readonly List<HtmlToken> ParsedTokens;
		protected readonly string Data;

		protected virtual bool IsTagged => !string.IsNullOrEmpty(Tag);

		public virtual int Length => ParsedTokens.Sum(x => x.Length) + (Data ?? "").Length + EscapedCharacters;

		protected readonly int EscapedCharacters;

		protected HtmlToken(string tag, string data, int escapedCharacters)
		{
			Tag = tag;
			Data = data;
			EscapedCharacters = escapedCharacters;
			ParsedTokens = new List<HtmlToken>();
		}

		protected HtmlToken(string tag, List<HtmlToken> parsedTokens, int escapedCharacters)
		{
			Tag = tag;
			ParsedTokens = parsedTokens;
			this.EscapedCharacters = escapedCharacters;
		}

		protected virtual string InsertInToTags(string dataToInsert, CssClassInfo cssClassInfo) => IsTagged
			? $"<{Tag}{GetCssClassDef(cssClassInfo)}>{dataToInsert}</{Tag}>"
			: dataToInsert;

		protected static string GetCssClassDef(CssClassInfo cssClassInfo)
		{
			return cssClassInfo == null ? "" : $" class=\"{cssClassInfo.ClassName}\"";
		}

		public virtual string Render(CssClassInfo cssClassInfo)
		{
			return ParsedTokens.Count > 0
				? InsertInToTags(string.Join("", ParsedTokens.Select(token => token.Render(cssClassInfo))), cssClassInfo)
				: InsertInToTags(Data, cssClassInfo);
		}
	}
}