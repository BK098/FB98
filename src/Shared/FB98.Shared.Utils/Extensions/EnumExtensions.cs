using System.ComponentModel;
using System.Reflection;

namespace FB98.Shared.Utils.Extensions
{
	public static class EnumExtensions
	{
		public static string GetDescription(this Enum value, params object[] args)
		{
			FieldInfo? field = value.GetType().GetField(value.ToString());
			DescriptionAttribute? attribute = field?.GetCustomAttribute<DescriptionAttribute>();
			string description = attribute == null ? value.ToString() : attribute.Description;

			return string.Format(description, args);
		}
	}
}