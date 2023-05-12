using System;
using MeeeApp.Models;

namespace MeeeApp.Services
{
	public static class ApiService
	{
        public static string API_URL = "";
        public static string ENDPOINT_REGISTER = API_URL + "";
        public static string ENDPOINT_LOGIN = API_URL + "";

        public static async Task<bool> LoginAsync(Login login)
        {
            await Task.Delay(3000);
            return true;
        }

        public static async Task<bool> RegisterAsync(Register register)
        {
            await Task.Delay(3000);
            return true;
        }
    }
}

