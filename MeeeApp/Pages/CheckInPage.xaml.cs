using MeeeApp.Models;

namespace MeeeApp.Pages;

public partial class CheckInPage : ContentPage
{
    private User _user;
    
	public CheckInPage()
	{
        InitializeComponent();
        _user = User.UserFromPreferences();
	}

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LblName.Text = "Morning " + _user.FirstName;
    }

    #region Actions

    async void BtnCancel_Clicked(System.Object sender, System.EventArgs e)
    {
        await Navigation.PopModalAsync(true);
    }

    async void TapCheckIn_Tapped(System.Object sender, Microsoft.Maui.Controls.TappedEventArgs e)
    {
        await Navigation.PopModalAsync(true);
    }

    void SliderCheckIn_ValueChanged(System.Object sender, Microsoft.Maui.Controls.ValueChangedEventArgs e)
    {
        
        var imageName = "face" + ((int)e.NewValue).ToString();

        MainThread.BeginInvokeOnMainThread(() =>
        {
            ImgFace.Source = imageName;
            LblFaceValue.Text = ((int)e.NewValue).ToString();
        });
    }
    #endregion
}
