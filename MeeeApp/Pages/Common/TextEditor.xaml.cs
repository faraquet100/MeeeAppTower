using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Core.Platform;
using MeeeApp.Models;
using MeeeApp.Services;

namespace MeeeApp.Pages.Common;

public partial class TextEditor : ContentPage
{

    private string _content = "";
    private string _title = "";
    private string _placeholder = "";
    private bool _isCheckIn = false;
    private bool _isCheckOut = false;
    public TextEditor(string title, string placeholder, string content, bool isCheckIn = false, bool isCheckOut = false)
    {
        InitializeComponent();
        _title = title;
        _content = content;
        _placeholder = placeholder;
    }
    
    protected override async void OnAppearing()
    {
        base.OnAppearing();
        EdtEditor.Text = _content;
        LblTitle.Text = _title;
        EdtEditor.Placeholder = _placeholder;

        await Task.Run(async () =>
        {
            await Task.Delay(500);
            Dispatcher.Dispatch(() => { EdtEditor.Focus(); });  // On main thred
        });
    }

    private void FocusEditor()
    {
        EdtEditor.Focus();
    }
    

    async void BtnCancel_OnClicked(object sender, EventArgs e)
    {
        AppSettings.CurrentEditor = null;
        
        if (KeyboardExtensions.IsSoftKeyboardShowing(EdtEditor))
        {
            await KeyboardExtensions.HideKeyboardAsync(EdtEditor, System.Threading.CancellationToken.None);
        }
        
        await Navigation.PopModalAsync();
    }

    async void BtnDone_OnClicked(object sender, EventArgs e)
    {
        if (AppSettings.CurrentEditor != null)
        {
            AppSettings.CurrentEditor.Text = EdtEditor.Text;
        }

        if (KeyboardExtensions.IsSoftKeyboardShowing(EdtEditor))
        {
            await KeyboardExtensions.HideKeyboardAsync(EdtEditor, System.Threading.CancellationToken.None);
        }
        
        await Navigation.PopModalAsync();
    }
}