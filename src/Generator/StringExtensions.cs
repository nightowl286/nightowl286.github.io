using System.Web;

namespace Generator;

public static class StringExtensions
{
	#region Methods
	public static string MaxLength(this string value, int maxLength)
	{
		if (value.Length <= maxLength)
			return value;

		value = value[(maxLength - 3)..];
		value += "...";

		return value;
	}

	public static string? UrlEncode(this string? value)
	{
		value = HttpUtility.UrlEncode(value);

		return value;
	}

	[return: NotNullIfNotNull(nameof(value))]
	public static string? AttributeEncode(this string? value)
	{
		value = HttpUtility.HtmlAttributeEncode(value);
		value = value?.Replace("&#39;", "'");

		return value;
	}

	[return: NotNullIfNotNull(nameof(value))]
	public static string? ContentEncode(this string? value)
	{
		value = HttpUtility.HtmlEncode(value);
		value = value?.Replace("&#39;", "'");

		return value;
	}
	#endregion
}
