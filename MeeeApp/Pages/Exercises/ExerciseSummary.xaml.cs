using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeeeApp.Controls;
using MeeeApp.Models;

namespace MeeeApp.Pages.Scrap;

public partial class ExerciseSummary : ContentPage
{
    private DateTime _calendarDate;
    private List<DailyRecord> _dailyRecords;
    
    public static int TabGratitude = 0;
    public static int TabAchievements = 1;
    public static int TabLookingForward = 2;
    
    private int _selectedTab = TabGratitude;

    public ExerciseSummary(DateTime calendarDate)
    {
        InitializeComponent();
        _dailyRecords = User.UserFromPreferences().LastDailyRecords(3);
        _calendarDate = calendarDate;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        FormatForSelectedTab();
    }

    async void BtnDone_OnClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync(true);
    }
    
    #region Formatting

    private void FormatForSelectedTab()
    {
        if (_selectedTab == TabGratitude)
        {
            FormatForGratitude();
        }
        else if (_selectedTab == TabAchievements)
        {
            FormatForAchievements();
        }
        else if (_selectedTab == TabLookingForward)
        {
            FormatForLookingForward();
        }
    }

    private void FormatForDefault()
    {
        ImgTabGratitude.Source = "exercise_heart_unfilled.png";
        LblTabGratitude.FontAttributes = FontAttributes.None;
        ImgGratitudeUnderline.IsVisible = false;
        LayoutRecordsGratitude.IsVisible = false;
        
        ImgTabAchievements.Source = "star_blue_unfilled.png";
        LblTabAchievements.FontAttributes = FontAttributes.None;
        ImgAchievementsUnderline.IsVisible = false;
        LayoutRecordsAchievements.IsVisible = false;
        
        ImgTabLookingForward.Source = "carrot_pink_unfilled.png";
        LblTabLookingForward.FontAttributes = FontAttributes.None;
        ImgLookingForwardUnderline.IsVisible = false;
        LayoutRecordsLookingForward.IsVisible = false;

    }
    
    private void FormatForGratitude()
    {
        FormatForDefault();
        ImgTabGratitude.Source = "exercise_heart_filled.png";
        LblTabGratitude.FontAttributes = FontAttributes.Bold;
        ImgGratitudeUnderline.IsVisible = true;
        LayoutRecordsGratitude.IsVisible = true;
        PopulateGratitudes();
        
    }
    
    private void FormatForAchievements()
    {
        FormatForDefault();
        ImgTabAchievements.Source = "star_blue.png";
        LblTabAchievements.FontAttributes = FontAttributes.Bold;
        ImgAchievementsUnderline.IsVisible = true;
        LayoutRecordsAchievements.IsVisible = true;
        PopulateAchievements();
    }
    
    private void FormatForLookingForward()
    {
        FormatForDefault();
        ImgTabLookingForward.Source = "carrot_pink.png";
        LblTabLookingForward.FontAttributes = FontAttributes.Bold;
        ImgLookingForwardUnderline.IsVisible = true;
        LayoutRecordsLookingForward.IsVisible = true;
        PopulateLookingForward();
    }
    
    #endregion

    #region Tab Selections
    async void TapGratitude_OnTapped(object sender, TappedEventArgs e)
    {
        var grid = sender as CobaltGrid;
        await grid.BounceOnPressAsync();
        _selectedTab = TabGratitude;
        FormatForSelectedTab();
    }
    
    async void TapAchievements_OnTapped(object sender, TappedEventArgs e)
    {
        var grid = sender as CobaltGrid;
        await grid.BounceOnPressAsync();
        _selectedTab = TabAchievements;
        FormatForSelectedTab();
    }

    async void TapLookingForward_OnTapped(object sender, TappedEventArgs e)
    {
        var grid = sender as CobaltGrid;
        await grid.BounceOnPressAsync();
        _selectedTab = TabLookingForward;
        FormatForSelectedTab();
    }
    
    #endregion

    #region Populate

    private void PopulateGratitudes()
    {
        if (CountOfGratitudes() == 0)
        {
            // Setting the items source as null displays the empty view template
            BindableLayout.SetItemsSource(LayoutRecordsGratitude, null);
        }
        else
        {
            BindableLayout.SetItemsSource(LayoutRecordsGratitude, _dailyRecords);
        }
    }
    
    private void PopulateAchievements()
    {
        if (CountOfAchievements() == 0)
        {
            // Setting the items source as null displays the empty view template
            BindableLayout.SetItemsSource(LayoutRecordsAchievements, null);
        }
        else
        {
            BindableLayout.SetItemsSource(LayoutRecordsAchievements, _dailyRecords);
        }
    }
    
    private void PopulateLookingForward()
    {
        if (CountOfLookingForward() == 0)
        {
            // Setting the items source as null displays the empty view template
            BindableLayout.SetItemsSource(LayoutRecordsLookingForward, null);
        }
        else
        {
            BindableLayout.SetItemsSource(LayoutRecordsLookingForward, _dailyRecords);
        }
    }

    private int CountOfGratitudes()
    {
        int itemCount = 0;
        foreach (var record in _dailyRecords)
        {
            itemCount += record.GratitudeList.Count;
        }

        return itemCount;
    }
    
    private int CountOfAchievements()
    {
        int itemCount = 0;
        foreach (var record in _dailyRecords)
        {
            itemCount += record.AchievementList.Count;
        }

        return itemCount;
    }
    
    private int CountOfLookingForward()
    {
        int itemCount = 0;
        foreach (var record in _dailyRecords)
        {
            itemCount += record.LookingForwardList.Count;
        }

        return itemCount;
    }

    #endregion
}