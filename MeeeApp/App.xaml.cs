using MeeeApp.Models;
using MeeeApp.Pages;

namespace MeeeApp;

public partial class App : Application
{
 	public App()
	{
		InitializeComponent();

		// Force Light Mode
		// Can do this for iOS in AppDelegate - but same trick for Android doesn't seem to work
		Application.Current.UserAppTheme = AppTheme.Light;
		this.RequestedThemeChanged += (s, e) => { Application.Current.UserAppTheme = AppTheme.Light; };

		User user = User.UserFromPreferences();
		if (user.IsValid)
		{
			MainPage = new MainTabbedPage();
		}
		else
		{
			MainPage = new NavigationPage(new LoginPage());
		}
	}
}

