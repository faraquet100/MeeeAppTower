using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using MeeeApp.Controls;

namespace MeeeApp;

public static class MauiProgram
{


	public static MauiApp CreateMauiApp()
	{
		var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.ConfigureFonts(fonts =>
			{
				fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
				fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
				fonts.AddFont("head-of-sean.ttf", "HandOfSean");
				fonts.AddFont("BearHugs.ttf", "BearHugs");
				fonts.AddFont("ByronRecCon.ttf", "Byron");
				fonts.AddFont("Montserrat.ttf", "Mont");
				fonts.AddFont("Montserrat-Italic.ttf", "MontItalic");
			});

#if ANDROID
/*
		Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping("NoUnderline", (h, v) =>
		{
			// Remove Underline from Android Entries
			h.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
		});

		Microsoft.Maui.Handlers.EditorHandler.Mapper.AppendToMapping("NoUnderline", (h, v) =>
        {
            // Remove Underline from Android Editors
            h.PlatformView.BackgroundTintList = Android.Content.Res.ColorStateList.ValueOf(Android.Graphics.Color.Transparent);
        });
*/
#endif
		
		// Borderless Entry Handler
		Microsoft.Maui.Handlers.EntryHandler.Mapper.AppendToMapping(nameof(BorderlessEntry), (handler, view) =>
		{
#if ANDROID
            handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#elif IOS
			handler.PlatformView.BackgroundColor = UIKit.UIColor.Clear;
			handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#endif
		});


#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}

