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

		private readonly Dictionary<Tag, Func<int, string, int, HtmlToken>> mdTagParserFuncMatch;
		private readonly Dictionary<Tag, Func<int, bool, bool>> validateFunctions;

		public Md(string plainMd)
		{
			this.plainMd = plainMd;
			root = new List<HtmlToken>();
			mdTagParserFuncMatch = new Dictionary<Tag, Func<int, string, int, HtmlToken>>
			{
				[Tag.Em] = ParseEmToken,
				[Tag.Empty] = ParseNoMarkup,
				[Tag.Strong] = ParseStrongToken
			};
			validateFunctions = new Dictionary<Tag, Func<int, bool, bool>>
			{
				[Tag.Em] = IsValidEmTag,
				[Tag.Strong] = IsValidStrongTag,
				[Tag.Empty] = (i, b) => false
			};
		}

		private HtmlToken ParseEmToken(int index, string alreadyParsed = "", int alreadyEscaped = 0)
		{
			if (!IsValidEmTag(index, true))
				return ParseNoMarkup(index);

			index++;
			var tokenData = new StringBuilder(alreadyParsed);

			while (index < plainMd.Length && !IsValidEmTag(index, false))
			{
				var tag = ParseTag(index);

				if (tag.Equals(Tag.Strong))
				{
					tokenData.Append("__");
					index += 2;
					continue;
				}

				if (plainMd[index] == '\\')
				{
					index++;
					alreadyEscaped++;
				}

				tokenData.Append(plainMd[index]);
				index++;
			}

			return index != plainMd.Length
				? new HtmlToken(Tag.Em, tokenData.ToString(), alreadyEscaped)
				: new HtmlToken(Tag.Empty, tokenData.Insert(0, '_').ToString(), alreadyEscaped);
		}

		private HtmlToken ParseStrongToken(int index, string alreadyParsed = "", int alreadyEscaped = 0)
		{
			if (!IsValidStrongTag(index, true))
				return ParseNoMarkup(index);

			var parsedTokens = new List<HtmlToken>();
			index += 2;
			var tokenData = new StringBuilder(alreadyParsed);

			while (index < plainMd.Length && !IsValidStrongTag(index, false))
			{
				var tag = ParseTag(index);

				if (Equals(tag, Tag.Em))
				{
					parsedTokens.Add(new HtmlToken(Tag.Empty, tokenData.ToString(), alreadyEscaped));
					alreadyEscaped = 0;
					tokenData.Clear();
					var htmlToken = ParseEmToken(index);
					parsedTokens.Add(htmlToken);
					index += htmlToken.Length;
					if (index == plainMd.Length)
						break;
				}

				if (plainMd[index] == '\\')
				{
					index++;
					alreadyEscaped++;
				}

				tokenData.Append(plainMd[index]);
				index++;
			}

			parsedTokens.Add(new HtmlToken(Tag.Empty, tokenData.ToString(), alreadyEscaped));
			return index != plainMd.Length
				? new HtmlToken(Tag.Strong, parsedTokens, 0)
				: new HtmlToken(Tag.Empty, tokenData.Insert(0, "__").ToString(), alreadyEscaped);
		}

		private HtmlToken ParseNoMarkup(int index, string alreadyParsed = "", int alreadyEscaped = 0)
		{
			var tokenData = new StringBuilder(alreadyParsed);
			var escaped = alreadyEscaped;
			while (index < plainMd.Length)
			{
				var tag = ParseTag(index);

				if (validateFunctions[tag].Invoke(index, true))
					break;
				if (plainMd[index] == '\\')
				{
					index++;
					escaped++;
				}
				tokenData.Append(plainMd[index]);
				index++;
			}
			return new HtmlToken(Tag.Empty, tokenData.ToString(), escaped);
		}

		private bool IsValidEmTag(int tagIndex, bool isOpenTag)
		{
			if (plainMd[tagIndex] != '_')
				return false;
			try
			{
				return !(tagIndex == plainMd.Length - 1 && isOpenTag)
				       && plainMd[tagIndex + (isOpenTag ? 1 : -1)] != ' '
				       && !(plainMd[tagIndex - 1] == '_' || plainMd[tagIndex + 1] == '_')
				       && (!char.IsDigit(plainMd[tagIndex - 1]) && !char.IsDigit(plainMd[tagIndex + 1]));
			}
			catch (IndexOutOfRangeException)
			{
				return true;
			}
		}

		private bool IsValidStrongTag(int tagIndex, bool isOpenTag)
		{
			if (plainMd[tagIndex] != '_' || !ParseTag(tagIndex).Equals(Tag.Strong))
				return false;
			try
			{
				return !(tagIndex == plainMd.Length - 1 && isOpenTag)
				       && plainMd[tagIndex + (isOpenTag ? 2 : -1)] != ' ';
			}
			catch (IndexOutOfRangeException)
			{
				return true;
			}
		}

		private Tag ParseTag(int tagIndex)
		{
			if (plainMd[tagIndex] != '_')
				return Tag.Empty;
			if (tagIndex != plainMd.Length - 1)
				return plainMd[tagIndex + 1] == '_' ? Tag.Strong : Tag.Em;
			return Tag.Em;
		}

		private void TryParseToHtml()
		{
			var i = 0;
			while (i < plainMd.Length)
			{
				var tag = ParseTag(i);
				var parsedToken = mdTagParserFuncMatch[tag].Invoke(i, "", 0);
				i += parsedToken.Length;
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