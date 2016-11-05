using NUnit.Framework;

namespace Markdown.Test
{
	[TestFixture]
	public class HtmlToken_Should
	{
		[TestCase("p", "asd", ExpectedResult = "<p>asd</p>")]
		[TestCase("i", "some text", ExpectedResult = "<i>some text</i>")]
		public string ShouldInsertDataInToTags_WhenToStringCalls(string tag, string data)
		{
			var token = new HtmlToken(tag, data);
			return token.ToString();
		}
	}
}