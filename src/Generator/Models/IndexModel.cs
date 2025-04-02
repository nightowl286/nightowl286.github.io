namespace Generator.Models;

public sealed class IndexModel(XmlDocument xml) : IPageModel
{
	#region Constants
	public const string DefaultTitle = "Nightowl's website";
	#endregion

	#region Properties
	public string Id => "index";
	public DateTime Date { get; } = xml.GetDateAttribute("date");
	public string Title => DefaultTitle;
	public string Description { get; } = xml.GetRequiredAttribute("description");
	public IReadOnlyList<ProfileLinkNode> ProfileLinks { get; } = xml.SelectNodes(".//profile").Select(n => new ProfileLinkNode(n));
	public TextNode Summary { get; } = xml.Parse(".//summary");
	public ParagraphTextNodeCollection Content { get; } = xml.ParseParagraphs(".//content");
	public TextNode Wishlist { get; } = xml.Parse(".//wishlist");
	#endregion
}
