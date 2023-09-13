using System.ComponentModel.DataAnnotations.Schema;
using MeeeApp.Services;
using Newtonsoft.Json;
using banditoth.MAUI.PreferencesExtension;

namespace MeeeApp.Models;

public class User
{
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
    
    public List<DailyRecord> DailyRecords = new List<DailyRecord>();


    public static User UserFromPreferences()
    {
        var user = Preferences.Default.GetObject<User>(ApiService.KEY_USER_OBJECT, new User());

        // We need to save the token independently because we only receive it with Login and Register requests, so we add it to the user record here
        user.Token = ApiService.GetToken();
        return user;
    }

    // Logout the user
    public static void WipeUser()
    {
        Preferences.Default.Set(ApiService.KEY_USER_TOKEN, "");
        Preferences.Default.SetObject<User>(ApiService.KEY_USER_OBJECT, null);
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

}