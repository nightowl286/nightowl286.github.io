namespace Generator.Models;

public sealed class ProjectModel : IPageModel
{
	#region Properties
	public string Id { get; }
	public string Name { get; }
	public DateTime Date { get; }
	public string Title { get; }
	public string Description { get; }
	public TextNode Summary { get; }
	public TextNode Content { get; }
	public IReadOnlyList<SectionNode> Sections { get; }
	#endregion

	#region Constructors
	public ProjectModel(string id, XmlDocument xml)
	{
		Id = id;
		Name = xml.GetRequiredAttribute("name");
		Date = xml.GetDateAttribute("date");
		Title = IndexModel.DefaultTitle + $" - {Name} wishlist project";

		string summary = xml.SelectRequiredSingleNode(".//summary").InnerText;
		Description = "A post about Nightowl's project idea to " + summary;
		Summary = new PlainTextNode("A project to " + summary);

		Content = xml.Parse(".//content");
		Sections = xml.SelectNodes(".//article").Select(n => new SectionNode(n));
	}
	#endregion
}
