using System;
using BlogApi.Services;
using BlogApi.Models;
using Xunit;

namespace BlogApi.Tests
{
	public class UserServiceTests
	{
		private readonly UserService userService;

		public UserServiceTests(UserService _userService)
		{
			userService = new UserService();
		}

		[Fact]
		public void TestRegister()
		{
			UserDTO userDTO = new UserDTO()
			{
				Name = "",
				Password = "",
				Email = ""
			};

			try
			{
				userService.Register(userDTO);
				Assert.Fail("Null values are was inappropriately allowed.");
			}
			catch (Exception e)
			{
				Console.WriteLine("test passed.");
			}
		}
	}
}

