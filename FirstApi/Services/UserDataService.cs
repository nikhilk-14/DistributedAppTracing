using FirstApi.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace FirstApi.Services
{
    public class UserDataService : IUserDataService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<UserDataService> _logger;

        public UserDataService(IConfiguration configuration, ILogger<UserDataService> logger)
        {
            _config = configuration;
            _logger = logger;
        }

        public async Task<User> GetUser(int id)
        {
            var user = new User();

            using (var _httpClient = new HttpClient())
            {
                var result = await _httpClient.GetAsync($"{_config.GetSection("Appsettings:Secondapi").Value}/api/crud/{id}");
                var data = await result.Content.ReadAsStringAsync();
                user = JsonConvert.DeserializeObject<User>(data);
            }

            return user;
        }

        public async Task<List<User>> GetUsers()
        {
            var users = new List<User>();

            using (var _httpClient = new HttpClient())
            {
                var result = await _httpClient.GetAsync($"{_config.GetSection("Appsettings:Secondapi").Value}/api/crud");

                var data = await result.Content.ReadAsStringAsync();
                users = JsonConvert.DeserializeObject<List<User>>(data);
            }

            return users;
        }

        public async Task<int> DeleteUser(int id)
        {
            int result = -1;

            using (var _httpClient = new HttpClient())
            {
                var output = await _httpClient.DeleteAsync($"{_config.GetSection("Appsettings:Secondapi").Value}/api/crud/{id}");

                result = Convert.ToInt32(await output.Content.ReadAsStringAsync());
            }

            return result;
        }

        public async Task<int> CreateUser(User user)
        {
            int result = -1;
            bool validationResult = true;

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                using var _httpClient = new HttpClient();
                var output = await _httpClient.GetAsync($"{_config.GetSection("Appsettings:Thirdapi").Value}/Validation/email/{user.Email}?userId=-1");
                var data = await output.Content.ReadAsStringAsync();
                validationResult = Convert.ToBoolean(data);
            }

            if (validationResult)
            {
                using (var _httpClient = new HttpClient())
                {
                    HttpContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");
                    var output = await _httpClient.PostAsync($"{_config.GetSection("Appsettings:Secondapi").Value}/api/crud", content);

                    result = Convert.ToInt32(await output.Content.ReadAsStringAsync());
                }
            }
            else
            {
                _logger.LogError($"Email validation failed. Input Email Id - {user.Email}");
            }

            return result;
        }

        public async Task<int> UpdateUser(int id, User user)
        {
            int result = -1;
            bool validationResult = true;

            if (!string.IsNullOrWhiteSpace(user.Email))
            {
                using var _httpClient = new HttpClient();
                var output = await _httpClient.GetAsync($"{_config.GetSection("Appsettings:Thirdapi").Value}/Validation/email/{user.Email}?userId={id}");
                var data = await output.Content.ReadAsStringAsync();
                validationResult = Convert.ToBoolean(data);
            }

            if (validationResult)
            {
                using (var _httpClient = new HttpClient())
                {
                    HttpContent content = new StringContent(JsonConvert.SerializeObject(user), Encoding.UTF8, "application/json");

                    var output = await _httpClient.PutAsync($"{_config.GetSection("Appsettings:Secondapi").Value}/api/crud/{id}", content);

                    result = Convert.ToInt32(await output.Content.ReadAsStringAsync());
                }
            }
            else
            {
                _logger.LogError($"Email validation failed. Input Email Id - {user.Email}");
            }

            return result;
        }
    }
}
