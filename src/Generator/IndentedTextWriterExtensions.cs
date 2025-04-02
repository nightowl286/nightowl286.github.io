namespace Generator;

public readonly ref struct TagScope(IndentedTextWriter writer, string tag, bool isBlock, bool suffixLineBreak, bool prefixLineBreakBeforeEnd = false) : IDisposable
{
	#region Fields
	private readonly IndentedTextWriter _writer = writer;
	private readonly string _tag = tag;
	private readonly bool _isBlock = isBlock;
	#endregion

	#region Methods
	public readonly void Dispose()
	{
		if (prefixLineBreakBeforeEnd)
			_writer.WriteLine();

		if (_isBlock)
		{
			_writer.Indent--;
			_writer.WriteLine($"</{_tag}>");
		}
		else
			_writer.Write($"</{_tag}>");

		if (suffixLineBreak)
			_writer.WriteLine();

		_writer.Flush();
	}
	#endregion
}

public static class IndentedTextWriterExtensions
{
	#region Methods
	public static void PageTemplate(this IPageModel model, Action<IndentedTextWriter> callback)
	{
		using StringWriter stringWriter = new();
		using (IndentedTextWriter writer = new(stringWriter, "\t") { NewLine = "\n" })
		using (writer.Html())
		{
			using (writer.Head())
			{
				writer
					.EmbedStyle(Program.InlineCssPath)
					.StyleLinks(model)
					.GeneralMeta()
					.Title(model)
					.Description(model);
			}

			using (writer.Body())
			{
				callback?.Invoke(writer);

				writer.WriteLine();

				using (writer.Footer())
					writer.Tag("p", "Disclaimer: I may be a programmer, but I am most certainly not a web developer.");
			}
		}

		string path = Program.BuildPath;

		if (model is ProjectModel)
			path = Path.Combine(path, "wishlist");

		if (Directory.Exists(path) is false)
			Directory.CreateDirectory(path);

		path = Path.Combine(path, $"{model.Id}.html");

		using (FileStream file = new(path, FileMode.CreateNew))
		using (StreamWriter writer = new(file, Encoding.UTF8))
		{
			stringWriter.Flush();
			string text = stringWriter
				.ToString()
				.Replace("\t ", "\t");

			writer.Write(text);
		}
	}
	public static IndentedTextWriter WriteDocType(this IndentedTextWriter writer)
	{
		writer.WriteLine("<!DOCTYPE html>");
		writer.WriteLine();

		return writer;
	}
	public static IndentedTextWriter Comment(this IndentedTextWriter writer, string comment)
	{
		if (comment.Contains("-->"))
			throw new ArgumentException($"Escaping comment end marks (-->) is not supported at the moment.", nameof(comment));

		writer.WriteLine($"<!-- {comment} -->");

		return writer;
	}
	public static IndentedTextWriter LineBreak(this IndentedTextWriter writer)
	{
		writer.WriteLine();
		return writer;
	}
	public static IndentedTextWriter Line(this IndentedTextWriter writer, string text)
	{
		writer.WriteLine(text);
		return writer;
	}
	public static IndentedTextWriter Space(this IndentedTextWriter writer)
	{
		writer.Write(' ');
		return writer;
	}
	public static IndentedTextWriter TableOfContentsItem(this IndentedTextWriter writer, string id, string text)
	{
		using (writer.Tag("li"))
		using (writer.Link($"#{id}", null, false))
			writer.Write(text);

		return writer;
	}
	public static IndentedTextWriter TableOfContents(this IndentedTextWriter writer, ProjectModel model)
	{
		using (writer.Comment("Table of contents").Section("toc", "section"))
		{
			using (writer.Card())
				writer.LinkHeading("h2", "#toc", "Anchor link to this section of the page", "Table of contents");

			using (writer.Region())
			using (writer.TagBlock("ol"))
			{
				writer
					.TableOfContentsItem("introduction", "Introduction")
					.TableOfContentsItem("toc", "Table of contents");

				foreach (SectionNode section in model.Sections)
				{
					writer.TableOfContentsItem(section.Id, section.Title);
					using (writer.TagBlock("ol"))
					{
						foreach (SubSectionNode subSection in section.SubSections)
							writer.TableOfContentsItem(subSection.Id, subSection.Title);
					}
				}
			}
		}

		return writer;
	}
	public static IndentedTextWriter TableOfContents(this IndentedTextWriter writer, IEnumerable<ProjectModel> projects)
	{
		using (writer.Comment("Table of contents").Section("toc", "section"))
		{
			using (writer.Card())
				writer.LinkHeading("h2", "#toc", "Anchor link to this section of the page", "Table of contents");

			using (writer.Region())
			using (writer.TagBlock("ol"))
			{
				writer
					.TableOfContentsItem("about-me", "About me")
					.TableOfContentsItem("toc", "Table of contents")
					.TableOfContentsItem("wishlist", "Wishlist");

				using (writer.TagBlock("ol"))
				{
					foreach (ProjectModel project in projects)
						writer.TableOfContentsItem(project.Id, project.Name);
				}
			}
		}

		return writer;
	}
	#endregion

