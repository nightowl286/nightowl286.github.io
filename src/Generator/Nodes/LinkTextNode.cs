namespace Generator.Nodes;

public sealed class LinkTextNode(XmlNode node, IReadOnlyList<TextNode> children) : TextNodeCollection(children)
{
	#region Properties
	public string Link { get; } = node.GetRequiredAttribute("href");
	public string? Title { get; } = node.GetAttributeOrDefault("title");
	#endregion
}
