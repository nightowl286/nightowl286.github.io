namespace Generator.Nodes;

public sealed class ListItemTextNode(IReadOnlyList<TextNode> children) : TextNodeCollection(children)
{
}
