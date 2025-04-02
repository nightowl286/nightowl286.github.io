namespace Generator.Nodes;

public sealed class SubSectionNode(XmlNode node)
{
	#region Properties
	public string Id { get; } = node.GetRequiredAttribute("id");
	public string Title { get; } = node.GetRequiredAttribute("title");
	public ParagraphTextNodeCollection Paragraphs { get; } = node.ParseParagraphs();
	#endregion
}
