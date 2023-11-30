using Microsoft.Extensions.Logging;
using CommunityToolkit.Maui;
using MeeeApp.Controls;
using Plugin.LocalNotification;
using Syncfusion.Maui.Core.Hosting;

namespace MeeeApp;

public static class MauiProgram
{


	public static MauiApp CreateMauiApp()
	{
		// Register SyncFusion Licence
        Syncfusion.Licensing.SyncfusionLicenseProvider
			//.RegisterLicense("Mgo+DSMBPh8sVXJ2S0d+X1VPd11dXmJWd1p/THNYflR1fV9DaUwxOX1dQl9gSXlSdkVlW3dceHVdRmU=;ORg4AjUWIQA/Gnt2V1hhQlJAfV5AQmBIYVp/TGpJfl96cVxMZVVBJAtUQF1hSn5Ud0VjX31WcnxUT2RZ;MjU1ODYzMUAzMjMyMmUzMDJlMzBEWjFJb210aTUzbG1tSTJYdXR4cDA0SkwzVkVnaWVlMndGdTQxNERFSzF3PQ==");
			.RegisterLicense("Ngo9BigBOggjHTQxAR8/V1NHaF5cXmpCf1FpRmJGdld5fUVHYVZUTXxaS00DNHVRdkdgWH9ednVWRGNZUkx/WkM=");

        var builder = MauiApp.CreateBuilder();
		builder
			.UseMauiApp<App>()
			.UseMauiCommunityToolkit()
			.UseMauiCommunityToolkitMediaElement()
			.UseLocalNotification()
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

        builder.ConfigureSyncfusionCore();

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

        // Remove the underline from the TimePicker in Android and the border in IOS
        Microsoft.Maui.Handlers.TimePickerHandler.Mapper.AppendToMapping("MyTimePickerCustomisation",
	        (handler, view) =>
	        {
		        if (view is TimePicker)
		        {
#if ANDROID

					handler.PlatformView.SetBackgroundColor(Android.Graphics.Color.Transparent);
#endif
#if IOS
			        handler.PlatformView.BorderStyle = UIKit.UITextBorderStyle.None;
#endif
		        }
	        });


#if DEBUG
        builder.Logging.AddDebug();
#endif

		return builder.Build();
	}
}

