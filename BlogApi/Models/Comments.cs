using System;
namespace BlogApi.Models
{
	public class Comments
	{
		private Guid id { get; set; }
        private Guid actorId { get; set; }
		private Guid blogId { get; set; }
		private Guid parentId { get; set; }
		private DateTime createdTime { get; set; }
		private String content { get; set; }
		private bool isReply { get; set; }

		public Comments() { }
    }
}

