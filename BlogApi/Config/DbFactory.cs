using System;
using MySqlConnector;

namespace BlogApi.Config
{
	public class DbFactory
	{
		private static MySqlConnection mySQlConnection;
		private readonly IConfiguration Configuration;

		public DbFactory(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public MySqlConnection getConnection()
		{
            if(mySQlConnection == null)
			{
				var server = Configuration["ConnectionStrings:server"];
                var database = Configuration["ConnectionStrings:database"];
                var user = Configuration["ConnectionStrings:user"];
                var password = Configuration["ConnectionStrings:password"];
                var port = Configuration["ConnectionStrings:port"];


                var connectionString = "SERVER="+server+";DATABASE="+database+";USER="+user+";PASSWORD="+password+";PORT="+port+";";
                var poolConfigString = "Connection Timeout=30;Connection Lifetime=0;Min Pool Size=0;Max Pool Size=50;Pooling=true;";

                Console.WriteLine(connectionString);

				mySQlConnection = new MySqlConnection(connectionString+poolConfigString);

            }
			return mySQlConnection;
        }
	}
}

