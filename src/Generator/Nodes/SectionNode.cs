namespace Generator.Nodes;

public sealed class SectionNode
{
	#region Properties
	public string Id { get; }
	public string Title { get; }
	public TextNode Summary { get; }
	public TextNode Content { get; }
	public IReadOnlyList<SubSectionNode> SubSections { get; }
	#endregion

	#region Constructors
	public SectionNode(XmlNode node)
	{
		Id = node.GetRequiredAttribute("id");
		Title = node.GetRequiredAttribute("title");
		Summary = node.Parse(".//summary");
		Content = node.Parse(".//content");

		SubSections = node.SelectNodes(".//section").Select(n => new SubSectionNode(n));
	}
	#endregion
}
