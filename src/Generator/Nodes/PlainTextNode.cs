using System.Text.RegularExpressions;

namespace Generator.Nodes;

[DebuggerDisplay($"{{{nameof(DebuggerDisplay)}(), nq}}")]
public sealed class PlainTextNode(string text) : TextNode
{
	#region Properties
#pragma warning disable SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
	public string Text { get; } = Regex.Replace(text, @"\n\t+", " ");
#pragma warning restore SYSLIB1045 // Convert to 'GeneratedRegexAttribute'.
	#endregion

	#region Methods
	private string DebuggerDisplay()
	{
		string text = Text
			.Replace("\n", "\\n")
			.Replace("\t", "\\t")
			.Replace("\"", "\\\"");

		return $"\"{text}\"";
	}
	#endregion
}
