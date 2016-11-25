using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Text;
using Markdown.Tokens;

namespace Markdown
{
	public class Md
	{
		private readonly string[] plainMd;
		private readonly string baseUrl;
		private readonly CssClassInfo cssClassInfo;

		private readonly Dictionary<Tag, Func<string, int, string, int, HtmlToken>> mdTagParserFuncMatch;

		private static readonly Dictionary<Tag, Func<string, int, bool, bool>> ValidateFunctions = new Dictionary
			<Tag, Func<string, int, bool, bool>>
			{
				[Tag.Em] = IsValidEmTag,
				[Tag.Strong] = IsValidStrongTag,
				[Tag.Empty] = (q, w, e) => false,
				[Tag.A] = IsValidATag
			};

		private int currLineIndex;
		private string CurrLine => plainMd[currLineIndex];

		public Md(string plainMd, string baseUrl = "", CssClassInfo cssClassInfo = null)
		{
			this.plainMd = plainMd.Replace("\n", "\0\n\0").Split('\0');
			this.baseUrl = baseUrl;
			this.cssClassInfo = cssClassInfo;

			mdTagParserFuncMatch = new Dictionary<Tag, Func<string, int, string, int, HtmlToken>>
			{
				[Tag.Em] = ParseEmToken,
				[Tag.Empty] = ParseNoMarkup,
				[Tag.Strong] = ParseStrongToken,
				[Tag.A] = ParseUrl
			};


			currLineIndex = 0;
		}

		private static HtmlToken ParseEmToken(string currLine, int index, string alreadyParsed = "", int alreadyEscaped = 0)
		{
			if (!IsValidEmTag(currLine, index, true))
				return ParseNoMarkup(currLine, index);

			index++;
			var tokenData = new StringBuilder(alreadyParsed);

			while (index < currLine.Length && !IsValidEmTag(currLine, index, false))
			{
				var tag = ParseTag(currLine, index);

				if (tag.Equals(Tag.Strong))
				{
					tokenData.Append("__");
					index += 2;
					continue;
				}

				if (currLine[index] == '\\')
				{
					index++;
					alreadyEscaped++;
				}

				tokenData.Append(currLine[index]);
				index++;
			}

			return index != currLine.Length
				? (HtmlToken) new EmHtmlToken(tokenData.ToString(), alreadyEscaped)
				: new EmptyHtmlToken(tokenData.Insert(0, '_').ToString(), alreadyEscaped);
		}

		private static HtmlToken ParseStrongToken(string currLine, int index, string alreadyParsed = "",
			int alreadyEscaped = 0)
		{
			if (!IsValidStrongTag(currLine, index, true))
				return ParseNoMarkup(currLine, index);

			var parsedTokens = new List<HtmlToken>();
			index += 2;
			var tokenData = new StringBuilder(alreadyParsed);

			while (index < currLine.Length && !IsValidStrongTag(currLine, index, false))
			{
				var tag = ParseTag(currLine, index);

				if (Equals(tag, Tag.Em))
				{
					parsedTokens.Add(ParseEmInStrong(currLine, ref index, ref alreadyEscaped, parsedTokens, tokenData));
					if (index == currLine.Length)
						break;
				}

				if (currLine[index] == '\\')
				{
					index++;
					alreadyEscaped++;
				}

				tokenData.Append(currLine[index]);
				index++;
			}

			parsedTokens.Add(new EmptyHtmlToken(tokenData.ToString(), alreadyEscaped));
			return index != currLine.Length
				? (HtmlToken) new StrongHtmlToken(parsedTokens, 0)
				: new EmptyHtmlToken(tokenData.Insert(0, "__").ToString(), alreadyEscaped);
		}

		private static HtmlToken ParseNoMarkup(string currLine, int index, string alreadyParsed = "", int alreadyEscaped = 0)
		{
			var tokenData = new StringBuilder(alreadyParsed);
			var escaped = alreadyEscaped;
			while (index < currLine.Length)
			{
				var tag = ParseTag(currLine, index);

				if (ValidateFunctions[tag].Invoke(currLine, index, true))
					break;
				if (currLine[index] == '\\')
				{
					index++;
					escaped++;
				}
				tokenData.Append(currLine[index]);
				index++;
			}
			return new EmptyHtmlToken(tokenData.ToString(), escaped);
		}

		private HtmlToken ParseUrl(string currLine, int index, string alreadyParsed = "", int alreadyEscaped = 0)
		{
			var returnedValue = ParseInsideBracers(']', index, alreadyEscaped, alreadyParsed, currLine);
			var escaped = returnedValue.Item2;
			index = returnedValue.Item1;
			var urlText = returnedValue.Item3;

			if (currLine[index] != '(')
				throw new MdParserException($"Can't parse link at index {index}");
			returnedValue = ParseInsideBracers(')', index, alreadyEscaped, alreadyParsed, currLine);
			return new AHtmlToken(urlText, returnedValue.Item3, escaped + returnedValue.Item2, baseUrl);
		}

