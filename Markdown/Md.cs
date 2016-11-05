using System;
using System.Collections.Generic;
using System.Linq;

namespace Markdown
{
	public class Md
	{
		private readonly string plainMd;
		private readonly List<HtmlToken> root;

		private static readonly Dictionary<char, Func<string, int, HtmlToken>> MdTagParserFuncMatch =
			new Dictionary<char, Func<string, int, HtmlToken>>
			{
				['_'] = ParseEmToken
			};

		public Md(string plainMd)
		{
			this.plainMd = plainMd;
			root = new List<HtmlToken>();
		}

		#region Parser Funcs

		private static HtmlToken ParseEmToken(string plainMd, int index)
		{
			var i = 1;
			while (index + i < plainMd.Length && plainMd[index + i] != '_')
			{
				i++;
			}
			return new HtmlToken("em", plainMd.Substring(index + 1, i - 1));
		}

		private static HtmlToken ParseNoMarkup(string plainMd, int index)
		{
			var i = 1;
			while (index + i < plainMd.Length)
			{
				if (MdTagParserFuncMatch.ContainsKey(plainMd[index + i++]))
					return new HtmlToken("", plainMd.Substring(index, i - 1));
			}
			return new HtmlToken("", plainMd.Substring(index, i));
		}

		private static bool IsActiveTag(string plainMd, int index, Tag mdTag)
		{

		}
		#endregion

		private void TryParseToHtml()
		{
			for (var i = 0; i < plainMd.Length; i++)
			{
				var parserFunc = MdTagParserFuncMatch.ContainsKey(plainMd[i])
					? MdTagParserFuncMatch[plainMd[i]]
					: ParseNoMarkup;
				var parsedToken = parserFunc.Invoke(plainMd, i);
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