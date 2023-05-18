using System.Collections.ObjectModel;

namespace MeeeApp.Pages;

public partial class ExercisesPage : ContentPage
{
	public ObservableCollection<Exercise> DsExerciseList = new ObservableCollection<Exercise>()
	{
		new Exercise
		{
			Title = "Bag of Crap",
			ImageName = "sun.png",
			Subtitle = "Life is really all about a series of choices around what we do and how we do it. What we make stuff mean and what we believe we can do and are capable of doing.",
			IsComplete = true,
			LastSession = "Today",
			TimeToComplete = "12 mins"
		},
		new Exercise
		{
			Title = "My Life Chart",
			ImageName = "sun.png",
			Subtitle = "Lorem ipsum dolor sit amet, consectetur adipiscing elit. Vestibulum finibus, nunc id commodo pulvinar, risus libero vestibulum ex, fermentum tincidunt mi leo faucibus quam.",
			IsComplete = false,
			LastSession = "14 Apr 23",
			TimeToComplete = "5 mins"
        },
		new Exercise
		{
			Title = "My Grateful List",
			ImageName = "sun.png",
			Subtitle = "Being grateful has many benefits - Helps put you in a better mood; Makes you more resilient; You’ll be better off (law of attraction).",
			IsComplete = true,
			LastSession = "Today",
			TimeToComplete = "10 mins"
        }
    };


    public ExercisesPage()
	{
		InitializeComponent();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
		LvExerciseList.ItemsSource = DsExerciseList;
    }


    public class Exercise
	{
		public string Title { get; set; }
		public string ImageName { get; set; }
		public string Subtitle { get; set; }
		public bool IsComplete { get; set; }
		public string LastSession { get; set; }
		public string TimeToComplete { get; set; }

	}

    #region Actions

    void LvExerciseList_ItemSelected(System.Object sender, Microsoft.Maui.Controls.SelectedItemChangedEventArgs e)
    {
		if (e.SelectedItem != null)
		{
			LvExerciseList.SelectedItem = null;
		}
    }

    #endregion
}
