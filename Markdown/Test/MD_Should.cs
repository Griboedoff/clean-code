using NUnit.Framework;

namespace Markdown.Test
{
	[TestFixture]
	class Md_Should
	{
		[TestCase("qwe asd zxc", ExpectedResult = "qwe asd zxc")]
		public string ParseNoMarkup(string md)
		{
//			throw new NotImplementedException();
			var mdParser = new Md(md);
			return mdParser.Render();
		}
	}
}