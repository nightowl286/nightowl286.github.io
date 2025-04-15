namespace Generator.Models;

public sealed class ProjectModel : IPageModel
{
	#region Properties
	public string Id { get; }
	public string Name { get; }
	public DateTime Published { get; }
	public DateTime Updated { get; }
	public string Title { get; }
	public string Description { get; }
	public TextNode Summary { get; }
	public ParagraphTextNodeCollection Content { get; }
	public IReadOnlyList<SectionNode> Sections { get; }
	public string PlainText { get; }
	public int WordCount { get; }
	public TimeSpan ReadTime { get; }
	#endregion

	#region Constructors
	public ProjectModel(string id, XmlDocument xml, DateTime publishedAt, DateTime updatedAt)
	{
		Id = id;
		Name = xml.GetRequiredAttribute("name");
		Published = publishedAt;
		Updated = updatedAt;
		Title = IndexModel.DefaultTitle + $" - {Name} wishlist project";

		string summary = xml.SelectRequiredSingleNode(".//summary").InnerText;
		Description = "A post about Nightowl's project idea to " + summary;
		Summary = new PlainTextNode("A project to " + summary);

		Content = xml.ParseParagraphs(".//content");
		Sections = xml.SelectNodes(".//article").Select(n => new SectionNode(n));

		PlainText = PlainTextExtractor.Extract(this);
		WordCount = PlainTextExtractor.GetWordCount(PlainText);
		ReadTime = PlainTextExtractor.GetReadTime(WordCount);
	}
	#endregion
}
