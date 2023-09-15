using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeeeApp.Controls;
using MeeeApp.Models;

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
    }

    async void TapContinue_OnTapped(object sender, TappedEventArgs e)
    {
        await GridContinue.BounceOnPressAsync();
        
        //MyActivityIndicator.IsVisible = true;
        var selectedTimeSpan = CheckInTime;
        var checkInDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, selectedTimeSpan.Hours, selectedTimeSpan.Minutes, selectedTimeSpan.Seconds);
        User.SaveCheckInTimeToPreferences(checkInDateTime);
        //TODO: Create (or update) the Local Notification
        // Then move to check out time
        await Navigation.PushAsync(new SetCheckOutTimes());
    }

    async void SkipButton_OnClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync(true);
    }
    
}