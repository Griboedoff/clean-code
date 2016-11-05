using System;

namespace Markdown
{
	public class Md
	{
		private readonly string plainMd;
		private HtmlToken root;

		public Md(string plainMd)
		{
			this.plainMd = plainMd;
		}

		public bool TryParseToHtml()
		{
			root = new HtmlToken("", plainMd);
			return true;
		}

		public string Render()
		{
			TryParseToHtml();
			return root.ToString();
		}
	}
}