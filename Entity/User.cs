using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace OrderManagement.Entity
{
    public class User
    {
        private int userId;
        private string username;
        private string password;
        private string role;

        public int UserId
        {
            get => userId;
            set
            {
                if (value <= 0)
                    throw new ArgumentException("User ID must be a positive integer.");
                userId = value;
            }
        }

        public string Username
        {
            get => username;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Username cannot be empty.");
                username = value;
            }
        }

        public string Password
        {
            get => password;
            set
            {
                if (!IsValidPassword(value))
                    throw new ArgumentException("Password must be at least 6 characters, contain one uppercase, one lowercase, one digit, and one special character.");
                password = value;
            }
        }

        public string Role
        {
            get => role;
            set
            {
                if (value != "Admin" && value != "User")
                    throw new ArgumentException("Role must be either 'Admin' or 'User'.");
                role = value;
            }
        }

        public User() { }

        public User(int userId, string username, string password, string role)
        {
            UserId = userId;
            Username = username;
            Password = password;
            Role = role;
        }

        private bool IsValidPassword(string password)
        {
            
            return Regex.IsMatch(password, @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^A-Za-z\d]).{6,}$");
        }
    }
}
