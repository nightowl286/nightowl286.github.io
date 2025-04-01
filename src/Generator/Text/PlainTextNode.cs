namespace Generator.Text;

public sealed class PlainTextNode(string text) : ITextNode
{
   #region Properties
   public string Text { get; } = text;
   public IReadOnlyList<ITextNode> Children { get; } = [];
   #endregion
}
