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

            MySqlCommand command = new MySqlCommand("INSERT INTO Post (PostId,AuthorId,title,content,isPublic) VALUES (@PostId,@AuthorId,@title,@content,@isPublic);", connection);
            string guid = Guid.NewGuid().ToString();
            command.Parameters.AddRange(new[]
            {
                new MySqlParameter("@PostId", guid),
                new MySqlParameter("@AuthorId", post.AuthorId),
                new MySqlParameter("@title",post.Title),
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
                    title = reader.GetString("title"),
                    content = reader.GetString("content"),
                    createdTime = reader.GetDateTime("createdAt"),
                    isPublic = reader.GetBoolean("isPublic")
                };
            }

            return null;


        }

        public Boolean LikePostById(string id,string userId)
        {
            if (id == String.Empty) return false;

            MySqlConnection connection = dbFactory.getConnection();
            if (connection.State != System.Data.ConnectionState.Open) connection.Open();

            MySqlCommand command = new MySqlCommand("SELECT COUNT(PostId) FROM Post WHERE PostId=@Id;", connection);
            command.Parameters.Add(new MySqlParameter("@Id", id));
            var reader = (Int64) command.ExecuteScalar();
            connection.Close();

            if (reader != 1) throw new DatabaseException("Post not Found!");

            connection.Open();
            command = new MySqlCommand("SELECT COUNT(Id) FROM Upvotes WHERE PostId=@Id AND actorId=@actorId;", connection);
            command.Parameters.AddRange(new[] {
                new MySqlParameter("@Id",id),
                new MySqlParameter("@actorId", userId)
                });
            reader = (Int64)command.ExecuteScalar();

            if (reader > 0)
            {
                connection.Close();
                return true;
            }

            string upvoteId = Guid.NewGuid().ToString();
            command = new MySqlCommand("INSERT INTO Upvotes (Id,actorId,postId) VALUES (@Id,@actorId,@postId);", connection);
            command.Parameters.AddRange(new[]
            {
                new MySqlParameter("@Id",upvoteId),
                new MySqlParameter("@postId",id),
                new MySqlParameter("@actorId", userId)
            });


            if (command.ExecuteNonQuery() != 1)
            {
                connection.Close();
                throw new DatabaseException("Unable to Like Post!");
            }


            return true;
        }

        public string UnlikePostById(string id, string userId)
        {
            if (id == null || userId == null) throw new InvalidDataException();

            MySqlConnection connection = dbFactory.getConnection();
            if (connection.State != System.Data.ConnectionState.Open) connection.Open();

            MySqlCommand command = new MySqlCommand("DELETE FROM Upvotes WHERE PostId=@Id AND actorId=@actorId;", connection);
            command.Parameters.AddRange(new[] {
                new MySqlParameter("@Id",id),
                new MySqlParameter("@actorId", userId)
                });

            if (command.ExecuteNonQuery() != 1)
            {
                connection.Close();
                throw new DatabaseException("Unable to Unlike Post!");
            }



            return "success";
        }
    }
}

