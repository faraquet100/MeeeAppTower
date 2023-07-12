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

    protected override void OnAppearing()
    {
        
        base.OnAppearing();
        FormatForPlan();
    }


    #endregion

    #region Returning from Check In / Check Out

    public void UpdateAfterCheckInOut()
    {
        LblDayTitle.Text = "HELLO I'VE UPDATED";
    }


    #endregion

    #region Actions

    /* Tab Buttons */

    void TapPlan_Tapped(System.Object sender, Microsoft.Maui.Controls.TappedEventArgs e)
    {
        FormatForPlan();
    }

    void TapReflection_Tapped(System.Object sender, Microsoft.Maui.Controls.TappedEventArgs e)
    {
        FormatForReflection();
    }

    async void GridPlanTap_Tapped(System.Object sender, Microsoft.Maui.Controls.TappedEventArgs e)
    {
        var grid = sender as CobaltGrid;
        await grid.BounceOnPressAsync();
        FormatForPlan();
    }

    async void GridReflectionTap_Tapped(System.Object sender, Microsoft.Maui.Controls.TappedEventArgs e)
    {
        var grid = sender as CobaltGrid;
        await grid.BounceOnPressAsync();
        FormatForReflection();
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
        await Navigation.PushModalAsync(new NavigationPage(new CheckInPage()));
    }

    async void TapCheckIn_Tapped(System.Object sender, Microsoft.Maui.Controls.TappedEventArgs e)
    {
        var grid = sender as CobaltGrid;
        await grid.BounceOnPressAsync();
        await Navigation.PushModalAsync(new NavigationPage(new CheckInPage()));
    }

    /* Check Out Button and Container */

    async void BtnCheckOut_Clicked(System.Object sender, System.EventArgs e)
    {
        var control = sender as CobaltImageButton;
        var grid = control.Parent as CobaltGrid;
        await grid.BounceOnPressAsync();
    }

    async void TapCheckOut_Tapped(System.Object sender, Microsoft.Maui.Controls.TappedEventArgs e)
    {
        var grid = sender as CobaltGrid;
        await grid.BounceOnPressAsync();
    }


    /* Calendar Controls */

    async void ImgBtnDayBack_Clicked(System.Object sender, System.EventArgs e)
    {
        var control = sender as CobaltImageButton;
        await control.BounceOnPressAsync();
    }

    async void ImgBtnDayForward_Clicked(System.Object sender, System.EventArgs e)
    {
        var control = sender as CobaltImageButton;
        await control.BounceOnPressAsync();
    }

    #endregion

    #region Formatting

    private void FormatForPlan()
    {
        //ImgDay.IsVisible = true;
        //ImgNight.IsVisible = false;

        ImgPlanSelection.Source = "sun_drawing.png";
        ImgReflectionSelection.Source = "moon_inactive.png";
        ImgPlanUnderline.IsVisible = true;
        ImgReflectionUnderline.IsVisible = false;
        
        //BorderPlanThoughts.IsVisible = true;
        GridCheckIn.IsVisible = true;
        GridCheckOut.IsVisible = false;
    }

    private void FormatForReflection()
    {
        //ImgDay.IsVisible = false;
        //ImgNight.IsVisible = true;

        ImgPlanSelection.Source = "sun_inactive.png";
        ImgReflectionSelection.Source = "moon.png";
        ImgPlanUnderline.IsVisible = false;
        ImgReflectionUnderline.IsVisible = true;

        GridCheckIn.IsVisible = false;
        GridCheckOut.IsVisible = true;
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
