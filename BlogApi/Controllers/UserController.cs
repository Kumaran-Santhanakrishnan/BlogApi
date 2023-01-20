using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using BlogApi.Models;
using BlogApi.Services;
using BlogApi.Config;
using BlogApi.Exceptions;
using MySqlConnector;
using System.Text.RegularExpressions;
using Xunit;

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
            if (userService.ValidateUser(Request.Headers["Authorization"].ToString()))
            {
                List<User> users = userService.getAllUsers();
                return Ok(users);
            }
            return Unauthorized("Unauthorized User!");
            
        }


        [HttpPost]
        public async Task<ActionResult<User>> Register(UserDTO user)
        {
            

            try
            {
                User registeredUser = await userService.Register(user);
                //if (registeredUser == null) return BadRequest
                return Ok(registeredUser);
            }
            catch(AuthenticationException e)
            {
                return BadRequest(e.Message);
            }
            catch(DatabaseException e)
            {
                return this.StatusCode(500, e.Message);
            }
            catch(Exception e)
            {
                return this.StatusCode(500);
            }
            
        }

        [HttpPost("login")]
        public async Task<ActionResult<string>> Login(UserDTO userDTO)
        {
            try{
                var token = userService.Login(userDTO,Response);
                return Ok(token);
            }
            catch(Exception e)
            {
                return Unauthorized(e.Message);
            }
        }



    }
}

