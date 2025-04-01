namespace Generator.Models;

public interface IPageModel
{
   #region Properties
   DateTime Date { get; }
   string Title { get; }
   string Description { get; }
   #endregion
}
