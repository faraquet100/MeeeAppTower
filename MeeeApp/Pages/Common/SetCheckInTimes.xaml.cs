using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeeeApp.Controls;
using MeeeApp.Models;
using MeeeApp.Services;
using Plugin.LocalNotification;

namespace MeeeApp.Pages;

public partial class SetCheckInTimes : ContentPage
{
    public TimeSpan CheckInTime { get; set; } = TimeSpan.FromHours(9);
    
    public SetCheckInTimes()
    {
        InitializeComponent();
        
        // Set binding vars before on appearing
        DateTime checkInTime = User.CheckInTimeFromPreferences();
        if (checkInTime.Year > 2020)
        {
            CheckInTime = checkInTime.TimeOfDay;
        }
        
        BindingContext = this;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        var user = User.UserFromPreferences();
        LblName.Text = "hi " + user.FirstName.ToLower() + "!";
        FixAndroid();
    }

    private void FixAndroid()
    {
        TimePicker.HeightRequest = 60;
        TimePicker.Margin = new Thickness(0, 0, 0, -4);
    }

    async void TapContinue_OnTapped(object sender, TappedEventArgs e)
    {
        await GridContinue.BounceOnPressAsync();
        
        //MyActivityIndicator.IsVisible = true;
        var selectedTimeSpan = CheckInTime;
        var checkInDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, selectedTimeSpan.Hours, selectedTimeSpan.Minutes, selectedTimeSpan.Seconds);
        User.SaveCheckInTimeToPreferences(checkInDateTime);
        
        // Create the notification
        var nextCheckInTime = checkInDateTime;
        
        if (nextCheckInTime < DateTime.Now)
        {
            nextCheckInTime = checkInDateTime.AddDays(1);
        }
        
        var secondsDiff = (nextCheckInTime - DateTime.Now).TotalSeconds;
        var request = NotificationHelper.CheckInNotification(secondsDiff);
        
        if (await LocalNotificationCenter.Current.AreNotificationsEnabled())
        {
            LocalNotificationCenter.Current.Clear(NotificationHelper.NOTIFICATION_ID_CHECKIN);
            await LocalNotificationCenter.Current.Show(request);
        }
        else
        {
            await DisplayAlert("Notifications Disabled", "You have disabled notifications for this app.  Please enable them in your device settings.", "OK");
        }
        
        // Then move to check out time
        await Navigation.PushAsync(new SetCheckOutTimes());
    }

    async void SkipButton_OnClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync(true);
    }
    
}