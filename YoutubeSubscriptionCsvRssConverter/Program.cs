using System.Globalization;
using System.Xml;
using CsvHelper;
using CsvHelper.Configuration;

var file = File.Create(GetUniqueFileName());
var xmlWriter = XmlWriter.Create(file, new XmlWriterSettings()
{
    Indent = true
});

xmlWriter.WriteStartElement("opml");
xmlWriter.WriteAttributeString("version", "2.0");
xmlWriter.WriteStartElement("head");
xmlWriter.WriteStartElement("title");
xmlWriter.WriteString("Youtube subscriptions export to rss");
xmlWriter.WriteEndElement();
xmlWriter.WriteEndElement();
xmlWriter.WriteStartElement("body");
xmlWriter.WriteStartElement("outline");
xmlWriter.WriteAttributeString("text","Youtube Subscriptions");
xmlWriter.WriteAttributeString("title","Youtube Subscriptions");

using var streamReader = new StreamReader("subscriptions.csv");
using var reader = new CsvReader(streamReader, new CsvConfiguration(CultureInfo.InvariantCulture)
{
    Delimiter = ",",
    PrepareHeaderForMatch = matchArgs => matchArgs.Header.Replace(" ", "").ToLowerInvariant(),
});

foreach (var subscription in reader.GetRecords<YoutubeSubscription>())
{
    var rssLink = $"https://www.youtube.com/feeds/videos.xml?channel_id={subscription.ChannelId}"!;
    xmlWriter.WriteStartElement("outline");
    xmlWriter.WriteAttributeString("title", subscription.ChannelTitle);
    xmlWriter.WriteAttributeString("text", subscription.ChannelTitle);
    xmlWriter.WriteAttributeString("xmlUrl", rssLink);
    xmlWriter.WriteAttributeString("type", "rss");
    xmlWriter.WriteAttributeString("htmlUrl", subscription.ChannelUrl);
    xmlWriter.WriteEndElement();
}
xmlWriter.WriteEndElement();
xmlWriter.WriteEndElement();
xmlWriter.WriteEndElement();
xmlWriter.Flush();

file.Flush(true);
file.Dispose();
string GetUniqueFileName()
{
    const string fileNameFormat = "subscriptions{0}.opml";
    var foundNonExistingFileName = false;
    int count = 1;
    string nonExistingFileName = "";
    while (!foundNonExistingFileName)
    {
        if (count is 1 && !File.Exists(string.Format(fileNameFormat, "")))
        {
            foundNonExistingFileName = true;
            nonExistingFileName = string.Format(fileNameFormat, "");
            continue;
        }

        string fileName = string.Format(fileNameFormat, $"({count})");
        if (!File.Exists(fileName))
        {
            foundNonExistingFileName = true;
            nonExistingFileName = fileName;
            continue;
        }
        
        count++;
    }

    return nonExistingFileName;
}

public class YoutubeSubscription
{
    public string ChannelId { get; set; }
    public string ChannelUrl { get; set; }
    public string ChannelTitle { get; set; }
}

