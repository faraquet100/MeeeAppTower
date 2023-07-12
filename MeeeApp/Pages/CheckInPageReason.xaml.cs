using MeeeApp.Models;
using MeeeApp.Services;

namespace MeeeApp.Pages;

public partial class CheckInPageReason : ContentPage
{
    private User _user;
    private int _score;

	public CheckInPageReason(User user, int score)
	{
		InitializeComponent();
        _user = user;
        _score = score;
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Salutation
        LBLTitle.Text = "you checked in with a " + _score.ToString();
    }

    async void BarButtonCancel_Clicked(System.Object sender, System.EventArgs e)
    {
        await Navigation.PopModalAsync(true);
    }

    async void TapCheckIn_Tapped(System.Object sender, Microsoft.Maui.Controls.TappedEventArgs e)
    {
        // Test
        DailyRecord daily = new DailyRecord();
        daily.UserId = _user.UserId;
        daily.CheckInScore = _score;
        daily.CheckInReason = EDTTellUsMore.Text ?? "";

        var result = await ApiService.CheckIn(daily);

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
                AppSettings.JournalPage.UpdateAfterCheckInOut();
                await Navigation.PopModalAsync(true);
                break;
        }
    }
}
