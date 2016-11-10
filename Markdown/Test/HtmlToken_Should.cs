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
	}
}