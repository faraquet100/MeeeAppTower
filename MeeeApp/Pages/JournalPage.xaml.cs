using CommunityToolkit.Maui;
using CommunityToolkit.Maui.Behaviors;
using MeeeApp.Services;

namespace MeeeApp.Pages;

public partial class JournalPage : ContentPage
{

	public JournalPage()
	{
        InitializeComponent();
    }

    protected override void OnAppearing()
    {
        
        base.OnAppearing();
        InitialDelayedSetup();

    }

    // For some reason, the behaviours won't work if run immediately
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
    }

    void TapPlan_Tapped(System.Object sender, Microsoft.Maui.Controls.TappedEventArgs e)
    {
        FormatForPlan();
    }

    void TapReflection_Tapped(System.Object sender, Microsoft.Maui.Controls.TappedEventArgs e)
    {
        FormatForReflection();
    }

    private void FormatForPlan()
    {
        FramePlan.BackgroundColor = AppSettings.MeeeColorMagenta;
        FrameReflection.BackgroundColor = AppSettings.MeeeColorGrey500;
    }

    private void FormatForReflection()
    {
        FrameReflection.BackgroundColor = AppSettings.MeeeColorMagenta;
        FramePlan.BackgroundColor = AppSettings.MeeeColorGrey500;
    }
}
