using PriceSignageSystem.Helper;
using PriceSignageSystem.Models.Constants;
using PriceSignageSystem.Models.DatabaseContext;
using PriceSignageSystem.Models.Dto;
using PriceSignageSystem.Models.Interface;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Entity.Validation;
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
            connectionString = ConfigurationManager.ConnectionStrings["MyConnectionString"].ConnectionString;
        }

        public IQueryable<User> GetAll()
        {
            var data = _db.Users;
            return data;
        }
        public List<User> GetUsers()
        {
            var users = _db.Users.ToList();
            return users;
        }

        public int AddUser(UserDto newUser)
        {
            var existingUser = _db.Users.Where(a => a.UserName == newUser.UserName).FirstOrDefault();
            var result = 0;
            if (existingUser == null)
            {
                var encryptedPw = EncryptionHelper.Encrypt(newUser.Password);
                var user = new User()
                {
                    EmployeeId = newUser.EmployeeId,
                    UserName = newUser.UserName,
                    Password = encryptedPw,
                    RoleId = newUser.RoleId,
                    IsActive = UserStatusConstants.Active
                };

                _db.Users.Add(user);
                result = _db.SaveChanges();
            }

            return result;
        }

        public List<Role> GetRoles()
        {
            return _db.Roles.OrderByDescending(a => a.Id).ToList();
        }

        public int UpdatePassword(string username, string newPassword)
        {
            var record = _db.Users.Where(f => f.UserName == username).FirstOrDefault();
            
            if(record != null)
            {
                var encryptedPw = EncryptionHelper.Encrypt(newPassword);
                record.Password = encryptedPw;
            }
            
            var result = _db.SaveChanges();
            return result;
        }

        public User SearchAccount(string username)
        {
            var result = _db.Users.Where(u => u.UserName == username).FirstOrDefault();

            return result;
        }

        public int UpdateAccount(string empid, string newpw, string username, int role, int status)
        {
            var user = _db.Users.Where(a => a.UserName == username).FirstOrDefault();
            var encryptedNewPw = string.Empty;

            if (!string.IsNullOrWhiteSpace(newpw))
                encryptedNewPw = EncryptionHelper.Encrypt(newpw);
            

            if (empid != user.EmployeeId)
                user.EmployeeId = empid;
            if (!string.IsNullOrEmpty(encryptedNewPw) && encryptedNewPw != user.Password)
                user.Password = encryptedNewPw;
            if (role != user.RoleId)
                user.RoleId = role;
            if (status != user.IsActive)
                user.IsActive = status;

            var result = _db.SaveChanges();

            return result;
        }

        public int UpdateInfo(UserStoreDto user)
        {
            var record = _db.Users.Where(a => a.UserName == user.UserName).FirstOrDefault();
            record.EmployeeId = user.EmployeeId;

            var result = _db.SaveChanges();
            return result;
        }

        public List<UserDto> GetUsersInfo()
        {
            var result = (from a in _db.Users
                         join b in _db.Roles on a.RoleId equals b.Id
                         select new UserDto
                         {
                             EmployeeId = a.EmployeeId ?? "",
                             UserId = a.UserId,
                             UserName = a.UserName,
                             Password = "",
                             RoleId = a.RoleId,
                             RoleName = b.Name
                         }).ToList();
            return result;
        }

        public int DeleteUser(int id)
        {
            var user = _db.Users.Where(a => a.UserId == id).FirstOrDefault();
            _db.Users.Remove(user);
            var result = _db.SaveChanges();

            return result;
        }

        public int UpdateUserInfo(UserDto dto)
        {

            var user = _db.Users.Where(a => a.UserId == dto.UserId).FirstOrDefault();
            user.EmployeeId = dto.EmployeeId;
            user.RoleId = dto.RoleId;
            if (!String.IsNullOrEmpty(dto.Password))
            {
                var encryptedPw = EncryptionHelper.Encrypt(dto.Password);
                user.Password = encryptedPw;
            }


            var result = _db.SaveChanges();
            return result;
        }

        public StoreDto GetStoreName(int storeId)
        {
            var record = new StoreDto();
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                try
                {
                    string query = "SELECT * FROM StoreLocations WHERE Id = @storeId";

                    using (SqlCommand cmd = new SqlCommand(query, conn))
                    {
                        cmd.Parameters.AddWithValue("@storeId", storeId);
                     
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                record = new StoreDto
                                {
                                    Id = (int)reader["Id"],
                                    Name = reader["Name"].ToString(),
                                };
                            }
                            reader.Close();
                        }
                    }
                }
                catch (Exception ex)
                {
                }

                conn.Close();
            }
            return record;
        }

        public User CheckIfEmpIdExisting(UserDto user)
        {
            var result = _db.Users.Where(a => a.EmployeeId == user.EmployeeId).FirstOrDefault();

            return result;
        }
    }
}