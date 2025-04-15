namespace Generator;

class Program
{
	#region Constants
	private static readonly string ContentPath = Path.GetFullPath("../../../../../content");
	public static readonly string BuildPath = Path.GetFullPath("../../../../../build");
	public static readonly string InlineCssPath = Path.Combine(ContentPath, "inline.css");
	private const string PlainDirectoryName = "plain";
	#endregion

	#region Functions
	public static void Main()
	{
		TimeInfo lastUpdateInfo = GetLastUpdateInfo();

		IndexModel indexModel = LoadIndexModel(lastUpdateInfo);
		ProjectModel[] projectModels = LoadWishlistModels(lastUpdateInfo).OrderBy(p => p.Id).ToArray();

		CleanDirectory(BuildPath);
		CopyAssets();
		BuildIndexPage(indexModel, projectModels);

		if (Directory.Exists(PlainDirectoryName) is false)
			Directory.CreateDirectory(PlainDirectoryName);

		Array.ForEach(projectModels, BuildProjectPage);
	}
	private static void BuildIndexPage(IndexModel model, IEnumerable<ProjectModel> projects)
	{
		model.PageTemplate(writer =>
		{
			using (writer.Comment("About me").Section("about-me", "section"))
			{
				using (writer.Card())
				{
					writer
						.LinkHeading("h1", "about-me", "Anchor to this section of the page", "About me")
						.Line("<img src=\"resources/pfp.png\" alt=\"Profile picture for Nightowl286\" title=\"Profile picture for Nightowl286\" style=\"grid-column: 1;\">")
						.Line("<img src=\"resources/pfp.png\" alt=\"Profile picture for Nightowl286\" title=\"Profile picture for Nightowl286\" style=\"grid-column: 3; transform: scaleX(-1);\">");

					using (writer.TagBlock("p"))
					{
						writer.TextNode(model.Summary).LineBreak().Br().Br();

						foreach (ProfileLinkNode profile in model.ProfileLinks)
						{
							string alt = $"{profile.Title} badge";
							string badgeUrl = $"https://img.shields.io/badge/{profile.Name}-{profile.Name}?style=flat&logo={profile.Logo}&labelColor=%23{profile.Color}&color=646464";

							using (writer.Link(profile.Url, profile.Title, true))
								writer.Line($"<img alt=\"{alt.AttributeEncode()} badge\" src=\"{badgeUrl.AttributeEncode()}\">");
						}
					}
				}

				using (writer.Region())
				{
					foreach (ParagraphTextNode paragraph in model.Content.Paragraphs)
						writer.Paragraph(paragraph);
				}
			}

			writer
				.LineBreak()
				.TableOfContents(projects);

			using (writer.LineBreak().Comment("Wishlist").Section("wishlist", "section"))
			{
				using (writer.Card())
				{
					writer.LinkHeading("h2", "wishlist", "Anchor to this section of the page", "Wishlist");
					using (writer.Paragraph())
						writer.TextNode(model.Wishlist);
				}
				foreach (ProjectModel project in projects)
				{
					if (project.IsHidden)
						continue;

					writer.WriteLine();

					using (writer.Comment(project.Name).Section(project.Id, "project"))
					{
						writer.LinkHeading("h3", project.Id, "Anchor link to this project summary", project.Name);

						using (writer.Region())
						using (writer.Paragraph())
						{
							writer
								.TextNode(project.Summary)
								.Space()
								.Link($"wishlist/{project.Id}.html", $"Read more about the {project.Name} project", false, "Read more.");
						}
					}
				}
			}
		});
	}
	private static void BuildProjectPage(ProjectModel model)
	{
		string plainPath = Path.Combine(PlainDirectoryName, $"{model.Id}.txt");
		File.WriteAllText(plainPath, model.PlainText);

		if (model.IsHidden)
			return;

		model.PageTemplate(writer =>
		{
			using (writer.Comment("Introduction").Section("introduction", "section"))
			{
				using (writer.Card())
				{
					writer
						.LinkHeading("h1", "introduction", "Anchor link to this section of the page", model.Name)
						.Br()
						.Paragraph(model.Summary)
						.Br();

					using (writer.Paragraph())
					{
						writer
							.Text($"~{model.WordCount:n0} words.").Br()
							.Duration(model.ReadTime).LineBreak()
							.Text($" to read at {PlainTextExtractor.WordCountSpeed} ")
							.Text($"<abbr title=\"Words per minute\">WPM</abbr>.");
					}

					writer
					.Br()
					.Link("../index.html", "Return to the home page", false, "Return to the home page.");
				}

				using (writer.LineBreak().Region())
				{
					foreach (ParagraphTextNode paragraph in model.Content.Paragraphs)
						writer.Paragraph(paragraph);
				}
			}

			writer
				.LineBreak()
				.TableOfContents(model);

			foreach (SectionNode section in model.Sections)
			{
				using (writer.LineBreak().Comment(section.Title).Section(section.Id, "section"))
				{
					using (writer.Card())
					{
						writer
							.LinkHeading("h2", section.Id, "Anchor link to this section of the page", section.Title)
							.Paragraph(section.Summary);
					}

					using (writer.Region())
					{
						foreach (ParagraphTextNode paragraph in section.Content.Paragraphs)
							writer.Paragraph(paragraph);
					}

					foreach (SubSectionNode subSection in section.SubSections)
					{
						using (writer.LineBreak().Comment(subSection.Title).Section(subSection.Id, "sub-section"))
						{
							writer.LinkHeading("h3", subSection.Id, "Anchor link to this section of the page", subSection.Title);
							using (writer.Region())
							{
								foreach (ParagraphTextNode paragraph in subSection.Paragraphs.Paragraphs)
									writer.Paragraph(paragraph);
							}
						}
					}
				}
			}
		});
	}
	private static void CopyAssets()
	{
		IEnumerable<string> files =
			Directory.EnumerateFiles(ContentPath, "*", SearchOption.AllDirectories)
			.Where(path => Path.GetExtension(path) is not ".xml");

		foreach (string file in files)
		{
			string relative = Path.GetRelativePath(ContentPath, file);
			string buildPath = Path.Combine(BuildPath, relative);
			string? directory = Path.GetDirectoryName(buildPath);

			if (Directory.Exists(directory) is false && directory is not null)
				Directory.CreateDirectory(directory);

			File.Copy(file, buildPath);
		}
	}
	#endregion

