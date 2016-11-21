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

		public virtual int Length => ParsedTokens.Sum(x => x.Length) + (Data ?? "").Length + escapedCharacters;

		protected readonly int escapedCharacters;

		protected HtmlToken(string tag, string data, int escapedCharacters)
		{
			Tag = tag;
			Data = data;
			this.escapedCharacters = escapedCharacters;
			ParsedTokens = new List<HtmlToken>();
		}

		protected HtmlToken(string tag, List<HtmlToken> parsedTokens, int escapedCharacters)
		{
			Tag = tag;
			ParsedTokens = parsedTokens;
			this.escapedCharacters = escapedCharacters;
		}

		protected virtual string InsertInToTags(string dataToInsert) => IsTagged
			? $"<{Tag}>{dataToInsert}</{Tag}>"
			: dataToInsert;

		public override string ToString()
		{
			return ParsedTokens.Count > 0
				? InsertInToTags(string.Join("", ParsedTokens.Select(token => token.ToString())))
				: InsertInToTags(Data);
		}
	}
}