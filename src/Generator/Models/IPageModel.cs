namespace Generator.Models;

public interface IPageModel
{
	#region Properties
	string Id { get; }
	DateTime Published { get; }
	DateTime Updated { get; }
	string Title { get; }
	string Description { get; }
	#endregion
}
