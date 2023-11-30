using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeeeApp.Models;
using MeeeApp.Services;
using MeeeApp.Controls;

namespace MeeeApp.Pages;

public partial class DailyMomentList : ContentPage
{
    private OnThisDay _onThisDay = new OnThisDay();
    private bool _isFavourites = false;
    public DailyMomentList(bool isFavourites)
    {
        InitializeComponent();
        _isFavourites = isFavourites;
        AddDailyRecordsToMoments();
    }

    private void AddDailyRecordsToMoments()
    {
        // When did the user view this
        var user = User.UserFromPreferences();
        var dailyRecords = user.DailyRecords.OrderByDescending(r => r.RecordDate).ToList();
        foreach (var moment in AppSettings.DailyMoments)
        {
            moment.IsFavouritesView = _isFavourites;
            var dailyRecord = dailyRecords.FirstOrDefault(r => r.DailyMoment.Id == moment.Id);
            if (dailyRecord != null)
            {
                moment.DailyRecord = dailyRecord;
            }
        }
        
        // Sort the list
        AppSettings.DailyMoments = AppSettings.DailyMoments.OrderByDescending(m => m.DailyRecord?.RecordDate).ToList();
    }
    
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _onThisDay = await ApiService.GetOnThisDay(DateTime.Now);

        if (AppSettings.DailyMoments.Count > 0)
        {
            LvDailyMoments.ItemsSource = AppSettings.DailyMoments;
            BordList.IsVisible = true;
            VsNoData.IsVisible = false;
        }
        else
        {
            BordList.IsVisible = false;
            VsNoData.IsVisible = true;
        }
        
    }

    async void LvDailyMoments_OnItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem != null)
        {
            //MyActivityIndicator.IsVisible = true;
            LvDailyMoments.SelectedItem = null;
            var moment = e.SelectedItem as DailyMoment;
            await Navigation.PushAsync(new DailyMomentDetail(moment, _onThisDay,  DateTime.Now, true, _isFavourites));
            LvDailyMoments.SelectedItem = null;
            LvDailyMoments.SelectedItem = null;
            //MyActivityIndicator.IsVisible = false;
        }
    }
}