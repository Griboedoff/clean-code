using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Markdown
{
	public class Md
	{
		private readonly string plainMd;
		private readonly List<HtmlToken> root;

		private readonly Dictionary<Tag, Func<int, string, HtmlToken>> mdTagParserFuncMatch;
		private readonly Dictionary<Tag, Func<int, bool, bool>> validateFunctions;

		public Md(string plainMd)
		{
			this.plainMd = plainMd;
			root = new List<HtmlToken>();
			mdTagParserFuncMatch = new Dictionary<Tag, Func<int, string, HtmlToken>>
			{
				[Tag.EmMd] = ParseEmToken,
				[Tag.Empty] = ParseNoMarkup
			};
			validateFunctions = new Dictionary<Tag, Func<int, bool, bool>>
			{
				[Tag.EmMd] = IsValidEmTag
			};
		}

		#region Parser Funcs

		private HtmlToken ParseEmToken(int index, string alreadyParsed = "")
		{
			if (!IsValidEmTag(index, true))
				return ParseNoMarkup(index);
			index++;
			var tokenData = new StringBuilder();
			while (index < plainMd.Length && !IsValidEmTag(index, false))
			{
				if (plainMd[index] == '_')
				{
					var tag = ParseTag(index);

					if (tag == Tag.StrongMd)
					{
						tokenData.Append("__");
						return ParseNoMarkup(index + 2, string.Join("", "_", tokenData.ToString()));
					}
				}
				if (plainMd[index] == '\\')
					index++;
				tokenData.Append(plainMd[index]);
				index++;
			}
			return new HtmlToken(Tag.EmMd, tokenData.ToString());
		}

		private HtmlToken ParseNoMarkup(int index, string alreadyParsed = "")
		{
			var tokenData = new StringBuilder(alreadyParsed);
			while (index < plainMd.Length)
			{
				if (plainMd[index] == '_')
				{
					var tag = ParseTag(index);

					if (validateFunctions[tag].Invoke(index, true))
						break;
				}
				if (plainMd[index] == '\\')
					index++;
				tokenData.Append(plainMd[index]);
				index++;
			}
			return new HtmlToken(Tag.Empty, tokenData.ToString());
		}

		#endregion

		#region Validation Functions

		private bool IsValidEmTag(int tokenIndex, bool isOpenTag)
		{
			if (plainMd[tokenIndex] != '_')
				return false;
			try
			{
				return !(tokenIndex == plainMd.Length - 1 && isOpenTag)
				       && plainMd[tokenIndex + (isOpenTag ? 1 : -1)] != ' '
				       && !(plainMd[tokenIndex - 1] == '_' || plainMd[tokenIndex + 1] == '_')
				       && (!char.IsDigit(plainMd[tokenIndex - 1]) && !char.IsDigit(plainMd[tokenIndex + 1]));
			}
			catch (IndexOutOfRangeException)
			{
				return true;
			}
		}

		#endregion

		private Tag ParseTag(int tagIndex)
		{
			if (tagIndex != plainMd.Length - 1)
				return plainMd[tagIndex + 1] == '_' ? Tag.StrongMd : Tag.EmMd;
			return Tag.EmMd;
		}

		private void TryParseToHtml()
		{
			for (var i = 0; i < plainMd.Length; i++)
			{
				var tag = Tag.Empty;
				if (plainMd[i] == '_')
					tag = ParseTag(i);
				var parsedToken = mdTagParserFuncMatch[tag].Invoke(i, "");
				i += parsedToken.Length + (parsedToken.IsTagged
					     ? 1
					     : -1);
				root.Add(parsedToken);
			}
		}

		public string Render()
		{
			TryParseToHtml();
			return string.Join("", root.Select(x => x.ToString()));
		}
	}
}