		private HtmlToken ParseParagraph()
		{
			var innerTags = new List<HtmlToken>();

			while (currLineIndex < plainMd.Length && !string.IsNullOrWhiteSpace(CurrLine))
			{
				var i = 0;
				while (i < CurrLine.Length)
				{
					var tag = ParseTag(CurrLine, i);
					var parsedToken = mdTagParserFuncMatch[tag].Invoke(CurrLine, i, "", 0);
					i += parsedToken.Length;
					innerTags.Add(parsedToken);
				}
				currLineIndex++;
			}

			return new PHtmlToken(innerTags);
		}

		private static Tuple<int, int, string> ParseInsideBracers(char closeBracer, int index, int escaped,
			string alreadyParsed, string currLine)
		{
			var data = new StringBuilder(alreadyParsed);
			index++;
			while (index < currLine.Length && currLine[index] != closeBracer)
			{
				if (currLine[index] == '\\')
				{
					index++;
					escaped++;
				}
				data.Append(currLine[index]);
				index++;
			}
			index++;
			var dataStr = data.ToString();
			return Tuple.Create(index, escaped, dataStr);
		}

		private static HtmlToken ParseEmInStrong(string currLine, ref int index, ref int alreadyEscaped,
			ICollection<HtmlToken> parsedTokens, StringBuilder tokenData)
		{
			parsedTokens.Add(new EmptyHtmlToken(tokenData.ToString(), alreadyEscaped));
			alreadyEscaped = 0;
			tokenData.Clear();
			var htmlToken = ParseEmToken(currLine, index);
			index += htmlToken.Length;
			return htmlToken;
		}

		private static bool NotInsideDigits(int tagIndex, string currLine)
		{
			if (tagIndex + 1 == currLine.Length || tagIndex - 1 == -1)
				return true;
			return !char.IsDigit(currLine[tagIndex - 1]) && !char.IsDigit(currLine[tagIndex + 1]);
		}

		private static bool IsNotStrongTag(int tagIndex, string currLine)
		{
			return !(tagIndex - 1 != -1 && currLine[tagIndex - 1] == '_' ||
			         tagIndex + 1 != currLine.Length && currLine[tagIndex + 1] == '_');
		}

		private static bool NoSpaceNearMdTag(int tagIndex, int tagLength, bool isOpenTag, string currLine)
		{
			var nextIndex = tagIndex + (isOpenTag ? tagLength : -1);
			return nextIndex >= 0 && nextIndex < currLine.Length && currLine[nextIndex] != ' ';
		}

		private static bool IsNotOpenTagInEndOfString(int tagIndex, int tagLength, bool isOpenTag, string currLine)
		{
			return !(tagIndex == currLine.Length - tagLength && isOpenTag);
		}

		private static bool IsValidEmTag(string currLine, int tagIndex, bool isOpenTag)
		{
			if (currLine[tagIndex] != '_')
				return false;
			return IsNotOpenTagInEndOfString(tagIndex, 1, isOpenTag, currLine)
			       && NoSpaceNearMdTag(tagIndex, 1, isOpenTag, currLine)
			       && IsNotStrongTag(tagIndex, currLine)
			       && NotInsideDigits(tagIndex, currLine);
		}

		private static bool IsValidStrongTag(string currLine, int tagIndex, bool isOpenTag)
		{
			if (currLine[tagIndex] != '_' || !ParseTag(currLine, tagIndex).Equals(Tag.Strong))
				return false;
			return IsNotOpenTagInEndOfString(tagIndex, 2, isOpenTag, currLine)
			       && NoSpaceNearMdTag(tagIndex, 2, isOpenTag, currLine);
		}

		private static bool IsValidATag(string currLine, int tagIndex, bool isOpenTag)
		{
			return currLine[tagIndex] == '[';
		}

		private static Tag ParseTag(string currLine, int tagIndex)
		{
			if (currLine[tagIndex] == '[')
				return Tag.A;
			if (currLine[tagIndex] != '_')
				return Tag.Empty;
			if (tagIndex != currLine.Length - 1)
				return currLine[tagIndex + 1] == '_'
					? Tag.Strong
					: Tag.Em;
			return Tag.Em;
		}

		private IEnumerable<HtmlToken> TryParseToHtml()
		{
			var root = new List<HtmlToken>();

			while (currLineIndex < plainMd.Length)
			{
				root.Add(ParseParagraph());
				currLineIndex++;
			}

			return root;
		}

		public string Render()
		{
			var htmlTokens = TryParseToHtml();
			return string.Join("",
				cssClassInfo == null ? "" : cssClassInfo.Description,
				string.Join("", htmlTokens.Select(x => x.Render(cssClassInfo))));
		}
	}
}