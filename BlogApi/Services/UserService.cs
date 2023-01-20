using System;
using BlogApi.Config;
using BlogApi.Models;
using BlogApi.Exceptions;
using MySqlConnector;
using System.Security.Cryptography;
using static System.Net.Mime.MediaTypeNames;
using NuGet.Common;
using System.Collections;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;
using System.Text.RegularExpressions;
using Org.BouncyCastle.Asn1.Ocsp;

namespace BlogApi.Services
{
	public class UserService
	{
        private readonly DbFactory dbFactory;
        private readonly IConfiguration Configuration;

        public UserService(DbFactory factory, IConfiguration configuration)
		{
            dbFactory = factory;
            Configuration = configuration;
		}

        public UserService() { }

		public List<User> getAllUsers()
		{
            List<User> users = new List<User>();
            MySqlConnection connection = dbFactory.getConnection();

            if (connection.State != System.Data.ConnectionState.Open) connection.Open();

            MySqlCommand command = new MySqlCommand("SELECT * FROM USER;", connection);

            var result =  command.ExecuteReader();

            while (result.Read())
            {
                users.Add(new User
                {
                    Id = result.GetString("Id"),
                    Mail = result.GetString("Email"),
                    Name = result.GetString("Name")

                });
            }

            connection.Close();

            return users;

        }

        public User? GetUserById(string Id)
        {
            if (Id == String.Empty) return null;

            MySqlConnection connection = dbFactory.getConnection();
            if (connection.State != System.Data.ConnectionState.Open) connection.Open();

            MySqlCommand command = new MySqlCommand("SELECT * FROM USER WHERE Id=@Id;", connection);
            command.Parameters.Add(new MySqlParameter("@Id", Id));
            var reader = command.ExecuteReader();
            if (reader.Read())
            {
                User user = new User()
                {
                    Id = reader.GetString("Id"),
                    Mail = reader.GetString("Email"),
                    Name = reader.GetString("Name")
                };
                connection.Close();
                return user;
            }

            connection.Close();
            return null;
        }

