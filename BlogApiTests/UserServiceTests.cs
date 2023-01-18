using System;
using BlogApi.Controllers;
using BlogApi.Models;
using Xunit;

namespace BlogApi.Tests
{
	public class UserServiceTests
	{
        private readonly UserController userController;

        public UserServiceTests()
		{
			userController = new UserController(new Services.UserService());
		}

		[Fact]
		public async void TestRegister()
		{
			UserDTO userDTO = new UserDTO()
			{
				Name = "",
				Password = "",
				Email = ""
			};

			try
			{
				await userController.Register(userDTO);
				Assert.Fail("Null values are was inappropriately allowed.");
			}
			catch (Exception e) {
				Console.WriteLine("test passed.");
			}

			userDTO.Name = "asdfa";
			userDTO.Email = "sahjdgfja";
			userDTO.Password = "ksah";


            try
            {
                await userController.Register(userDTO);
                Assert.Fail("Invalid Email is allowed");
            }
            catch (Exception e)
            {
                Console.WriteLine("test passed.");
            }

			userDTO.Email = "ajhsbf@ajbs.in";
            try
            {
                await userController.Register(userDTO);
                Assert.Fail("Invalid Password allowed");
            }
            catch (Exception e)
            {
                Console.WriteLine("test passed.");
            }
        }
	}
}

