using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MeeeApp.Pages;

public partial class MeeeWebView : ContentPage
{
    private string _webUrl;
    public MeeeWebView(string webUrl)
    {
        InitializeComponent();
        _webUrl = webUrl;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        WebView.Source = _webUrl;
    }

    async void BtnClose_OnClicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}