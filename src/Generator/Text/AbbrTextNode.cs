namespace Generator.Text;

public sealed class AbbrTextNode(IReadOnlyList<ITextNode> children, string description) : BaseTextNode(children)
{
   #region Properties
   public string Description { get; } = description;
   #endregion
}
