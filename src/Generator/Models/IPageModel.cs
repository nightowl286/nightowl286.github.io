namespace Generator.Models;

public interface IPageModel
{
	#region Properties
	string Id { get; }
	DateTime Date { get; }
	string Title { get; }
	string Description { get; }
	#endregion
}
