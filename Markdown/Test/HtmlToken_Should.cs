using System.Collections.Generic;
using NUnit.Framework;
using FluentAssertions;
using Markdown.Tokens;

namespace Markdown.Test
{
	[TestFixture]
	public class HtmlToken_Should
	{
		[Test]
		public void ShouldInsertDataInToTags_WhenToStringCalls()
		{
			var token = new EmHtmlToken("data", 0);

			token.Render(null).Should().Be("<em>data</em>");
		}

		[Test]
		public void ShouldConcatManyTags_WhenHasInsertedTags()
		{
			var tokenList = new List<HtmlToken>
			{
				new EmHtmlToken("italic", 0),
				new EmptyHtmlToken("empty", 0),
				new StrongHtmlToken("bold", 0)
			};

			var token = new StrongHtmlToken(tokenList, 0);
			token.Render(null).Should().Be("<strong><em>italic</em>empty<strong>bold</strong></strong>");
		}
	}
}