using System;
using System.Net.Http;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using MeeeApp.Models;
using Newtonsoft.Json;
using banditoth.MAUI.PreferencesExtension;
using System.Net.Http.Headers;
using System.Web;

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
        public static string API_URL = "https://meeeapi.azurewebsites.net/api/";
        
        // Endpoints
        public static string ENDPOINT_REGISTER = API_URL + "Users/Register";
        public static string ENDPOINT_LOGIN = API_URL + "Users/Login";
        public static string ENDPOINT_USERS_LIST = API_URL + ""; // Just for testing this one, should be deleted later
        public static string ENDPOINT_CHECK_IN = API_URL + "DailyRecord/CheckIn";
        public static string ENDPOINT_CHECK_OUT = API_URL + "DailyRecord/CheckOut";
        public static string ENDPOINT_SAVE_JOURNAL_EXERCISE = API_URL + "DailyRecord/SaveExerciseAndJournal";
        public static string ENDPOINT_DAILY_MOMENT = API_URL + "DailyMoment/";
        public static string ENDPOINT_ONTHISDAY = API_URL + "OnThisDay?";
        public static string ENDPOINT_DAILY_RECORDS = API_URL + "DailyRecord/";

        // Preference Keys
        // Using banditoth.MAUI.PreferencesExtension which allows us to serialize obkects
        public static string KEY_USER_OBJECT = "user_object";
        public static string KEY_USER_TOKEN = "user_token";
        public static string KEY_DAILYRECORDS = "dailyrecords";

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
            
            // Get this users Daily Records, they aren't returned at login (but are for other ops)
            var dailyRecords = await DailyRecords(result.UserId, result.Token);
            result.DailyRecords = dailyRecords;

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

        // We only need to get the daily records after login
        // In all other cases we get them with the user object
        public static async Task<List<DailyRecord>> DailyRecords(int userId, string token)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", token);
            
            string url = ENDPOINT_DAILY_RECORDS + userId.ToString();
            var response = await httpClient.GetAsync(url);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || !response.IsSuccessStatusCode)
            {
                return new List<DailyRecord>();
            }
            
            var jsonResult = await response.Content.ReadAsStringAsync();
            var dailyRecords = JsonConvert.DeserializeObject<List<DailyRecord>>(jsonResult);

            if (dailyRecords == null) return new List<DailyRecord>();
            return dailyRecords;
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
        
        public static async Task<ApiResult> CheckOut(DailyRecord dailyRecord)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", GetToken());
            var json = JsonConvert.SerializeObject(dailyRecord);
            var payload = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(ENDPOINT_CHECK_OUT, payload);

            return await ProcessUserReturnResponse(response);
        }

        // Get the Daily Moment for the current day
        public static async Task<ApiResult> GetDailyMoment(User user, int score)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", GetToken());
            
            string queryString = user.UserId.ToString() + "," + DateTime.Now.ToString("yyyy-MM-dd") + "," + score.ToString();
            string url = ENDPOINT_DAILY_MOMENT + queryString;
            var response = await httpClient.GetAsync(url);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return ApiResult.NotAuthorized;
            }
            
            if (!response.IsSuccessStatusCode) return ApiResult.BadRequest;
            
            var jsonResult = await response.Content.ReadAsStringAsync();
            var dailyMoment = JsonConvert.DeserializeObject<DailyMoment>(jsonResult);

            if (dailyMoment == null) return ApiResult.BadRequest;

            AppSettings.DailyMoment = dailyMoment;
            return ApiResult.Success;
        }
        
        // Get the OnThisDay for the current day
        public static async Task<OnThisDay> GetOnThisDay(DateTime date)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", GetToken());
            
            string queryString = "month=" + date.Month.ToString() + "&day=" + date.Day.ToString();
            string url = ENDPOINT_ONTHISDAY + queryString;
            var response = await httpClient.GetAsync(url);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized || !response.IsSuccessStatusCode)
            {
                return new OnThisDay();
            }
            
            var jsonResult = await response.Content.ReadAsStringAsync();
            var onThisDay = JsonConvert.DeserializeObject<OnThisDay>(jsonResult);

            if (onThisDay == null) return new OnThisDay();
            return onThisDay;
        }
        
        // Get all Daily Moments - For Testing Only
        public static async Task<ApiResult> GetAllDailyMoments()
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", GetToken());

            string url = ENDPOINT_DAILY_MOMENT;
            var response = await httpClient.GetAsync(url);
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                return ApiResult.NotAuthorized;
            }
            
            if (!response.IsSuccessStatusCode) return ApiResult.BadRequest;
            
            var jsonResult = await response.Content.ReadAsStringAsync();
            var dailyMoments = JsonConvert.DeserializeObject<List<DailyMoment>>(jsonResult);

            if (dailyMoments == null) return ApiResult.BadRequest;

            AppSettings.DailyMoments = dailyMoments;
            return ApiResult.Success;
        }


        public static async Task<ApiResult> SaveJournalAndExercise(DailyRecord dailyRecord)
        {
            var httpClient = new HttpClient();
            httpClient.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("bearer", GetToken());
            var json = JsonConvert.SerializeObject(dailyRecord);
            var payload = new StringContent(json, Encoding.UTF8, "application/json");
            var response = await httpClient.PostAsync(ENDPOINT_SAVE_JOURNAL_EXERCISE, payload);

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
            // The Daily Records don't decode properly if we save the whole user object
            // so we need to save them separately nd reload them in User.UserFromPreferences()
            Preferences.Default.SetObject<User>(KEY_USER_OBJECT, user);
            Preferences.Default.SetObject<List<DailyRecord>>(KEY_DAILYRECORDS, user.DailyRecords);
            //var user1 = Preferences.Default.GetObject<User>(ApiService.KEY_USER_OBJECT, new User());
            //var dailyRecords = Preferences.Default.GetObject<List<DailyRecord>>(ApiService.KEY_DAILYRECORDS, new List<DailyRecord>());
        }

        public static string GetToken()
        {
            return Preferences.Default.Get(KEY_USER_TOKEN, "");
        }

        #endregion
    }
}

