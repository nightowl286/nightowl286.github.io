namespace Generator.Nodes;

public enum ListKind : byte
{
	Bullet,
	Number,
}

public sealed class ListTextNode(XmlNode node, IReadOnlyList<TextNode> children) : TextNodeCollection(children)
{
	#region Properties
	public ListKind Kind { get; } = GetKind(node);
	#endregion

	#region Helpers
	private static ListKind GetKind(XmlNode node)
	{
		string str = node.GetAttributeOrDefault("type", "bullet");

		return str switch
		{
			"bullet" => ListKind.Bullet,
			"number" => ListKind.Number,

			_ => throw new InvalidOperationException($"Unknown list kind ({str}).")
		};
	}
	#endregion
}
