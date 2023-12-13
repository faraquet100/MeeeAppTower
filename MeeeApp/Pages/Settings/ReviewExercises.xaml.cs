using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeeeApp.Models;
using Syncfusion.Maui.Data;

namespace MeeeApp.Pages.Settings;

public partial class ReviewExercises : ContentPage
{
    private List<DailyRecord> _exerciseRecords = new List<DailyRecord>();
    
    public ReviewExercises()
    {
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LoadExercises();
        FormatList();
    }

    private void LoadExercises()
    {
        _exerciseRecords = User.UserFromPreferences().DailyRecords
            .Where(r => r.ExerciseAchieved.Length > 0 || r.ExerciseGrateful.Length > 0 ||
                        r.ExerciseLookingForward.Length > 0)
            .OrderByDescending(r => r.RecordDate)
            .ToList();
    }

    private void FormatList()
    {
        if (_exerciseRecords.Count > 0)
        {
            LvExercises.ItemsSource = _exerciseRecords;
            BordList.IsVisible = true;
            VsNoData.IsVisible = false;
        }
        else
        {
            BordList.IsVisible = false;
            VsNoData.IsVisible = true;
        }
    }
    
    
}