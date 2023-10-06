using System.Collections.ObjectModel;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Behaviors;
using MeeeApp.Controls;
using MeeeApp.Models;
using MeeeApp.Services;
using Microsoft.Maui.Controls.Shapes;

namespace MeeeApp.Pages;

public partial class JournalPage : ContentPage
{
    private DateTime calendarDate = DateTime.Now;
    private User _user;
    private bool planIsActive = true;   // If false then reflection is active

    enum CalendarType
    {
        CheckIn,
        CheckOut
    }

	public JournalPage()
	{
        _user = User.UserFromPreferences();
        InitializeComponent();
        AppSettings.JournalPage = this;
    }

    #region StartUp

    // OnAppearing is triggered when the tab is selected
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        _user = User.UserFromPreferences(); // Reload the user on each view
        
        FormatForPlan();
        Console.WriteLine("JournalPage.OnAppearing()");
        UpdateAfterCheckInOut();    
        
        // We need to use this pattern to ensure the page has fully loaded before loading any modals
        await Task.Run(async () =>
        {
            await Task.Delay(1000);

            if (ShouldRequestCheckInTimes())
            {
                Dispatcher.Dispatch(() =>
                {
                    AppSettings.LastCheckInTimesAlert = DateTime.Now;
                    Navigation.PushModalAsync(new NavigationPage(new SetCheckInTimes()));
                });
            }
        });
    }
    
    private bool ShouldRequestCheckInTimes()
    {
        // Does the user record already have checkin times?
        var checkInTime = User.CheckInTimeFromPreferences();
        var checkOutTime = User.CheckOutTimeFromPreferences();
        
        if (checkInTime.Year > 2022)
        {
            return false;
        }
        
        // If the user has never been asked, ask them
        if (AppSettings.LastCheckInTimesAlert == null)
        {
            return true;
        }

        // If the user has been asked, but it's been more than 1 days, ask them again
        if (AppSettings.LastCheckInTimesAlert.Value.AddDays(1) < DateTime.Now)
        {
            return true;
        }

        return false;
    }


    #endregion

    #region Returning from Check In / Check Out

    public void UpdateAfterCheckInOut()
    {
        if (planIsActive)
        {
            FormatForPlan();
        }
        else
        {
            FormatForReflection();
        }
    }


    #endregion

    #region Actions

    /* Tab Buttons */

    void TapPlan_Tapped(System.Object sender, Microsoft.Maui.Controls.TappedEventArgs e)
    {
        planIsActive = true;
        MeMomentVideo.Play();
        FormatForPlan();
    }

    void TapReflection_Tapped(System.Object sender, Microsoft.Maui.Controls.TappedEventArgs e)
    {
        planIsActive = false;
        MeMomentVideo.Play();
        FormatForReflection();
    }

    async void GridPlanTap_Tapped(System.Object sender, Microsoft.Maui.Controls.TappedEventArgs e)
    {
        var grid = sender as CobaltGrid;
        await grid.BounceOnPressAsync();
        planIsActive = true;
        PlayIntroVideo();
        FormatForPlan();
    }

    async void GridReflectionTap_Tapped(System.Object sender, Microsoft.Maui.Controls.TappedEventArgs e)
    {
        var grid = sender as CobaltGrid;
        await grid.BounceOnPressAsync();
        planIsActive = false;
        MeMomentVideo.Play();
        FormatForReflection();
        PlayIntroVideo();
    }
    
    private void PlayIntroVideo()
    {
        MeMomentVideo.Source = "";
        MeMomentVideo.Source = "example_animation.m4v";
        MeMomentVideo.Play();
    }

    /* Daily Exercises and Container */

    async void ImgBtnDailyExcercises_Clicked(System.Object sender, System.EventArgs e)
    {
        var control = sender as CobaltImageButton;
        var grid = control.Parent as CobaltGrid;
        await grid.BounceOnPressAsync();
        await Navigation.PushAsync(new ExercisesPage());
    }

    async void TapDailyExercises_Tapped(System.Object sender, Microsoft.Maui.Controls.TappedEventArgs e)
    {
        var grid = sender as CobaltGrid;
        await grid.BounceOnPressAsync();
        await Navigation.PushAsync(new ExercisesPage());
    }

    /* Check In Button and Container */

    async void BtnCheckIn_Clicked(System.Object sender, System.EventArgs e)
    {
        var control = sender as CobaltImageButton;
        var grid = control.Parent as CobaltGrid;
        await grid.BounceOnPressAsync();
        await Navigation.PushModalAsync(new NavigationPage(new CheckInPage(CheckInPage.CheckingDirection.In, calendarDate)));
    }

    async void TapCheckIn_Tapped(System.Object sender, Microsoft.Maui.Controls.TappedEventArgs e)
    {
        var grid = sender as CobaltGrid;
        await grid.BounceOnPressAsync();
        await Navigation.PushModalAsync(new NavigationPage(new CheckInPage(CheckInPage.CheckingDirection.In, calendarDate)));
    }

    /* Check Out Button and Container */

    async void BtnCheckOut_Clicked(System.Object sender, System.EventArgs e)
    {
        var control = sender as CobaltImageButton;
        var grid = control.Parent as CobaltGrid;
        await grid.BounceOnPressAsync();
        await Navigation.PushModalAsync(new NavigationPage(new CheckInPage(CheckInPage.CheckingDirection.Out, calendarDate)));
    }

    async void TapCheckOut_Tapped(System.Object sender, Microsoft.Maui.Controls.TappedEventArgs e)
    {
        var grid = sender as CobaltGrid;
        await grid.BounceOnPressAsync();
        await Navigation.PushModalAsync(new NavigationPage(new CheckInPage(CheckInPage.CheckingDirection.Out, calendarDate)));
    }


    /* Calendar Controls */

    async void ImgBtnDayBack_Clicked(System.Object sender, System.EventArgs e)
    {
        var control = sender as CobaltImageButton;
        await control.BounceOnPressAsync();
        calendarDate = calendarDate.AddDays(-1);

        if (planIsActive)
        {
            FormatForPlan();
        }
        else
        {
            FormatForReflection();
        }
    }

    async void ImgBtnDayForward_Clicked(System.Object sender, System.EventArgs e)
    {
        var control = sender as CobaltImageButton;
        await control.BounceOnPressAsync();
        
        if (calendarDate.Year == DateTime.Now.Year && calendarDate.Month == DateTime.Now.Month && calendarDate.Day == DateTime.Now.Day)
        {
            return;
        }
        
        calendarDate = calendarDate.AddDays(1);

        if (planIsActive)
        {
            FormatForPlan();
        }
        else
        {
            FormatForReflection();
        }
    }

    #endregion

    #region Formatting

    // Update last Check In Label and set the day title
    private void UpdateLastCheckInLabel()
    {
        if (planIsActive)
        {
            LblLastCheckedIn.Text = _user.LastCheckedInLabelText();
        }
        else
        {
            LblLastCheckedIn.Text = _user.LastCheckedOutLabelText();
        }

        LblDayTitle.Text = _user.DayTitle(calendarDate);
    }
    
    private void FormatForPlan()
    {
        UpdateLastCheckInLabel();

        ImgPlanSelection.Source = "sun_drawing.png";
        ImgReflectionSelection.Source = "moon_inactive.png";
        ImgPlanUnderline.IsVisible = true;
        ImgReflectionUnderline.IsVisible = false;
        
        //BorderPlanThoughts.IsVisible = true;
        GridCheckIn.IsVisible = true;
        GridCheckOut.IsVisible = false;
        GridHasCheckedIn.IsVisible = false;
        LblCheckedInReason.IsVisible = false;
        GridHasCheckedOut.IsVisible = false;
        LblCheckedOutReason.IsVisible = false;
        LblCheckedOutReasonTitle.IsVisible = false;
        LblCheckedInReasonTitle.IsVisible = false;
        
        // Have they checked-in
        DailyRecord dailyRecord = _user.DailyRecordForDate(calendarDate);
        if (dailyRecord != null)
        {
            if (dailyRecord.CheckInTime.Year > 2022)
            {
                GridHasCheckedIn.IsVisible = true;
               
                GridCheckIn.IsVisible = false;
                LblCheckInTime.Text = dailyRecord.CheckInTime.ToString("hh:mmtt").ToLower() + " : " + dailyRecord.CheckInScore.ToString();

                LblCheckedInReasonTitle.Text = "I Checked-In at a " + dailyRecord.CheckInScore.ToString() + " because:";
                LblCheckedInReason.Text = dailyRecord.CheckInReason;

                if (dailyRecord.CheckInReason.Length > 0)
                {
                    LblCheckedInReason.IsVisible = true;
                    LblCheckedInReasonTitle.IsVisible = true;
                }
                else
                {
                    LblCheckedInReason.IsVisible = false;
                    LblCheckedInReasonTitle.IsVisible = false;
                    LblCheckedInReason.Text = "";
                }
            }
        }
    }

    private void FormatForReflection()
    {
        UpdateLastCheckInLabel();

        ImgPlanSelection.Source = "sun_inactive.png";
        ImgReflectionSelection.Source = "moon.png";
        ImgPlanUnderline.IsVisible = false;
        ImgReflectionUnderline.IsVisible = true;

        GridCheckIn.IsVisible = false;
        GridCheckOut.IsVisible = true;
        GridHasCheckedIn.IsVisible = false;
        LblCheckedInReason.IsVisible = false;
        GridHasCheckedOut.IsVisible = false;
        LblCheckedOutReason.IsVisible = false;
        LblCheckedOutReasonTitle.IsVisible = false;
        LblCheckedInReasonTitle.IsVisible = false;
        
        // Have they checked-out
        DailyRecord dailyRecord = _user.DailyRecordForDate(calendarDate);
        if (dailyRecord != null)
        {
            if (dailyRecord.CheckOutTime.Year > 2022)
            {
                GridHasCheckedOut.IsVisible = true;
               
                GridCheckOut.IsVisible = false;
                LblCheckOutTime.Text = dailyRecord.CheckOutTime.ToString("hh:mmtt").ToLower() + " : " + dailyRecord.CheckOutScore.ToString();
                
                LblCheckedOutReasonTitle.Text = "I Checked-Out at a " + dailyRecord.CheckOutScore.ToString() + " because:";
                LblCheckedOutReason.Text = dailyRecord.CheckOutReason;

                if (dailyRecord.CheckOutReason.Length > 0)
                {
                    LblCheckedOutReason.IsVisible = true;
                    LblCheckedOutReasonTitle.IsVisible = true;
                }
                else
                {
                    LblCheckedOutReason.IsVisible = false;
                    LblCheckedOutReasonTitle.IsVisible = false;
                    LblCheckedOutReason.Text = "";
                }
            }
        }
    }

    #endregion

    #region Calendar

    // We only need to build this once, since it's just for the last 14 days
    /*
    private void BuildChart(CalendarType calendarType)
    {
        var entries = GetCalendarEntries(calendarType, DateTime.Now);
        int BOTTOM_ROW = 9;

        var startDate = DateTime.Now.AddDays(-15);
        for (int x = 1; x < 15; x++)
        {
            var currentDate = startDate.AddDays(x);
            var entry = CalendarEntryForDate(entries, currentDate);
            if (entry != null)
            {
                var cell = new BoxView
                {
                    Color = AppSettings.MeeeColorCyan,
                    CornerRadius = 8,
                    Margin = new Thickness(2, 0)
                };

                GridChart.Add(cell, x, BOTTOM_ROW - entry.CheckInScore);
                GridChart.SetRowSpan(cell, entry.CheckInScore);
            }
        }

    }
    */

    /*
    private void BuildCalendar(CalendarType calendarType)
    {
        
        var monthTitle = calendarDate.ToString("MMMM yyyy");
        LblCalendarMonth.Text = monthTitle;

        var entries = GetCalendarEntries(calendarType, calendarDate);
        var firstDay = new DateTime(calendarDate.Year, calendarDate.Month, 1);
        
        var buildDate = firstDay;
        var buildDayOfWeek = (int)firstDay.DayOfWeek;
        var currentRow = 1;
        var currentCol = 1;

        GridRow5.IsVisible = false;
        GridRow6.IsVisible = false;


        var normalBehaviour = new IconTintColorBehavior
        {
            TintColor = AppSettings.MeeeColorGrey300
        };

        var todayBehaviour = new IconTintColorBehavior
        {
            TintColor = AppSettings.MeeeColorRed
        };

        var checkInBehaviour = new IconTintColorBehavior
        {
            TintColor = AppSettings.MeeeColorMagenta
        };

        var checkOutBehaviour = new IconTintColorBehavior
        {
            TintColor = AppSettings.MeeeColorCyan
        };

        HideAllDayImages();

        while (buildDate < firstDay.AddMonths(1))
        {
            var currentDow = ConvertDayOfWeek(buildDate.DayOfWeek);
            var gridRowName = "GridRow" + currentRow;
            if (currentRow > 4)
            {
                ((Grid)FindByName(gridRowName)).IsVisible = true;
            }

            currentCol = ConvertDayOfWeek(buildDate.DayOfWeek);
            while (currentCol < 8 && buildDate < firstDay.AddMonths(1))
            {
                var dayOfMonth = buildDate.Day;
                var dayNumberImage = "sf_" + dayOfMonth + "_circle.png";
                var imageControlName = "ImgR" + currentRow + "C" + currentCol;
                var imageControl = (Image)FindByName(imageControlName);
                imageControl.Source = dayNumberImage;
                imageControl.IsVisible = true;

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    imageControl.Behaviors.Add(normalBehaviour);
                });


                // Today?
                if (buildDate.Year == DateTime.Now.Year && buildDate.Month == DateTime.Now.Month && buildDate.Day == DateTime.Now.Day)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        imageControl.Behaviors.Add(todayBehaviour);
                    });
                    
                }

                // Check In?
                if (calendarType == CalendarType.CheckIn &&  IsCheckedIn(entries, buildDate) != null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        imageControl.Behaviors.Add(checkInBehaviour);
                    });
                }

                // Check Out?
                if (calendarType == CalendarType.CheckOut && IsCheckedIn(entries, buildDate) != null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        imageControl.Behaviors.Add(checkOutBehaviour);
                    });
                }

                buildDate = buildDate.AddDays(1);
                currentCol = currentCol + 1;
            }

            currentRow = currentRow + 1;
        }

    }
    */

    /*
    private CalendarEntry IsCheckedIn(List<CalendarEntry> calendarEntries, DateTime date)
    {
        var entry = calendarEntries.Where(e => e.Date.Year == date.Year && e.Date.Month == date.Month && e.Date.Day == date.Day).FirstOrDefault();
        return entry;
    }
    */

    // Converts the day of week to match our columns
    /*
    private int ConvertDayOfWeek(DayOfWeek dayOfWeek)
    {
        if (dayOfWeek == DayOfWeek.Sunday)
        {
            return 7;
        }
        else
        {
            return (int)dayOfWeek;
        }
    }
    */

    /*
    private void HideAllDayImages()
    {
        for (int row = 1; row < 7; row++)
        {
            for (int col = 1; col < 8; col++)
            {
                string imgName = "ImgR" + row + "C" + col;
                ((Image)FindByName(imgName)).IsVisible = false;
            }
        }
    }
    */

    /*
    private void LoadCalendarEntries()
    {
        var startDate = new DateTime(2023, 6, 1);
        CalendarEntries = new List<CalendarEntry>()
            {
                new CalendarEntry { Date = startDate, CheckInScore = 5 },
                new CalendarEntry { Date = startDate.AddDays(1), CheckInScore = 5 },
                new CalendarEntry { Date = startDate.AddDays(2), CheckInScore = 4 },
                new CalendarEntry { Date = startDate.AddDays(3), CheckInScore = 8 },
                new CalendarEntry { Date = startDate.AddDays(4), CheckInScore = 9 },
                new CalendarEntry { Date = startDate.AddDays(5), CheckInScore = 2 },
                new CalendarEntry { Date = startDate.AddDays(6), CheckInScore = 1 },
                new CalendarEntry { Date = startDate.AddDays(7), CheckInScore = 3 },
                //new CalendarEntry { Date = startDate.AddDays(8), CheckInScore = 5 },
                //new CalendarEntry { Date = startDate.AddDays(9), CheckInScore = 5 },
                new CalendarEntry { Date = startDate.AddDays(10), CheckInScore = 7 },
                new CalendarEntry { Date = startDate.AddDays(11), CheckInScore = 5 },
                new CalendarEntry { Date = startDate.AddDays(12), CheckInScore = 8 },
                new CalendarEntry { Date = startDate.AddDays(13), CheckInScore = 8 },
                new CalendarEntry { Date = startDate.AddDays(14), CheckInScore = 6 },
                new CalendarEntry { Date = startDate.AddDays(15), CheckInScore = 5 },
                new CalendarEntry { Date = startDate.AddDays(16), CheckInScore = 2 },
                new CalendarEntry { Date = startDate.AddDays(17), CheckInScore = 4 },
                new CalendarEntry { Date = startDate.AddDays(18), CheckInScore = 5 },
                new CalendarEntry { Date = startDate.AddDays(19), CheckInScore = 5 },
                new CalendarEntry { Date = startDate.AddDays(20), CheckInScore = 5 },
                new CalendarEntry { Date = startDate.AddDays(21), CheckInScore = 5 },
                new CalendarEntry { Date = startDate.AddDays(22), CheckInScore = 5 }
            };
    }
    */

    /*
    private List<CalendarEntry> GetCalendarEntries(CalendarType calendarType, DateTime startDate)
    {
        return CalendarEntries;
    }
    */

    /*
    private CalendarEntry CalendarEntryForDate(List<CalendarEntry> entries, DateTime date)
    {
        foreach (var entry in entries)
        {
            if (entry.Date.Year == date.Year && entry.Date.Month == date.Month && entry.Date.Day == date.Day)
            {
                return entry;
            }
        }

        return null;
    }
    */

    #endregion

    /*
    public class CalendarEntry
    {
        public DateTime Date { get; set; }
        public int CheckInScore { get; set; }
    }
    */
    


}
