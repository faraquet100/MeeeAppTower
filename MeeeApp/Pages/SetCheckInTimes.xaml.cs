using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeeeApp.Controls;

namespace MeeeApp.Pages;

public partial class SetCheckInTimes : ContentPage
{
    public TimeSpan CheckInTime { get; set; } = TimeSpan.FromHours(9);
    
    public SetCheckInTimes()
    {
        InitializeComponent();
        BindingContext = this;
    }
    
    async void TapContinue_OnTapped(object sender, TappedEventArgs e)
    {
        await GridContinue.BounceOnPressAsync();
        
        MyActivityIndicator.IsVisible = true;
        var selectedTimeSpan = CheckInTime;
        var checkInDateTime = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, selectedTimeSpan.Hours, selectedTimeSpan.Minutes, selectedTimeSpan.Seconds);
        
        // Actually set the checkin time via the api
        // Create (or update) the Local Notification
        // Then move to check out time
    }

    async void SkipButton_OnClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync(true);
    }
    
}