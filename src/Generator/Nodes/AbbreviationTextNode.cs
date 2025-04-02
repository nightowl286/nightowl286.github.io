namespace Generator.Nodes;

public sealed class AbbreviationTextNode(XmlNode node, IReadOnlyList<TextNode> children) : TextNodeCollection(children)
{
	#region Properties
	public string Title { get; } = node.GetRequiredAttribute("title");
	#endregion
}
