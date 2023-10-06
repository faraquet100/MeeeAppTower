using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeeeApp.Pages.Exercises;
using MeeeApp.Services;

namespace MeeeApp.Pages;

public partial class CheckingOutThanks : ContentPage
{
    private DateTime _calendarDate;
    public CheckingOutThanks(DateTime calendarDate)
    {
        InitializeComponent();
        _calendarDate = calendarDate;
    }

    async void Button_OnClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync(true);
    }

    async void TapContinue_OnTapped(object sender, TappedEventArgs e)
    {
        await GridContinue.BounceOnPressAsync();
        await Navigation.PushAsync(new ExcerciseStart(_calendarDate));
    }
}