	#region Tag block methods
	public static TagScope Html(this IndentedTextWriter writer)
	{
		WriteDocType(writer);

		writer.WriteLine($"<html lang=\"en\">");
		writer.Indent++;

		return new(writer, "html", true, false);
	}
	public static TagScope Head(this IndentedTextWriter writer) => TagBlock(writer, "head", true);
	public static TagScope Body(this IndentedTextWriter writer) => TagBlock(writer, "body");
	public static TagScope Footer(this IndentedTextWriter writer) => TagBlock(writer, "footer");
	public static TagScope MainBlock(this IndentedTextWriter writer) => TagBlock(writer, "main");
	public static TagScope Style(this IndentedTextWriter writer) => TagBlock(writer, "style", true);
	public static TagScope Paragraph(this IndentedTextWriter writer) => TagBlock(writer, "p", false, true);
	public static IndentedTextWriter Paragraph(this IndentedTextWriter writer, TextNode node)
	{
		using (Paragraph(writer))
			return TextNode(writer, node);
	}
	public static TagScope Section(this IndentedTextWriter writer, string? id = null, string? cls = null) => TagBlock(writer, "section", id: id, cls: cls);
	public static TagScope Card(this IndentedTextWriter writer) => TagBlock(writer, "div", cls: "card");
	public static TagScope Region(this IndentedTextWriter writer) => TagBlock(writer, "div", cls: "region");
	public static IndentedTextWriter Br(this IndentedTextWriter writer)
	{
		writer.WriteLine("<br>");
		return writer;
	}
	public static IndentedTextWriter LinkHeading(this IndentedTextWriter writer, string tag, string id, string? title, string heading)
	{
		using (Tag(writer, tag))
		using (Link(writer, $"#{id}", title, false))
			writer.Write(heading.ContentEncode());

		return writer;
	}
	public static TagScope TagBlock(this IndentedTextWriter writer, string tag, bool suffixLineBreak = false, bool prefixLineBreakBeforeEnd = false, string? id = null, string? cls = null)
	{
		writer.Write($"<{tag}");

		if (id is not null) writer.Write($" id=\"{id.AttributeEncode()}\"");
		if (cls is not null) writer.Write($" class=\"{cls.AttributeEncode()}\"");

		writer.WriteLine(">");
		writer.Indent++;

		return new(writer, tag, true, suffixLineBreak, prefixLineBreakBeforeEnd);
	}
	#endregion

