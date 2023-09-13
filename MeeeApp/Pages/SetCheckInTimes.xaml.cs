using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeeeApp.Pages;

public partial class SetCheckInTimes : ContentPage
{
    public SetCheckInTimes()
    {
        InitializeComponent();
    }

    async void BarButtonSkip_OnClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync(true);
    }
}