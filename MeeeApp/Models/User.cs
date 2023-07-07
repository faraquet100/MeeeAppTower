using System.ComponentModel.DataAnnotations.Schema;
using MeeeApp.Services;
using Newtonsoft.Json;

namespace MeeeApp.Models;

public class User
{
    [JsonProperty("token")] public string Token { get; set; }
    [JsonProperty("id")] public int UserId { get; set; }
    [JsonProperty("email")] public string Email { get; set; }
    [JsonProperty("firstname")] public string FirstName { get; set; }
    [JsonProperty("lastname")] public string LastName { get; set; }

    public static User UserFromPreferences()
    {
        var token = Preferences.Get(ApiService.KEY_TOKEN, "");
        var email = Preferences.Get(ApiService.KEY_USER_EMAIL, "");
        var userId = Preferences.Get(ApiService.KEY_USER_ID, 0);
        var first = Preferences.Get(ApiService.KEY_USER_FIRST, "");
        var last = Preferences.Get(ApiService.KEY_USER_LAST, "");

        return new User
        {
            Token = token,
            Email = email,
            UserId = userId,
            FirstName = first,
            LastName = last
        };
    }

    // Logout the user
    public static void WipeUser()
    {
        Preferences.Set(ApiService.KEY_TOKEN, "");
        Preferences.Set(ApiService.KEY_USER_EMAIL, "");
        Preferences.Set(ApiService.KEY_USER_ID, 0);
        Preferences.Set(ApiService.KEY_USER_FIRST, "");
        Preferences.Set(ApiService.KEY_USER_LAST, "");
    }
    
    public bool IsValid => Token.Length > 0 && Email.Length > 0 && UserId > 0;

}