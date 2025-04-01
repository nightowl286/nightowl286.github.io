namespace Generator.Text;

public abstract class BaseTextNode(IReadOnlyList<ITextNode> children) : ITextNode
{
	#region Properties
	public IReadOnlyList<ITextNode> Children { get; } = children;
	#endregion
}
