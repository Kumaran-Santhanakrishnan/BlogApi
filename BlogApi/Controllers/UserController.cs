using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BlogApi.Models;
using BlogApi.Services;
using BlogApi.Config;
using MySqlConnector;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BlogApi.Controllers
{
    [Route("api/[controller]")]
    public class UserController : Controller
    {
        private readonly UserService userService;

        public UserController(UserService _userService)
        {
            userService = _userService;
        }

        [HttpGet]
        public async Task<ActionResult<List<User>>> GetAllUsers()
        {
            List<User> users = userService.getAllUsers().Result;


            return Ok(users);
        }


        [HttpPost]
        public async Task<ActionResult<User>> Register(UserDTO user)
        {
            if (user.Name == null || user.Password == null || user.Email == null)
            {
                return BadRequest();
            }

            User registeredUser = await userService.Register(user);
            if (registeredUser == null) return BadRequest();

            return Ok(registeredUser); 
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDTO userDTO)
        {
            var token = userService.Login(userDTO);

            var refreshToken = userService.GenerateRefreshToken();
            SetRefreshToken(refreshToken);
            /*
                Use CustomException
             */
            return Ok(token);
        }

        private void SetRefreshToken(RefreshToken newRefreshToken)
        {
            //user.RefreshToken = newRefreshToken.Token;
            //user.TokenCreated = newRefreshToken.Created;
            //user.TokenExpires = newRefreshToken.Expires;



            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Expires = newRefreshToken.Expires
            };
            Response.Cookies.Append("refreshToken", newRefreshToken.Token, cookieOptions);


            
        }
    }
}

