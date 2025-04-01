namespace Generator;

public readonly ref struct TagScope(IndentedTextWriter writer, string tag, bool isBlock, bool suffixLineBreak) : IDisposable
{
	#region Fields
	private readonly IndentedTextWriter _writer = writer;
	private readonly string _tag = tag;
	private readonly bool _isBlock = isBlock;
	#endregion

	#region Methods
	public readonly void Dispose()
	{
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
		string path = Path.Combine(Program.BuildPath, $"{model.Id}.html");

		using FileStream file = new(path, FileMode.CreateNew);
		using StreamWriter streamWriter = new(file, Encoding.UTF8);
		using IndentedTextWriter writer = new(streamWriter, "\t") { NewLine = "\n" };
		using (writer.Html())
		{
			using (writer.Head())
			{
				writer
					.EmbedStyle(Program.InlineCssPath)
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
	#endregion

	#region Tag block methods
	public static TagScope Html(this IndentedTextWriter writer)
	{
		WriteDocType(writer);

		writer.WriteLine($"<html lang=\"en\" itemscope itemtype=\"https://schema.org/Article\">");
		writer.Indent++;

		return new(writer, "html", true, false);
	}
	public static TagScope Head(this IndentedTextWriter writer) => TagBlock(writer, "head", true);
	public static TagScope Body(this IndentedTextWriter writer) => TagBlock(writer, "body");
	public static TagScope Footer(this IndentedTextWriter writer) => TagBlock(writer, "footer");
	public static TagScope MainBlock(this IndentedTextWriter writer) => TagBlock(writer, "main");
	public static TagScope Style(this IndentedTextWriter writer) => TagBlock(writer, "style", true);
	public static TagScope TagBlock(this IndentedTextWriter writer, string tag, bool suffixLineBreak = false)
	{
		writer.WriteLine($"<{tag}>");
		writer.Indent++;

		return new(writer, tag, true, suffixLineBreak);
	}
	#endregion

	#region Tag methods
	public static TagScope Tag(this IndentedTextWriter writer, string tag, bool appendLineBreak = false)
	{
		writer.Write($"<{tag}>");
		return new(writer, tag, false, appendLineBreak);
	}
	public static IndentedTextWriter Tag(this IndentedTextWriter writer, string tag, string value, bool appendLineBreak = true)
	{
		using (Tag(writer, tag, appendLineBreak))
			writer.Write(value.ContentEncode());

		return writer;
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
