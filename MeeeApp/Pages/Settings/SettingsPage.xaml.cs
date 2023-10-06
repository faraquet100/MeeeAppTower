using MeeeApp.Controls;
using MeeeApp.Models;
using MeeeApp.Services;

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

	async void GridSavedMomentsTap_OnTapped(object sender, TappedEventArgs e)
	{
		var grid = sender as CobaltGrid;
		var favourites = DailyMoment.GetFavourites();
		await grid.BounceOnPressAsync();

		MyActivityIndicator.IsVisible = true;
		var success = await LoadDailyMoments();
		if (success)
		{
			AppSettings.DailyMoments = AppSettings.DailyMoments.Where(m => favourites.Contains(m.Id)).ToList();
			await Navigation.PushAsync(new DailyMomentList());
		}
		
		MyActivityIndicator.IsVisible = false;
	}

	async void GridReviewMomentsTap_OnTapped(object sender, TappedEventArgs e)
	{
		var grid = sender as CobaltGrid;
		await grid.BounceOnPressAsync();
		
		MyActivityIndicator.IsVisible = true;
		var success = await LoadDailyMoments();
		if (success)
		{
			//var firstMoment = AppSettings.DailyMoments[0];
			await Navigation.PushAsync(new DailyMomentList());
		}

		MyActivityIndicator.IsVisible = false;
	}

	async void GridResetTimesTap_OnTapped(object sender, TappedEventArgs e)
	{
		var grid = sender as CobaltGrid;
		await grid.BounceOnPressAsync();
		
		AppSettings.LastCheckInTimesAlert = DateTime.Now;
		await Navigation.PushModalAsync(new NavigationPage(new SetCheckInTimes()));
	}
	
	async Task<bool> LoadDailyMoments()
	{
		var result = await ApiService.GetAllDailyMoments();

		switch (result)
		{
			case ApiService.ApiResult.BadRequest:
				await DisplayAlert("Unexpected Error", "An unexpected error occured whilst communicating with the server.  Please try again later", "OK");
				break;
			case ApiService.ApiResult.NotAuthorized:
				await DisplayAlert("Login Expired", "Your login credentials have expired.  Continue to re-enter your credentials.", "OK");
				User.WipeUser();
				Application.Current.MainPage = new NavigationPage(new LoginPage());
				break;
			case ApiService.ApiResult.NoInternet:
				await DisplayAlert("No Network", "We can't communicate with the Internet at this time, please check your network connection and try again.", "OK");
				break;
			case ApiService.ApiResult.Success:
				return true;
				break;
		}

		return false;
	}
}
