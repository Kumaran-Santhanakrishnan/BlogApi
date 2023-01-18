using System;
namespace BlogApi.Exceptions
{
	public class DatabaseException : Exception
	{
        public DatabaseException(string message)
        : base(message) { }

        public DatabaseException(string message, Exception inner)
        : base(message, inner)
        {
        }
    }
}

