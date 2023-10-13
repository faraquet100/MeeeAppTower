using Foundation;
using UIKit;
using UserNotifications;

namespace MeeeApp;

[Register("AppDelegate")]
public class AppDelegate : MauiUIApplicationDelegate
{
	protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();

	public override void OnActivated(UIApplication application)
	{
		base.OnActivated(application);
		
		// Force Light Mode
		application.KeyWindow.OverrideUserInterfaceStyle = UIUserInterfaceStyle.Light;
		
		// Request notification permissions
		UNUserNotificationCenter.Current.GetNotificationSettings((settings) =>
		{
			var status = settings.AuthorizationStatus;
			if (status != UNAuthorizationStatus.Authorized)
			{
				UNUserNotificationCenter.Current.RequestAuthorization(
					UNAuthorizationOptions.Alert | UNAuthorizationOptions.Badge | UNAuthorizationOptions.Sound,
					(granted, error) =>
					{
						if (granted)
						{
							/*
							InvokeOnMainThread(() =>
							{
								application.RegisterForRemoteNotifications();
							});
							*/
						}
					});
			}
		});
	}
}

