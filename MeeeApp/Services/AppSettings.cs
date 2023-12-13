using System;
using MeeeApp.Models;
using MeeeApp.Pages;

namespace MeeeApp.Services
{
	public static class AppSettings
	{
		public static ContentPage CurrentPage { get; set; }
        public static JournalPage JournalPage { get; set; }
        
        public static DateTime? LastCheckInTimesAlert { get; set; } // When was the last time was asked to set check in/out times
        public static DailyMoment DailyMoment { get; set; }
        public static List<DailyMoment> DailyMoments { get; set; }
        
        public static Editor CurrentEditor { get; set; }	// Used in conjunction with the TextEditor view
        public static Label CurrentLabel { get; set; }	// Used in conjunction with the TextEditor view
        
        public static string MOMENT_IMAGE_URL = "https://meeeweb.azurewebsites.net/moment-images/";
        

        public static Microsoft.Maui.Graphics.Color MeeeColorMagenta = Color.Parse("#D40F7D");
        public static Microsoft.Maui.Graphics.Color MeeeColorCyan = Color.Parse("#009FDF");
        public static Microsoft.Maui.Graphics.Color MeeeColorGrey500 = Color.Parse("#6E6E6E");
        public static Microsoft.Maui.Graphics.Color MeeeColorGrey300 = Color.Parse("#ACACAC");
        public static Microsoft.Maui.Graphics.Color MeeeColorRed = Color.Parse("#DA291C");
        public static Microsoft.Maui.Graphics.Color MeeeColorYellow = Color.Parse("#FAE100");

        public static string TimeOfDay
        {
	        get
	        {
		        var hour = DateTime.Now.Hour;
		        if (hour >= 0 && hour < 12)
		        {
			        return "morning";
		        }
		        else if (hour >= 12 && hour < 18)
		        {
			        return "afternoon";
		        }
		        else
		        {
			        return "evening";
		        }
	        }
        }
    }
}

