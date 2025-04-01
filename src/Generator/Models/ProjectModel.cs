namespace Generator.Models;

public sealed class ProjectModel : IPageModel
{
   #region Properties
   public string Name { get; }
   public DateTime Date { get; }
   public string Title { get; }
   public string Description { get; }
   #endregion

   #region Constructors
   public ProjectModel(XmlElement xml)
   {
      Name = xml.GetRequiredAttribute("name");
      Date = xml.GetDateAttribute("date");
      Title = IndexModel.DefaultTitle + $" - {Name} wishlist project";

      string summary = xml.GetDirectElementByTagName("summary").InnerText;
      Description = "A post about Nightowl's project idea to " + summary;
   }
   #endregion
}
