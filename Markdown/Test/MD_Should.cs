using System;
using System.Diagnostics;
using System.Linq;
using System.Text;
using FluentAssertions;
using NUnit.Framework;

namespace Markdown.Test
{
	[TestFixture]
	internal class Md_Should
	{
		[TestCase("qwe asd zxc", ExpectedResult = "<p>qwe asd zxc</p>")]
		public string ParseNoMarkup(string plainMd)
		{
			return new Md(plainMd).Render();
		}

		[TestCase("_asd_", ExpectedResult = "<p><em>asd</em></p>")]
		[TestCase("_a s d_", ExpectedResult = "<p><em>a s d</em></p>")]
		[TestCase("_1_2_3_", ExpectedResult = "<p><em>1_2_3</em></p>")]
		[TestCase("_a_ _s d_", ExpectedResult = "<p><em>a</em> <em>s d</em></p>")]
		[TestCase("_a_ _s d", ExpectedResult = "<p><em>a</em> _s d</p>")]
		[TestCase("_aas__abc__abc_", ExpectedResult = "<p><em>aas__abc__abc</em></p>")]
		public string ParseEmTagCorrectly(string plainMd)
		{
			return new Md(plainMd).Render();
		}

		[TestCase("_ d_", ExpectedResult = "<p>_ d_</p>")]
		[TestCase("_a _", ExpectedResult = "<p>_a _</p>")]
		[TestCase("__ d__", ExpectedResult = "<p>__ d__</p>")]
		[TestCase("__a __", ExpectedResult = "<p>__a __</p>")]
		[TestCase("_ abc_abc_", ExpectedResult = "<p>_ abc<em>abc</em></p>")]
		public string ParseTrailingWhitespaceCorrectly(string plainMd)
		{
			return new Md(plainMd).Render();
		}

		[TestCase("_ab cd", ExpectedResult = "<p>_ab cd</p>")]
		[TestCase("__ab cd", ExpectedResult = "<p>__ab cd</p>")]
		public string ParseNoMarkup_IfMissingCloseTag(string plainMd)
		{
			return new Md(plainMd).Render();
		}

		[TestCase(@"_a\_b_", ExpectedResult = "<p><em>a_b</em></p>")]
		[TestCase(@"__a\_b__", ExpectedResult = "<p><strong>a_b</strong></p>")]
		[TestCase(@"a\_b", ExpectedResult = "<p>a_b</p>")]
		public string ParseEscapedCorrectly(string plainMd)
		{
			return new Md(plainMd).Render();
		}

		[TestCase("__abc_abc", ExpectedResult = "<p>__abc_abc</p>")]
		[TestCase("__abc_abc_", ExpectedResult = "<p>__abc<em>abc</em></p>")]
		[TestCase("_abc__abc", ExpectedResult = "<p>_abc__abc</p>")]
		[TestCase("_abc__abc__", ExpectedResult = "<p>_abc__abc__</p>")]
		public string ParseUnpairTags(string plainMd)
		{
			return new Md(plainMd).Render();
		}

		[TestCase("__abc__", ExpectedResult = "<p><strong>abc</strong></p>")]
		[TestCase("__abc_abc_abc__", ExpectedResult = "<p><strong>abc<em>abc</em>abc</strong></p>")]
		public string ParseStrongTagCorrectrly(string plainMd)
		{
			return new Md(plainMd).Render();
		}

		[TestCase("[url](www.url.com)", "", ExpectedResult = "<p><a href=\"www.url.com\">url</a></p>")]
		[TestCase("[url](/url)", "www.base.com", ExpectedResult = "<p><a href=\"www.base.com/url\">url</a></p>")]
		[TestCase("[url](www.url.com)\n[url](/url)", "www.base.com",
			 ExpectedResult = "<p><a href=\"www.url.com\">url</a></p><p><a href=\"www.base.com/url\">url</a></p>")]
		public string ParseUrlTagCorrectrly(string plainMd, string baseUrl)
		{
			return new Md(plainMd, baseUrl).Render();
		}

		[TestCase("_asd_", "css", "", ExpectedResult = "<p class=\"css\"><em class=\"css\">asd</em></p>", TestName = "No def")]
		[TestCase("_asd_ __qwe__", "css", "",
			 ExpectedResult = "<p class=\"css\"><em class=\"css\">asd</em> <strong class=\"css\">qwe</strong></p>",
			 TestName = "No def, may tags")]
		[TestCase("_asd_", "css", "definition\n", ExpectedResult = "definition\n<p class=\"css\"><em class=\"css\">asd</em></p>",
			 TestName = "Defined")
		]
		public string ParseWithDefinedCss(string plainMd, string cssClassName, string cssClassDef)
		{
			var css = new CssClassInfo(cssClassName, cssClassDef);

			return new Md(plainMd, "", css).Render();
		}

		[TestCase("asd", ExpectedResult = "<p>asd</p>")]
		public string ParseParagraphsCorrectly(string plainMd)
		{
			return new Md(plainMd).Render();
		}

		[TestCase("r _i_ r _i_", ExpectedResult = "<p>r <em>i</em> r <em>i</em></p>")]
		[TestCase("r __b__ r __b__", ExpectedResult = "<p>r <strong>b</strong> r <strong>b</strong></p>")]
		[TestCase("_i_ __b__ r", ExpectedResult = "<p><em>i</em> <strong>b</strong> r</p>")]
		public string ParseMixedTagsCorrectrly(string plainMd)
		{
			return new Md(plainMd).Render();
		}


		private static string GenerateMdTag(Tag tag, int length)
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			var rnd = new Random(100);
			return tag.Equals(Tag.A)
				? $@"[{new string(
					Enumerable
						.Repeat(chars, length)
						.Select(s => s[rnd.Next(s.Length)])
						.ToArray())}]({new string(
					Enumerable
						.Repeat(chars, length)
						.Select(s => s[rnd.Next(s.Length)])
						.ToArray())})"
				: $@"{tag.Md}{new string(
					Enumerable
						.Repeat(chars, length)
						.Select(s => s[rnd.Next(s.Length)])
						.ToArray())}{tag.Md}";
		}

		private static string GenerateMd(Random rnd)
		{
			var md = new StringBuilder();
			for (var i = 0; i < 1000; i++)
				md.Append(GenerateMdTag(Tag.GetRandomTag(rnd), 100000));
			return md.ToString();
		}

		[Test]
		[Explicit]
		public void PerformanceTest()
		{
			var iterationWatch = new Stopwatch();
			var parseWatch = new Stopwatch();
			var rnd = new Random(10);
			var plainMd = GenerateMd(rnd);
			Console.WriteLine($"Length = {plainMd.Length}");

			var c = 0;
			iterationWatch.Start();
			for (var i = 0; i < plainMd.Length; i++)
				c = rnd.Next();
			iterationWatch.Stop();

			var parser = new Md(plainMd);
			parseWatch.Start();
			parser.Render();
			parseWatch.Stop();

			Console.WriteLine(
				$"iteration elapsed = {iterationWatch.ElapsedMilliseconds}, parse elapsed = {parseWatch.ElapsedMilliseconds}");
			(parseWatch.ElapsedMilliseconds / iterationWatch.ElapsedMilliseconds)
				.Should()
				.BeLessThan(iterationWatch.ElapsedMilliseconds / 10);
		}
	}
}