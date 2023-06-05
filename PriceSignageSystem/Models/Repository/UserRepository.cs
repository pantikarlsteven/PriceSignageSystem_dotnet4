﻿using PriceSignageSystem.Models.DatabaseContext;
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
    }
}