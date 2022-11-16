using Npgsql;

namespace Discount.API.Extentions
{

    // This is Extention Method
    public static class HostExtentions
    {

        public static IHost MigrageDatabase<TContext>(this IHost host, int? retry = 0)
        {
            retry ??= 0;

            int retryForAvailability = retry.Value;
            using var scope = host.Services.CreateScope();
            var services = scope.ServiceProvider;
            var configuration = services.GetRequiredService<IConfiguration>();
            var logger = services.GetRequiredService<ILogger>();
            try
            {

                logger.LogInformation("Migrating Postgresql database.");

                using var connection = new NpgsqlConnection(
                  configuration.GetValue<string>("DatabaseSettings:ConnectionString")
                );

                connection.Open();
                using var command = new NpgsqlCommand
                {
                    Connection = connection
                };

                command.CommandText = "DROP TABLE IF EXISTS Coupon";
                command.ExecuteNonQuery();

                command.CommandText = @"
                    CREATE TABLE Coupon(
                        Id SERIAL PRIMARY KEY,
                        ProductName NVARCHAR(24) NOT NULL
                        Description TEXT,
                        Amount INT
                    )
                ";
                command.ExecuteNonQuery();

                command.CommandText = @"
                    INSERT INTO Coupon (ProductName, Description, Amount) 
                    VALUES ('IPhone X', 'IPhone Discount', 150);
                ";
                command.ExecuteNonQuery();

                command.CommandText = @"
                    INSERT INTO Coupon (ProductName, Description, Amount)
                    VALUES ('Samsung 10', 'Samsung Discount', 100);
                ";
                command.ExecuteNonQuery();

                logger.LogInformation("Migrated Postgresql Database");

                command.Dispose();

            }
            catch (NpgsqlException ex)
            {

                logger.LogError(ex, "An error occurred while migrating the postgresql database");

                if (retryForAvailability < 50)
                {
                    retryForAvailability++;
                    Thread.Sleep(2000);
                    MigrageDatabase<TContext>(host, retryForAvailability);
                }

            }

            scope.Dispose();

            return host;
        }

    }

}
