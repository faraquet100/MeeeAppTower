namespace MeeeApp.Pages;

public partial class RegisterPage : ContentPage
{
	public RegisterPage()
	{
		InitializeComponent();
	}

    void BtnRegister_Clicked(System.Object sender, System.EventArgs e)
    {
    }

    void TapLogin_Tapped(System.Object sender, Microsoft.Maui.Controls.TappedEventArgs e)
    {
        Navigation.PopAsync();
    }
}