	#region Tag methods
	public static TagScope Tag(this IndentedTextWriter writer, string tag, bool appendLineBreak = true, string? id = null, string? cls = null)
	{
		writer.Write($"<{tag}");

		if (id is not null) writer.Write($" id=\"{id.AttributeEncode()}\"");
		if (cls is not null) writer.Write($" class=\"{cls.AttributeEncode()}\"");

		writer.Write(">");
		return new(writer, tag, false, appendLineBreak);
	}
	public static IndentedTextWriter Tag(this IndentedTextWriter writer, string tag, string value, bool appendLineBreak = true, string? id = null, string? cls = null)
	{
		using (Tag(writer, tag, appendLineBreak, id, cls))
			writer.Write(value.ContentEncode());

		return writer;
	}
	#endregion

	#region Text node methods
	public static IndentedTextWriter Thought(this IndentedTextWriter writer, ThoughtTextNode thought)
	{
		using (Tag(writer, "i", false, cls: "thought"))
			return TextNodeChildren(writer, thought);
	}
	public static IndentedTextWriter Link(this IndentedTextWriter writer, LinkTextNode link)
	{
		using (Link(writer, link.Link, link.Title, link.ShouldMultiline()))
			return TextNodeChildren(writer, link);
	}
	public static IndentedTextWriter Abbreviation(this IndentedTextWriter writer, AbbreviationTextNode abbreviation)
	{
		return Abbreviation(writer, abbreviation.Title, abbreviation);
	}
	public static IndentedTextWriter Abbreviation(this IndentedTextWriter writer, string title, TextNodeCollection children)
	{
		writer.Write($"<abbr title=\"{title.AttributeEncode()}\">");
		TextNodeChildren(writer, children);
		writer.Write("</abbr>");
		return writer;
	}
	public static IndentedTextWriter TextNode(this IndentedTextWriter writer, TextNode node)
	{
		if (node is PlainTextNode plain)
		{
			writer.Write(plain.Text.ContentEncode().Replace("&quot;", "\""));
			return writer;
		}
		else if (node is LinkTextNode link)
			return Link(writer, link);
		else if (node is ThoughtTextNode thought)
			return Thought(writer, thought);
		else if (node is AbbreviationTextNode abbreviation)
			return Abbreviation(writer, abbreviation);
		else if (node is ParagraphTextNode paragraph)
			return TextNodeChildren(writer, paragraph);
		else if (node is EmphasisTextNode emphasis)
		{
			using (Tag(writer, "em", false))
				return TextNodeChildren(writer, emphasis);
		}
		else if (node is CodeTextNode code)
		{
			using (Tag(writer, "code", false))
				return TextNodeChildren(writer, code);
		}
		else if (node is SuperScriptTextNode super)
		{
			using (Tag(writer, "sup", false))
				return TextNodeChildren(writer, super);
		}
		else if (node is TextNodeCollection collection && node.GetType() == typeof(TextNodeCollection)) // Direct class, not as a base.
			return TextNodeChildren(writer, collection);

		throw new ArgumentException($"Unknown text node ({node}).", nameof(node));
	}
	public static IndentedTextWriter TextNodeChildren(this IndentedTextWriter writer, TextNodeCollection collection)
	{
		TextNode? last = null;
		foreach (TextNode node in collection.Children)
		{
			if (last is PlainTextNode plain && (plain.Text.EndsWith(' ') is false))
				writer.Write(" ");

			TextNode(writer, node);

			last = node;
		}

		return writer;
	}
	public static IndentedTextWriter Link(this IndentedTextWriter writer, string link, string? title, bool isBlock, string text)
	{
		using (Link(writer, link, title, isBlock))
			writer.Write(text.ContentEncode());

		return writer;
	}
	public static TagScope Link(this IndentedTextWriter writer, string link, string? title, bool isBlock)
	{
		writer.Write($"<a href=\"{link.AttributeEncode()}\"");

		if (title is not null)
			writer.Write($" title=\"{title.ContentEncode()}\"");

		if (isBlock)
		{
			writer.WriteLine(">");
			writer.Indent++;
		}
		else
			writer.Write(">");

		return new(writer, "a", isBlock, false);
	}
	private static bool ShouldMultiline(this TextNodeCollection collection)
	{
		if (collection.Children.Count is not 1)
			return true;

		if (collection.Children[0] is PlainTextNode text)
			return text.Text.Length < 20;

		return true;
	}
	#endregion

