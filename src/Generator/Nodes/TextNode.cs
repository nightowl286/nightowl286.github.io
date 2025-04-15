namespace Generator.Nodes;

public abstract class TextNode
{
	#region Functions
	public static ParagraphTextNodeCollection ParseParagraphs(XmlNode xml)
	{
		List<ParagraphTextNode> paragraphs = [];
		List<TextNode> children = [];

		void MakeParagraph()
		{
			if (children.Count > 0)
			{
				ParagraphTextNode paragraph = new(children);
				paragraphs.Add(paragraph);
			}

			children = [];
		}

		foreach (XmlNode current in xml.ChildNodes)
		{
			if (current.NodeType is not XmlNodeType.Text)
			{
				TextNode node = Parse(current);
				children.Add(node);

				continue;
			}

			string text = current.Value ?? string.Empty;
			int index = text.IndexOf("\n\n");

			while (index >= 0)
			{
				string prefix = text[..index];

				if (string.IsNullOrWhiteSpace(prefix) is false)
					children.Add(new PlainTextNode(prefix));

				MakeParagraph();

				text = text[(index + 2)..];
				index = text.IndexOf("\n\n");
			}

			if (string.IsNullOrWhiteSpace(text) is false)
				children.Add(new PlainTextNode(text));
		}

		MakeParagraph();

		return new(paragraphs);
	}
	public static TextNode Parse(XmlNode xml)
	{
		if (xml.NodeType is XmlNodeType.Text)
			return new PlainTextNode(xml.Value ?? string.Empty);

		if (xml is not XmlElement element)
			throw new InvalidOperationException($"Unknown XML node type ({xml.NodeType}).");

		IReadOnlyList<TextNode> children = ParseChildren(xml);

		return element.Name switch
		{
			"proile" => new ProfileLinkNode(element),
			"link" => new LinkTextNode(element, children),
			"abbr" => new AbbreviationTextNode(element, children),
			"thought" => new ThoughtTextNode(children),
			"em" => new EmphasisTextNode(children),
			"sup" => new SuperScriptTextNode(children),
			"code" => new CodeTextNode(children),
			"list" => new ListTextNode(element, children),
			"item" => new ListItemTextNode(children),

			_ => throw new ArgumentException($"Unknown XML element ({element.Name}).", nameof(xml))
		};
	}
	public static IReadOnlyList<TextNode> ParseChildren(XmlNode xml)
	{
		List<TextNode> children = [];

		foreach (XmlNode current in xml.ChildNodes)
		{
			TextNode node = Parse(current);
			children.Add(node);
		}

		return children;
	}
	#endregion
}
