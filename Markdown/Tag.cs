using System.Collections.Generic;

namespace Markdown
{
	public class Tag
	{
		public static Tag EmMd = new Tag("_");
		public static Tag StrongMd = new Tag("__");
		public static Tag EmHtml = new Tag("em");
		public static Tag StrongHtml = new Tag("strong");
		public readonly string Value;

		private Tag(string value)
		{
			Value = value;
		}
	}
}