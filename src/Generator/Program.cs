namespace Generator;

class Program
{
   #region Constants
   private static readonly string ContentPath = Path.GetFullPath("../../../../../content");
   private static readonly string BuildPath = Path.GetFullPath("../../../../../build");
   #endregion

   #region Functions
   public static void Main()
   {
      IndexModel indexModel = LoadIndexModel();
      ProjectModel[] projectModels = LoadWishlistModels();
   }
   #endregion

   #region Helpers
   private static ProjectModel[] LoadWishlistModels()
   {
      string wishlistDirectory = Path.Combine(ContentPath, "wishlist");
      string[] files = Directory.GetFiles(wishlistDirectory, "*.xml");

      ProjectModel[] projects = new ProjectModel[files.Length];
      for (int i = 0; i < files.Length; i++)
      {
         XmlDocument xml = LoadXmlDocument(files[i]);
         ProjectModel project = new(xml.GetRootElement());

         projects[i] = project;
      }

      return projects;
   }
   private static IndexModel LoadIndexModel()
   {
      string path = Path.Combine(ContentPath, "index.xml");

      XmlDocument xml = LoadXmlDocument(path);
      IndexModel model = new(xml.GetRootElement());

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