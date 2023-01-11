using System;
using BlogApi.Config;
using BlogApi.Models;
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

namespace BlogApi.Services
{
	public class UserService
	{
        public readonly UserService _userservice;
        private readonly DbFactory dbFactory;
        private readonly IConfiguration Configuration;

        public UserService(DbFactory factory, IConfiguration configuration)
		{
            dbFactory = factory;
            Configuration = configuration;
		}

		public async Task<List<User>> getAllUsers()
		{
            List<User> users = new List<User>();
            MySqlConnection connection = dbFactory.getConnection();

           
            if(connection.State != System.Data.ConnectionState.Open) connection.Open();


            MySqlCommand command = new MySqlCommand("SELECT * FROM USER;", connection);

            var result = await command.ExecuteReaderAsync();



            while (await result.ReadAsync())
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

        public async Task<User> Register(UserDTO userDTO)
        {
            MySqlConnection connection = dbFactory.getConnection();
            if (connection.State != System.Data.ConnectionState.Open) connection.Open();

            MySqlCommand command = new MySqlCommand("SELECT COUNT(Id) FROM USER WHERE Email=@Email;", connection);
            MySqlParameter email = new MySqlParameter("@Email", userDTO.Email);
            command.Parameters.Add(email);
            var result = (Int64) command.ExecuteScalar();

            if (result > 0) return null;

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
                return null;
            }

            command = new MySqlCommand("INSERT INTO AUTH (UserId,Password) VALUES (@Id,@Password);",connection);


            string hashedPassword = hashPassword(userDTO.Password);
            Console.WriteLine("hashed password : "+hashedPassword);
            MySqlParameter password = new MySqlParameter("@Password", hashedPassword);
            command.Parameters.Add(Id);
            command.Parameters.Add(password);

            if (command.ExecuteNonQuery() != 1)
            {
                Console.WriteLine("Problem writing to Auth table");
                return null;
            }

            return user;
        }

        
        public string Login(UserDTO userDTO)
        {
            MySqlConnection connection = dbFactory.getConnection();
            if (connection.State != System.Data.ConnectionState.Open) connection.Open();

            MySqlCommand command = new MySqlCommand("SELECT Id FROM USER WHERE Email=@Email;", connection);
            MySqlParameter email = new MySqlParameter("@Email", userDTO.Email);
            command.Parameters.Add(email);

            var reader = command.ExecuteReader();

            if (reader.Read())
            {
                string userId = reader.GetString("Id");
                command = new MySqlCommand("SELECT Password FROM Auth WHERE UserId=@Id;", connection);
                MySqlParameter Id = new MySqlParameter("@Id", userId);
                command.Parameters.Add(Id);

                reader = command.ExecuteReader();

                string passoword = reader.GetString("Password");

                if (passoword.Equals(hashPassword(userDTO.Password)))
                {
                    string token = CreateToken(userDTO);
                    var refreshToken = GenerateRefreshToken();
                    return token;
                }
                return null;

            }
            else
            {
                return null;
            }
            
        }

        private string CreateToken(UserDTO userDTO)
        {
            List<Claim> claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, userDTO.Email),
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

        public Boolean SetRefreshToken(RefreshToken refreshToken)
        {
            MySqlConnection connection = dbFactory.getConnection();
            if (connection.State != System.Data.ConnectionState.Open) connection.Open();

            MySqlCommand command = new MySqlCommand("INSERT INTO AuthToken (Token,UserId,CreatedAt,Validity) VALUES (@Token,@UserId,@CreatedAt,@Validity);", connection);
            MySqlParameter email = new MySqlParameter("@Email", userDTO.Email);

            return true;
        }

        private string hashPassword(string password)
        {
            var sha = new System.Security.Cryptography.SHA256Managed();
            string salt = "temporary_salt";
            byte[] textbytes = System.Text.Encoding.UTF8.GetBytes(password + salt);

            return Encoding.Default.GetString(sha.ComputeHash(textbytes));
        }


    }


}

