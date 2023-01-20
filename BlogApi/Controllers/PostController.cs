using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BlogApi.Models;
using BlogApi.Services;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace BlogApi.Controllers
{
    [Route("api/[controller]")]
    public class PostController : Controller
    {
        private readonly PostService postService;

        public PostController(PostService _postService)
        {
            postService = _postService;
        }

        [HttpGet]
        public async Task<ActionResult<List<Blog>>> GetAllPosts()
        {
            if (Request.Headers["Authorization"].ToString() != String.Empty)
            {
                string bearerToken = Request.Headers["Authorization"];

                var token = bearerToken.Split(" ")[1];
                Console.WriteLine("------ Token: " + token);

                List<Blog> posts = postService.getAllPosts();

                return Ok(posts);
            }
            return Unauthorized("Unauthorized User!");

        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Blog>> GetPostById()
        {

            return null;
        }

        [HttpPost]
        public async Task<ActionResult<Blog>> CreatePost(PostDTO postDTO)
        {

            return null;
        }
    }
}
