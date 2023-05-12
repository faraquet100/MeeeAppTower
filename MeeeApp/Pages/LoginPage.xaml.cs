using MeeeApp.Services;

namespace MeeeApp.Pages;

public partial class LoginPage : ContentPage
{
	public LoginPage()
	{
        InitializeComponent();

	}

    async void BtnLogin_Clicked(System.Object sender, System.EventArgs e)
    {
        var email = TxtEmail.Text ?? "";
        var password = TxtPassword.Text ?? "";

        if (email.Length < 1 && password.Length < 1)
        {
            await DisplayAlert("Oops!", "Please complete your email and password before tapping Sign In.", "OK");
            return;
        }

        FormatForBusy(true);
        var result = await ApiService.LoginAsync(new Models.Login { Email = email, Password = password });
        FormatForBusy(false);

        Application.Current.MainPage = new MainTabbedPage();
    }

    private void FormatForBusy(bool busy)
    {
        MainThread.BeginInvokeOnMainThread(() =>
        {
            MyActivityIndicator.IsVisible = busy;
            TxtEmail.IsEnabled = !busy;
            TxtPassword.IsEnabled = !busy;
            BtnLogin.IsEnabled = !busy;
        });
    }

    async void TapRegister_Tapped(System.Object sender, Microsoft.Maui.Controls.TappedEventArgs e)
    {
        var page = new RegisterPage();
        await Navigation.PushAsync(page);
    }

}
