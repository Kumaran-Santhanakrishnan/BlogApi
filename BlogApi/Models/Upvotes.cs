using System;
namespace BlogApi.Models
{
	public class Upvotes
	{
		private Guid id { get; set; }
		private Guid actorId { get; set; }
		private Guid blogUuid { get; set; }
		private DateTime createdTime { get; set; }

        public Upvotes() { }

    }

}

