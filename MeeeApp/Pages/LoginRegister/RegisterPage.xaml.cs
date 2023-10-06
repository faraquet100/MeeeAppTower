using System.Net.Mail;
using CommunityToolkit.Maui.Behaviors;
using MeeeApp.Controls;
using MeeeApp.Models;
using MeeeApp.Services;

namespace MeeeApp.Pages;

public partial class RegisterPage : ContentPage
{
	public RegisterPage()
	{
		InitializeComponent();
        AppSettings.CurrentPage = this;
        FormatForBusy(false);
    }

    // Return to the login page
    void TapLogin_Tapped(System.Object sender, Microsoft.Maui.Controls.TappedEventArgs e)
    {
        Navigation.PopAsync();
    }

    async void TapRegister_Tapped(System.Object sender, Microsoft.Maui.Controls.TappedEventArgs e)
    {
        var grid = sender as CobaltGrid;
        await grid.BounceOnPressAsync();
        await Register();
    }

    async void ImgBtnRegister_Clicked(System.Object sender, System.EventArgs e)
    {
        var control = sender as CobaltImageButton;
        var grid = control.Parent as CobaltGrid;
        await grid.BounceOnPressAsync();
        await Register();
    }

    void TxtPassword_TextChanged(System.Object sender, Microsoft.Maui.Controls.TextChangedEventArgs e)
    {
        ValidatePasswordEntries();
    }

    void TxtConfirmPassword_TextChanged(System.Object sender, Microsoft.Maui.Controls.TextChangedEventArgs e)
    {
        ValidatePasswordEntries();
    }

    private async Task Register()
    {
        if (!ValidateEntries())
        {
            await DisplayAlert("Oops!", "Please complete all fields before tapping the Sign Up button.", "OK");
            return;
        }

        if (TxtPassword.Text != TxtConfirmPassword.Text)
        {
            await DisplayAlert("Oops!", "Passwords don't match.", "OK");
            return;
        }

        if (!ValidateEmail())
        {
            await DisplayAlert("Oops!", "Please enter a valid email address.", "OK");
            return;
        }

        var deviceInfo = DeviceInfo.Current;
        string platform = deviceInfo.Platform.ToString() + " " + deviceInfo.Name;
        string version = deviceInfo.VersionString;

        FormatForBusy(true);
        var json = new Register
        {
            FirstName = TxtFirst.Text,
            LastName = TxtLast.Text,
            Email = TxtEmail.Text,
            Password = TxtPassword.Text,
            SourcePlatform = platform,
            SourceVersion = version
        };
        var result = await ApiService.RegisterAsync(json);
        FormatForBusy(false);

        if (result == ApiService.ApiResult.BadRequest)
        {
            await DisplayAlert("Oops!", "An account already exists for email " + TxtEmail.Text + ".", "OK");
            return;
        }

        if (result == ApiService.ApiResult.NoInternet)
        {
            await DisplayAlert("Oops!", "You do not appear to be connected to the Internet, please check you network connections and try again.", "OK");
            return;
        }

        Application.Current.MainPage = new MainTabbedPage();
    }

    private bool ValidateEntries()
    {
        string first = TxtFirst.Text ?? "";
        string last = TxtLast.Text ?? "";
        string email = TxtEmail.Text ?? "";
        string password = TxtPassword.Text ?? "";
        string confirm = TxtConfirmPassword.Text ?? "";

        return first.Length > 0 && last.Length > 0 && email.Length > 0 && password.Length > 0 && confirm.Length > 0;
    }

    private bool ValidateEmail()
    {
        var emailIsValid = false;
        try
        {
            var email = new MailAddress(TxtEmail.Text);
            emailIsValid = email.Address == TxtEmail.Text.Trim();
        }
        catch
        {
            emailIsValid = false;
        }

        return emailIsValid;
    }

    private void ValidatePasswordEntries()
    {
        var password = TxtPassword.Text ?? "";
        var confirm = TxtConfirmPassword.Text ?? "";

        ImgConfirmPasswordNotOkay.IsVisible = false;
        ImgConfirmPasswordOkay.IsVisible = false;
        ImgPasswordNotOkay.IsVisible = false;
        ImgPasswordOkay.IsVisible = false;

        if (password.Length > 0)
        {
            if (password.Length > 7)
            {
                ImgPasswordOkay.IsVisible = true;
            }
            else
            {
                ImgPasswordNotOkay.IsVisible = true;
            }
        }

        if (confirm.Length > 0)
        {
            if (confirm == password)
            {
                ImgConfirmPasswordOkay.IsVisible = true;
            }
            else
            {
                ImgConfirmPasswordNotOkay.IsVisible = true;
            }
        }
    }

    private void FormatForBusy(bool busy)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            MyActivityIndicator.IsVisible = busy;
            TxtFirst.IsEnabled = !busy;
            TxtLast.IsEnabled = !busy;
            TxtEmail.IsEnabled = !busy;
            TxtPassword.IsEnabled = !busy;
            TxtConfirmPassword.IsEnabled = !busy;
            ImgBtnRegister.IsEnabled = !busy;
        });
    }







}
