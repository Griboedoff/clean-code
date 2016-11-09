using NUnit.Framework;

namespace Markdown.Test
{
	[TestFixture]
	internal class Md_Should
	{
		[TestCase("qwe asd zxc", ExpectedResult = "qwe asd zxc")]
		public string ParseNoMarkup(string plainMd)
		{
			return new Md(plainMd).Render();
		}

		[TestCase("_asd_", ExpectedResult = "<em>asd</em>")]
		[TestCase("_a s d_", ExpectedResult = "<em>a s d</em>")]
		[TestCase("_a_ _s d_", ExpectedResult = "<em>a</em> <em>s d</em>")]
		public string ParseEmTag_IfTextInOneUnderscore(string plainMd)
		{
			return new Md(plainMd).Render();
		}

		[TestCase("_ d_", ExpectedResult = "_ d_")]
		[TestCase("_a _", ExpectedResult = "<em>a _</em>")]
		[TestCase("_aas__",ExpectedResult = "_aas__")]
		public string ParseEmTagCorrect_IfWhiteSpaceInTheStartOrEndOfToken(string plainMd)
		{
			return new Md(plainMd).Render();
		}
	}
}