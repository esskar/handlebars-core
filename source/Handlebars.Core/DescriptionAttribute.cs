using System;

namespace Handlebars.Core
{
	public class DescriptionAttribute : Attribute
	{
		public string Description { get; set; }

		public DescriptionAttribute(string description)
		{
			Description = description;
		}
	}
}