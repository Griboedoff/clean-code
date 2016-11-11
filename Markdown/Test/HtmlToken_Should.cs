using System.Collections.Generic;
using NUnit.Framework;
using FluentAssertions;
namespace Markdown.Test
{
	[TestFixture]
	public class HtmlToken_Should
	{
		[Test]
		public void ShouldInsertDataInToTags_WhenToStringCalls()
		{
			var token = new HtmlToken(Tag.Em, "data", 0);

			token.ToString().Should().Be("<em>data</em>");
		}

		[Test]
		public void ShouldConcatManyTags_WhenHasInsertedTags()
		{
			var tokenList = new List<HtmlToken>
			{
				new HtmlToken(Tag.Em, "italic", 0),
				new HtmlToken(Tag.Empty, "empty", 0),
				new HtmlToken(Tag.Strong, "bold", 0)
			};

			var token = new HtmlToken(Tag.Strong, tokenList, 0);
			token.ToString().Should().Be("<strong><em>italic</em>empty<strong>bold</strong></strong>");
		}
	}
}