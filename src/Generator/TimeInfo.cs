namespace Generator;

public sealed class TimeInfo
{
	#region Fields
	private readonly Dictionary<string, DateTimeOffset> _updateTimes = [];
	private readonly Dictionary<string, DateTimeOffset> _publishTimes = [];
	#endregion

	#region Methods
	public void Add(string path, DateTimeOffset publishedAt, DateTimeOffset updatedAt)
	{
		_publishTimes.Add(path, publishedAt);
		_updateTimes.Add(path, updatedAt);
	}
	public void Get(string path, out DateTime publishedAtUtc, out DateTime updatedAtUtc)
	{
		publishedAtUtc = _publishTimes[path].UtcDateTime;
		updatedAtUtc = _updateTimes[path].UtcDateTime;
	}
	#endregion
}
