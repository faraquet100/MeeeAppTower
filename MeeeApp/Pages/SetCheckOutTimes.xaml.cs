using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MeeeApp.Models;
using MeeeApp.Services;

namespace MeeeApp.Pages;

public partial class SetCheckOutTimes : ContentPage
{
    private DateTime _checkingInTime;
    public SetCheckOutTimes(DateTime checkingInTime)
    {
        InitializeComponent();
        _checkingInTime = checkingInTime;
        
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
    }
}