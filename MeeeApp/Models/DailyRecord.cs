using System;
using Newtonsoft.Json;
using Syncfusion.XlsIO.Parser.Biff_Records.Charts;

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
        [JsonProperty("checkOutJournalEntry")] public string CheckOutJournalEntry { get; set; } = "";
        [JsonProperty("exerciseGrateful")] public string ExerciseGrateful { get; set; } = "";
        [JsonProperty("exerciseAchieved")] public string ExerciseAchieved { get; set; } = "";
        [JsonProperty("exerciseLookingForward")] public string ExerciseLookingForward { get; set; } = "";
        
        public void AddGratitude(string gratitude)
		{
			if (ExerciseGrateful.Length > 0)
			{
				ExerciseGrateful += "," + gratitude;
			}
			else
			{
				ExerciseGrateful = gratitude;
			}
		}

        public void RemoveGratitude(string gratitude)
        {
	        var newString = "";
	        foreach (var word in ExerciseGratefulList())
	        {
		        if (word != gratitude)
		        {
			        if (newString.Length > 0)
			        {
				        newString += "," + word;
			        }
			        else
			        {
				        newString = word;
			        }
		        }
	        }
        }

        public void EditGratitude(string oldWord, string newWord)
        {
	        var newString = "";
	        foreach (var word in ExerciseGratefulList())
	        {
		        if (word == oldWord)
		        {
			        if (newString.Length > 0)
			        {
				        newString += "," + newWord;
			        }
			        else
			        {
				        newString = newWord;
			        }
		        }
		        else
		        {
			        if (newString.Length > 0)
			        {
				        newString += "," + word;
			        }
			        else
			        {
				        newString = word;
			        }
		        }
	        }

	        ExerciseGrateful = newString;
        }
        
        public List<string> ExerciseGratefulList()
		{
			if (ExerciseGrateful.Length < 1)
			{
				return new List<string>();
			}
			else
			{
				return ExerciseGrateful.Split(',').ToList();
			}
		}
    }
}

