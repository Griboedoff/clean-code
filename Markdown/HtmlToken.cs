using System.Collections.Generic;
using System.Linq;

namespace Markdown
{
	public class HtmlToken
	{
		private readonly Tag tag;
		private readonly List<HtmlToken> parsedTokens;
		private readonly string data;
		private bool IsTagged => !tag.Equals(Tag.Empty);
		public int Length => parsedTokens.Sum(x => x.Length) + (data ?? "").Length + escapedCharacters + tag.Md.Length * 2;
		private readonly int escapedCharacters;

		public HtmlToken(Tag tag, string data, int escapedCharacters)
		{
			this.tag = tag;
			this.data = data;
			this.escapedCharacters = escapedCharacters;
			parsedTokens = new List<HtmlToken>();
		}

		public HtmlToken(Tag tag, List<HtmlToken> parsedTokens, int escapedCharacters)
		{
			this.tag = tag;
			this.parsedTokens = parsedTokens;
			this.escapedCharacters = escapedCharacters;
		}

		private string InsertInToTags(string dataToInsert) => IsTagged
			? $"<{tag.Html}>{dataToInsert}</{tag.Html}>"
			: dataToInsert;

		public override string ToString()
		{
			return parsedTokens.Count > 0
				? InsertInToTags(string.Join("", parsedTokens.Select(token => token.ToString())))
				: InsertInToTags(data);
		}
	}
}