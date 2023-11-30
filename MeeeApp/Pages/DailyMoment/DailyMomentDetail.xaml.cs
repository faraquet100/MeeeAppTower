using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeeeApp.Controls;
using MeeeApp.Models;
using MeeeApp.Pages.Common;
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
    private bool _isFavourites = false;
    private int _currentDailyMomentIndex = 0;
    
    public DailyMomentDetail(DailyMoment dailyMoment, OnThisDay onThisDay, DateTime calendarDate, bool isReview = false, bool isFavourites = false)
    {
        InitializeComponent();
        _dailyMoment = dailyMoment;

        //AppSettings.CurrentPage = this; // We need to do this so we can get the current page in the FixedScrollView class
        _isReview = isReview;
        _isFavourites = isFavourites;
        _onThisDay = onThisDay;
        _calendarDate = calendarDate;
        _dailyRecord = User.UserFromPreferences().DailyRecordForDate(_calendarDate);

        // When reviewing saved daily moments
        if (isFavourites && dailyMoment.DailyRecord != null)
        {
            _dailyRecord = dailyMoment.DailyRecord;
        }

        if (User.TestModeFromPreferences())
        {
            _dailyRecord = User.UserFromPreferences().DailyRecordForDate(_calendarDate.AddYears(-5));
        }
    }
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        
        if (AppSettings.CurrentEditor == EDTTellUsMore)
        {
            _dailyRecord.CheckOutJournalEntry = AppSettings.CurrentEditor.Text;
        }
        
        FormatMoment();
        FixAndroid();

        if (_isReview)
        {
            BtnNext.IsVisible = false;
        }

        if (_dailyRecord != null && AppSettings.CurrentEditor != EDTTellUsMore)
        {
            EDTTellUsMore.Text = _dailyRecord.CheckOutJournalEntry;
        }

        if (_dailyRecord != null && AppSettings.CurrentEditor == EDTTellUsMore)
        {
            // We need to save an edit
            SaveDailyRecord();
        }

        AppSettings.CurrentEditor = null;
        
        DelayedSetup();
        
        if (KeyboardExtensions.IsSoftKeyboardShowing(EDTTellUsMore))
        {
            await KeyboardExtensions.HideKeyboardAsync(EDTTellUsMore, System.Threading.CancellationToken.None);
        }
    }

    private void FixAndroid()
    {
        // Because Android puts the grid view after the back button
#if ANDROID 
            LblNavTitle.Margin = new Thickness(-70, 0, 0, 0);
#endif
    }

    private void FormatMoment()
    {
        var favouriteMoments = DailyMoment.GetFavourites();
        
        ImgMomentImage.Source = _dailyMoment.FullImageUrl;
        LblMomentTitle.Text = _dailyMoment.Heading.ToLower();
        LblCallToAction.Text = _dailyMoment.CallToActionText;
        
        LblQuote.Text = "“" + _dailyMoment.QuoteText.Replace("\"", "").Trim() + "”";  // Option+[ for opening quote Option+Shift+[ for closing quote
        LblQuoteAuthor.Text = _dailyMoment.QuoteAuthor;
        LblContent.Text = _dailyMoment.Content;
        
        if (_dailyMoment.QuoteText.Length == 0)
        {
            LblQuote.IsVisible = false;
            LblQuoteAuthor.IsVisible = false;
        }
        else
        {
            LblQuote.IsVisible = true;
            LblQuoteAuthor.IsVisible = true;
        }
        
        //WebViewSource.Html = _dailyMoment.ContentWithHtml;

        FormatImageVideo();

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
        
        // Testing review 
        if (_isReview && !_isFavourites)
        {
            GridJournal.IsVisible = false;
            GridCheckIn.IsVisible = false;
        }

        if (_isReview && _isFavourites)
        {
            GridCheckIn.IsVisible = false;
        }
        
    }

    private void FormatImageVideo()
    {
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
    }
    
    async void DelayedSetup()
    {
        await Task.Delay(1000);
        FormatImageVideo();
        
        // Sometimes the image doesn't load, this is a fallback to reload after a delay
        MainThread.BeginInvokeOnMainThread(() =>
        {
            FormatImageVideo();
            
        });
    }
    
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

        if (_isFavourites)
        {
            AppSettings.DailyMoments = AppSettings.DailyMoments.Where(m => m.Id != _dailyMoment.Id).ToList();
        }
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
            FixedScrollView.Margin = new Thickness(0, 0, 0, 360);
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

    async void GridJournalTap_OnTapped(object sender, TappedEventArgs e)
    {
        AppSettings.CurrentEditor = this.EDTTellUsMore;
        await Navigation.PushModalAsync(new NavigationPage(new TextEditor("daily moment", "Take a minute to reflect on today's 'Daily Moment' . . .", EDTTellUsMore.Text)));
    }
}