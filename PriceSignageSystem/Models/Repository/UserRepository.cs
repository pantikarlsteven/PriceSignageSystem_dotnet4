using PriceSignageSystem.Models.DatabaseContext;
using PriceSignageSystem.Models.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace PriceSignageSystem.Models.Repository
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _db;
        private readonly string connectionString;
        public UserRepository(ApplicationDbContext db)
        {
            _db = db;
            connectionString = ConfigurationManager.ConnectionStrings["PriceSignageDbConnectionString"].ConnectionString;
        }

        public IQueryable<User> GetAll()
        {
            var data = _db.Users;
            return data;
        }
        public List<User> GetUsers()
        {
            var users = new List<User>();
            // Set up the connection and command
            using (var connection = new SqlConnection(connectionString))
            using (var command = new SqlCommand("sp_FetchUsers", connection))
            {
                command.CommandType = CommandType.StoredProcedure;

                // Add parameters if required
                //command.Parameters.AddWithValue("@Param1", value1);
                //command.Parameters.AddWithValue("@Param2", value2);

                // Open the connection and execute the command
                connection.Open();
                SqlDataReader reader = command.ExecuteReader();

                // Process the result set
                while (reader.Read())
                {
                    var user = new User
                    {
                        UserId = (int)reader["UserId"],
                        UserName = reader["Username"].ToString(),
                        Password = reader["Password"].ToString(),
                        IsActive = (int)reader["IsActive"]
                    };

                    users.Add(user);
                }

                // Close the reader and connection
                reader.Close();
                connection.Close();
            }

            return users;
        }
    }
}