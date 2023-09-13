using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeeeApp.Models;

namespace MeeeApp.Pages;

public partial class DailyMomentIntro : ContentPage
{
    private User _user;
    private DailyMoment _dailyMoment;
    
    public DailyMomentIntro(User user, DailyMoment dailyMoment)
    {
        InitializeComponent();
        _user = user;
        _dailyMoment = dailyMoment;
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Salutation
        LblHeading.Text = _dailyMoment.Heading;
    }

    async void BtnSkip_OnClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync(true);
    }
}