using MeeeApp.Services;
using MeeeApp.Controls;
using CommunityToolkit.Maui.Behaviors;
using MeeeApp.Pages.LoginRegister;

namespace MeeeApp.Pages;

public partial class LoginPage : ContentPage
{
    private bool _showPassword = false;
    
	public LoginPage()
	{
        InitializeComponent();
        AppSettings.CurrentPage = this;
    }
    
    private void FormatForBusy(bool busy)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            MyActivityIndicator.IsVisible = busy;
            TxtEmail.IsEnabled = !busy;
            TxtPassword.IsEnabled = !busy;
            ImgBtnLogin.IsEnabled = !busy;
        });
    }

    // ImgBtn and TapSignIn work as a pair depending where the user clicks on the button
    async void ImgBtnLogin_Clicked(System.Object sender, System.EventArgs e)
    {
        var control = sender as CobaltImageButton;
        var grid = control.Parent as CobaltGrid;
        await grid.BounceOnPressAsync();
        await Login();
    }

    async void TapSignIn_Tapped(System.Object sender, Microsoft.Maui.Controls.TappedEventArgs e)
    {
        var grid = sender as CobaltGrid;
        await grid.BounceOnPressAsync();
        await Login();
    }

    async void BtnForgot_Clicked(System.Object sender, System.EventArgs e)
    {
        var control = sender as CobaltButton;
        await control.BounceOnPressAsync();
        await Navigation.PushAsync(new ForgotPassword());
    }

    async void BtnRegister_Clicked(System.Object sender, System.EventArgs e)
    {
        var control = sender as CobaltButton;
        await control.BounceOnPressAsync();
        await Register();
    }

    private async Task Login()
    {
        var email = TxtEmail.Text ?? "";
        var password = TxtPassword.Text ?? "";

        if (email.Length < 1 || password.Length < 1)
        {
            await DisplayAlert("Oops!", "Please complete your email and password before tapping Sign In.", "OK");
            return;
        }

        FormatForBusy(true);
        var result = await ApiService.LoginAsync(new Models.Login { Email = email, Password = password });
        FormatForBusy(false);

        if (result == ApiService.ApiResult.BadRequest)
        {
            await DisplayAlert("Oops!", "We could not log you in. Please check your email and password and try again.", "OK");
            return;
        }

        if (result == ApiService.ApiResult.NoInternet)
        {
            await DisplayAlert("Oops!", "You do not appear to be connected to the Internet, please check you network connections and try again.", "OK");
            return;
        }

        Application.Current.MainPage = new MainTabbedPage();

    }

    private async Task Register()
    {
        var page = new RegisterPage();
        await Navigation.PushAsync(page);
    }


    private void TxtEmail_OnFocused(object sender, FocusEventArgs e)
    {
        ImgEmailHighlight.IsVisible = true;
        ImgPasswordHighlight.IsVisible = false;
    }

    private void TxtPassword_OnFocused(object sender, FocusEventArgs e)
    {
        ImgEmailHighlight.IsVisible = false;
        ImgPasswordHighlight.IsVisible = true;
    }

    private void ImgBtnEye_OnClicked(object sender, EventArgs e)
    {
        if (!_showPassword)
        {
            TxtPassword.IsPassword = false;
            ImgBtnEye.Source = "eyeoff.png";
            _showPassword = true;
        }
        else
        {
            TxtPassword.IsPassword = true;
            ImgBtnEye.Source = "eye.png";
            _showPassword = false;
        }
    }
}
