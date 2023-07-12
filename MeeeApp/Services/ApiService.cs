using System;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MeeeApp.Models;
using Newtonsoft.Json;
using banditoth.MAUI.PreferencesExtension;
using System.Net.Http.Headers;

namespace MeeeApp.Services
{
	public static class ApiService
	{
        public enum ApiResult
        {
            Success = 1,
            BadRequest = 2,
            NoInternet = 3,
            NotAuthorized = 4
        }

        // Base Url
        public static string API_URL = "https://cobaltmeeeapi.azurewebsites.net/api/";
        
        // Endpoints
        public static string ENDPOINT_REGISTER = API_URL + "Users/Register";
        public static string ENDPOINT_LOGIN = API_URL + "Users/Login";
        public static string ENDPOINT_USERS_LIST = API_URL + ""; // Just for testing this one, should be deleted later
        public static string ENDPOINT_CHECK_IN = API_URL + "DailyRecord/CheckIn";
        public static string ENDPOINT_CHECK_OUT = API_URL + "DailyRecord/CheckOut";

        // Preference Keys
        // Using banditoth.MAUI.PreferencesExtension which allows us to serialize obkects
        public static string KEY_USER_OBJECT = "user_object";
        public static string KEY_USER_TOKEN = "user_token";

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

            // We need to save the token independently because we only receive it with Login and Register requests
            Preferences.Default.Set(KEY_USER_TOKEN, result.Token);
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

            // We need to save the token independently because we only receive it with Login and Register requests
            Preferences.Default.Set(KEY_USER_TOKEN, result.Token);
            SaveUserDetailsToPreferences(result);

            return ApiResult.Success;
        }

        #endregion

        #region Check In/Out

        public static async Task<ApiResult> CheckIn(DailyRecord dailyRecord)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", GetToken());
            var json = JsonConvert.SerializeObject(dailyRecord);
            var payload = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(ENDPOINT_CHECK_IN, payload);

            return await ProcessUserReturnResponse(response);
        }

        #endregion

        #region Helpers

        // For API calls that return the user object
        public static async Task<ApiResult> ProcessUserReturnResponse(HttpResponseMessage response)
        {
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return ApiResult.NotAuthorized;
            }

            if (!response.IsSuccessStatusCode) return ApiResult.BadRequest;

            var jsonResult = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<User>(jsonResult);

            if (result == null) return ApiResult.BadRequest;

            SaveUserDetailsToPreferences(result);
            return ApiResult.Success;
        }

        public static void SaveUserDetailsToPreferences(User user)
        {
            Preferences.Default.SetObject<User>(KEY_USER_OBJECT, user);
            var user1 = Preferences.Default.GetObject<User>(ApiService.KEY_USER_OBJECT, new User());
        }

        public static string GetToken()
        {
            return Preferences.Default.Get(KEY_USER_TOKEN, "");
        }

        #endregion
    }
}

