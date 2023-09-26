using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeeeApp.Controls;
using MeeeApp.Models;

namespace MeeeApp.Pages;

public partial class DailyMomentDetail : ContentPage
{
    private DailyMoment _dailyMoment;
    
    public DailyMomentDetail(DailyMoment dailyMoment)
    {
        InitializeComponent();
        _dailyMoment = dailyMoment;
        
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        LblMomentTitle.Text = _dailyMoment.Heading.ToLower();
        LblLinkText.Text = _dailyMoment.CallToActionText;
        LblCallToAction.Text = _dailyMoment.CallToActionText;
        ImgMomentImage.Source = _dailyMoment.FullImageUrl;
        LblQuote.Text = "\"" + _dailyMoment.QuoteText + "\"";
        LblQuoteAuthor.Text = _dailyMoment.QuoteAuthor;
        LblContent.Text = _dailyMoment.ContentWithHtml; // This loads but doesn't format correctly, is fixed is the delayed setup
        WebViewSource.Html = _dailyMoment.ContentWithHtml;

        if (_dailyMoment.LinkUrl.Length < 2)
        {
            GridLink.IsVisible = false;
            LblCallToAction.IsVisible = true;
        }
        else
        {
            GridLink.IsVisible = true;
            LblCallToAction.IsVisible = false;
        }
        
        if (!_dailyMoment.UseShareButton)
        {
            ImgBtnShare.IsVisible = false;
        }

        if (!_dailyMoment.UseHeartButton)
        {
            ImgBtnHeart.IsVisible = false;
            ImgBtnHeartPressed.IsVisible = false;
        }

        if (!_dailyMoment.UseLikeButton)
        {
            ImgBtnLike.IsVisible = false;
            ImgBtnLikePressed.IsVisible = false;
        }

    }

    /*
    async void DelayedSetup()
    {
        await Task.Delay(1000);
        
        MainThread.BeginInvokeOnMainThread(() =>
        {
            LblContent.Text = _dailyMoment.ContentWithHtml;
        });
    }
    */
    
    
    async void BtnSkip_OnClicked(object sender, EventArgs e)
    {
        await Navigation.PopModalAsync(true);
    }

    async void ImgBtnLink_OnClicked(object sender, EventArgs e)
    {
        var control = sender as CobaltImageButton;
        var grid = control.Parent as CobaltGrid;
        await grid.BounceOnPressAsync();
        OpenWebLink(_dailyMoment.LinkUrl);
    }

    async void TapLink_OnTapped(object sender, TappedEventArgs e)
    {
        var grid = sender as CobaltGrid;
        await grid.BounceOnPressAsync();
        OpenWebLink(_dailyMoment.LinkUrl);
    }
    
    async void OpenWebLink(string url)
    {
        await Navigation.PushAsync(new MeeeWebView(url));
    }

    private void ImgBtnLike_OnClicked(object sender, EventArgs e)
    {
        ImgBtnLike.IsVisible = false;
        ImgBtnLikePressed.IsVisible = true;
        // TODO: Needs Implementing
    }

    private void ImgBtnLikePressed_OnClicked(object sender, EventArgs e)
    {
        ImgBtnLike.IsVisible = true;
        ImgBtnLikePressed.IsVisible = false;
        // TODO: Needs Implementing
    }

    private void ImgBtnHeart_OnClicked(object sender, EventArgs e)
    {
        ImgBtnHeart.IsVisible = false;
        ImgBtnHeartPressed.IsVisible = true;
        // TODO: Needs Implementing
    }

    private void ImgBtnHeartPressed_OnClicked(object sender, EventArgs e)
    {
        ImgBtnHeart.IsVisible = true;
        ImgBtnHeartPressed.IsVisible = false;
        // TODO: Needs Implementing
    }

    async void ImgBtnShare_OnClicked(object sender, EventArgs e)
    {
        await Share.Default.RequestAsync(new ShareTextRequest
        {
            Uri = _dailyMoment.LinkUrl,
            Title = _dailyMoment.Heading,
            Text = "We can put some text here"
        });
    }
}