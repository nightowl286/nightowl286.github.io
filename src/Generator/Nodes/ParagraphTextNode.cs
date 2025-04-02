namespace Generator.Nodes;

public sealed class ParagraphTextNode(IReadOnlyList<TextNode> children) : TextNodeCollection(children) { }

public sealed class ParagraphTextNodeCollection(IReadOnlyList<ParagraphTextNode> paragraphs)
{
	#region Properties
	public IReadOnlyList<ParagraphTextNode> Paragraphs { get; } = paragraphs;
	#endregion
}