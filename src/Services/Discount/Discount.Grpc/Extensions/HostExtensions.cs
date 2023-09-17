using System;
using Microsoft.Extensions.Configuration;
using Npgsql;

namespace Discount.Grpc.Extensions
{
	public static class HostExtensions
	{
        public static IHost MigrateDatabase<TContext>(this IHost host, int? retry = 0)
        {
            int retryForAvaliability = retry.Value;
            using (var scope = host.Services.CreateScope())
            {
                var services = scope.ServiceProvider;
                var configuration = services.GetRequiredService<IConfiguration>();
                var logger = services.GetRequiredService<ILogger<TContext>>();

                try
                {
                    logger.LogInformation("Migrating postgres database");
                    var connection = new NpgsqlConnection(configuration.GetValue<string>("DatabaseSettings:ConnectionString"));
                    connection.Open();

                    var command = new NpgsqlCommand
                    {
                        Connection = connection

                    };
                    command.CommandText="Drop table if exists public.\"Coupon\"";
                    command.ExecuteNonQuery();
                    command.CommandText = "create table public.\"Coupon\"(\"Id\" serial primary key, \"ProductName\" varchar(24) not null, \"Description\" Text, \"Amount\" INT)";
                    command.ExecuteNonQuery();
                    command.CommandText = "INSERT INTO public.\"Coupon\"(\"ProductName\", \"Description\", \"Amount\") VALUES('IPhone X', 'IPhone Discount', 150);";
                    command.ExecuteNonQuery();

                    command.CommandText = "INSERT INTO public.\"Coupon\"(\"ProductName\", \"Description\", \"Amount\") VALUES('Samsung 10', 'Samsung Discount', 100);";
                    command.ExecuteNonQuery();

                    logger.LogInformation("Migration completed");
                }
                catch (NpgsqlException ex)
                {
                    logger.LogError(ex, "An error occured");
                    if (retryForAvaliability < 50)
                    {
                        retryForAvaliability++;
                        System.Threading.Thread.Sleep(2000);
                        MigrateDatabase<TContext>(host, retryForAvaliability);
                    }
                }
            }

            return host;
        }
    }
}

