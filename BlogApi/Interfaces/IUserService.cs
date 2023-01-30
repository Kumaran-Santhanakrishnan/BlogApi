using System;
using System.Collections.Generic;
using BlogApi.Models;

namespace BlogApi.Interfaces
{

    public interface IUserService
    {
        public List<User> getAllUsers();

        public User? GetUserById(string Id);

        public User Register(UserDTO userDTO);

        public string Login(UserDTO userDTO,HttpResponse Response);

        public RefreshToken GenerateRefreshToken();

        public string ValidateUser(string token);

    }


}