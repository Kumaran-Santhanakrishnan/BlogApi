using System;


namespace BlogApi.Models
{
	public class AuthToken
	{
        public string Token { get; set; }
        public string UserId { get; set; }
        public string CreatedTime { get; set; }
        public string Validity { get; set; }


        public AuthToken()
		{
		}
	}
}

