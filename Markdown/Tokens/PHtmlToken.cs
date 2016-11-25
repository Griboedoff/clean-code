using System.Collections.Generic;
using System.Linq;

namespace Markdown.Tokens
{
	public class PHtmlToken : HtmlToken
	{
		public PHtmlToken(string data) : base("p", data, 0)
		{
		}

		public PHtmlToken(List<HtmlToken> parsedTokens) : base("p", parsedTokens, 0)
		{
		}

	}
}