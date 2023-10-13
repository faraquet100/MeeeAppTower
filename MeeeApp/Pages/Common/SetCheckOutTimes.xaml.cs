using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeeeApp.Models;
using MeeeApp.Services;
using Plugin.LocalNotification;

namespace MeeeApp.Pages;

public partial class SetCheckOutTimes : ContentPage
{
    public TimeSpan CheckOutTime { get; set; } = TimeSpan.FromHours(18);

    public SetCheckOutTimes()
    {
        InitializeComponent();

        DateTime checkOutTime = User.CheckOutTimeFromPreferences();
        if (checkOutTime.Year > 2020)
        {
            CheckOutTime = checkOutTime.TimeOfDay;
        }

        BindingContext = this;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        FixAndroid();
    }
    
    private void FixAndroid()
    {
        TimePicker.HeightRequest = 60;
        TimePicker.Margin = new Thickness(0, 0, 0, -4);
    }

    async void SkipButton_OnClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync(true);
    }

    async void TapContinue_OnTapped(object sender, TappedEventArgs e)
    {
        await GridContinue.BounceOnPressAsync();
        var selectedTimeSpan = CheckOutTime;
        var checkOutDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, selectedTimeSpan.Hours, selectedTimeSpan.Minutes, selectedTimeSpan.Seconds);
        User.SaveCheckOutTimeToPreferences(checkOutDateTime);
        
        // Create the notification
        var nextCheckOutTime = checkOutDateTime;
        if (nextCheckOutTime < DateTime.Now)
        {
            nextCheckOutTime = checkOutDateTime.AddDays(1);
        }
        
        //var request = NotificationHelper.CheckInNotification(nextCheckOutTime);
        var secondsDiff = (nextCheckOutTime - DateTime.Now).TotalSeconds;
        var request = NotificationHelper.CheckOutNotification(secondsDiff);
        
        if (await LocalNotificationCenter.Current.AreNotificationsEnabled())
        {
            LocalNotificationCenter.Current.Clear(NotificationHelper.NOTIFICAITON_ID_CHECKOUT);
            await LocalNotificationCenter.Current.Show(request);
        }
        else
        {
            await DisplayAlert("Notifications Disabled", "You have disabled notifications for this app.  Please enable them in your device settings.", "OK");
        }

        await Navigation.PopModalAsync(true);
    }
}