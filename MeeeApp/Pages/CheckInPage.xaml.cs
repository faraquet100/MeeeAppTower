using MeeeApp.Extensions;
using MeeeApp.Models;

namespace MeeeApp.Pages;

public partial class CheckInPage : ContentPage
{
    private User _user;
    private int selectedScore = 5;

	public CheckInPage()
	{
        InitializeComponent();
        _user = User.UserFromPreferences();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();

        // Salutation
        switch (DateExtensions.GetPartOfDay())
        {
            case DateExtensions.PartOfDay.Morning:
                LblName.Text = "Good Morning " + _user.FirstName + "!";
                break;
            case DateExtensions.PartOfDay.Afternoon:
                LblName.Text = "Good Afternoon " + _user.FirstName + "!";
                break;
            case DateExtensions.PartOfDay.Evening:
                LblName.Text = "Good Evening " + _user.FirstName + "!";
                break;
        }
    }


    #region Actions


    async void TapCheckIn_Tapped(System.Object sender, Microsoft.Maui.Controls.TappedEventArgs e)
    {
        await Navigation.PopModalAsync(true);
    }

    void SliderCheckIn_ValueChanged(System.Object sender, Microsoft.Maui.Controls.ValueChangedEventArgs e)
    {
        selectedScore = (int)e.NewValue;
        var imageName = "face" + (selectedScore).ToString();

        MainThread.BeginInvokeOnMainThread(() =>
        {
            ImgFace.Source = imageName;
            LblFaceValue.Text = ((int)e.NewValue).ToString();
        });
    }

    async void TapContinue_Tapped(System.Object sender, Microsoft.Maui.Controls.TappedEventArgs e)
    {
        await GridContinue.BounceOnPressAsync();
        await Navigation.PushAsync(new CheckInPageReason(_user, selectedScore));
    }

    async void BarButtonCancel_Clicked(System.Object sender, System.EventArgs e)
    {
        await Navigation.PopModalAsync(true);
    }

    #endregion

    async void BtnSkip_OnClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync(true);
    }
}
