using MeeeApp.Models;

namespace MeeeApp.Pages;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
	}

	private void BtnLogout_OnClicked(object sender, EventArgs e)
	{
		User.WipeUser();
		Application.Current.MainPage = new NavigationPage(new LoginPage());
	}
}
