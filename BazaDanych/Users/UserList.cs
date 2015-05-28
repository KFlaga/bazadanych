using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BazaDanych.Users
{
    public class UserList
    {
        private List<User> __users;
        public UserList()
        {
            __users = new List<User>();
        }

        public void ParseUser(int id, string login, string passwd, int type, int emplId)
        {
            User user = new User(id, login, passwd, type, emplId);
            __users.Add(user);
            Console.WriteLine(user);
        }

        public User FindUser(String login) {
            User user = __users.Find(u => u.Login == login);
            return user;
        }
    }
}
