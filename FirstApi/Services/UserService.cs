using FirstApi.Models;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace FirstApi.Services
{
    public class UserService : IUserService
    {
        private readonly string connectionString = "Server=tcp:test-poc-db.database.windows.net,1433;Initial Catalog=test-db;Persist Security Info=False;User ID=testadmin;Password=Pass@123;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

        public User GetUser(int id)
        {
            var user = new User();

            var sqlQuery = @$"SELECT [UserID], [UserName], [Age], [City], [Occupation], [ContactNo],[Email] FROM [Users] WHERE [UserID] = {id}";

            var sqlConnectionBuilder = new SqlConnectionStringBuilder(connectionString);

            using (SqlConnection sqlConnection = new SqlConnection(sqlConnectionBuilder.ConnectionString))
            {
                sqlConnection.Open();
                using (SqlCommand command = new SqlCommand(sqlQuery, sqlConnection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            user.UserID = Convert.ToInt32(reader["UserID"]);
                            user.UserName = Convert.ToString(reader["UserName"]);
                            user.Age = Convert.ToInt32(reader["Age"]);
                            user.City = Convert.ToString(reader["City"]);
                            user.Occupation = Convert.ToString(reader["Occupation"]);
                            user.ContactNo = Convert.ToString(reader["ContactNo"]);
                            user.Email = Convert.ToString(reader["Email"]);
                        }
                    }
                }
            }

            return user;
        }

        public List<User> GetUsers()
        {
            var users = new List<User>();

            var sqlQuery = @$"SELECT [UserID], [UserName], [Age], [City], [Occupation], [ContactNo],[Email] FROM [Users]";

            var sqlConnectionBuilder = new SqlConnectionStringBuilder(connectionString);

            using (SqlConnection sqlConnection = new SqlConnection(sqlConnectionBuilder.ConnectionString))
            {
                sqlConnection.Open();
                using (SqlCommand command = new SqlCommand(sqlQuery, sqlConnection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var user = new User();
                            user.UserID = Convert.ToInt32(reader["UserID"]);
                            user.UserName = Convert.ToString(reader["UserName"]);
                            user.Age = Convert.ToInt32(reader["Age"]);
                            user.City = Convert.ToString(reader["City"]);
                            user.Occupation = Convert.ToString(reader["Occupation"]);
                            user.ContactNo = Convert.ToString(reader["ContactNo"]);
                            user.Email = Convert.ToString(reader["Email"]);
                            users.Add(user);
                        }
                    }
                }
            }

            return users;
        }

        public int DeleteUser(int id)
        {
            var sqlQuery = @$"DELETE FROM [Users] WHERE [UserID] = {id}";

            var sqlConnectionBuilder = new SqlConnectionStringBuilder(connectionString);

            using (SqlConnection sqlConnection = new SqlConnection(sqlConnectionBuilder.ConnectionString))
            {
                sqlConnection.Open();
                using (SqlCommand command = new SqlCommand(sqlQuery, sqlConnection))
                {
                    var result = command.ExecuteNonQuery();
                    return result;
                }
            }
        }

        public int CreateUser(User user)
        {
            int result = -1;
            var sqlQuery = $@"INSERT INTO [USERS] (UserName, Age, City, Occupation, ContactNo, Email) VALUES ('{user.UserName}', {user.Age}, '{user.City}', '{user.Occupation}', '{user.ContactNo}', '{user.Email}')";

            var sqlConnectionBuilder = new SqlConnectionStringBuilder(connectionString);

            using (SqlConnection sqlConnection = new SqlConnection(sqlConnectionBuilder.ConnectionString))
            {
                sqlConnection.Open();
                using (SqlCommand command = new SqlCommand(sqlQuery, sqlConnection))
                {
                    result = command.ExecuteNonQuery();
                }
            }

            return result;
        }

        public int UpdateUser(int id, User user)
        {
            int result = -1;
            var sqlQuery = $@"UPDATE [USERS] SET UserName = '{user.UserName}', Age = {user.Age}, City = '{user.City}', Occupation = '{user.Occupation}', ContactNo = '{user.ContactNo}', Email = '{user.Email}' WHERE [UserID] = {id}";

            var sqlConnectionBuilder = new SqlConnectionStringBuilder(connectionString);

            using (SqlConnection sqlConnection = new SqlConnection(sqlConnectionBuilder.ConnectionString))
            {
                sqlConnection.Open();
                using (SqlCommand command = new SqlCommand(sqlQuery, sqlConnection))
                {
                    result = command.ExecuteNonQuery();
                }
            }

            return result;
        }
    }
}
