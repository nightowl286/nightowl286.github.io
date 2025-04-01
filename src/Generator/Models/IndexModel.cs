namespace Generator.Models;

public sealed class IndexModel : IPageModel
{
   #region Constants
   public const string DefaultTitle = "Nightowl's website";
   #endregion

   #region Properties
   public DateTime Date { get; }
   public string Title => DefaultTitle;
   public string Description { get; }
   #endregion

   #region Constructors
   public IndexModel(XmlElement xml)
   {
      Date = xml.GetDateAttribute("date");
      Description = xml.GetRequiredAttribute("description");
   }
   #endregion
}
