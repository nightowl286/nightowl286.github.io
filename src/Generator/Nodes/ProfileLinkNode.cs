namespace Generator.Nodes;

public class ProfileLinkNode(XmlNode node) : TextNode
{
	#region Properties
	public string Name { get; } = node.GetRequiredAttribute("name");
	public string Title { get; } = node.GetRequiredAttribute("title");
	public string Url { get; } = node.GetRequiredAttribute("url");
	public string Color { get; } = node.GetRequiredAttribute("color");
	public string Logo { get; } = node.GetRequiredAttribute("logo");
	#endregion
}
