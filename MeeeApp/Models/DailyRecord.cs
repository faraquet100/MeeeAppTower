using System;
using Newtonsoft.Json;

namespace MeeeApp.Models
{
	public class DailyRecord
	{
        [JsonProperty("id")] public int Id { get; set; }
        [JsonProperty("userId")] public int UserId { get; set; }
        [JsonProperty("dateRecorded")] public DateTime DateRecorded { get; set; }
        [JsonProperty("recordDate")] public DateTime RecordDate { get; set; }
        [JsonProperty("checkInScore")] public int CheckInScore { get; set; }
        [JsonProperty("checkInReason")] public string CheckInReason { get; set; } = "";
        [JsonProperty("checkInTime")] public DateTime CheckInTime { get; set; }
        [JsonProperty("checkOutScore")] public int CheckOutScore { get; set; }
        [JsonProperty("checkOutReason")] public string CheckOutReason { get; set; } = "";
        [JsonProperty("checkOutTime")] public DateTime CheckOutTime { get; set; }
    }
}

