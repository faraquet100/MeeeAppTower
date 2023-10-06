using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeeeApp.Models;
using MeeeApp.Services;

namespace MeeeApp.Pages;

public partial class DailyMomentTester : ContentPage
{
    private List<DailyMoment> _dailyMoments = new List<DailyMoment>();
    private int _currentDailyMomentIndex = 0;
    private OnThisDay _onThisDay = new OnThisDay();
    
    public DailyMomentTester()
    {
        InitializeComponent();
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadDailyMoments();
    }

    private void FormatForCurrentIndex()
    {
        var moment = AppSettings.DailyMoments[_currentDailyMomentIndex];
        LblTitle.Text = moment.Heading;
        LblCaption.Text = "Viewing " + (_currentDailyMomentIndex + 1).ToString() + " of " + AppSettings.DailyMoments.Count;
    }

    async void LoadDailyMoments()
    {
        var result = await ApiService.GetAllDailyMoments();
        _onThisDay = await ApiService.GetOnThisDay(DateTime.Now);

        switch (result)
        {
            case ApiService.ApiResult.BadRequest:
                await DisplayAlert("Unexpected Error", "An unexpected error occured whilst communicating with the server.  Please try again later", "OK");
                break;
            case ApiService.ApiResult.NotAuthorized:
                await DisplayAlert("Login Expired", "Your login credentials have expired.  Continue to re-enter your credentials.", "OK");
                User.WipeUser();
                Application.Current.MainPage = new NavigationPage(new LoginPage());
                break;
            case ApiService.ApiResult.NoInternet:
                await DisplayAlert("No Network", "We can't communicate with the Internet at this time, please check your network connection and try again.", "OK");
                break;
            case ApiService.ApiResult.Success:
                _currentDailyMomentIndex = 0;
                FormatForCurrentIndex();
                break;
        }
    }

    async void BarButtonClose_OnClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync(true);
    }

    private void BtnBack_OnClicked(object sender, EventArgs e)
    {
        if (_currentDailyMomentIndex > 0)
        {
            _currentDailyMomentIndex = _currentDailyMomentIndex - 1;
        }
        
        FormatForCurrentIndex();
    }

    private void BtnReload_OnClicked(object sender, EventArgs e)
    {
        LoadDailyMoments();
    }

    async void BtnView_OnClicked(object sender, EventArgs e)
    {
        var moment = AppSettings.DailyMoments[_currentDailyMomentIndex];
        await Navigation.PushAsync(new DailyMomentDetail(moment, _onThisDay, DateTime.Now));
    }

    private void BtnNext_OnClicked(object sender, EventArgs e)
    {
        if (_currentDailyMomentIndex < AppSettings.DailyMoments.Count - 1)
        {
            _currentDailyMomentIndex = _currentDailyMomentIndex + 1;
        }
        
        FormatForCurrentIndex();
    }
}