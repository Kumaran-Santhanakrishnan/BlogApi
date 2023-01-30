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
            if (userService.ValidateUser(Request.Headers["Authorization"].ToString())==null)
                return Unauthorized("Unauthorized User!");
            List<Blog> posts = postService.getAllPosts();
            return Ok(posts);
            

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Blog>> GetPostById(string id)
        {
            if (userService.ValidateUser(Request.Headers["Authorization"].ToString())==null)
                return Unauthorized("Unauthorized User!");
            try
            {
                Blog? post = postService.GetPostById(id);
                if (post != null) return Ok(post);
            }
            catch(Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
            return NotFound();
        }

        [HttpPost]
        public async Task<ActionResult<Blog>> CreatePost(PostDTO postDTO)
        {
            Console.WriteLine("Create Post Controller invoked");

            if (userService.ValidateUser(Request.Headers["Authorization"].ToString())==null)
                return Unauthorized("User not Authorized");
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

        [HttpDelete("{id}")]
        public async Task<ActionResult<Blog>> DeletePost(string id) {
            if (userService.ValidateUser(Request.Headers["Authorization"].ToString())==null)
                return Unauthorized("User not Authorized");

            // Get current user from token


            return null;
        }

        [HttpPost("like/{id}")]
        public async Task<ActionResult<String>> LikePost(string id)
        {
            string userId = userService.ValidateUser(Request.Headers["Authorization"].ToString());
            if (userId == null)
                return Unauthorized("User not Authorized");

            try
            {
                postService.LikePostById(id, userId);
                return Ok("Success");
            }
            catch(Exception e)
            {
                return this.StatusCode(500, e.Message);
                Console.WriteLine(e.StackTrace);
            }

            return "Unknown error";
        }


        [HttpPost("unlike/{id}")]
        public async Task<ActionResult<String>> UnlikePost(string id)
        {
            string userId = userService.ValidateUser(Request.Headers["Authorization"].ToString());
            if (userId == null)
                return Unauthorized("User not Authorized");

            try
            {
                postService.UnlikePostById(id, userId);
                return Ok("Success");
            }
            catch (Exception e)
            {
                return this.StatusCode(500, e.Message);
                Console.WriteLine(e.StackTrace);
            }

            return "Unknown error";
        }

        [HttpPost("comment")]
        public async Task<ActionResult<String>> addComment(CommentDTO commentDTO)
        {
            string userId = userService.ValidateUser(Request.Headers["Authorization"].ToString());
            if (userId == null)
                return Unauthorized("User not Authorized");

            try
            {
                postService.AddCommentByPostId(commentDTO);
                return Ok("Success");
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message+e.StackTrace);
                return this.StatusCode(500, e.Message);
            }

            return "Unknown error";
        }
    }
}
