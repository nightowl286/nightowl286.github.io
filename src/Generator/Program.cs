﻿namespace Generator;

class Program
{
	#region Constants
	private static readonly string ContentPath = Path.GetFullPath("../../../../../content");
	public static readonly string BuildPath = Path.GetFullPath("../../../../../build");
	public static readonly string InlineCssPath = Path.Combine(ContentPath, "inline.css");
	#endregion

	#region Functions
	public static void Main()
	{
		IndexModel indexModel = LoadIndexModel();
		ProjectModel[] projectModels = LoadWishlistModels();

		CleanDirectory(BuildPath);
		BuildIndexPage(indexModel, projectModels.OrderBy(p => p.Id));

		Array.ForEach(projectModels, BuildProjectPage);
	}
	#endregion

	#region Helpers
	private static void BuildIndexPage(IndexModel model, IEnumerable<ProjectModel> projects)
	{
		model.PageTemplate(writer =>
		{
			using (writer.Section("wishlist"))
			{
				using (writer.TagBlock("div", cls: "card"))
				{
					writer.Tag("h2", "Wishlist");
					using (writer.Paragraph())
						writer.TextNode(model.Wishlist);
				}

				foreach (ProjectModel project in projects)
				{
					writer.WriteLine();

					using (writer.Section(project.Id, "projecct"))
					{
						using (writer.Link($"#{project.Id}", project.Name, true))
							writer.Tag("h3", project.Name);

						using (writer.Paragraph())
							writer.TextNode(project.Summary);
					}
				}
			}
		});
	}
	private static void BuildProjectPage(ProjectModel model)
	{
		model.PageTemplate(writer =>
		{
			using (writer.Section("introduction"))
			{

			}

			writer
				.LineBreak()
				.TableOfContents(model);
		});
	}
	private static void CleanDirectory(string directory)
	{
		if (Directory.Exists(directory))
			Directory.Delete(directory, true);

		Directory.CreateDirectory(directory);
	}
	private static ProjectModel[] LoadWishlistModels()
	{
		string wishlistDirectory = Path.Combine(ContentPath, "wishlist");
		string[] files = Directory.GetFiles(wishlistDirectory, "*.xml");

		ProjectModel[] projects = new ProjectModel[files.Length];
		for (int i = 0; i < files.Length; i++)
		{
			string id = Path.GetFileNameWithoutExtension(files[i]);
			XmlDocument xml = LoadXmlDocument(files[i]);
			ProjectModel project = new(id, xml);

			projects[i] = project;
		}

		return projects;
	}
	private static IndexModel LoadIndexModel()
	{
		string path = Path.Combine(ContentPath, "index.xml");

		XmlDocument xml = LoadXmlDocument(path);
		IndexModel model = new(xml);

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
