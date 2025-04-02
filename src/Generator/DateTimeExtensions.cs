namespace Generator;

public static class DateTimeExtensions
{
	#region Methods
	public static string GetMonthDaySuffix(this DateTime date)
	{
		int day = date.Day;

		return day switch
		{
			1 => "st",
			2 => "nd",
			3 => "rd",

			21 => "st",
			22 => "nd",
			23 => "rd",

			31 => "st",

			_ => "th"
		};
	}
	#endregion
}
