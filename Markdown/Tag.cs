using System.Collections.Generic;

namespace Markdown
{
	public class Tag
	{
		public static Tag EmMd = new Tag("_");
		public static Tag StrongMd = new Tag("__");
		public static Tag EmHtml = new Tag("em");
		public static Tag StrongHtml = new Tag("strong");
		public static Tag Empty = new Tag("");
		public readonly string Value;

		public static readonly Dictionary<Tag, Tag> MdToHtml = new Dictionary<Tag, Tag>
		{
			[EmMd] = EmHtml,
			[StrongMd] = StrongHtml,
			[Empty] = Empty
		};

		private Tag(string value)
		{
			Value = value;
		}

		public override string ToString()
		{
			return Value;
		}
	}
}