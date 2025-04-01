namespace Generator.Text;

public sealed class LinkTextNode(IReadOnlyList<ITextNode> children, Uri link, string? description) : BaseTextNode(children)
{
   #region Properties
   public Uri Link { get; } = link;
   public string? Description { get; } = description;
   #endregion
}
