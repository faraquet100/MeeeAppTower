using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeeeApp.Models;
using MeeeApp.Services;

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
        //TODO: Create (or update) the Local Notification
        // Then move to check out time
        await Navigation.PopModalAsync(true);
    }
}