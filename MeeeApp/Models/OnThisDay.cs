using Newtonsoft.Json;

namespace MeeeApp.Models;

public class OnThisDay
{
    [JsonProperty("id")] public int Id { get; set; }
    [JsonProperty("displayMonth")] public int DisplayMonth { get; set; }
    [JsonProperty("displayDay")] public int DisplayDay { get; set; }
    [JsonProperty("title")] public string Title { get; set; }
    [JsonProperty("content")] public string Content { get; set; }
    [JsonProperty("dateShort")] public string DateShort { get; set; }
    [JsonProperty("displayDateFormatted")] public string DisplayDateFormatted { get; set; }
    
    // Get the month name from the month number
    public string DisplayMonthName()
    {
        return new DateTime(2021, DisplayMonth, 1).ToString("MMMM");
    }
}