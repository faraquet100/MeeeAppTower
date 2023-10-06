using MeeeApp.Services;
using Newtonsoft.Json;
using banditoth.MAUI.PreferencesExtension;

namespace MeeeApp.Models;

public class DailyMoment
{
    public static string KEY_FAVOURITE_DAILY_MOMENTS = "dm_favourites";
    
    [JsonProperty("id")] public int Id { get; set; }
    [JsonProperty("heading")] public string Heading { get; set; } = "";
    [JsonProperty("quoteText")] public string QuoteText { get; set; } = "";
    [JsonProperty("quoteAuthor")] public string QuoteAuthor { get; set; } = "";
    [JsonProperty("content")] public string Content { get; set; } = "";
    [JsonProperty("callToActionText")] public string CallToActionText { get; set; } = "";
    [JsonProperty("imageUrl")] public string ImageUrl { get; set; } = "";
    [JsonProperty("peopleRef")] public string PeopleRef { get; set; } = "";
    [JsonProperty("linkUrl")] public string LinkUrl { get; set; } = "";
    [JsonProperty("tags")] public string Tags { get; set; } = "";
    [JsonProperty("useLikeButton")] public bool UseLikeButton { get; set; }
    [JsonProperty("useHeartButton")] public bool UseHeartButton { get; set; }
    [JsonProperty("useShareButton")] public bool UseShareButton { get; set; }

    public string FullImageUrl
    {
        get
        {
            if (ImageUrl.Length > 0)
            {
                return AppSettings.MOMENT_IMAGE_URL + ImageUrl;
            }

            return "";
        }
    }
    
    public string ContentWithHtml
    {
        get
        {
            var htmlContent = Content ?? "";
            var htmlHeader = "<html><head><meta name='viewport' content='width=300, initial-scale=1.0' /></head><body style='text-align:left; color: white; line-height: 1.2; font-family: Arial, sans-serif; background-color: #0EB5DE;'>";
            var html = htmlHeader + htmlContent + "<body></html>";
            return html;
        }
    }

    public void AddToFavourites()
    {
        var favourites = Preferences.Default.GetObject<List<int>>(KEY_FAVOURITE_DAILY_MOMENTS, new List<int>());
        if (!favourites.Contains(Id))
        {
            favourites.Add(Id);
            Preferences.Default.SetObject(KEY_FAVOURITE_DAILY_MOMENTS, favourites);
        }
    }

    public void RemoveFromFavourites()
    {
        var favourites = Preferences.Default.GetObject<List<int>>(KEY_FAVOURITE_DAILY_MOMENTS, new List<int>());
        if (favourites.Contains(Id))
        {
            favourites.Remove(Id);
            Preferences.Default.SetObject(KEY_FAVOURITE_DAILY_MOMENTS, favourites);
        }
    }

    public static List<int> GetFavourites()
    {
        var favourites = Preferences.Default.GetObject<List<int>>(KEY_FAVOURITE_DAILY_MOMENTS, new List<int>());
        return favourites;
    }
    
    public bool ImageIsVideo()
    {
        if (ImageUrl.Length > 0)
        {
            return ImageUrl.Contains(".mp4") || ImageUrl.Contains(".m4v");
        }

        return false;
    }
    
    
}