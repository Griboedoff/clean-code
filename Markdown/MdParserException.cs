using System;

namespace Markdown
{
	public class MdParserException : Exception
	{
		public MdParserException(string message) : base(message)
		{
		}
	}
}