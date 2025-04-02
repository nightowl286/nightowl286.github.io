namespace Generator;

public static class XmlExtensions
{
	#region Methods
	public static IEnumerable<T> Select<T>(this XmlNodeList? list, Func<XmlNode, T> callback)
	{
		List<T> values = [];

		if (list is null)
			return values;

		foreach (XmlNode current in list)
		{
			T value = callback.Invoke(current);
			values.Add(value);
		}

		return values;
	}
	public static List<XmlNode> SelectChildNodes(this XmlNode node, string name)
	{
		List<XmlNode> nodes = [];

		foreach (XmlNode child in node.ChildNodes)
		{
			if (child.Name == name)
				nodes.Add(child);
		}

		return nodes;
	}
	public static List<T> SelectChildNodes<T>(this XmlNode node, string name, Func<XmlNode, T> conversion)
	{
		List<T> children = [];

		foreach (XmlNode child in node.ChildNodes)
		{
			if (child.Name == name)
			{
				T item = conversion.Invoke(child);
				children.Add(item);
			}
		}

		return children;
	}
	public static ParagraphTextNodeCollection ParseParagraphs(this XmlNode node, string? xpath = null)
	{
		if (xpath is not null)
			node = node.SelectRequiredSingleNode(xpath);

		return TextNode.ParseParagraphs(node);
	}
	public static TextNode Parse(this XmlNode node, string xpath)
	{
		node = node.SelectRequiredSingleNode(xpath);
		IReadOnlyList<TextNode> children = TextNode.ParseChildren(node);

		return new TextNodeCollection(children);
	}
	public static XmlNode SelectRequiredSingleNode(this XmlNode node, string xpath)
	{
		XmlNode? foundNode = node.SelectSingleNode(xpath);

		if (foundNode is not null)
			return foundNode;

		throw new ArgumentException($"A single node couldn't be selected using the given xpath ({xpath}).", nameof(node));
	}
	private static XmlElement AsElement(this XmlNode node)
	{
		if (node is XmlElement element)
			return element;

		if (node is XmlDocument document && document.DocumentElement is not null)
			return document.DocumentElement;

		throw new ArgumentException($"The given XML node was not an element.", nameof(node));
	}
	#endregion

	#region GetDateAttribute Methods
	public static DateTime GetDateAttribute(this XmlNode node, string attributeName) => node.AsElement().GetDateAttribute(attributeName);
	public static DateTime GetDateAttribute(this XmlElement element, string attributeName)
	{
		string text = element.GetRequiredAttribute(attributeName);
		DateTime date = DateTime.Parse(text);

		return date.ToUniversalTime();
	}
	#endregion

	#region GetRequiredAttribute methods
	public static string GetRequiredAttribute(this XmlNode node, string attributeName) => node.AsElement().GetRequiredAttribute(attributeName);
	public static string GetRequiredAttribute(this XmlElement element, string attributeName)
	{
		XmlAttribute? attribute = element.GetAttributeNode(attributeName);
		if (attribute is null || string.IsNullOrEmpty(attribute.Value))
			throw new ArgumentException($"The given XML element did not have the expected attribute ({attributeName}).", nameof(attributeName));

		return attribute.Value;
	}
	#endregion

	#region GetAttributeOrDefault methods
	[return: NotNullIfNotNull(nameof(defaultValue))]
	public static string? GetAttributeOrDefault(this XmlNode node, string attributeName, string? defaultValue = null)
	{
		return node.AsElement().GetAttributeOrDefault(attributeName, defaultValue);
	}

	[return: NotNullIfNotNull(nameof(defaultValue))]
	public static string? GetAttributeOrDefault(this XmlElement element, string attributeName, string? defaultValue = null)
	{
		XmlAttribute? attribute = element.GetAttributeNode(attributeName);

		if (attribute is null || string.IsNullOrEmpty(attribute.Value))
			return defaultValue;

		return attribute.Value;
	}
	#endregion
}
