using System.Xml;
using System.Text.RegularExpressions;

if (args is [string url, string pattern, string xpath] && Uri.TryCreate(url, UriKind.Absolute, out var uri))
{
    var xmlDocument = new XmlDocument();
    var regex = new Regex(pattern, RegexOptions.Compiled);
    using (var http = new HttpClient())
        xmlDocument.Load(await http.GetStreamAsync(uri));
    var first = ((xmlDocument["feed"]?["entry"])??(xmlDocument["rss"]?["channel"]?["item"]));
    for (var node = (XmlNode)first!; node != null; node = node.NextSibling)
    {
        if (!node.HasChildNodes || node["title"] is null)
            continue;
        if (regex.IsMatch(node["title"]!.FirstChild!.Value!))
        {
            var xPathNavigator = node.CreateNavigator()!.Select(xpath);
            if (!xPathNavigator.MoveNext())
                return 1;
            Console.WriteLine(xPathNavigator.Current!.Value);
            return 0;
        }
    }
}
return 1;
