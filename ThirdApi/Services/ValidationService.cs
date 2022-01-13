using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ThirdApi.Services
{
    public class ValidationService : IValidationService
    {
        private readonly IConfiguration _config;
        private readonly ILogger<ValidationService> _logger;

        public ValidationService(IConfiguration configuration, ILogger<ValidationService> logger)
        {
            _config = configuration;
            _logger = logger;
        }

        public async Task<bool> ValidateEmail(string emailId, int userId)
        {
            var result = false;

            if (CheckIfEmailIdIsValid(emailId) && !await CheckIfEmailIdExists(emailId, userId))
            {
                _logger.LogInformation($"Input EmailId validation successful");
                result = true;
            }
            else
            {
                _logger.LogError($"Input EmailId validation failed");
            }

            return result;
        }

        private bool CheckIfEmailIdIsValid(string emailId)
        {
            var result = true;

            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(emailId);

            if (match.Success)
            {
                _logger.LogInformation($"Input EmailId is valid");
                result = true;
            }
            else
            {
                _logger.LogError($"Input EmailId is not valid");
                result = false;
            }

            return result;
        }

        private async Task<bool> CheckIfEmailIdExists(string emailId, int userId)
        {
            var connectionString = _config.GetSection("Appsettings:SQLConnectionString").Value;
            var cnt = 0;

            var userIdFilter = userId == -1 ? "" : $" AND UserId != {userId} ";
            var sqlQuery = @$"SELECT UserID, COUNT(Email) AS CNT FROM [Users] WHERE [Email] = '{emailId}' {userIdFilter} GROUP BY UserId";

            var sqlConnectionBuilder = new SqlConnectionStringBuilder(connectionString);

            using (SqlConnection sqlConnection = new SqlConnection(sqlConnectionBuilder.ConnectionString))
            {
                sqlConnection.Open();
                using (SqlCommand command = new SqlCommand(sqlQuery, sqlConnection))
                {
                    using (SqlDataReader reader = await command.ExecuteReaderAsync())
                    {
                        while (reader.Read())
                        {
                           cnt  = Convert.ToInt32(reader["CNT"]);
                        }
                    }
                }
            }

            bool result = cnt > 0 ? true : false;

            if (result)
            {
                _logger.LogError($"Input EmailId already exists");
            }
            else
            {
                _logger.LogInformation($"Input EmailId does not exists");
            }

            return result;
        }
    }
}
