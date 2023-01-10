// See https://aka.ms/new-console-template for more information

using System.Globalization;
using AngleSharp;
using CsvHelper;
using CsvHelper.Configuration;

using var streamreader = new StreamReader("subscriptions.csv");
using var reader = new CsvReader(streamreader, new CsvConfiguration(CultureInfo.InvariantCulture)
{
    Delimiter = ","
});
var subcriptions = reader.GetRecords<YoutubeSubscription>();
var rssLinks = subcriptions.Select(subscription => $"https://www.youtube.com/feeds/videos.xml?channel_id={subscription.ChannelId}"!).ToList();
Console.WriteLine(string.Join('\n', rssLinks));
public class YoutubeSubscription
{
    public string ChannelId { get; set; }
    public string ChannelUrl { get; set; }
    public string ChannelName { get; set; }
}