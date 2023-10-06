using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeeeApp.Models;

namespace MeeeApp.Pages;

public partial class DailyMomentIntro : ContentPage
{
    // WE NO LONGER USE THIS PAGE
    
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

        ImgMomentImage.Source = _dailyMoment.FullImageUrl;
        LblMomentTitle.Text = _dailyMoment.Heading.ToLower();
        
    }

    async void BtnSkip_OnClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync(true);
    }

    async void TapViewDailyMoment_OnTapped(object sender, TappedEventArgs e)
    {
        //await Navigation.PushAsync(new DailyMomentDetail(_dailyMoment));
    }
}