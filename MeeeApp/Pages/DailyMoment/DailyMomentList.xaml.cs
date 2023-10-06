using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeeeApp.Models;
using MeeeApp.Services;

namespace MeeeApp.Pages;

public partial class DailyMomentList : ContentPage
{
    private OnThisDay _onThisDay = new OnThisDay();
    public DailyMomentList()
    {
        InitializeComponent();
    }
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _onThisDay = await ApiService.GetOnThisDay(DateTime.Now);
        LvDailyMoments.ItemsSource = AppSettings.DailyMoments;
    }

    async void LvDailyMoments_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem != null)
        {
            MyActivityIndicator.IsVisible = true;
            LvDailyMoments.SelectedItem = null;
            var moment = e.SelectedItem as DailyMoment;
            await Navigation.PushAsync(new DailyMomentDetail(moment, _onThisDay,  DateTime.Now, true));
            LvDailyMoments.SelectedItem = null;
            LvDailyMoments.SelectedItem = null;
            MyActivityIndicator.IsVisible = false;
        }
    }
}