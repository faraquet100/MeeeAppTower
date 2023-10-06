using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeeeApp.Models;
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
        
        PopulateGratitudes();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        //BindableLayout.SetItemsSource(LayoutGratitudes, gratitudes);
    }

    async void TapSubmit_OnTapped(object sender, TappedEventArgs e)
    {
        await GridSubmit.BounceOnPressAsync();
        //await Navigation.PushAsync(new ExcerciseStart());
    }

    async void Button_OnClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync(true);
    }
    
    async void BtnAddGratitude_OnClicked(object sender, EventArgs e)
    { 
        var result = await DisplayPromptAsync("I am grateful for . . .", message: null, initialValue: "", keyboard: Keyboard.Text);
        if (result != null && result.Length > 0)
        {
            _dailyRecord.AddGratitude(result);
            PopulateGratitudes();
        }
    }
    
    private void PopulateGratitudes()
    {
        // Using a binding context with an observable collection, crashed the app, is stable without it
        BindableLayout.SetItemsSource(LayoutGratitudes, _dailyRecord.ExerciseGratefulList());
    }

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
}