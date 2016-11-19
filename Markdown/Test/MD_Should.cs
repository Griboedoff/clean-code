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
		[TestCase("qwe asd zxc", ExpectedResult = "qwe asd zxc")]
		public string ParseNoMarkup(string plainMd)
		{
			return new Md(plainMd).Render();
		}

		[TestCase("_asd_", ExpectedResult = "<em>asd</em>")]
		[TestCase("_a s d_", ExpectedResult = "<em>a s d</em>")]
		[TestCase("_1_2_3_", ExpectedResult = "<em>1_2_3</em>")]
		[TestCase("_a_ _s d_", ExpectedResult = "<em>a</em> <em>s d</em>")]
		[TestCase("_a_ _s d", ExpectedResult = "<em>a</em> _s d")]
		[TestCase("_aas__abc__abc_", ExpectedResult = "<em>aas__abc__abc</em>")]
		public string ParseEmTagCorrectly(string plainMd)
		{
			return new Md(plainMd).Render();
		}

		[TestCase("_ d_", ExpectedResult = "_ d_")]
		[TestCase("_a _", ExpectedResult = "_a _")]
		[TestCase("__ d__", ExpectedResult = "__ d__")]
		[TestCase("__a __", ExpectedResult = "__a __")]
		[TestCase("_ abc_abc_", ExpectedResult = "_ abc<em>abc</em>")]
		public string ParseTrailingWhitespaceCorrectly(string plainMd)
		{
			return new Md(plainMd).Render();
		}

		[TestCase("_ab cd", ExpectedResult = "_ab cd")]
		[TestCase("__ab cd", ExpectedResult = "__ab cd")]
		public string ParseNoMarkup_IfMissingCloseTag(string plainMd)
		{
			return new Md(plainMd).Render();
		}

		[TestCase(@"_a\_b_", ExpectedResult = "<em>a_b</em>")]
		[TestCase(@"__a\_b__", ExpectedResult = "<strong>a_b</strong>")]
		[TestCase(@"a\_b", ExpectedResult = "a_b")]
		public string ParseEscapedCorrectly(string plainMd)
		{
			return new Md(plainMd).Render();
		}

		[TestCase("__abc_abc", ExpectedResult = "__abc_abc")]
		[TestCase("__abc_abc_", ExpectedResult = "__abc<em>abc</em>")]
		[TestCase("_abc__abc", ExpectedResult = "_abc__abc")]
		[TestCase("_abc__abc__", ExpectedResult = "_abc__abc__")]
		public string ParseUnpairTags(string plainMd)
		{
			return new Md(plainMd).Render();
		}

		[TestCase("__abc__", ExpectedResult = "<strong>abc</strong>")]
		[TestCase("__abc_abc_abc__", ExpectedResult = "<strong>abc<em>abc</em>abc</strong>")]
		public string ParseStrongTagCorrectrly(string plainMd)
		{
			return new Md(plainMd).Render();
		}

		[TestCase("r _i_ r _i_", ExpectedResult = "r <em>i</em> r <em>i</em>")]
		[TestCase("r __b__ r __b__", ExpectedResult = "r <strong>b</strong> r <strong>b</strong>")]
		[TestCase("_i_ __b__ r", ExpectedResult = "<em>i</em> <strong>b</strong> r")]
		public string ParseMixedTagsCorrectrly(string plainMd)
		{
			return new Md(plainMd).Render();
		}

		private static string GenerateMd(Tag tag, int length)
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			var rnd = new Random(100);
			return $@"{tag.Md}{new string(
				Enumerable
					.Repeat(chars, length)
					.Select(s => s[rnd.Next(s.Length)])
					.ToArray())}{tag.Md}";
		}

		[Test]
		public void PerformanceTest()
		{
			var iterationWatch = new Stopwatch();
			var parseWatch = new Stopwatch();
			var md = new StringBuilder();
			var rnd = new Random(10);
			for (var i = 0; i < 1000; i++)
				md.Append(GenerateMd(Tag.GetRandomTag(rnd), 100000));
			var plainMd = md.ToString();
			Console.WriteLine($"Length = {plainMd.Length}");

			var c = 0;
			iterationWatch.Start();
			for (var i = 0; i < plainMd.Length; i++)
				c = rnd.Next();
			iterationWatch.Stop();

			var parser = new Md(plainMd);
			parseWatch.Start();
			var html = parser.Render();
			parseWatch.Stop();

			Console.WriteLine(
				$"iteration elapsed = {iterationWatch.ElapsedMilliseconds}, parse elapsed = {parseWatch.ElapsedMilliseconds}");
			(parseWatch.ElapsedMilliseconds / iterationWatch.ElapsedMilliseconds)
				.Should()
				.BeLessThan(iterationWatch.ElapsedMilliseconds / 10);
		}
	}
}