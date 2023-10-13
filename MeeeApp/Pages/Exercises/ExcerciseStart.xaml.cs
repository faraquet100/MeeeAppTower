using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeeeApp.Controls;
using MeeeApp.Models;
using MeeeApp.Pages.Scrap;
using MeeeApp.Services;
using Syncfusion.Maui.Buttons;

namespace MeeeApp.Pages.Exercises;

public partial class ExcerciseStart : ContentPage
{
    private DateTime _calendarDate;
    private DailyRecord _dailyRecord;
    
    public ExcerciseStart(DateTime calendarDate)
    {
        InitializeComponent();
        _calendarDate = calendarDate;
        _dailyRecord = User.UserFromPreferences().DailyRecordForDate(calendarDate);
        
        
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        MyActivityIndicator.IsVisible = false;
        
        PopulateGratitudes();
        PopulateAchievements();
        PopulateLookingForward();
    }

    #region Navigation Buttons
    async void TapSubmit_OnTapped(object sender, TappedEventArgs e)
    {
        await GridSubmit.BounceOnPressAsync();
        MyActivityIndicator.IsVisible = true;
        await ApiService.SaveJournalAndExercise(_dailyRecord); 
        await Navigation.PushAsync(new ExerciseSummary(_calendarDate));
        MyActivityIndicator.IsVisible = false;
    }

    async void Button_OnClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync(true);
    }
    
    #endregion

    #region Add Item Buttons
  
    async void BtnAddGratitude_OnClicked(object sender, EventArgs e)
    { 
        var result = await DisplayPromptAsync("I am grateful for . . .", message: null, initialValue: "", keyboard: Keyboard.Text);
        if (result != null && result.Length > 0)
        {
            _dailyRecord.AddGratitude(result);
            PopulateGratitudes();
        }
    }
    
    async void BtnAddAchievement_OnClicked(object sender, EventArgs e)
    {
        var result = await DisplayPromptAsync("I have achieved . . .", message: null, initialValue: "", keyboard: Keyboard.Text);
        if (result != null && result.Length > 0)
        {
            _dailyRecord.AddAchievement(result);
            PopulateAchievements();
        }
    }
    
    async void BtnLookingForward_OnClicked(object sender, EventArgs e)
    {
        var result = await DisplayPromptAsync("I am looking forward to . . .", message: null, initialValue: "", keyboard: Keyboard.Text);
        if (result != null && result.Length > 0)
        {
            _dailyRecord.AddLookingForward(result);
            PopulateLookingForward();
        }
    }
    
    #endregion

    #region Populate Items

    private void PopulateGratitudes()
    {
        // Using a binding context with an observable collection, crashed the app, is stable without it
        BindableLayout.SetItemsSource(LayoutGratitudes, _dailyRecord.ExerciseGratefulList());
    }
    
    private void PopulateAchievements()
    {
        // Using a binding context with an observable collection, crashed the app, is stable without it
        BindableLayout.SetItemsSource(LayoutAchievements, _dailyRecord.ExerciseAchievementList());
    }
    
    private void PopulateLookingForward()
    {
        // Using a binding context with an observable collection, crashed the app, is stable without it
        BindableLayout.SetItemsSource(LayoutLookingForward, _dailyRecord.ExerciseLookingFowardList());
    }

    #endregion
    
    #region Items Edit

    async void GratefulButton_OnClicked(object sender, EventArgs e)
    {
        var button = (SfButton) sender;
        var oldWord = button.Text;
        string result = await DisplayPromptAsync("I am grateful for . . .", message: null, initialValue: oldWord, keyboard: Keyboard.Text);
        if (result != null && result.Length > 0)
        {
            _dailyRecord.EditGratitude(oldWord, result);
            PopulateGratitudes();
        }
    }
    
    async void AchievementButton_OnClicked(object sender, EventArgs e)
    {
        var button = (SfButton) sender;
        var oldWord = button.Text;
        string result = await DisplayPromptAsync("I have achieved . . .", message: null, initialValue: oldWord, keyboard: Keyboard.Text);
        if (result != null && result.Length > 0)
        {
            _dailyRecord.EditAchievement(oldWord, result);
            PopulateAchievements();
        }
    }
    
    async void LookingForwardButton_OnClicked(object sender, EventArgs e)
    {
        var button = (SfButton) sender;
        var oldWord = button.Text;
        string result = await DisplayPromptAsync("I am looking forward to . . .", message: null, initialValue: oldWord, keyboard: Keyboard.Text);
        if (result != null && result.Length > 0)
        {
            _dailyRecord.EditLookingForward(oldWord, result);
            PopulateLookingForward();
        }
    }
    
    #endregion

    #region Delete Items

    async void BtnDeleteGratitude_OnClicked(object sender, EventArgs e)
    {
        var result = await DisplayAlert("Delete", "Are you sure you want to delete this item?", "Yes", "No");
        if (result)
        {
            var button = (CobaltImageButton)sender;
            var oldWord = button.Tag;
            _dailyRecord.RemoveGratitude(oldWord);
            PopulateGratitudes();
        }
    }
    
    async void BtnDeleteAchievement_OnClicked(object sender, EventArgs e)
    {
        var result = await DisplayAlert("Delete", "Are you sure you want to delete this item?", "Yes", "No");
        if (result)
        {
            var button = (CobaltImageButton)sender;
            var oldWord = button.Tag;
            _dailyRecord.RemoveAchievement(oldWord);
            PopulateAchievements();
        }
    }

    async void BtnDeleteLookingForward_OnClicked(object sender, EventArgs e)
    {
        var result = await DisplayAlert("Delete", "Are you sure you want to delete this item?", "Yes", "No");
        if (result)
        {
            var button = (CobaltImageButton)sender;
            var oldWord = button.Tag;
            _dailyRecord.RemoveLookingForward(oldWord);
            PopulateLookingForward();
        }
    }

    #endregion

    
}