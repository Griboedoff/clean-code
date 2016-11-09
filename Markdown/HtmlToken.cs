namespace Markdown
{
	public class HtmlToken
	{
		public readonly Tag Tag;
		private readonly string data;
		public bool IsTagged => Tag != Tag.Empty;
		public int Length => data.Length;

		public HtmlToken(Tag tag, string data)
		{
			Tag = tag;
			this.data = data;
		}

		private string InsertInToTags(string body) => IsTagged
			? $"<{Tag.MdToHtml[Tag]}>{body}</{Tag.MdToHtml[Tag]}>"
			: body;

		public override string ToString()
		{
			return InsertInToTags(data);
		}
	}
}