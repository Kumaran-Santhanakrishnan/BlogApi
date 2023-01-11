using System;
using System.Text.Json.Serialization;

namespace BlogApi.Models
{
	public class User
	{
		public string Id { get; set; }
		public string Name { get; set; }
		public string Mail { get; set; }
		

		public User()
		{
		}
	}
}

