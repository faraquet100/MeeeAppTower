using System.Collections.ObjectModel;
using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Behaviors;
using MeeeApp.Services;

namespace MeeeApp.Pages;

public partial class JournalPage : ContentPage
{
    public ObservableCollection<ExerciseListContent> DsExerciseList = new ObservableCollection<ExerciseListContent>()
    {
        new ExerciseListContent { LabelText = "Daily Exercises (2/3)" }
    };

    private DateTime calendarDate = DateTime.Now;

    enum CalendarType
    {
        CheckIn,
        CheckOut
    }

	public JournalPage()
	{
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        
        base.OnAppearing();
        InitialDelayedSetup();

        LvExercises.ItemsSource = DsExerciseList;
        
    }

    // For some reason, the behaviours won't work if run immediately
    // So for the icon tint to work we need a very short delay
    async void InitialDelayedSetup()
    {
        await Task.Delay(50);

        var behaviour = new IconTintColorBehavior
        {
            TintColor = Microsoft.Maui.Graphics.Colors.White
        };

        MainThread.BeginInvokeOnMainThread(() =>
        {
            ImgPlan.Behaviors.Add(behaviour);
            ImgReflection.Behaviors.Add(behaviour);
        });

        BuildCalendar(CalendarType.CheckIn);
    }

    #region Actions

    void TapPlan_Tapped(System.Object sender, Microsoft.Maui.Controls.TappedEventArgs e)
    {
        FormatForPlan();
    }

    void TapReflection_Tapped(System.Object sender, Microsoft.Maui.Controls.TappedEventArgs e)
    {
        FormatForReflection();
    }

    void LvExercises_ItemSelected(System.Object sender, Microsoft.Maui.Controls.SelectedItemChangedEventArgs e)
    {
        LvExercises.SelectedItem = null;
    }

    void BtnCheckIn_Clicked(System.Object sender, System.EventArgs e)
    {
        /*
        Label label = new Label();
        label.Text = "Hello, I'm Dynamic!";
        label.FontAttributes = FontAttributes.Bold;

        VsCalendar.Children.Add(label);
        */
    }

    void BtnPreviousMonth_Clicked(System.Object sender, System.EventArgs e)
    {
        calendarDate = calendarDate.AddMonths(-1);
        BuildCalendar(CalendarType.CheckIn);
    }

    void BtnNextMonth_Clicked(System.Object sender, System.EventArgs e)
    {
        calendarDate = calendarDate.AddMonths(1);
        BuildCalendar(CalendarType.CheckIn);
    }

    #endregion

    #region Formatting

    private void FormatForPlan()
    {
        VsPlan.IsVisible = true;
        VsReflection.IsVisible = false;
        FramePlan.BackgroundColor = AppSettings.MeeeColorMagenta;
        FrameReflection.BackgroundColor = AppSettings.MeeeColorGrey500;

    }

    private void FormatForReflection()
    {
        VsPlan.IsVisible = false;
        VsReflection.IsVisible = true;
        FrameReflection.BackgroundColor = AppSettings.MeeeColorMagenta;
        FramePlan.BackgroundColor = AppSettings.MeeeColorGrey500;
    }

    #endregion

    #region Calendar

    private void BuildCalendar(CalendarType calendarType)
    {
        
        var monthTitle = calendarDate.ToString("MMMM yyyy");
        LblCalendarMonth.Text = monthTitle;

        var entries = GetCalendarEntries(calendarDate);
        var firstDay = new DateTime(calendarDate.Year, calendarDate.Month, 1);
        
        var buildDate = firstDay;
        var buildDayOfWeek = (int)firstDay.DayOfWeek;
        var currentRow = 1;
        var currentCol = 1;

        GridRow5.IsVisible = false;
        GridRow6.IsVisible = false;

        var todayBehaviour = new IconTintColorBehavior
        {
            TintColor = AppSettings.MeeeColorRed
        };

        var checkInBehaviour = new IconTintColorBehavior
        {
            TintColor = AppSettings.MeeeColorMagenta
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

                // Today?
                if (buildDate.Year == DateTime.Now.Year && buildDate.Month == DateTime.Now.Month && buildDate.Day == DateTime.Now.Day)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        imageControl.Behaviors.Add(todayBehaviour);
                    });
                    
                }

                // Check In?
                if (IsCheckedIn(entries, buildDate) != null)
                {
                    MainThread.BeginInvokeOnMainThread(() =>
                    {
                        imageControl.Behaviors.Add(checkInBehaviour);
                    });
                }

                buildDate = buildDate.AddDays(1);
                currentCol = currentCol + 1;
            }

            currentRow = currentRow + 1;
        }

    }

    private CalendarEntry IsCheckedIn(List<CalendarEntry> calendarEntries, DateTime date)
    {
        var entry = calendarEntries.Where(e => e.Date.Year == date.Year && e.Date.Month == date.Month && e.Date.Day == date.Day).FirstOrDefault();
        return entry;
    }

    // Converts the day of week to match our columns
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


    private List<CalendarEntry> GetCalendarEntries(DateTime date)
    {
        // Just a bunch of dummy entries for last two months

        return new List<CalendarEntry>()
        {
            new CalendarEntry { Date = DateTime.Now.AddDays(-2), CheckInScore = 5 },
            new CalendarEntry { Date = DateTime.Now.AddDays(-5), CheckInScore = 5 },
            new CalendarEntry { Date = DateTime.Now.AddDays(-6), CheckInScore = 5 },
            new CalendarEntry { Date = DateTime.Now.AddDays(-11), CheckInScore = 5 },
            new CalendarEntry { Date = DateTime.Now.AddDays(-16), CheckInScore = 5 },
            new CalendarEntry { Date = DateTime.Now.AddDays(-21), CheckInScore = 5 },
            new CalendarEntry { Date = DateTime.Now.AddDays(-30), CheckInScore = 5 },
            new CalendarEntry { Date = DateTime.Now.AddDays(-31), CheckInScore = 5 },
            new CalendarEntry { Date = DateTime.Now.AddDays(-32), CheckInScore = 5 },
            new CalendarEntry { Date = DateTime.Now.AddDays(-33), CheckInScore = 5 },
            new CalendarEntry { Date = DateTime.Now.AddDays(-36), CheckInScore = 5 },
            new CalendarEntry { Date = DateTime.Now.AddDays(-38), CheckInScore = 5 },
        };
    }

    #endregion



    public class ExerciseListContent
    {
        public string LabelText { get; set; }
    }

    public class CalendarEntry
    {
        public DateTime Date { get; set; }
        public int CheckInScore { get; set; }
    }

    
}