	#region Helpers
	private static TimeInfo GetLastUpdateInfo()
	{
		string? path = Repository.Discover(Environment.CurrentDirectory);

		if (path is null)
			throw new InvalidOperationException("The generator was expected to be inside of a git repository.");

		string? repoDirectory = Path.GetDirectoryName(Path.GetDirectoryName(path));

		Debug.Assert(repoDirectory is not null);

		using Repository repo = new(path);

		TimeInfo info = new();
		foreach (string file in Directory.GetFiles(ContentPath, "*.xml", SearchOption.AllDirectories))
		{
			string relative = Path.GetRelativePath(repoDirectory, file);

			IEnumerable<LogEntry> entries = repo.Commits.QueryBy(relative, new CommitFilter { SortBy = CommitSortStrategies.Topological });
			LogEntry oldest = entries.Last();
			LogEntry newest = entries.First();

			DateTimeOffset publishedAt = oldest.Commit.Author.When;
			DateTimeOffset updatedAt = newest.Commit.Author.When;

			info.Add(file, publishedAt, updatedAt);
		}

		return info;
	}
	private static void CleanDirectory(string directory)
	{
		if (Directory.Exists(directory))
			Directory.Delete(directory, true);

		Directory.CreateDirectory(directory);
	}
	private static ProjectModel[] LoadWishlistModels(TimeInfo timeInfo)
	{
		string wishlistDirectory = Path.Combine(ContentPath, "wishlist");
		string[] files = Directory.GetFiles(wishlistDirectory, "*.xml");

		ProjectModel[] projects = new ProjectModel[files.Length];
		for (int i = 0; i < files.Length; i++)
		{
			string file = files[i];

			string id = Path.GetFileNameWithoutExtension(file);
			XmlDocument xml = LoadXmlDocument(file);

			timeInfo.Get(file, out DateTime publishedAt, out DateTime updatedAt);
			ProjectModel project = new(id, xml, publishedAt, updatedAt);

			projects[i] = project;
		}

		return projects;
	}
	private static IndexModel LoadIndexModel(TimeInfo timeInfo)
	{
		string path = Path.Combine(ContentPath, "index.xml");

		XmlDocument xml = LoadXmlDocument(path);

		timeInfo.Get(path, out DateTime publishedAt, out DateTime updatedAt);
		IndexModel model = new(xml, publishedAt, updatedAt);

		return model;
	}
	private static XmlDocument LoadXmlDocument(string path)
	{
		XmlDocument document = new();
		document.Load(path);

		return document;
	}
	#endregion
}
