using System;
using System.Collections.Generic;

namespace Markdown
{
	public class Tag
	{
		public string Md { get; }
		public string Html { get; }
		public static readonly Tag Em = new Tag("_", "em");
		public static readonly Tag Strong = new Tag("__", "strong");
		public static readonly Tag Empty = new Tag("", "");
		public static readonly Tag A = new Tag("", "a");
		private static readonly List<Tag> Tags = new List<Tag> {Em, Strong, Empty};

		private Tag(string md, string html)
		{
			Md = md;
			Html = html;
		}

		public static Tag GetRandomTag(Random rnd)
		{
			return Tags[rnd.Next(Tags.Count)];
		}

		public override bool Equals(object other)
		{
			var tag = other as Tag;
			if (tag == null)
				return false;
			return string.Equals(Md, tag.Md) && string.Equals(Html, tag.Html);
		}

		public override int GetHashCode()
		{
			unchecked
			{
				return ((Md?.GetHashCode() ?? 0) * 397) ^ (Html?.GetHashCode() ?? 0);
			}
		}
	}
}