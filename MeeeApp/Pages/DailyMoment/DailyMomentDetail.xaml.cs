using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeeeApp.Controls;
using MeeeApp.Models;
using MeeeApp.Services;
using Microsoft.Maui.Platform;
using KeyboardExtensions = CommunityToolkit.Maui.Core.Platform.KeyboardExtensions;

namespace MeeeApp.Pages;

public partial class DailyMomentDetail : ContentPage
{
    private DailyMoment _dailyMoment;
    private OnThisDay _onThisDay;
    private DailyRecord _dailyRecord;
    private DateTime _calendarDate;
    
    // For review
    private bool _isReview = false;
    private int _currentDailyMomentIndex = 0;
    
    public DailyMomentDetail(DailyMoment dailyMoment, OnThisDay onThisDay, DateTime calendarDate, bool isReview = false)
    {
        InitializeComponent();

        //AppSettings.CurrentPage = this; // We need to do this so we can get the current page in the FixedScrollView class
        _dailyMoment = dailyMoment;
        _isReview = isReview;
        _onThisDay = onThisDay;
        _calendarDate = calendarDate;
        _dailyRecord = User.UserFromPreferences().DailyRecordForDate(_calendarDate);
    }
    
    protected override void OnAppearing()
    {
        base.OnAppearing();
        FormatMoment();

        if (_isReview)
        {
            BtnNext.IsVisible = false;
        }

        if (_dailyRecord != null)
        {
            EDTTellUsMore.Text = _dailyRecord.CheckOutJournalEntry;
        }
    }

    private void FormatMoment()
    {
        var favouriteMoments = DailyMoment.GetFavourites();
        
        LblMomentTitle.Text = _dailyMoment.Heading.ToLower();
        LblCallToAction.Text = _dailyMoment.CallToActionText;
        
        LblQuote.Text = "\"" + _dailyMoment.QuoteText + "\"";
        LblQuoteAuthor.Text = _dailyMoment.QuoteAuthor;
        LblContent.Text = _dailyMoment.Content;
        //WebViewSource.Html = _dailyMoment.ContentWithHtml;

        if (_dailyMoment.ImageIsVideo())
        {
            MeMomentVideo.Source = _dailyMoment.FullImageUrl;
            ImgMomentImage.IsVisible = false;
            MeMomentVideo.IsVisible = true;
        }
        else
        {
            ImgMomentImage.Source = _dailyMoment.FullImageUrl;
            ImgMomentImage.IsVisible = true;
            MeMomentVideo.IsVisible = false;
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
        else
        {
            if (favouriteMoments.Contains(_dailyMoment.Id))
            {
                ImgBtnHeart.IsVisible = false;
                ImgBtnHeartPressed.IsVisible = true;
            }
            else
            {
                ImgBtnHeart.IsVisible = true;
                ImgBtnHeartPressed.IsVisible = false;
            }
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
        SaveDailyRecord();
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
        _dailyMoment.AddToFavourites();
    }

    private void ImgBtnHeartPressed_OnClicked(object sender, EventArgs e)
    {
        ImgBtnHeart.IsVisible = true;
        ImgBtnHeartPressed.IsVisible = false;
        _dailyMoment.RemoveFromFavourites();
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

    async void EDTTellUsMore_OnFocused(object sender, FocusEventArgs e)
    {
        // Here we are accounting for the display of the soft keyboard
        // We add a margin to the FixedScrollView to push the view up
        // then scroll to the control
        if (KeyboardExtensions.IsSoftKeyboardShowing(EDTTellUsMore))
        {
            FixedScrollView.Margin = new Thickness(0, 0, 0, 320);
            await FixedScrollView.ScrollToAsync(GridJournal, ScrollToPosition.Start, true);
            //await FixedScrollView.ScrollToAsync(GridCheckIn, ScrollToPosition.End, true);
        }
    }

    async void ImgBtnOnThisDay_OnClicked(object sender, EventArgs e)
    {
        var control = sender as CobaltImageButton;
        var grid = control.Parent as CobaltGrid;
        await grid.BounceOnPressAsync();
        SaveDailyRecord();
        await Navigation.PushModalAsync(new NavigationPage(new OnThisDayViewer(_onThisDay)));
    }

    async void TapOnThisDay_OnTapped(object sender, TappedEventArgs e)
    {
        var grid = sender as CobaltGrid;
        await grid.BounceOnPressAsync();
        SaveDailyRecord();
        await Navigation.PushModalAsync(new NavigationPage(new OnThisDayViewer(_onThisDay)));
    }

    private void EDTTellUsMore_OnTextChanged(object sender, TextChangedEventArgs e)
    {
        _dailyRecord.CheckOutJournalEntry = EDTTellUsMore.Text;
    }

    private void SaveDailyRecord()
    {
        ApiService.SaveJournalAndExercise(_dailyRecord);    // Deliberately not awaited
    }
}