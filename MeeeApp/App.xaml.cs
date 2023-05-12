using MeeeApp.Pages;

namespace MeeeApp;

public partial class App : Application
{
	public App()
	{
		InitializeComponent();

		MainPage = new NavigationPage(new LoginPage());
		//MainPage = new LoginPage();
	}
}

