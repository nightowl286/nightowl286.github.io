namespace Generator.Text;

public interface ITextNode
{
   #region Properties
   IReadOnlyList<ITextNode> Children { get; }
   #endregion
}
