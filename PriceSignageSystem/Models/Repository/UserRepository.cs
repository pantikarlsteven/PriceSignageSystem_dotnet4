using PriceSignageSystem.Helper;
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
        public UserRepository(ApplicationDbContext db)
        {
            _db = db;
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

        public User AddUser(User user)
        {
            var data = _db.Users.Add(user);
            _db.SaveChanges();
            return data;

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
    }
}