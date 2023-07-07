using MeeeApp.Models;
using MeeeApp.Pages;

namespace MeeeApp;

public partial class App : Application
{
 	public App()
	{
		InitializeComponent();

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

