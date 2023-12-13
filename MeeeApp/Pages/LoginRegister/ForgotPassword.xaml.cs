using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeeeApp.Controls;
using MeeeApp.Services;

namespace MeeeApp.Pages.LoginRegister;

public partial class ForgotPassword : ContentPage
{
    public ForgotPassword()
    {
        InitializeComponent();
    }

    async void ImgBtnSubmit_OnClicked(object sender, EventArgs e)
    {
        var control = sender as CobaltImageButton;
        var grid = control.Parent as CobaltGrid;
        await grid.BounceOnPressAsync();
        await Submit();
    }

    async void TapSubmit_OnTapped(object sender, TappedEventArgs e)
    {
        var grid = sender as CobaltGrid;
        await grid.BounceOnPressAsync();
        await Submit();
    }

    async Task Submit()
    {
        var email = TxtEmail.Text ?? "";
        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
        {
            await DisplayAlert("Oops!", "Please enter a valid email address before pressing submit", "OK");
            return;
        }
        
        FormatForBusy(true);
        var result = await ApiService.ForgotPassword(email);
        
        if (result == ApiService.ApiResult.BadRequest)
        {
            await DisplayAlert("Oops!", "There is current a problem with processing your request.  Please try again later.", "OK");
            return;
        }

        if (result == ApiService.ApiResult.NoInternet)
        {
            await DisplayAlert("Oops!", "You do not appear to be connected to the Internet, please check you network connections and try again.", "OK");
            return;
        }
        
        FormatForBusy(false);
        FormatForReturn(email);
    }
    
    private void FormatForBusy(bool busy)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            MyActivityIndicator.IsVisible = busy;
            TxtEmail.IsEnabled = !busy;
            ImgBtnSubmit.IsEnabled = !busy;
        });
    }

    private void FormatForReturn(string email)
    {
        LayoutEnter.IsVisible = false;
        LayoutReturn.IsVisible= true;
        LblResetLink.Text = "A password reset link has been sent to " + email +
                            ".  Please check your email and follow the instructions.";
    }

    async void ImgBtnReturn_OnClicked(object sender, EventArgs e)
    {
        var control = sender as CobaltImageButton;
        var grid = control.Parent as CobaltGrid;
        await grid.BounceOnPressAsync();
        await Navigation.PopAsync();
    }

    async void TapReturn_OnTapped(object sender, TappedEventArgs e)
    {
        var grid = sender as CobaltGrid;
        await grid.BounceOnPressAsync();
        await Navigation.PopAsync();
    }
}