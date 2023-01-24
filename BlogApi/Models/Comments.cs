using System;
namespace BlogApi.Models
{
	public class Comments
	{
		private string id { get; set; }
        private string actorId { get; set; }
		private string blogId { get; set; }
		private string parentId { get; set; }
		private DateTime createdTime { get; set; }
		private string content { get; set; }
		private bool isReply { get; set; }

		public Comments() { }
    }
}

