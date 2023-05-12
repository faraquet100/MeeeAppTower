using System;
namespace MeeeApp.Services
{
	public static class AppSettings
	{
        
        public static ContentPage CurrentPage { get; set; }

        public static Microsoft.Maui.Graphics.Color MeeeColorMagenta = Color.Parse("#D40F7D");
        public static Microsoft.Maui.Graphics.Color MeeeColorGrey500 = Color.Parse("#6E6E6E");
    }
}

