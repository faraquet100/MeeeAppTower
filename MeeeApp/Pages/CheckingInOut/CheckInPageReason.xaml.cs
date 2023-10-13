using MeeeApp.Models;
using MeeeApp.Services;
using KeyboardExtensions = CommunityToolkit.Maui.Core.Platform.KeyboardExtensions;

namespace MeeeApp.Pages;

public partial class CheckInPageReason : ContentPage
{
    private User _user;
    private int _score;
    private DateTime _calendarDate;
    private CheckInPage.CheckingDirection _direction;

	public CheckInPageReason(User user, int score, CheckInPage.CheckingDirection direction, DateTime calendarDate)
	{
		InitializeComponent();
        _user = user;
        _score = score;
        _direction = direction;
        _calendarDate = calendarDate;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        MyActivityIndicator.IsVisible = false;
        var dailyRecord = _user.DailyRecordForDate(_calendarDate);
        FixAndroid();

        // Salutation
        if (_direction == CheckInPage.CheckingDirection.In)
        {
            LBLTitle.Text = "you checked-in :" + _score.ToString();
        }

        if (_direction == CheckInPage.CheckingDirection.Out)
        {
            LblNavTitle.Text = "checking-out";
            
            if (dailyRecord == null)
            {
                LBLTitle.Text = "you checked-out :" + _score.ToString();
            }
            else
            {
                if (_score > dailyRecord.CheckInScore)
                {
                    LBLTitle.Text = "your score went up";
                } else if (_score == dailyRecord.CheckInScore)
                {
                    LBLTitle.Text = "your score remained the same";
                }
                else
                {
                    LBLTitle.Text = "your score went down";
                }
            }

            InputLayoutReason.Hint = "Why was this?";
        }

        if (dailyRecord != null)
        {
            if (_direction == CheckInPage.CheckingDirection.In)
            {
                EDTTellUsMore.Text = dailyRecord.CheckInReason;
            }
            else
            {
                EDTTellUsMore.Text = dailyRecord.CheckOutReason;
            }
        }
    }

    private void FixAndroid()
    {
        // Because Android puts the grid view after the back button
        #if ANDROID 
            LblNavTitle.Margin = new Thickness(-70, 0, 0, 0);
        #endif
    }

    async void BarButtonCancel_Clicked(System.Object sender, System.EventArgs e)
    {
        await Navigation.PopModalAsync(true);
    }

    async void TapCheckIn_Tapped(System.Object sender, Microsoft.Maui.Controls.TappedEventArgs e)
    {
        await GridCheckIn.BounceOnPressAsync();

        if (_direction == CheckInPage.CheckingDirection.In)
        {
            await SaveCheckingIn(EDTTellUsMore.Text ?? "");
        }
        else
        {
            await SaveCheckingOut(EDTTellUsMore.Text ?? "");
        }
    }

    async Task SaveCheckingIn(string reason = "")
    {
        DailyRecord daily = new DailyRecord();
        daily.UserId = _user.UserId;
        daily.CheckInScore = _score;
        daily.CheckInReason = reason;
        daily.RecordDate = _calendarDate;

        FormatForBusy(true);
        var result = await ApiService.CheckIn(daily);

        switch (result)
        {
            case ApiService.ApiResult.BadRequest:
                FormatForBusy(false);
                await DisplayAlert("Unexpected Error", "An unexpected error occured whilst communicating with the server.  Please try again later", "OK");
                break;
            case ApiService.ApiResult.NotAuthorized:
                FormatForBusy(false);
                await DisplayAlert("Login Expired", "Your login credentials have expired.  Continue to re-enter your credentials.", "OK");
                User.WipeUser();
                Application.Current.MainPage = new NavigationPage(new LoginPage());
                break;
            case ApiService.ApiResult.NoInternet:
                FormatForBusy(false);
                await DisplayAlert("No Network", "We can't communicate with the Internet at this time, please check your network connection and try again.", "OK");
                break;
            case ApiService.ApiResult.Success:
                // Update the Journal Page view
                AppSettings.JournalPage.UpdateAfterCheckInOut();
                // Get today's Daily Moment
                await GetDailyMomentAndContinue(_score);
                break;
        }
    }
    
    async Task SaveCheckingOut(string reason = "")
    {
        // The api handles matching the checkout scores to the correct daily record if it exists
        // it does not overwrite any check-in values
        DailyRecord daily = new DailyRecord();
        daily.UserId = _user.UserId;
        daily.CheckOutScore = _score;
        daily.CheckOutReason = reason;
        daily.CheckOutTime = DateTime.Now;
        daily.RecordDate = _calendarDate;

        FormatForBusy(true);
        var result = await ApiService.CheckOut(daily);

        switch (result)
        {
            case ApiService.ApiResult.BadRequest:
                FormatForBusy(false);
                await DisplayAlert("Unexpected Error", "An unexpected error occured whilst communicating with the server.  Please try again later", "OK");
                break;
            case ApiService.ApiResult.NotAuthorized:
                FormatForBusy(false);
                await DisplayAlert("Login Expired", "Your login credentials have expired.  Continue to re-enter your credentials.", "OK");
                User.WipeUser();
                Application.Current.MainPage = new NavigationPage(new LoginPage());
                break;
            case ApiService.ApiResult.NoInternet:
                FormatForBusy(false);
                await DisplayAlert("No Network", "We can't communicate with the Internet at this time, please check your network connection and try again.", "OK");
                break;
            case ApiService.ApiResult.Success:
                // Update the Journal Page view
                AppSettings.JournalPage.UpdateAfterCheckInOut();
                // Get today's Daily Moment
                if (_direction == CheckInPage.CheckingDirection.In)
                {
                    await GetDailyMomentAndContinue(_score);
                }
                else
                {
                    await Navigation.PushAsync(new CheckingOutThanks(_calendarDate));
                }

                break;
        }
    }

    async Task GetDailyMomentAndContinue(int score)
    {
        var result = await ApiService.GetDailyMoment(_user, score); // Daily moment is stored in AppSettings
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
                var onThisDay = await ApiService.GetOnThisDay(DateTime.Now);
                await Navigation.PushAsync(new DailyMomentDetail(AppSettings.DailyMoment, onThisDay, _calendarDate));
                break;
        }
        
        FormatForBusy(false);
    }
    
    async void BtnSkip_OnClicked(object sender, EventArgs e)
    {
        var dailyRecord = _user.DailyRecordForDate(DateTime.Now);
        if (_direction == CheckInPage.CheckingDirection.In)
        {
            if (dailyRecord == null)
            {
                await SaveCheckingIn("");
            }
            else
            {
                await SaveCheckingIn(dailyRecord.CheckInReason);
            }
        }
        else
        {
            if (dailyRecord == null)
            {
                await SaveCheckingOut("");
            }
            else
            {
                await SaveCheckingOut(dailyRecord.CheckOutReason);
            }
        }
    }
    
    private void FormatForBusy(bool busy)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            MyActivityIndicator.IsVisible = busy;
            EDTTellUsMore.IsEnabled = !busy;
        });
    }

    async void EDTTellUsMore_OnFocused(object sender, FocusEventArgs e)
    {
        if (KeyboardExtensions.IsSoftKeyboardShowing(EDTTellUsMore))
        {
            FixedScrollView.Margin = new Thickness(0, 0, 0, 320);
            //await FixedScrollView.ScrollToAsync(EDTTellUsMore, ScrollToPosition.Start, true);
            //await FixedScrollView.ScrollToAsync(GridCheckIn, ScrollToPosition.End, true);
        }
    }
}
