using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApi.Models;
using BlogApi.Exceptions;
using BlogApi.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BlogApi.Controllers
{
    [Route("api/[controller]")]
    public class PostController : Controller
    {
        private readonly PostService postService;
        private readonly UserService userService;

        public PostController(PostService _postService,UserService _userService)
        {
            postService = _postService;
            userService = _userService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Blog>>> GetAllPosts()
        {
            if (userService.ValidateUser(Request.Headers["Authorization"].ToString()))
            {
                List<Blog> posts = postService.getAllPosts();
                return Ok(posts);
            }
            return Unauthorized("Unauthorized User!");

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Blog>> GetPostById(string id)
        {
            try
            {
                Blog? post = postService.GetPostById(id);
                if (post != null) return Ok(post);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            return null;
        }

        [HttpPost]
        public async Task<ActionResult<Blog>> CreatePost(PostDTO postDTO)
        {
            Console.WriteLine("Create Post Controller invoked");

            if (!userService.ValidateUser(Request.Headers["Authorization"].ToString())) return Unauthorized("User not Authorized");
            try
            {
                Blog blog = postService.createPost(postDTO);
                return blog;
            }
            catch(InvalidDataException e)
            {
                return BadRequest(e.Message);
            }
            catch(AuthenticationException e)
            {
                return Unauthorized(e.Message);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.StackTrace);
                return this.StatusCode(500,e.Message);
            }
        }
    }
}
