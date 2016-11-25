using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Markdown.Tokens;

namespace Markdown
{
	public class Md
	{
		private readonly string[] plainMd;
		private readonly string baseUrl;
		private readonly CssClassInfo cssClassInfo;

		private readonly Dictionary<Tag, Func<string, int, string, int, HtmlToken>> stringTagParserFuncMatch;

		private static readonly Dictionary<Tag, Func<string, int, bool, bool>> ValidateFunctions = new Dictionary
			<Tag, Func<string, int, bool, bool>>
			{
				[Tag.Em] = IsValidEmTag,
				[Tag.Strong] = IsValidStrongTag,
				[Tag.Empty] = (q, w, e) => false,
				[Tag.A] = IsValidATag
			};

		private readonly Dictionary<LineType, Func<HtmlToken>> lineTagParserFuncMatch;

		private int currLineIndex;
		private string CurrLine => plainMd[currLineIndex];
		private bool IsInPlainMd => currLineIndex < plainMd.Length;

		public Md(string plainMd, string baseUrl = "", CssClassInfo cssClassInfo = null)
		{
			this.plainMd = plainMd.Split('\n');
			this.baseUrl = baseUrl;
			this.cssClassInfo = cssClassInfo;

			stringTagParserFuncMatch = new Dictionary<Tag, Func<string, int, string, int, HtmlToken>>
			{
				[Tag.Em] = ParseItalic,
				[Tag.Empty] = ParseNoMarkup,
				[Tag.Strong] = ParseBold,
				[Tag.A] = ParseUrl,
			};

			lineTagParserFuncMatch = new Dictionary<LineType, Func<HtmlToken>>
			{
				[LineType.Header] = ParseHeader,
				[LineType.Simple] = ParseParagraph,
				[LineType.CodeBlock] = ParseCodeBlock
			};

			currLineIndex = 0;
		}

		private HtmlToken ParseHeader()
		{
			var headerText = CurrLine.Replace("#", "");
			headerText = headerText.Replace("\\", "");

			var headerImportance = CurrLine.Length - headerText.Length;

			currLineIndex++;
			return new HHtmlToken(headerText, headerImportance);
		}

		private HtmlToken ParseCodeBlock()
		{
			var builder = new StringBuilder();

			while (IsInPlainMd && GetLineTypeTag(CurrLine) == LineType.CodeBlock)
			{
				builder.Append(CurrLine.Substring(CurrLine.StartsWith("\t") ? 1 : 4));
				builder.Append("\n");
				currLineIndex++;

			}

			builder.Remove(builder.Length - 1, 1);

			return new CodeHtmlToken(builder.ToString());
		}

		private HtmlToken ParseParagraph()
		{
			var innerTags = new List<HtmlToken>();

			while (IsInPlainMd && !string.IsNullOrWhiteSpace(CurrLine) && GetLineTypeTag(CurrLine) == LineType.Simple)
			{
				if (innerTags.Count != 0)
					innerTags.Add(new EmptyHtmlToken("\n", 0));
				var i = 0;
				while (i < CurrLine.Length)
				{
					var tag = ParseTag(CurrLine, i);
					var parsedToken = stringTagParserFuncMatch[tag].Invoke(CurrLine, i, "", 0);
					i += parsedToken.Length;
					innerTags.Add(parsedToken);
				}
				currLineIndex++;
			}

			return new PHtmlToken(innerTags);
		}

		private static HtmlToken ParseItalic(string currLine, int index, string alreadyParsed = "", int alreadyEscaped = 0)
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

		private static HtmlToken ParseBold(string currLine, int index, string alreadyParsed = "",
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
			var htmlToken = ParseItalic(currLine, index);
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
			if (tagIndex == currLine.Length - 1)
				return Tag.Em;
			return currLine[tagIndex + 1] == '_'
				? Tag.Strong
				: Tag.Em;
		}

		private static LineType GetLineTypeTag(string currLine)
		{
			if (currLine.StartsWith("#"))
				return LineType.Header;
			if (currLine.StartsWith("    ") || currLine.StartsWith("\t"))
				return LineType.CodeBlock;
			return LineType.Simple;
		}

		private IEnumerable<HtmlToken> TryParseToHtml()
		{
			var root = new List<HtmlToken>();

			while (currLineIndex < plainMd.Length)
			{
				var htmlToken = lineTagParserFuncMatch[GetLineTypeTag(CurrLine)].Invoke();
				root.Add(htmlToken);
				while (currLineIndex < plainMd.Length && string.IsNullOrWhiteSpace(CurrLine) && string.IsNullOrEmpty(CurrLine))
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