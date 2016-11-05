namespace Markdown
{
	public class HtmlToken
	{
		public readonly string Tag;
		private readonly string data;

		public HtmlToken(string tag, string data)
		{
			Tag = tag;
			this.data = data;
		}

		private string InsertInToTags(string body)
		{
			return string.IsNullOrEmpty(Tag) ? body : $"<{Tag}>{body}</{Tag}>";
		}

		public override string ToString()
		{
			return InsertInToTags(data);
		}
	}
}