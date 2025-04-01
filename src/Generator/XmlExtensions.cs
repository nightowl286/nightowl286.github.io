namespace Generator;

public static class XmlExtensions
{
   #region Methods
   public static XmlElement GetRootElement(this XmlDocument document)
   {
      if (document.DocumentElement is null)
         throw new InvalidOperationException("The xml document didn't have a root element.");

      return document.DocumentElement;
   }

   public static DateTime GetDateAttribute(this XmlElement element, string attributeName)
   {
      string text = element.GetRequiredAttribute(attributeName);
      DateTime date = DateTime.Parse(text);

      return date.ToUniversalTime();
   }
   public static string GetRequiredAttribute(this XmlElement element, string attributeName)
   {
      XmlAttribute? attribute = element.GetAttributeNode(attributeName);
      if (attribute is null || string.IsNullOrEmpty(attribute.Value))
         throw new ArgumentException($"The given xml element did not have the expected attribute ({attributeName}).", nameof(attributeName));

      return attribute.Value;
   }
   public static XmlNode GetDirectElementByTagName(this XmlElement element, string name)
   {
      XmlElement? node = element["summary"];

      if (node is not null)
         return node;

      throw new ArgumentException($"The given xml element did not have any child elements with the name ({name}).", nameof(name));
   }
   #endregion
}
