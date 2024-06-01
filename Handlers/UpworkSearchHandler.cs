using System.ServiceModel.Syndication;
using System.Xml;

namespace Zordon_Bot.Handlers;

public class UpworkSearchHandler
{
    public string SearchString { get; set; } = string.Empty;

    public string SortFilter { get; set; } = string.Empty;

    public string UsOnly { get; set; } = string.Empty;

    public void BuildSearchString(string allWords, string anyWords, string noneWords, string exactPhrase, string titleWords)
    {
        string searchString = string.Empty;

        if (!string.IsNullOrWhiteSpace(allWords))
        {
            var words = string.Join(" AND ", allWords
                .Split(" ", StringSplitOptions.RemoveEmptyEntries));
            searchString += $"&q=({Uri.EscapeDataString(words)})";
        }

        if (!string.IsNullOrWhiteSpace(anyWords))
        {
            var words = string.Join("%20OR%20", anyWords
                .Split(" ", StringSplitOptions.RemoveEmptyEntries));
            searchString += string.IsNullOrWhiteSpace(allWords)
                ? $"&q=({words})" : $"%20AND%20({words})";
        }

        if (!string.IsNullOrWhiteSpace(noneWords))
        {
            var words = string.Join("%20OR%20", noneWords
                .Split(" ", StringSplitOptions.RemoveEmptyEntries));
            searchString += (string.IsNullOrWhiteSpace(allWords)
                && string.IsNullOrWhiteSpace(anyWords))
                ? $"&q=%20NOT%20({words})" : $"%20AND%20NOT%20({words})";
        }

        if (!string.IsNullOrWhiteSpace(exactPhrase))
        {
            var words = string.Join("%20", exactPhrase
                .Split(" ", StringSplitOptions.RemoveEmptyEntries));
            searchString += (string.IsNullOrWhiteSpace(allWords)
                && string.IsNullOrWhiteSpace(anyWords)
                && string.IsNullOrWhiteSpace(noneWords))
                ? $"&q=\"{words}\"" : $"%20AND%20\"{words}\"";
        }

        if (!string.IsNullOrWhiteSpace(titleWords))
        {
            var words = string.Join("%20", titleWords
                .Split(" ", StringSplitOptions.RemoveEmptyEntries));
            searchString += (string.IsNullOrWhiteSpace(allWords)
                && string.IsNullOrWhiteSpace(anyWords)
                && string.IsNullOrWhiteSpace(noneWords)
                && string.IsNullOrWhiteSpace(exactPhrase))
                ? $"&q=title%3A({words})" : $"%20AND%20title%3A({words})";
        }

        SearchString = searchString;
    }

    public string ReturnResultTitles()
    {
        var rssUrl = $"https://www.upwork.com/ab/feed/jobs/rss?paging=NaN-undefined{SearchString}&sort={SortFilter}{UsOnly}&api_params=1&securityToken=0e9d47173a7c58c1bb50b13d4add42c9eb807439fbeacc66d0eac4b51854fae9cc77eb6d46193784218d796061af7d70ab9e34fea7830109a13fee41642c741c&userUid=1787728711931502592&orgUid=1787728711931502593";

        string titles = "Titles: ";

        var count = 1;

        try
        {
            using var reader = XmlReader.Create(rssUrl);

            var feed = SyndicationFeed.Load(reader);

            foreach (var item in feed.Items)
            {

                titles += $" \n {count}. {item.Title.Text}";
                count++;
            }

        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

        return titles;
    }
}