	#region Title methods
	public static IndentedTextWriter Title(this IndentedTextWriter writer, IPageModel model) => Title(writer, model.Title);
	public static IndentedTextWriter Title(this IndentedTextWriter writer, string title)
	{
		writer
			.Comment("Page title")
			.Tag("title", title)
			.Meta(property: "og:title", content: title)
			.Meta(name: "twitter:title", content: title)
			.Meta(itemprop: "name", content: title)
			.Meta(name: "apple-mobile-web-app-title", content: title)
			.LineBreak();

		return writer;
	}
	#endregion

	#region Description methods
	public static IndentedTextWriter Description(this IndentedTextWriter writer, IPageModel model) => Description(writer, model.Description);
	public static IndentedTextWriter Description(this IndentedTextWriter writer, string description)
	{
		writer
			.Comment("Page description")
			.Meta(name: "description", content: description)
			.Meta(property: "og:description", content: description)
			.Meta(name: "twitter:description", content: description)
			.Meta(itemprop: "description", content: description.MaxLength(200))
			.Meta(name: "description", itemprop: "description", content: description);

		return writer;
	}
	#endregion

	#region Embed methods
	public static IndentedTextWriter EmbedFile(this IndentedTextWriter writer, string path)
	{
		using StreamReader reader = new(path);

		string? line = reader.ReadLine();
		while (line is not null)
		{
			writer.WriteLine(line);
			line = reader.ReadLine();
		}

		return writer;
	}
	public static IndentedTextWriter EmbedStyle(this IndentedTextWriter writer, string path)
	{
		writer.Comment("Inline some basic CSS to stop the user from getting flash-banged on slow connections");
		using (writer.Style())
			return EmbedFile(writer, path);
	}
	public static IndentedTextWriter StyleLinks(this IndentedTextWriter writer, IPageModel model)
	{
		writer.Comment("Stylesheets");

		ReadOnlySpan<string> shared = ["normalize.css", "inline.css", "style.css"];
		bool isProject = model is ProjectModel;

		foreach (string style in shared)
			writer.MetaLink("stylesheet", isProject ? $"../{style}" : style);

		if (model is IndexModel)
			writer.MetaLink("stylesheet", "index.css");

		return writer;
	}
	#endregion

	#region Meta methods
	public static IndentedTextWriter Meta(
			this IndentedTextWriter writer,
			string? charset = null,
			string? name = null, string? property = null, string? itemprop = null,
			string? content = null)
	{
		Dictionary<string, string?> parts = new()
		{
			{ "charset", charset },
			{ "name", name },
			{ "property", property },
			{ "itemprop", itemprop },
			{ "content", content },
		};

		writer.Write("<meta");

		foreach (KeyValuePair<string, string?> pair in parts)
		{
			if (pair.Value is not null)
				writer.Write($" {pair.Key}=\"{pair.Value}\"");
		}

		writer.WriteLine(">");
		return writer;
	}
	public static IndentedTextWriter MetaLink(this IndentedTextWriter writer, string rel, string href)
	{
		rel = rel.AttributeEncode();
		href = href.AttributeEncode();

		writer.Write("<link");

		writer.Write($" rel=\"{rel}\"");
		writer.Write($" href=\"{href}\"");

		writer.WriteLine(">");
		return writer;
	}
	public static IndentedTextWriter GeneralMeta(this IndentedTextWriter writer)
	{
		return writer
			.LineBreak()
			.Comment("General meta")
			.Meta(charset: "utf-8")
			.Meta(name: "viewport", content: "width=device-width, initial-scale=1")
			.Meta(name: "theme-color", content: "#2a2a2a")
			.Meta(property: "og:type", content: "website")
			.MetaLink("me", "mailto:nightowl286@protonmail.com")
			.LineBreak();
	}
	#endregion
}
