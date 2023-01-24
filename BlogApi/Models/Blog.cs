using System;
namespace BlogApi.Models
{
	public class Blog
	{
		public string id { get; set; }
        public string authorId { get; set; }
		public string content { get; set; }
        public string title { get; set; }
        public DateTime createdTime { get; set; }
		public bool isPublic { get; set; }

		public Blog() { }


	}



	
}

