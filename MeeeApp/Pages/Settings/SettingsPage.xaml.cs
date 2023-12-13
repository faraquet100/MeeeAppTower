using MeeeApp.Controls;
using MeeeApp.Models;
using MeeeApp.Pages.Settings;
using MeeeApp.Services;
using Plugin.LocalNotification;

namespace MeeeApp.Pages;

public partial class SettingsPage : ContentPage
{
	public SettingsPage()
	{
		InitializeComponent();
	}

	async protected override void OnAppearing()
	{
		base.OnAppearing();

		FormatForTestMode();
		FormatForAdminMode();
		LoadVersionInfo();
	}

	public void FormatForAdminMode()
	{
		BorderAdmin.IsVisible = User.AdminModeFromPreferences();
	}

	private void FormatForTestMode()
	{
		if (User.TestModeFromPreferences())
		{
			GridTurnTestModeOff.IsVisible = true;
			GridTurnTestModeOn.IsVisible = false;
		}
		else
		{
			GridTurnTestModeOff.IsVisible = false;
			GridTurnTestModeOn.IsVisible = true;
		}
	}

	private void LoadVersionInfo()
	{
		LblVersion.Text = "Version: " + VersionTracking.Default.CurrentVersion.ToString();
		LblBuild.Text = "Build: " + VersionTracking.Default.CurrentBuild.ToString();
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
			await Navigation.PushAsync(new DailyMomentList(true));
		}
		
		MyActivityIndicator.IsVisible = false;
	}
	
	async void GridJournalEntriesTap_OnTapped(object sender, TappedEventArgs e)
	{
		var grid = sender as CobaltGrid;
		await grid.BounceOnPressAsync();

		MyActivityIndicator.IsVisible = true;
		var success = await LoadDailyMoments();
		if (success)
		{
			var journalEntries = User.UserFromPreferences().DailyRecords.Where(r => r.CheckOutJournalEntry.Length > 0)
				.Select(m => m.DailyMoment.Id)
				.Distinct()
				.ToList();

			AppSettings.DailyMoments = AppSettings.DailyMoments.Where(m => journalEntries.Contains(m.Id))
				.ToList();
			await Navigation.PushAsync(new DailyMomentList(true));
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
			await Navigation.PushAsync(new DailyMomentList(false));
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

	async void TapTestCheckInNotification_OnTapped(object sender, TappedEventArgs e)
	{
		var grid = sender as CobaltGrid;
		await grid.BounceOnPressAsync();
		
		var request = NotificationHelper.CheckInTestNotification();
		if (await LocalNotificationCenter.Current.AreNotificationsEnabled())
		{
			LocalNotificationCenter.Current.Clear(NotificationHelper.NOTIFICATION_ID_CHECKIN_TEST);
			await LocalNotificationCenter.Current.Show(request);
			await DisplayAlert("Notifications Scheduled", "Your notification has been sent", "OK");
		}
		else
		{
			await DisplayAlert("Notifications Disabled", "You have disabled notifications for this app.  Please enable them in your device settings.", "OK");
		}
	}

	async void TapTestCheckPutNotification_OnTapped(object sender, TappedEventArgs e)
	{
		var grid = sender as CobaltGrid;
		await grid.BounceOnPressAsync();
		
		var request = NotificationHelper.CheckOutTestNotification();
		if (await LocalNotificationCenter.Current.AreNotificationsEnabled())
		{
			LocalNotificationCenter.Current.Clear(NotificationHelper.NOTIFICAITON_ID_CHECKOUT_TEST);
			await LocalNotificationCenter.Current.Show(request);
			await DisplayAlert("Notifications Scheduled", "Your notification has been sent", "OK");
		}
		else
		{
			await DisplayAlert("Notifications Disabled", "You have disabled notifications for this app.  Please enable them in your device settings.", "OK");
		}
	}

	private void TapTurnTestModeOn_OnTapped(object sender, TappedEventArgs e)
	{
		User.SaveTestModeToPreferences(true);
		FormatForTestMode();
	}

	private void TapTurnTestModeOff_OnTapped(object sender, TappedEventArgs e)
	{
		User.SaveTestModeToPreferences(false);
		FormatForTestMode();
	}


	async void GridExercisesTap_OnTapped(object sender, TappedEventArgs e)
	{
		var grid = sender as CobaltGrid;
		await grid.BounceOnPressAsync();
		await Navigation.PushAsync(new ReviewExercises());
	}

	async void GridVersionInfoTap_OnTapped(object sender, TappedEventArgs e)
	{
		string response = await DisplayPromptAsync("Admin", "Please enter the admin password");

		if (response.ToLower() == "meeeadmin")
		{
			if (!User.AdminModeFromPreferences())
			{
				User.SaveAdminModeToPreferences(true);
				await DisplayAlert("Admin Mode ON",
					"Admin mode has been turned ON and you will now see additional options in Settings.", "OK");
			}
			else
			{
				User.SaveAdminModeToPreferences(false);
				await DisplayAlert("Admin Mode OFF",
					"Admin mode has been turned OFF and you will no longer see additional options in Settings.", "OK");
			}

			FormatForAdminMode();
		}
	}
}
