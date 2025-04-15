using System.Text.RegularExpressions;

namespace Generator;

public static partial class PlainTextExtractor
{
	#region Functions
	public static string Extract(ProjectModel model)
	{
		StringBuilder builder = new();

		AddLine(builder, model.Summary);
		AddLine(builder, model.Content.Paragraphs);

		foreach (SectionNode section in model.Sections)
		{
			AddLine(builder, section.Title);
			AddLine(builder, section.Summary);
			AddLine(builder, section.Content.Paragraphs);

			foreach (SubSectionNode subSection in section.SubSections)
			{
				AddLine(builder, subSection.Title);
				AddLine(builder, subSection.Paragraphs.Paragraphs);
			}
		}

		string text = builder.ToString();
		text = PostProcess(text);

		return text;
	}
	public static int GetWordCount(string text)
	{
		int count = WordRegex().Count(text);
		return count;
	}
	#endregion

	#region Text functions
	private static void AddLine(StringBuilder builder, string text)
	{
		builder.AppendLine(text);
		builder.AppendLine();
	}
	private static void AddLine(StringBuilder builder, TextNode node)
	{
		Add(builder, node);
		builder.AppendLine();
		builder.AppendLine();
	}
	private static void AddLine(StringBuilder builder, IEnumerable<TextNode> collection)
	{
		Add(builder, collection);
		builder.AppendLine();
		builder.AppendLine();
	}
	private static void Add(StringBuilder builder, TextNode node)
	{
		if (node is PlainTextNode plain)
			builder.Append(plain.Text);
		else if (node is ParagraphTextNode paragraph)
			Add(builder, paragraph.Children);
		else if (node is TextNodeCollection collection)
			Add(builder, collection.Children);
	}
	private static void Add(StringBuilder builder, IEnumerable<TextNode> collection)
	{
		TextNode? last = null;

		foreach (TextNode node in collection)
		{
			if (last is PlainTextNode plain && (plain.Text.EndsWith(' ') is false) && (node is not SuperScriptTextNode))
				builder.Append(' ');

			if ((last is LinkTextNode && node is ThoughtTextNode) || (last is ThoughtTextNode && node is LinkTextNode))
				builder.Append(' ');

			Add(builder, node);

			last = node;
		}
	}
	#endregion

	#region Helpers
	private static string PostProcess(string text)
	{
		text = SpaceRegex().Replace(text, " ");
		text = IndentRegex().Replace(text, string.Empty);

		return text.Trim();
	}

	[GeneratedRegex(@"^ +", RegexOptions.Multiline)]
	private static partial Regex IndentRegex();

	[GeneratedRegex(@" {2,}")]
	private static partial Regex SpaceRegex();

	[GeneratedRegex(@"\b(?:e\.t\.c|i\.e|e\.g|\d+(?:\.\d+)?|\w+(?:['-]\w+)?)\b", RegexOptions.IgnoreCase)]
	private static partial Regex WordRegex();
	#endregion
}
