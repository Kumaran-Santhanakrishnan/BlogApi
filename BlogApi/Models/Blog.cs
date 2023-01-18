using System;
namespace BlogApi.Models
{
	public class Blog
	{
		private Guid id { get; set; }
		private Guid authorId { get; set; }
		private String content { get; set; }
		private DateTime createdTime { get; set; }
		private bool isPublic { get; set; }

		public Blog() { }
	}

	
}

