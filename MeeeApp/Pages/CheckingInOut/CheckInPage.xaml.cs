using MeeeApp.Extensions;
using MeeeApp.Models;

namespace MeeeApp.Pages;

public partial class CheckInPage : ContentPage
{
    private User _user;
    private int selectedScore = 5;
    private CheckingDirection _direction;
    private DateTime _calendarDate;

    public enum CheckingDirection
    {
        In,
        Out
    }

	public CheckInPage(CheckingDirection direction, DateTime calendarDate)
	{
        InitializeComponent();
        _user = User.UserFromPreferences();
        _direction = direction;
        _calendarDate = calendarDate;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        FixAndroid();

        // Salutation
        switch (DateExtensions.GetPartOfDay())
        {
            case DateExtensions.PartOfDay.Morning:
                LblName.Text = "good morning " + _user.FirstName.ToLower() + "!";
                break;
            case DateExtensions.PartOfDay.Afternoon:
                LblName.Text = "good afternoon " + _user.FirstName.ToLower() + "!";
                break;
            case DateExtensions.PartOfDay.Evening:
                LblName.Text = "good evening " + _user.FirstName.ToLower() + "!";
                break;
        }

        if (_direction == CheckingDirection.Out)
        {
            LblNavTitle.Text = "checking-out";
            LblWelcome.Text = "Are you ready for Checking-Out?";
        }

        var dailyRecord = _user.DailyRecordForDate(_calendarDate);
        if (User.TestModeFromPreferences())
        {
            dailyRecord = null;
        }
        
        if (dailyRecord != null)
        {
            if (_direction == CheckingDirection.In)
            {
                if (dailyRecord.CheckInTime.Year > 2000)
                {
                    selectedScore = dailyRecord.CheckInScore;
                }
            }
            else
            {
                if (dailyRecord.CheckOutTime.Year > 2000)
                {
                    selectedScore = dailyRecord.CheckOutScore;
                }
            }
            
            SliderCheckIn.Value = selectedScore;
        }
    }

    private void FixAndroid()
    {
        
    }


    #region Actions


    async void TapCheckIn_Tapped(System.Object sender, Microsoft.Maui.Controls.TappedEventArgs e)
    {
        await Navigation.PopModalAsync(true);
    }

    void SliderCheckIn_ValueChanged(System.Object sender, Microsoft.Maui.Controls.ValueChangedEventArgs e)
    {
        selectedScore = (int)e.NewValue;
        var imageName = "newface" + (selectedScore).ToString();

        MainThread.BeginInvokeOnMainThread(() =>
        {
            ImgFace.Source = imageName;
            LblFaceValue.Text = ((int)e.NewValue).ToString();
        });
    }

    async void TapContinue_Tapped(System.Object sender, Microsoft.Maui.Controls.TappedEventArgs e)
    {
        await GridContinue.BounceOnPressAsync();
        await Navigation.PushAsync(new CheckInPageReason(_user, selectedScore, _direction, _calendarDate));
    }

    async void BarButtonCancel_Clicked(System.Object sender, System.EventArgs e)
    {
        await Navigation.PopModalAsync(true);
    }

    #endregion

    async void BtnSkip_OnClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync(true);
    }
}
