namespace Markdown
{
	public class CssClassInfo
	{
		public readonly string ClassName;
		public readonly string Description;

		public CssClassInfo(string className, string description)
		{
			Description = description;
			ClassName = className;
		}
	}
}