        public async Task<User> Register(UserDTO userDTO)
        {
            if (userDTO.Email == null || userDTO.Name == null || userDTO.Password == null) throw new AuthenticationException("Missing Credentials");

            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(userDTO.Email);

            if (!match.Success) throw new AuthenticationException("Ivalid Email!");

            if (userDTO.Password.Length < 8) throw new AuthenticationException("Password must be atleast 8 characters!");

            regex = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$");
            match = regex.Match(userDTO.Password);

            if (!match.Success) throw new AuthenticationException("Ivalid Password! Password must contain atleast one uppercase character , one lowercase character , one digit and one special character!");



            MySqlConnection connection = dbFactory.getConnection();
            if (connection.State != System.Data.ConnectionState.Open) connection.Open();

            MySqlCommand command = new MySqlCommand("SELECT COUNT(Id) FROM USER WHERE Email=@Email;", connection);
            MySqlParameter email = new MySqlParameter("@Email", userDTO.Email);
            command.Parameters.Add(email);
            var result = (Int64) command.ExecuteScalar();

            if (result > 0) throw new DatabaseException("User with email already exists!");

            User user = new User();
            user.Id = Guid.NewGuid().ToString();
            user.Mail = userDTO.Email;
            user.Name = userDTO.Name;


            command = new MySqlCommand("INSERT INTO USER (Id,Name,Email) VALUES (@Id,@Name,@Email);",connection);
            MySqlParameter Id = new MySqlParameter("@Id", user.Id);
            MySqlParameter name = new MySqlParameter("@Name", userDTO.Name);
            command.Parameters.Add(email);
            command.Parameters.Add(Id);
            command.Parameters.Add(name);

            if (command.ExecuteNonQuery() != 1)
            {
                connection.Close();
                throw new DatabaseException("Unable to create user!");
            }

            command = new MySqlCommand("INSERT INTO AUTH (UserId,Password,Salt) VALUES (@Id,@Password,@Salt);",connection);

            string salt = generateSalt();
            string hashedPassword = hashPassword(userDTO.Password,salt);
            Console.WriteLine("hashed password : "+hashedPassword);
            command.Parameters.AddRange(new[] {
                Id,
                new MySqlParameter("@Password", hashedPassword),
                new MySqlParameter("@Salt",salt)
            });
            
            

            if (command.ExecuteNonQuery() != 1)
            {
                Console.WriteLine("Problem writing to Auth table");
                connection.Close();
                throw new DatabaseException("Unable to create user!");
            }
            connection.Close();
            return user;
        }

        
        public string Login(UserDTO userDTO,HttpResponse Response)
        {
            if (userDTO.Email == null || userDTO.Password == null) throw new AuthenticationException("Missing Credentials");

            Regex regex = new Regex(@"^([\w\.\-]+)@([\w\-]+)((\.(\w){2,3})+)$");
            Match match = regex.Match(userDTO.Email);

            if (!match.Success) throw new AuthenticationException("Ivalid Email!");

            if (userDTO.Password.Length < 8) throw new AuthenticationException("Password must be atleast 8 characters!");

            regex = new Regex("^(?=.*?[A-Z])(?=.*?[a-z])(?=.*?[0-9])(?=.*?[#?!@$%^&*-]).{8,}$");
            match = regex.Match(userDTO.Password);

            if (!match.Success) throw new AuthenticationException("Ivalid Password! Password must contain atleast one uppercase character , one lowercase character , one digit and one special character!");

            MySqlConnection connection = dbFactory.getConnection();
            if (connection.State != System.Data.ConnectionState.Open) connection.Open();

            MySqlCommand command = new MySqlCommand("SELECT Id FROM USER WHERE Email=@Email;", connection);
            MySqlParameter email = new MySqlParameter("@Email", userDTO.Email);
            command.Parameters.Add(email);

            var reader = command.ExecuteReader();

            if (reader.Read())
            {
                string userId = reader.GetString("Id");
                //connection.Close();

                //connection.Open();
                command = new MySqlCommand("SELECT (Password,Salt) FROM Auth WHERE UserId=@Id;", connection);
                MySqlParameter Id = new MySqlParameter("@Id", userId);
                command.Parameters.Add(Id);

                reader = command.ExecuteReader(); ;
                reader.Read();

                string passoword = reader.GetString("Password");
                string salt = reader.GetString("Salt");

                if (passoword.Equals(hashPassword(userDTO.Password,salt)))
                {
                    string token = CreateToken(userDTO);
                    var refreshToken = GenerateRefreshToken();

                    
                    SetRefreshToken(refreshToken, Response);
                    connection.Close();
                    return token;
                }
                connection.Close();
                throw new AuthenticationException("Wrong Password!");
            }
            else
            {
                connection.Close();
                throw new DatabaseException("User not found!");
            }
            
        }

        private string CreateToken(UserDTO userDTO)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, userDTO.Email),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(
                Configuration.GetSection("Jwt:Key").Value));

            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: creds);

            var jwt = new JwtSecurityTokenHandler().WriteToken(token);

            return jwt;
        }

        public RefreshToken GenerateRefreshToken()
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
                Expires = DateTime.Now.AddDays(7),
                Created = DateTime.Now
            };

            return refreshToken;
        }

        private void SetRefreshToken(RefreshToken newRefreshToken, HttpResponse Response)
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

        public bool ValidateUser(string token)
        {
            if (token == String.Empty) return false;
            
            var bearertoken = token.Split(" ")[1];

            // TODO : implement verification logic

            return true;
            
        }

        private string hashPassword(string password,string salt)
        {
            var sha = new System.Security.Cryptography.SHA256Managed();
            byte[] textbytes = System.Text.Encoding.UTF8.GetBytes(password + salt);
            return Encoding.Default.GetString(sha.ComputeHash(textbytes));
        }

        private string generateSalt()
        {
            Random random = new Random();
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, (int)random.NextInt64(10,15))
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }


    }


}

