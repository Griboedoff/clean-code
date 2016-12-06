using System.Collections.Generic;
using System.Linq;

namespace Markdown.Tokens
{
	//Todo много однотипных классов XxxHtmlToken - цель?
	public abstract class HtmlToken
	{
		protected readonly string Tag;
		protected readonly List<HtmlToken> ParsedTokens;
		protected readonly string Data;

		public int Length;

		protected readonly int EscapedCharsCount;

		protected HtmlToken(string tag, string data, int escapedCharsCount)
		{
			Tag = tag;
			Data = data;
			EscapedCharsCount = escapedCharsCount;
			ParsedTokens = new List<HtmlToken>();
			Length = CalcLength();
		}

		protected HtmlToken(string tag, List<HtmlToken> parsedTokens, int escapedCharsCount)
		{
			Tag = tag;
			ParsedTokens = parsedTokens;
			EscapedCharsCount = escapedCharsCount;
			Length = CalcLength();
		}

		private int CalcLength()
		{
			return ParsedTokens.Sum(x => x.Length) + (Data ?? "").Length + EscapedCharsCount;
		}

		protected virtual string WrapWithTag(string dataToInsert, CssClassInfo cssClassInfo)
		{
			return !string.IsNullOrEmpty(Tag)
				? WrapWithTag(Tag, dataToInsert, cssClassInfo)
				: dataToInsert;
		}

		protected virtual string WrapWithTag(string tag, string dataToInsert, CssClassInfo cssClassInfo)
		{
			return $"<{tag}{GetCssClassDef(cssClassInfo)}>{dataToInsert}</{tag}>";
		}

		protected static string GetCssClassDef(CssClassInfo cssClassInfo)
		{
			return cssClassInfo == null ? "" : $" class=\"{cssClassInfo.ClassName}\"";
		}

		public virtual string Render(CssClassInfo cssClassInfo)
		{
			return ParsedTokens.Count > 0
				? WrapWithTag(string.Join("", ParsedTokens.Select(token => token.Render(cssClassInfo))), cssClassInfo)
				: WrapWithTag(Data, cssClassInfo);
		}
	}
}