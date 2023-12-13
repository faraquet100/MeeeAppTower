using System;
using System.Globalization;
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
        
        [JsonProperty("dailyMoment")] public DailyMoment DailyMoment { get; set; } = new DailyMoment();

        public string DayDescription
        {
	        get
	        {
		        if (RecordDate.Year == DateTime.Now.Year && RecordDate.Month == DateTime.Now.Month && RecordDate.Day == DateTime.Now.Day)
		        {
			        return "Today";
		        }
		        else if (RecordDate.Year == DateTime.Now.Year && RecordDate.Month == DateTime.Now.Month && RecordDate.Day == DateTime.Now.Day - 1)
		        {
			        return "Yesterday";
		        }
		        else
		        {
			        int days = (RecordDate - DateTime.Now).Days;
			        return Math.Abs(days).ToString() + " days ago";
		        }
	        }
        }
        
        // Exercises - Add an Item
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
        
        public void AddAchievement(string achievement)
        {
	        if (ExerciseAchieved.Length > 0)
	        {
		        ExerciseAchieved += "," + achievement;
	        }
	        else
	        {
		        ExerciseAchieved = achievement;
	        }
        }
        
        public void AddLookingForward(string lookingForward)
        {
	        if (ExerciseLookingForward.Length > 0)
	        {
		        ExerciseLookingForward += "," + lookingForward;
	        }
	        else
	        {
		        ExerciseLookingForward = lookingForward;
	        }
        }
        
        // Exercises - Remove an Item

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

	        ExerciseGrateful = newString;
        }
        
        public void RemoveAchievement(string achievement)
        {
	        var newString = "";
	        foreach (var word in ExerciseAchievementList())
	        {
		        if (word != achievement)
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

	        ExerciseAchieved = newString;
        }
        
        public void RemoveLookingForward(string lookingForward)
        {
	        var newString = "";
	        foreach (var word in ExerciseLookingFowardList())
	        {
		        if (word != lookingForward)
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

	        ExerciseLookingForward = newString;
        }
        
        // Exercises - Edit an Item

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
        
        public void EditAchievement(string oldWord, string newWord)
        {
	        var newString = "";
	        foreach (var word in ExerciseAchievementList())
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

	        ExerciseAchieved = newString;
        }
        
        public void EditLookingForward(string oldWord, string newWord)
        {
	        var newString = "";
	        foreach (var word in ExerciseLookingFowardList())
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

	        ExerciseLookingForward = newString;
        }
        
        // Exercise - Items as Lists
        
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

        public List<string> GratitudeList
        {
	        get
	        {
		        return ExerciseGratefulList();
	        }
        }
        
        public List<string> ExerciseAchievementList()
        {
	        if (ExerciseAchieved.Length < 1)
	        {
		        return new List<string>();
	        }
	        else
	        {
		        return ExerciseAchieved.Split(',').ToList();
	        }
        }
        
        public List<string> AchievementList
		{
	        get
	        {
		        return ExerciseAchievementList();
	        }
		}
        
        public List<string> ExerciseLookingFowardList()
        {
	        if (ExerciseLookingForward.Length < 1)
	        {
		        return new List<string>();
	        }
	        else
	        {
		        return ExerciseLookingForward.Split(',').ToList();
	        }
        }

        public List<string> LookingForwardList
        {
	        get { return ExerciseLookingFowardList(); }
        }

        public string RecordDateFormattedForExerciseList
        {
	        get
	        {
		        if (RecordDate == null)
		        {
			        return "";
		        }
		        else
		        {
			        return RecordDate.ToString("ddd, dd MMM yy").ToUpper();
		        }
	        }
        }
        
        public bool HasGratitude => ExerciseGrateful.Length > 0;
        public bool HasAchievement => ExerciseAchieved.Length > 0;
        public bool HasLookingForward => ExerciseLookingForward.Length > 0;

        public string GratitudeString
        {
	        get
	        {
		        var returnValue = "";
		        foreach (var g in ExerciseGratefulList())
		        {
			        if (g != ExerciseGratefulList().Last())
			        {
				        returnValue += CultureInfo.CurrentCulture.TextInfo.ToTitleCase(g) + ", ";
			        }
			        else
			        {
				        returnValue += CultureInfo.CurrentCulture.TextInfo.ToTitleCase(g);
			        }
		        }

		        return returnValue;
	        }
        }
        
        public string AchievedString
        {
	        get
	        {
		        var returnValue = "";
		        foreach (var g in ExerciseAchievementList())
		        {
			        if (g != ExerciseAchievementList().Last())
			        {
				        returnValue += CultureInfo.CurrentCulture.TextInfo.ToTitleCase(g) + ", ";
			        }
			        else
			        {
				        returnValue += CultureInfo.CurrentCulture.TextInfo.ToTitleCase(g);
			        }
		        }

		        return returnValue;
	        }
        }
        
        public string LookingForwardString
        {
	        get
	        {
		        var returnValue = "";
		        foreach (var g in ExerciseLookingFowardList())
		        {
			        if (g != ExerciseLookingFowardList().Last())
			        {
				        returnValue += CultureInfo.CurrentCulture.TextInfo.ToTitleCase(g) + ", ";
			        }
			        else
			        {
				        returnValue += CultureInfo.CurrentCulture.TextInfo.ToTitleCase(g);
			        }
		        }

		        return returnValue;
	        }
        }
	}
}

