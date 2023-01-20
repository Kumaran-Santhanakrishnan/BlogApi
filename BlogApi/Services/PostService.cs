using System;
using BlogApi.Config;
using BlogApi.Exceptions;
using BlogApi.Models;
using MySqlConnector;

namespace BlogApi.Services
{
	public class PostService
	{
        private readonly DbFactory dbFactory;
        private readonly UserService userService;
        private readonly IConfiguration Configuration;

        public PostService(DbFactory factory, IConfiguration configuration,UserService _userService)
        {
            dbFactory = factory;
            Configuration = configuration;
            userService = _userService;
        }

        internal List<Blog> getAllPosts()
        {
            throw new NotImplementedException();
        }

        public Blog createPost(PostDTO post)
        {
            if (post == null) throw new InvalidDataException();

            if(post.AuthorId==null || post.Content==null) throw new InvalidDataException();

            if (userService.GetUserById(post.AuthorId) == null) throw new AuthenticationException("Unauthorized");

            MySqlConnection connection = dbFactory.getConnection();

            if (connection.State != System.Data.ConnectionState.Open) connection.Open();

            MySqlCommand command = new MySqlCommand("INSERT INTO Post (PostId,AuthorId,content,isPublic) VALUES (@PostId,@AuthorId,@content,@isPublic);", connection);
            string guid = Guid.NewGuid().ToString();
            command.Parameters.AddRange(new[]
            {
                new MySqlParameter("@PostId", guid),
                new MySqlParameter("@AuthorId", post.AuthorId),
                new MySqlParameter("@content", post.Content),
                new MySqlParameter("@isPublic", post.IsPublic)
            });

            if (command.ExecuteNonQuery() != 1)
            {
                connection.Close();
                throw new DatabaseException("Unable to create Post!");
            }
            connection.Close();
            return new BlogApi.Models.Blog()
            {
               id = guid,
               authorId = post.AuthorId,
               content = post.Content,
               isPublic = post.IsPublic,
               createdTime = DateTime.Now
            };
        }

        public Blog? GetPostById(string id)
        {
            if (id == String.Empty) return null;

            MySqlConnection connection = dbFactory.getConnection();
            if (connection.State != System.Data.ConnectionState.Open) connection.Open();

            MySqlCommand command = new MySqlCommand("SELECT * FROM Post WHERE PostId=@Id;", connection);
            command.Parameters.Add(new MySqlParameter("@Id", id));
            var reader = command.ExecuteReader();
            if (reader.Read())
            {
                return new Blog()
                {
                    id = reader.GetString("PostId"),
                    authorId = reader.GetString("AuthorId"),
                    content = reader.GetString("content"),
                    createdTime = reader.GetDateTime("createdAt"),
                    isPublic = reader.GetBoolean("isPublic")
                };
            }

            return null;


        }
    }
}

