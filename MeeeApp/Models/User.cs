using System.ComponentModel.DataAnnotations.Schema;
using MeeeApp.Services;
using Newtonsoft.Json;
using banditoth.MAUI.PreferencesExtension;

namespace MeeeApp.Models;

public class User
{
    public static string KEY_CHECKIN_TIME = "user_checkin_time";
    public static string KEY_CHECKOUT_TIME = "user_checkout_time";
    [JsonProperty("token")] public string Token { get; set; }
    [JsonProperty("id")] public int UserId { get; set; }
    [JsonProperty("email")] public string Email { get; set; }
    [JsonProperty("firstname")] public string FirstName { get; set; }
    [JsonProperty("lastname")] public string LastName { get; set; }
    [JsonProperty("organisationId")] public int OrganisationId { get; set; }
    [JsonProperty("userType")] public int UserType { get; set; }
    [JsonProperty("gender")] public int Gender { get; set; }
    [JsonProperty("dateOfBirth")] public DateTime DateOfBirth { get; set; }
    [JsonProperty("appCheckinTime")] public DateTime CheckInTime { get; set; }
    [JsonProperty("appCheckOutTime")] public DateTime CheckOutTime { get; set; }
    [JsonProperty("dailyRecords")] public List<DailyRecord> DailyRecords = new List<DailyRecord>();


    public DailyRecord DailyRecordForDate(DateTime dateTime)
    {
        var record = DailyRecords.FirstOrDefault(r => r.RecordDate.Year == dateTime.Year &&
                                                             r.RecordDate.Month == dateTime.Month &&
                                                             r.RecordDate.Day == dateTime.Day);
        return record;
    }

    public string LastCheckedInLabelText()
    {
        if (DailyRecords.Count == 0)
        {
            return "You have yet to Check-In";
        }

        var lastDailyRecord = DailyRecords.MaxBy(d => d.RecordDate);
        var difference = DateTime.Now - lastDailyRecord.RecordDate;
        var days = difference.Days;

        if (lastDailyRecord.CheckInTime.Year > 2000)
        {
            if (days == 0)
            {
                return "You last Checked-In today";
            }
            else if (days == 1)
            {
                return "You last Checked-In yesterday";
            }
            else
            {
                return $"You last Checked-In {days} days ago";
            }
        }
        
        return "You have yet to Check-In";
    }
    
    public string LastCheckedOutLabelText()
    {
        if (DailyRecords.Count == 0)
        {
            return "You have yet to Check-Out";
        }

        var lastDailyRecord = DailyRecords.MaxBy(d => d.RecordDate);
        var difference = DateTime.Now - lastDailyRecord.RecordDate;
        var days = difference.Days;

        if (lastDailyRecord.CheckOutTime.Year > 2000)
        {
            if (days == 0)
            {
                return "You last Checked-Out today";
            }
            else if (days == 1)
            {
                return "You last Checked-Out yesterday";
            }
            else
            {
                return $"You last Checked-Out {days} days ago";
            }
        }
        
        return "You have yet to Check-Out";
    }

    public string DayTitle(DateTime calendarDate)
    {
        // Is Today
        if (calendarDate.Year == DateTime.Now.Year && calendarDate.Month == DateTime.Now.Month && calendarDate.Day == DateTime.Now.Day)
        {
            return "today";
        }
        else
        {
            return calendarDate.ToString("dd MMM yy").ToLower();
        }
    }
    
    public static User UserFromPreferences()
    {
        var user = Preferences.Default.GetObject<User>(ApiService.KEY_USER_OBJECT, new User());
        var dailyRecords = Preferences.Default.GetObject<List<DailyRecord>>(ApiService.KEY_DAILYRECORDS, new List<DailyRecord>());
        user.DailyRecords = dailyRecords;

        // We need to save the token independently because we only receive it with Login and Register requests, so we add it to the user record here
        user.Token = ApiService.GetToken();
        return user;
    }

    // Logout the user
    public static void WipeUser()
    {
        Preferences.Default.Set(ApiService.KEY_USER_TOKEN, "");
        Preferences.Default.SetObject<User>(ApiService.KEY_USER_OBJECT, null);
        Preferences.Default.SetObject<List<DailyRecord>>(ApiService.KEY_DAILYRECORDS, null);
    }
    
    
    
    public bool IsValid
    {
        get
        {
            if (Token == null || Email == null)
            {
                return false;
            }
            else
            {
                return Token.Length > 0 && Email.Length > 0 && UserId > 0;
            }
        }
    }
    
    public static void SaveCheckInTimeToPreferences(DateTime checkInTime)
    {
        Preferences.Default.Set(KEY_CHECKIN_TIME, checkInTime);
    }
    
    public static void SaveCheckOutTimeToPreferences(DateTime checkOutTime)
    {
        Preferences.Default.Set(KEY_CHECKOUT_TIME, checkOutTime);
    }

    public static DateTime CheckInTimeFromPreferences()
    {
        var checkInTime = Preferences.Default.Get<DateTime>(KEY_CHECKIN_TIME, DateTime.MinValue);
        return checkInTime;
    }
    
    public static DateTime CheckOutTimeFromPreferences()
    {
        var checkInTime = Preferences.Default.Get<DateTime>(KEY_CHECKOUT_TIME, DateTime.MinValue);
        return checkInTime;
    }
    
    public static void WipeCheckInOutTimes()
    {
        SaveCheckInTimeToPreferences(DateTime.MinValue);
        SaveCheckOutTimeToPreferences(DateTime.MinValue);
    }
    

}