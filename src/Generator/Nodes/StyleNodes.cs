namespace Generator.Nodes;

public sealed class EmphasisTextNode(IReadOnlyList<TextNode> children) : TextNodeCollection(children) { }
public sealed class SuperScriptTextNode(IReadOnlyList<TextNode> children) : TextNodeCollection(children) { }
public sealed class CodeTextNode(IReadOnlyList<TextNode> children) : TextNodeCollection(children) { }
