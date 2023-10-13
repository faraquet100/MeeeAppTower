using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeeeApp.Models;

namespace MeeeApp.Pages;

public partial class OnThisDayViewer : ContentPage
{
    private OnThisDay _onThisDay;

    protected override void OnAppearing()
    {
        base.OnAppearing();
        LblDay.Text = _onThisDay.DisplayDay.ToString();
        LblMonth.Text = _onThisDay.DisplayMonthName();
        LblContent.Text = _onThisDay.Content;
        LblTitle.Text = _onThisDay.Title;
    }

    public OnThisDayViewer(OnThisDay onThisDay)
    {
        InitializeComponent();
        _onThisDay = onThisDay;    
    }

    private void BtnClose_OnClicked(object sender, EventArgs e)
    {
        Navigation.PopModalAsync();
    }
}