using System.Xml;
using System.Text.RegularExpressions;

if (args is [string url, string pattern, string xpath] && Uri.TryCreate(url, UriKind.Absolute, out var uri))
{
    var xmlDocument = new XmlDocument();
    var regex = new Regex(pattern, RegexOptions.Compiled);
    using (var http = new HttpClient())
        xmlDocument.Load(await http.GetStreamAsync(uri));
    foreach (var node in xmlDocument["rss"]!["channel"]!.OfType<XmlNode>())
        if (node.Name == "item")
            if (regex.IsMatch(node["title"]!.FirstChild!.Value!))
            {
                var xPathNavigator = node.CreateNavigator()!;
                Console.WriteLine(xPathNavigator.SelectSingleNode(xpath)!.Value);
                return;
            }
}
