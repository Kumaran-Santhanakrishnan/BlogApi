using System;
namespace BlogApi.Models
{
	public class Comments
	{
		internal string id { get; set; }
        internal string actorId { get; set; }
		internal string blogId { get; set; }
		internal string parentId { get; set; }
		internal DateTime createdTime { get; set; }
		internal string content { get; set; }
		internal bool isReply { get; set; }

		public Comments() { }
    }
}

