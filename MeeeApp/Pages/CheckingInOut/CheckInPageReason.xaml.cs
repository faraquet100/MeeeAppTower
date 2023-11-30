using MeeeApp.Controls;
using MeeeApp.Models;
using MeeeApp.Pages.Common;
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
        if (User.TestModeFromPreferences())
        {
            dailyRecord = null;
        }
        
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

        // If we are returning from a modal text entry we don't want to overwrite the text
        
        if (dailyRecord != null && AppSettings.CurrentEditor != EDTTellUsMore)
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

        AppSettings.CurrentEditor = null;
        
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
        
        if (User.TestModeFromPreferences())
        {
            daily.RecordDate = _calendarDate.AddYears(-5);
        }

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
                await GetDailyMomentAndContinue();
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

        if (User.TestModeFromPreferences())
        {
            daily.CheckOutTime = DateTime.Now.AddYears(-5);
            daily.RecordDate = _calendarDate.AddYears(-5);
        }

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
                
                if (_direction == CheckInPage.CheckingDirection.In)
                {
                    await GetDailyMomentAndContinue();
                }
                else
                {
                    await Navigation.PushAsync(new CheckingOutThanks(_calendarDate));
                }

                break;
        }
    }

    async Task GetDailyMomentAndContinue()
    {
        _user = User.UserFromPreferences(); // Needs reloading with the new data
        var onThisDay = await ApiService.GetOnThisDay(_calendarDate);
        var dailyRecord = _user.DailyRecordForDate(_calendarDate);
        await Navigation.PushAsync(new DailyMomentDetail(dailyRecord.DailyMoment, onThisDay, _calendarDate));
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

    async void GridReasonTap_OnTapped(object sender, TappedEventArgs e)
    {
        AppSettings.CurrentEditor = EDTTellUsMore;
        InputLayoutReason.Opacity = 0.5;
        
        string title = _direction == CheckInPage.CheckingDirection.In ? "checking-in" : "checking-out";
        string placeholder = _direction == CheckInPage.CheckingDirection.In ? "Tell us more . . ." : "Why was this?";
        
        await Navigation.PushModalAsync(new NavigationPage(new TextEditor(title, placeholder, EDTTellUsMore.Text)));
        InputLayoutReason.Opacity = 1.0;
    }
}
