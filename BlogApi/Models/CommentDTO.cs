using System;
namespace BlogApi.Models
{
	public class CommentDTO
	{
		public string PostId { get; set; }
		public string ActorId { get; set; }
		public string content { get; set; }
        public Boolean? IsReply { get; set; }
        public string? ParentId { get; set; }

        public CommentDTO()
		{
		}
	}
}

