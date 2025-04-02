namespace Generator.Nodes;

public class TextNodeCollection(IReadOnlyList<TextNode> children) : TextNode
{
	#region Properties
	public IReadOnlyList<TextNode> Children { get; } = children;
	#endregion
}
