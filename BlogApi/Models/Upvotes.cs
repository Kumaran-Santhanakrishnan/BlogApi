using System;
namespace BlogApi.Models
{
	public class Upvotes
	{
		private string id { get; set; }
		private string actorId { get; set; }
		private string blogUuid { get; set; }
		private DateTime createdTime { get; set; }

        public Upvotes() { }

    }

}

