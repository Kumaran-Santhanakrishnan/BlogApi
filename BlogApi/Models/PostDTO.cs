using System;
namespace BlogApi.Models
{
	public class PostDTO
	{
        public string? PostId { get; set; }
        public string AuthorId { get; set; }
		public string Content { get; set; }
		public bool IsPublic { get; set; }

		public PostDTO()
		{

		}
	}
}

