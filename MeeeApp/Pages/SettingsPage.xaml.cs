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

	private void BtnWipeCheckInTimes_OnClicked(object sender, EventArgs e)
	{
		User.WipeCheckInOutTimes();
	}

	async void BtnDailyMoments_OnClicked(object sender, EventArgs e)
	{
		await Navigation.PushModalAsync(new NavigationPage(new DailyMomentTester()));
	}
}
