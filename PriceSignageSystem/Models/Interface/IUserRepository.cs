using PriceSignageSystem.Models.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PriceSignageSystem.Models.Interface
{
    public interface IUserRepository
    {
        IQueryable<User> GetAll();
        List<User> GetUsers();
        User AddUser(User user);
        List<Role> GetRoles();
        int UpdatePassword(string username, string newPassword);
        User SearchAccount(string username);
        int UpdateAccount(string empid, string newpw, string username, int role, int status);
        int UpdateInfo(UserStoreDto user);
        List<UserDto> GetUsersInfo();
        int DeleteUser(int id);
        int UpdateUserInfo(UserDto dto);
    }
}
