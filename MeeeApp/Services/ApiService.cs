using System;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MeeeApp.Models;
using Newtonsoft.Json;

namespace MeeeApp.Services
{
	public static class ApiService
	{
        public enum ApiResult
        {
            Success = 1,
            BadRequest = 2,
            NoInternet = 3
        }

        // Base Url
        public static string API_URL = "https://cobaltmeeeapi.azurewebsites.net/api/";
        
        // Endpoints
        public static string ENDPOINT_REGISTER = API_URL + "Users/Register";
        public static string ENDPOINT_LOGIN = API_URL + "Users/Login";
        public static string ENDPOINT_USERS_LIST = API_URL + ""; // Just for testing this one, should be deleted later
        
        // Preference Keys
        public static string KEY_TOKEN = "access_token";
        public static string KEY_USER_ID = "userid";
        public static string KEY_USER_EMAIL = "useremail";
        public static string KEY_USER_FIRST = "userfirst";
        public static string KEY_USER_LAST = "user_second";

        #region Users
        
        public static async Task<ApiResult> LoginAsync(Login login)
        {
            var httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(login);
            var payload = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response;

            try
            {
                response = await httpClient.PostAsync(ENDPOINT_LOGIN, payload);
            }
            catch
            {
                return ApiResult.NoInternet;
            }

            
            if (!response.IsSuccessStatusCode) return ApiResult.BadRequest;
            
            var jsonResult = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<User>(jsonResult);

            if (result == null) return ApiResult.BadRequest;

            SaveUserDetailsToPreferences(result);

            return ApiResult.Success;
        }

        public static async Task<ApiResult> RegisterAsync(Register register)
        {
            var httpClient = new HttpClient();
            var json = JsonConvert.SerializeObject(register);
            var payload = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(ENDPOINT_REGISTER, payload);

            if (!response.IsSuccessStatusCode) return ApiResult.BadRequest;

            var jsonResult = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<User>(jsonResult);

            if (result == null) return ApiResult.BadRequest;

            SaveUserDetailsToPreferences(result);

            return ApiResult.Success;
        }

        private static void SaveUserDetailsToPreferences(User user)
        {
            Preferences.Set(KEY_TOKEN, user.Token);
            Preferences.Set(KEY_USER_EMAIL, user.Email);
            Preferences.Set(KEY_USER_ID, user.UserId);
            Preferences.Set(KEY_USER_FIRST, user.FirstName);
            Preferences.Set(KEY_USER_LAST, user.LastName);
        }
        
        
        #endregion
    }
}

