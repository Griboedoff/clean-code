namespace Markdown
{
	public class HtmlToken
	{
		public readonly string Tag;
		private readonly string data;
		public bool IsTagged => !string.IsNullOrEmpty(Tag);
		public int Length => data.Length;

		public HtmlToken(string tag, string data)
		{
			Tag = tag;
			this.data = data;
		}

		private string InsertInToTags(string body)
		{
			return IsTagged
				? $"<{Tag}>{body}</{Tag}>"
				: body;
		}

		public override string ToString()
		{
			return InsertInToTags(data);
		}
	}
}